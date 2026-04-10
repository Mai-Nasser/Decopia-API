using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Rules_Engine_API.Models;
using Rules_Engine_API.Repositories;
using Rules_Engine_API.Services;

namespace Rules_Engine_API.Controllers;

[ApiController]
[Route("api/rules")]
public class RulesController : ControllerBase
{
    private readonly IRulesEngine _rulesEngine;
    private readonly IEvaluationRepository _evalRepo;

    public RulesController(IRulesEngine rulesEngine, IEvaluationRepository evalRepo)
    {
        _rulesEngine = rulesEngine;
        _evalRepo = evalRepo;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] AnalyzeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Payload))
            return BadRequest("Payload is required");

        // ── Run rules engine ──────────────────────────────────────
        var matches = _rulesEngine.Analyze(request.Payload);

        var sourceIp = !string.IsNullOrWhiteSpace(request.ClientId)
            ? request.ClientId
            : GetClientIpFromRequest();

        // Extract unique attack names for display
        var attackNames = matches
            .Select(m => m.AttackName)
            .Distinct()
            .ToList();

        string attackType = string.Join("\n", attackNames);

        // Store full match details (attackName + pattern + score)
        var matchDetails = matches.Select(m => new
        {
            AttackName = m.AttackName,
            Pattern = m.Pattern,
            Score = m.Score
        }).ToList();

        //// ── Send syslog ───────────────────────────────────────────
        //var securityEvent = new SecurityEvent
        //{
        //    EventType = "web_attack",
        //    AttackType = attackType,
        //    SourceIp = sourceIp,
        //    Payload = request.Payload,
        //    MatchedAttacks = attackNames,
        //    Decoy = "webtrap-login",
        //    Timestamp = DateTime.UtcNow
        //};

        //SyslogSender.Send(securityEvent);

        // ── Persist evaluation session in DB ──────────────────────
        var evalSession = new EvaluationSession
        {
            Payload = request.Payload,
            SourceIp = sourceIp,
            AssignedTo = request.AssignedTo,
            UserAgent = request.UserAgent,
            DetectedAttackType = attackType,
            MatchedAttacksJson = JsonSerializer.Serialize(matchDetails),
            CreatedAt = DateTime.UtcNow
            // ✅ Removed: IsEvaluated = false (frontend enforces full evaluation)
        };

        var savedSession = await _evalRepo.CreateSessionAsync(evalSession);

        if (savedSession == null)
            return StatusCode(500, "Failed to save evaluation session");

        // ── Return response ───────────────────────────────────────
        return Ok(new
        {
            sessionId = savedSession.Id,
            payload = request.Payload,
            attackType,
            matches = matchDetails
        });
    }

    // ─────────────────────────────────────────────────────────────
    private string GetClientIpFromRequest()
    {
        if (HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var ip = forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(ip))
                return ip.Trim();
        }

        if (HttpContext.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
            return realIp.ToString();

        return HttpContext.Connection.RemoteIpAddress?
                   .MapToIPv4()
                   .ToString()
               ?? "unknown";
    }
}