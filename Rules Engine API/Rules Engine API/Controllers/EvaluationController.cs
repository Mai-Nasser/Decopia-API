


using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rules_Engine_API.Models;
using Rules_Engine_API.Repositories;

namespace Rules_Engine_API.Controllers;

[ApiController]
[Route("api/evaluation")]
public class EvaluationController : ControllerBase
{
    private readonly IEvaluationRepository _repo;

    public EvaluationController(IEvaluationRepository repo)
    {
        _repo = repo;
    }

    // ─────────────────────────────────────────────────────────────
    // POST api/evaluation/submit
    // ─────────────────────────────────────────────────────────────
    //[HttpPost("submit")]
    //public async Task<IActionResult> Submit([FromBody] SubmitEvaluationRequest request)
    //{
    //    if (request.SessionId <= 0)
    //        return BadRequest("Valid SessionId is required.");

    //    if (request.AttackEvaluations == null || request.AttackEvaluations.Count == 0)
    //        return BadRequest("At least one attack evaluation is required.");

    //    var session = await _repo.SubmitEvaluationAsync(request);

    //    if (session == null)
    //        return NotFound($"Session {request.SessionId} not found.");

    //    return Ok(new
    //    {
    //        message = "Evaluation submitted successfully.",
    //        sessionId = session.Id,
    //        evaluatedAt = session.EvaluatedAt,
    //        attacksEvaluated = request.AttackEvaluations.Count
    //    });
    //}

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitEvaluationRequest request)
    {
        try
        {
            if (request.SessionId <= 0)
                return BadRequest("Valid SessionId is required.");

            if (request.AttackEvaluations == null || request.AttackEvaluations.Count == 0)
                return BadRequest("At least one attack evaluation is required.");

            var session = await _repo.SubmitEvaluationAsync(request);

            if (session == null)
                return NotFound($"Session {request.SessionId} not found.");

            return Ok(new
            {
                message = "Evaluation submitted successfully.",
                sessionId = session.Id,
                evaluatedAt = session.EvaluatedAt,
                attacksEvaluated = request.AttackEvaluations.Count
            });
        }
        catch (DbUpdateException dbEx)
        {
            // هيرجعلك الرسالة الداخلية من EF Core
            return StatusCode(500, new
            {
                error = "Database update failed",
                message = dbEx.InnerException?.Message ?? dbEx.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Unknown error",
                message = ex.Message
            });
        }
    }

    // ─────────────────────────────────────────────────────────────
    // GET api/evaluation/sessions
    // ─────────────────────────────────────────────────────────────
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 100) pageSize = 20;

        var (items, total) = await _repo.GetSessionsAsync(page, pageSize);

        var mapped = items.Select(s => MapSession(s)).ToList();

        // filter by status if provided
        if (!string.IsNullOrWhiteSpace(status))
            mapped = mapped.Where(s => s.Status == status.ToLower()).ToList();

        return Ok(new
        {
            page,
            pageSize,
            totalCount = total,
            totalPages = (int)Math.Ceiling((double)total / pageSize),
            items = mapped
        });
    }

    // ─────────────────────────────────────────────────────────────
    // GET api/evaluation/sessions/{id}
    // ─────────────────────────────────────────────────────────────
    [HttpGet("sessions/{id:int}")]
    public async Task<IActionResult> GetSession(int id)
    {
        var session = await _repo.GetSessionByIdAsync(id);

        if (session == null)
            return NotFound();

        return Ok(MapSession(session));
    }

    // ─────────────────────────────────────────────────────────────
    // GET api/evaluation/stats
    // ─────────────────────────────────────────────────────────────
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _repo.GetStatsAsync();
        return Ok(stats);
    }

    // ─────────────────────────────────────────────────────────────
    // GET api/evaluation/failed
    // ─────────────────────────────────────────────────────────────
    [HttpGet("failed")]
    public async Task<IActionResult> GetFailed([FromQuery] string type = "all")
    {
        if (!new[] { "detection", "classification", "all" }.Contains(type.ToLower()))
            return BadRequest("type must be: detection | classification | all");

        var items = await _repo.GetFailedAsync(type);

        return Ok(new
        {
            type,
            totalCount = items.Count,
            items
        });
    }

    // ─────────────────────────────────────────────────────────────
    // GET api/evaluation/attack-summary
    // Attack-level statistics with error rates and failed payloads
    // ─────────────────────────────────────────────────────────────
    [HttpGet("attack-summary")]
    public async Task<IActionResult> GetAttackSummary()
    {
        var attacks = await _repo.GetAttackSummaryAsync();

        // ✅ Removed totalAttackTypes - frontend can use attacks.length
        return Ok(new
        {
            attacks
        });
    }

    // ─────────────────────────────────────────────────────────────
    // Helper: map session to response shape
    // ─────────────────────────────────────────────────────────────
    private static dynamic MapSession(EvaluationSession s)
    {
        var matchDetails = ParseMatchDetails(s.MatchedAttacksJson);
        var totalAttacks = matchDetails.Count;
        var evaluatedCount = s.AttackEvaluations.Count;

        // Merge each match (attackName + pattern) with its evaluation
        var attacks = matchDetails.Select(match =>
        {
            var eval = s.AttackEvaluations
                .FirstOrDefault(e => e.AttackName == match.AttackName
                                  && e.Pattern == match.Pattern);

            return new
            {
                attackName = match.AttackName,
                pattern = match.Pattern,
                score = match.Score,
                evaluation = eval == null ? null : (object)new
                {
                    eval.DetectionCorrect,
                    eval.ClassificationCorrect,
                    eval.Notes,
                    eval.EvaluatedAt
                }
            };
        }).ToList();

        return new
        {
            s.Id,
            s.Payload,
            s.SourceIp,
            s.AssignedTo,
            s.CreatedAt,
            TotalAttacks = totalAttacks,
            EvaluatedCount = evaluatedCount,
            PendingCount = totalAttacks - evaluatedCount,
            Status = GetStatus(totalAttacks, evaluatedCount),
            Attacks = attacks
        };
    }

    // ─────────────────────────────────────────────────────────────
    // Helper: calculate session status
    // ─────────────────────────────────────────────────────────────
    private static string GetStatus(int totalAttacks, int evaluatedCount)
    {
        if (evaluatedCount == 0) return "pending";
        if (evaluatedCount < totalAttacks) return "partial";
        return "complete";
    }

    // ─────────────────────────────────────────────────────────────
    // Helper: Parse match details (attackName + pattern + score)
    // ─────────────────────────────────────────────────────────────
    private static List<MatchDetail> ParseMatchDetails(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new();

        try
        {
            return JsonSerializer.Deserialize<List<MatchDetail>>(json) ?? new();
        }
        catch
        {
            return new();
        }
    }
}

// ─────────────────────────────────────────────────────────────
// Helper class for deserializing match details
// ─────────────────────────────────────────────────────────────
public class MatchDetail
{
    public string AttackName { get; set; }
    public string Pattern { get; set; }
    public int Score { get; set; }
}