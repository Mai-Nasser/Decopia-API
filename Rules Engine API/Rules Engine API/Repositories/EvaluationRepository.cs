

using Microsoft.EntityFrameworkCore;
using Rules_Engine_API.Data;
using Rules_Engine_API.Models;

namespace Rules_Engine_API.Repositories;

public class EvaluationRepository : IEvaluationRepository
{
    private readonly AppDbContext _context;

    public EvaluationRepository(AppDbContext context)
    {
        _context = context;
    }

     
    public async Task<EvaluationSession> CreateSessionAsync(EvaluationSession session)
    {
        _context.EvaluationSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<EvaluationSession?> GetSessionByIdAsync(int id)
    {
        return await _context.EvaluationSessions
            .Include(s => s.AttackEvaluations)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<(List<EvaluationSession> items, int total)> GetSessionsAsync(int page, int pageSize)
    {
        var query = _context.EvaluationSessions
            .Include(s => s.AttackEvaluations)
            .OrderByDescending(s => s.CreatedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

     
    public async Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request)
    {
        var session = await _context.EvaluationSessions
            .Include(s => s.AttackEvaluations)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId);

        if (session == null)
            return null;

         foreach (var eval in request.AttackEvaluations)
        {
             var existingEval = session.AttackEvaluations
                .FirstOrDefault(e => e.AttackName == eval.AttackName
                                  && e.Pattern == eval.Pattern);

            if (existingEval != null)
            {
                 existingEval.DetectionCorrect = eval.DetectionCorrect;
                existingEval.ClassificationCorrect = eval.ClassificationCorrect;
                existingEval.Notes = eval.Notes;
                existingEval.EvaluatedAt = DateTime.UtcNow;
            }
            else
            {
                 session.AttackEvaluations.Add(new AttackEvaluationRecord
                {
                    EvaluationSessionId = session.Id,
                    AttackName = eval.AttackName,
                    Pattern = eval.Pattern,
                    DetectionCorrect = eval.DetectionCorrect,
                    ClassificationCorrect = eval.ClassificationCorrect,
                    Notes = eval.Notes,
                    EvaluatedAt = DateTime.UtcNow
                });
            }
        }

         session.EvaluatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return session;
    }

     
    public async Task<EvaluationStats> GetStatsAsync()
    {
        var allSessions = await _context.EvaluationSessions.ToListAsync();
        var allEvaluations = await _context.AttackEvaluationRecords.ToListAsync();

        var totalPayloads = allSessions.Count;

         var detectionCorrect = allEvaluations.Count(e => e.DetectionCorrect);
        var detectionIncorrect = allEvaluations.Count(e => !e.DetectionCorrect);

         var classificationCorrect = allEvaluations.Count(e => e.ClassificationCorrect);
        var classificationIncorrect = allEvaluations.Count(e => !e.ClassificationCorrect);

        var totalEvaluations = allEvaluations.Count;

        return new EvaluationStats
        {
            TotalPayloadsTested = totalPayloads,

             DetectionCorrectCount = detectionCorrect,
            DetectionIncorrectCount = detectionIncorrect,
            DetectionAccuracyPercent = totalEvaluations > 0
                ? Math.Round((double)detectionCorrect / totalEvaluations * 100, 2)
                : 0,

             ClassificationCorrectCount = classificationCorrect,
            ClassificationIncorrectCount = classificationIncorrect,
            ClassificationAccuracyPercent = totalEvaluations > 0
                ? Math.Round((double)classificationCorrect / totalEvaluations * 100, 2)
                : 0
        };
    }

    public async Task<List<FailedPayloadEntry>> GetFailedAsync(string type)
    {
         var evaluatedSessions = await _context.EvaluationSessions
            .Include(s => s.AttackEvaluations)
            .Where(s => s.EvaluatedAt != null)
            .ToListAsync();

        var failed = new List<FailedPayloadEntry>();

        foreach (var session in evaluatedSessions)
        {
            foreach (var eval in session.AttackEvaluations)
            {
                bool isFailed = type.ToLower() switch
                {
                    "detection" => !eval.DetectionCorrect,
                    "classification" => !eval.ClassificationCorrect,
                    "all" => !eval.DetectionCorrect || !eval.ClassificationCorrect,
                    _ => false
                };

                if (isFailed)
                {
                    string failureType = (!eval.DetectionCorrect, !eval.ClassificationCorrect) switch
                    {
                        (true, true) => "MissedDetection + WrongClassification",
                        (true, false) => "MissedDetection",
                        (false, true) => "WrongClassification",
                        _ => ""
                    };

                    failed.Add(new FailedPayloadEntry
                    {
                        SessionId = session.Id,
                        Payload = session.Payload,
                        AttackName = eval.AttackName,
                        Pattern = eval.Pattern,
                        FailureType = failureType,
                        Notes = eval.Notes,
                        CreatedAt = session.CreatedAt
                    });
                }
            }
        }

        return failed;
    }
 
    public async Task<List<AttackSummary>> GetAttackSummaryAsync()
    {
        
        var evaluatedAttacks = await _context.AttackEvaluationRecords
            .Include(e => e.EvaluationSession)
            .ToListAsync();

         var attackGroups = evaluatedAttacks
            .GroupBy(e => e.AttackName)
            .Select(g => new
            {
                AttackName = g.Key,
                Evaluations = g.ToList()
            })
            .ToList();

         var summaries = new List<AttackSummary>();

        foreach (var group in attackGroups)
        {
            var evaluatedCount = group.Evaluations.Count;

             var detectionErrors = group.Evaluations.Count(e => !e.DetectionCorrect);
            var classificationErrors = group.Evaluations.Count(e => !e.ClassificationCorrect);

            var detectionErrorRate = evaluatedCount > 0
                ? Math.Round((double)detectionErrors / evaluatedCount * 100, 2)
                : 0;

            var classificationErrorRate = evaluatedCount > 0
                ? Math.Round((double)classificationErrors / evaluatedCount * 100, 2)
                : 0;

             var failedPayloads = group.Evaluations
                .Where(e => !e.DetectionCorrect || !e.ClassificationCorrect)
                .Select(e => new FailedPayloadInfo
                {
                    SessionId = e.EvaluationSession.Id,   
                    Payload = e.EvaluationSession.Payload,
                    Pattern = e.Pattern,
                    DetectionCorrect = e.DetectionCorrect,
                    ClassificationCorrect = e.ClassificationCorrect,
                    AssignedTo = e.EvaluationSession.AssignedTo,
                    Notes = e.Notes,
                    CreatedAt = e.EvaluationSession.CreatedAt
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            summaries.Add(new AttackSummary
            {
                AttackName = group.AttackName,
                EvaluatedCount = evaluatedCount,
                DetectionErrorRate = detectionErrorRate,
                ClassificationErrorRate = classificationErrorRate,
                FailedPayloads = failedPayloads
            });
        }

        return summaries.OrderByDescending(s => s.EvaluatedCount).ToList();
    }
}
