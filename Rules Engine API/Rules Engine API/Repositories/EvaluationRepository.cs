
////////using Microsoft.EntityFrameworkCore;
////////using Rules_Engine_API.Data;
////////using Rules_Engine_API.Models;

////////namespace Rules_Engine_API.Repositories;

////////public class EvaluationRepository : IEvaluationRepository
////////{
////////    private readonly AppDbContext _db;

////////    public EvaluationRepository(AppDbContext db)
////////    {
////////        _db = db;
////////    }

////////    // ─────────────────────────────────────────────────────────────
////////    // Create session after analyze
////////    // ─────────────────────────────────────────────────────────────
////////    public async Task<EvaluationSession> CreateSessionAsync(EvaluationSession session)
////////    {
////////        _db.EvaluationSessions.Add(session);
////////        await _db.SaveChangesAsync();
////////        return session;
////////    }

////////    // ─────────────────────────────────────────────────────────────
////////    // Submit evaluation by pentest engineer
////////    // ─────────────────────────────────────────────────────────────
////////    public async Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request)
////////    {
////////        var session = await _db.EvaluationSessions.FindAsync(request.SessionId);

////////        if (session == null)
////////            return null;

////////        session.DetectionCorrect = request.DetectionCorrect;
////////        session.ClassificationCorrect = request.ClassificationCorrect;
////////        session.Notes = request.Notes;
////////        session.IsEvaluated = true;
////////        session.EvaluatedAt = DateTime.UtcNow;

////////        await _db.SaveChangesAsync();
////////        return session;
////////    }

////////    // ─────────────────────────────────────────────────────────────
////////    // Get single session
////////    // ─────────────────────────────────────────────────────────────
////////    public async Task<EvaluationSession?> GetSessionByIdAsync(int id)
////////        => await _db.EvaluationSessions.FindAsync(id);

////////    // ─────────────────────────────────────────────────────────────
////////    // Get sessions (paginated)
////////    // ─────────────────────────────────────────────────────────────
////////    public async Task<(List<EvaluationSession> Items, int TotalCount)> GetSessionsAsync(
////////        int page,
////////        int pageSize,
////////        bool? evaluatedOnly = null)
////////    {
////////        var query = _db.EvaluationSessions.AsQueryable();

////////        if (evaluatedOnly.HasValue)
////////            query = query.Where(s => s.IsEvaluated == evaluatedOnly.Value);

////////        var total = await query.CountAsync();

////////        var items = await query
////////            .OrderByDescending(s => s.CreatedAt)
////////            .Skip((page - 1) * pageSize)
////////            .Take(pageSize)
////////            .ToListAsync();

////////        return (items, total);
////////    }

////////    // ─────────────────────────────────────────────────────────────
////////    // Stats – numbers only, no payload arrays
////////    // ─────────────────────────────────────────────────────────────
////////    public async Task<EvaluationStats> GetStatsAsync()
////////    {
////////        var all = await _db.EvaluationSessions.ToListAsync();
////////        var evaluated = all.Where(s => s.IsEvaluated).ToList();

////////        int totalEvaluated = evaluated.Count;
////////        int detectionCorrect = evaluated.Count(s => s.DetectionCorrect == true);
////////        int classCorrect = evaluated.Count(s => s.ClassificationCorrect == true);

////////        return new EvaluationStats
////////        {
////////            TotalPayloadsTested = all.Count,
////////            TotalEvaluated = totalEvaluated,
////////            TotalPending = all.Count - totalEvaluated,

////////            DetectionCorrectCount = detectionCorrect,
////////            DetectionIncorrectCount = evaluated.Count(s => s.DetectionCorrect == false),
////////            DetectionAccuracyPercent = totalEvaluated == 0 ? 0
////////                : Math.Round((double)detectionCorrect / totalEvaluated * 100, 1),

////////            ClassificationCorrectCount = classCorrect,
////////            ClassificationIncorrectCount = evaluated.Count(s => s.ClassificationCorrect == false),
////////            ClassificationAccuracyPercent = totalEvaluated == 0 ? 0
////////                : Math.Round((double)classCorrect / totalEvaluated * 100, 1)
////////        };
////////    }

////////    // ─────────────────────────────────────────────────────────────
////////    // Failed payloads – all at once, no pagination
////////    // type: "detection" | "classification" | "all"
////////    // ─────────────────────────────────────────────────────────────
////////    public async Task<List<FailedPayloadEntry>> GetFailedAsync(string type)
////////    {
////////        var evaluated = _db.EvaluationSessions.Where(s => s.IsEvaluated);

////////        IQueryable<EvaluationSession> query = type.ToLower() switch
////////        {
////////            "detection" => evaluated.Where(s => s.DetectionCorrect == false),
////////            "classification" => evaluated.Where(s => s.ClassificationCorrect == false),
////////            _ => evaluated.Where(s => s.DetectionCorrect == false || s.ClassificationCorrect == false)
////////        };

////////        return await query
////////            .OrderByDescending(s => s.CreatedAt)
////////            .Select(s => new FailedPayloadEntry
////////            {
////////                SessionId = s.Id,
////////                Payload = s.Payload,
////////                DetectedAttackType = s.DetectedAttackType,
////////                FailureType = s.DetectionCorrect == false && s.ClassificationCorrect == false
////////                    ? "MissedDetection + WrongClassification"
////////                    : s.DetectionCorrect == false
////////                        ? "MissedDetection"
////////                        : "WrongClassification",
////////                Notes = s.Notes,
////////                CreatedAt = s.CreatedAt
////////            })
////////            .ToListAsync();
////////    }
////////}

////////////////////////////////////////////
/////////

//////using Microsoft.EntityFrameworkCore;
//////using Rules_Engine_API.Data;
//////using Rules_Engine_API.Models;

//////namespace Rules_Engine_API.Repositories;

//////public class EvaluationRepository : IEvaluationRepository
//////{
//////    private readonly AppDbContext _db;

//////    public EvaluationRepository(AppDbContext db)
//////    {
//////        _db = db;
//////    }

//////    // ─────────────────────────────────────────────────────────────
//////    // Create session after analyze
//////    // ─────────────────────────────────────────────────────────────
//////    public async Task<EvaluationSession> CreateSessionAsync(EvaluationSession session)
//////    {
//////        _db.EvaluationSessions.Add(session);
//////        await _db.SaveChangesAsync();
//////        return session;
//////    }

//////    // ─────────────────────────────────────────────────────────────
//////    // Submit per-attack evaluations
//////    // ─────────────────────────────────────────────────────────────
//////    public async Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request)
//////    {
//////        var session = await _db.EvaluationSessions
//////            .Include(s => s.AttackEvaluations)
//////            .FirstOrDefaultAsync(s => s.Id == request.SessionId);

//////        if (session == null)
//////            return null;

//////        // Remove old evaluations if re-submitting
//////        _db.AttackEvaluationRecords.RemoveRange(session.AttackEvaluations);

//////        // Add new per-attack evaluations
//////        foreach (var eval in request.AttackEvaluations)
//////        {
//////            _db.AttackEvaluationRecords.Add(new AttackEvaluationRecord
//////            {
//////                EvaluationSessionId = session.Id,
//////                AttackName = eval.AttackName,
//////                DetectionCorrect = eval.DetectionCorrect,
//////                ClassificationCorrect = eval.ClassificationCorrect,
//////                Notes = eval.Notes,
//////                EvaluatedAt = DateTime.UtcNow
//////            });
//////        }

//////        session.IsEvaluated = true;
//////        session.EvaluatedAt = DateTime.UtcNow;

//////        await _db.SaveChangesAsync();
//////        return session;
//////    }

//////    // ─────────────────────────────────────────────────────────────
//////    // Get single session with its attack evaluations
//////    // ─────────────────────────────────────────────────────────────
//////    public async Task<EvaluationSession?> GetSessionByIdAsync(int id)
//////        => await _db.EvaluationSessions
//////            .Include(s => s.AttackEvaluations)
//////            .FirstOrDefaultAsync(s => s.Id == id);

//////    // ─────────────────────────────────────────────────────────────
//////    // Get sessions (paginated)
//////    // ─────────────────────────────────────────────────────────────
//////    public async Task<(List<EvaluationSession> Items, int TotalCount)> GetSessionsAsync(
//////        int page, int pageSize, bool? evaluatedOnly = null)
//////    {
//////        var query = _db.EvaluationSessions
//////            .Include(s => s.AttackEvaluations)
//////            .AsQueryable();

//////        if (evaluatedOnly.HasValue)
//////            query = query.Where(s => s.IsEvaluated == evaluatedOnly.Value);

//////        var total = await query.CountAsync();

//////        var items = await query
//////            .OrderByDescending(s => s.CreatedAt)
//////            .Skip((page - 1) * pageSize)
//////            .Take(pageSize)
//////            .ToListAsync();

//////        return (items, total);
//////    }

//////    // ─────────────────────────────────────────────────────────────
//////    // Stats – calculated at attack level
//////    // ─────────────────────────────────────────────────────────────
//////    public async Task<EvaluationStats> GetStatsAsync()
//////    {
//////        var totalSessions = await _db.EvaluationSessions.CountAsync();
//////        var evaluatedSessions = await _db.EvaluationSessions.CountAsync(s => s.IsEvaluated);

//////        // Stats are calculated across all attack-level evaluations
//////        var allAttackEvals = await _db.AttackEvaluationRecords.ToListAsync();

//////        int detectionCorrect = allAttackEvals.Count(a => a.DetectionCorrect);
//////        int detectionIncorrect = allAttackEvals.Count(a => !a.DetectionCorrect);
//////        int classCorrect = allAttackEvals.Count(a => a.ClassificationCorrect);
//////        int classIncorrect = allAttackEvals.Count(a => !a.ClassificationCorrect);
//////        int total = allAttackEvals.Count;

//////        return new EvaluationStats
//////        {
//////            TotalPayloadsTested = totalSessions,
//////            TotalEvaluated = evaluatedSessions,
//////            TotalPending = totalSessions - evaluatedSessions,

//////            DetectionCorrectCount = detectionCorrect,
//////            DetectionIncorrectCount = detectionIncorrect,
//////            DetectionAccuracyPercent = total == 0 ? 0
//////                : Math.Round((double)detectionCorrect / total * 100, 1),

//////            ClassificationCorrectCount = classCorrect,
//////            ClassificationIncorrectCount = classIncorrect,
//////            ClassificationAccuracyPercent = total == 0 ? 0
//////                : Math.Round((double)classCorrect / total * 100, 1)
//////        };
//////    }

//////    // ─────────────────────────────────────────────────────────────
//////    // Failed attacks – all at once, no pagination
//////    // ─────────────────────────────────────────────────────────────
//////    public async Task<List<FailedPayloadEntry>> GetFailedAsync(string type)
//////    {
//////        var query = _db.AttackEvaluationRecords
//////            .Include(a => a.EvaluationSession)
//////            .AsQueryable();

//////        query = type.ToLower() switch
//////        {
//////            "detection" => query.Where(a => !a.DetectionCorrect),
//////            "classification" => query.Where(a => !a.ClassificationCorrect),
//////            _ => query.Where(a => !a.DetectionCorrect || !a.ClassificationCorrect)
//////        };

//////        return await query
//////            .OrderByDescending(a => a.EvaluatedAt)
//////            .Select(a => new FailedPayloadEntry
//////            {
//////                SessionId = a.EvaluationSessionId,
//////                Payload = a.EvaluationSession.Payload,
//////                AttackName = a.AttackName,
//////                FailureType = !a.DetectionCorrect && !a.ClassificationCorrect
//////                    ? "MissedDetection + WrongClassification"
//////                    : !a.DetectionCorrect
//////                        ? "MissedDetection"
//////                        : "WrongClassification",
//////                Notes = a.Notes,
//////                CreatedAt = a.EvaluationSession.CreatedAt
//////            })
//////            .ToListAsync();
//////    }
//////}

////////////////////////////////////////
///////
////using Microsoft.EntityFrameworkCore;
////using Rules_Engine_API.Data;
////using Rules_Engine_API.Models;
////namespace Rules_Engine_API.Repositories;

////public class EvaluationRepository : IEvaluationRepository
////{
////    private readonly AppDbContext _db;

////    public EvaluationRepository(AppDbContext db)
////    {
////        _db = db;
////    }

////    // ─────────────────────────────────────────────────────────────
////    // Create session after analyze
////    // ─────────────────────────────────────────────────────────────
////    public async Task<EvaluationSession> CreateSessionAsync(EvaluationSession session)
////    {
////        _db.EvaluationSessions.Add(session);
////        await _db.SaveChangesAsync();
////        return session;
////    }

////    // ─────────────────────────────────────────────────────────────
////    // Submit per-attack evaluations
////    // ─────────────────────────────────────────────────────────────
////    public async Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request)
////    {
////        var session = await _db.EvaluationSessions
////            .Include(s => s.AttackEvaluations)
////            .FirstOrDefaultAsync(s => s.Id == request.SessionId);

////        if (session == null)
////            return null;

////        foreach (var eval in request.AttackEvaluations)
////        {
////            var existingEval = session.AttackEvaluations
////                .FirstOrDefault(e => e.AttackName == eval.AttackName
////                                  && e.Pattern == eval.Pattern);

////            if (existingEval != null)
////            {
////                // UPDATE
////                existingEval.DetectionCorrect = eval.DetectionCorrect;
////                existingEval.ClassificationCorrect = eval.ClassificationCorrect;
////                existingEval.Notes = eval.Notes;
////                existingEval.EvaluatedAt = DateTime.UtcNow;
////            }
////            else
////            {
////                // INSERT
////                session.AttackEvaluations.Add(new AttackEvaluationRecord
////                {
////                    EvaluationSessionId = session.Id,
////                    AttackName = eval.AttackName,
////                    Pattern = eval.Pattern, // ✅ الجديد
////                    DetectionCorrect = eval.DetectionCorrect,
////                    ClassificationCorrect = eval.ClassificationCorrect,
////                    Notes = eval.Notes,
////                    EvaluatedAt = DateTime.UtcNow
////                });
////            }
////        }

////        // حساب إذا كل الاتاكس اتقيمت
////        var matchDetails = ParseMatchDetails(session.MatchedAttacksJson);
////        var totalAttacks = matchDetails.Count;
////        var evaluatedCount = session.AttackEvaluations.Count;

////        session.IsEvaluated = (evaluatedCount >= totalAttacks);
////        session.EvaluatedAt = DateTime.UtcNow;

////        await _db.SaveChangesAsync();
////        return session;
////    }

////    private static List<MatchDetail> ParseMatchDetails(string? json)
////    {
////        if (string.IsNullOrWhiteSpace(json))
////            return new();

////        try
////        {
////            return System.Text.Json.JsonSerializer.Deserialize<List<MatchDetail>>(json) ?? new();
////        }
////        catch
////        {
////            return new();
////        }
////    }

////    public class MatchDetail
////    {
////        public string AttackName { get; set; }
////        public string Pattern { get; set; }
////        public int Score { get; set; }
////    }
////    // ─────────────────────────────────────────────────────────────
////    // Get single session with its attack evaluations
////    // ─────────────────────────────────────────────────────────────
////    public async Task<EvaluationSession?> GetSessionByIdAsync(int id)
////        => await _db.EvaluationSessions
////            .Include(s => s.AttackEvaluations)
////            .FirstOrDefaultAsync(s => s.Id == id);

////    // ─────────────────────────────────────────────────────────────
////    // Get sessions (paginated)
////    // ─────────────────────────────────────────────────────────────
////    public async Task<(List<EvaluationSession> Items, int TotalCount)> GetSessionsAsync(int page, int pageSize)
////    {
////        var query = _db.EvaluationSessions
////            .Include(s => s.AttackEvaluations)
////            .AsQueryable();

////        var total = await query.CountAsync();

////        var items = await query
////            .OrderByDescending(s => s.CreatedAt)
////            .Skip((page - 1) * pageSize)
////            .Take(pageSize)
////            .ToListAsync();

////        return (items, total);
////    }

////    // ─────────────────────────────────────────────────────────────
////    // Stats – calculated at attack level
////    // ─────────────────────────────────────────────────────────────
////    public async Task<EvaluationStats> GetStatsAsync()
////    {
////        var totalSessions = await _db.EvaluationSessions.CountAsync();
////        var evaluatedSessions = await _db.EvaluationSessions.CountAsync(s => s.IsEvaluated);

////        // Stats are calculated across all attack-level evaluations
////        var allAttackEvals = await _db.AttackEvaluationRecords.ToListAsync();

////        int detectionCorrect = allAttackEvals.Count(a => a.DetectionCorrect);
////        int detectionIncorrect = allAttackEvals.Count(a => !a.DetectionCorrect);
////        int classCorrect = allAttackEvals.Count(a => a.ClassificationCorrect);
////        int classIncorrect = allAttackEvals.Count(a => !a.ClassificationCorrect);
////        int total = allAttackEvals.Count;

////        return new EvaluationStats
////        {
////            TotalPayloadsTested = totalSessions,
////            TotalEvaluated = evaluatedSessions,
////            TotalPending = totalSessions - evaluatedSessions,

////            DetectionCorrectCount = detectionCorrect,
////            DetectionIncorrectCount = detectionIncorrect,
////            DetectionAccuracyPercent = total == 0 ? 0
////                : Math.Round((double)detectionCorrect / total * 100, 1),

////            ClassificationCorrectCount = classCorrect,
////            ClassificationIncorrectCount = classIncorrect,
////            ClassificationAccuracyPercent = total == 0 ? 0
////                : Math.Round((double)classCorrect / total * 100, 1)
////        };
////    }

////    // ─────────────────────────────────────────────────────────────
////    // Failed attacks – all at once, no pagination
////    // ─────────────────────────────────────────────────────────────
////    public async Task<List<FailedPayloadEntry>> GetFailedAsync(string type)
////    {
////        var evaluatedSessions = await _db.EvaluationSessions
////            .Include(s => s.AttackEvaluations)
////            .Where(s => s.IsEvaluated)
////            .ToListAsync();

////        var failed = new List<FailedPayloadEntry>();

////        foreach (var session in evaluatedSessions)
////        {
////            foreach (var eval in session.AttackEvaluations)
////            {
////                bool isFailed = type.ToLower() switch
////                {
////                    "detection" => !eval.DetectionCorrect,
////                    "classification" => !eval.ClassificationCorrect,
////                    "all" => !eval.DetectionCorrect || !eval.ClassificationCorrect,
////                    _ => false
////                };

////                if (isFailed)
////                {
////                    string failureType = (!eval.DetectionCorrect, !eval.ClassificationCorrect) switch
////                    {
////                        (true, true) => "MissedDetection + WrongClassification",
////                        (true, false) => "MissedDetection",
////                        (false, true) => "WrongClassification",
////                        _ => ""
////                    };

////                    failed.Add(new FailedPayloadEntry
////                    {
////                        SessionId = session.Id,
////                        Payload = session.Payload,
////                        AttackName = eval.AttackName,
////                        Pattern = eval.Pattern, // ✅ الجديد
////                        FailureType = failureType,
////                        Notes = eval.Notes,
////                        CreatedAt = session.CreatedAt
////                    });
////                }
////            }
////        }

////        return failed;
////    }
////}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////





//using System.Text.Json;
//using Microsoft.EntityFrameworkCore;
//using Rules_Engine_API.Data;
//using Rules_Engine_API.Models;

//namespace Rules_Engine_API.Repositories;

//public class EvaluationRepository : IEvaluationRepository
//{
//    private readonly AppDbContext _context;

//    public EvaluationRepository(AppDbContext context)
//    {
//        _context = context;
//    }

//    // ─────────────────────────────────────────────────────────────
//    // Session Management
//    // ─────────────────────────────────────────────────────────────

//    public async Task<EvaluationSession> CreateSessionAsync(EvaluationSession session)
//    {
//        _context.EvaluationSessions.Add(session);
//        await _context.SaveChangesAsync();
//        return session;
//    }

//    public async Task<EvaluationSession?> GetSessionByIdAsync(int id)
//    {
//        return await _context.EvaluationSessions
//            .Include(s => s.AttackEvaluations)
//            .FirstOrDefaultAsync(s => s.Id == id);
//    }

//    public async Task<(List<EvaluationSession> items, int total)> GetSessionsAsync(int page, int pageSize)
//    {
//        var query = _context.EvaluationSessions
//            .Include(s => s.AttackEvaluations)
//            .OrderByDescending(s => s.CreatedAt);

//        var total = await query.CountAsync();
//        var items = await query
//            .Skip((page - 1) * pageSize)
//            .Take(pageSize)
//            .ToListAsync();

//        return (items, total);
//    }

//    // ─────────────────────────────────────────────────────────────
//    // Evaluation Submission
//    // ─────────────────────────────────────────────────────────────

//    public async Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request)
//    {
//        var session = await _context.EvaluationSessions
//            .Include(s => s.AttackEvaluations)
//            .FirstOrDefaultAsync(s => s.Id == request.SessionId);

//        if (session == null)
//            return null;

//        foreach (var eval in request.AttackEvaluations)
//        {
//            // Check if this specific attack+pattern combo was already evaluated
//            var existingEval = session.AttackEvaluations
//                .FirstOrDefault(e => e.AttackName == eval.AttackName
//                                  && e.Pattern == eval.Pattern);

//            if (existingEval != null)
//            {
//                // Update existing
//                existingEval.DetectionCorrect = eval.DetectionCorrect;
//                existingEval.ClassificationCorrect = eval.ClassificationCorrect;
//                existingEval.Notes = eval.Notes;
//                existingEval.EvaluatedAt = DateTime.UtcNow;
//            }
//            else
//            {
//                // Create new
//                session.AttackEvaluations.Add(new AttackEvaluationRecord
//                {
//                    EvaluationSessionId = session.Id,
//                    AttackName = eval.AttackName,
//                    Pattern = eval.Pattern,
//                    DetectionCorrect = eval.DetectionCorrect,
//                    ClassificationCorrect = eval.ClassificationCorrect,
//                    Notes = eval.Notes,
//                    EvaluatedAt = DateTime.UtcNow
//                });
//            }
//        }

//        // Update session's EvaluatedAt timestamp
//        session.EvaluatedAt = DateTime.UtcNow;

//        await _context.SaveChangesAsync();
//        return session;
//    }

//    // ─────────────────────────────────────────────────────────────
//    // Statistics & Reporting
//    // ─────────────────────────────────────────────────────────────

//    public async Task<EvaluationStats> GetStatsAsync()
//    {
//        var allSessions = await _context.EvaluationSessions.ToListAsync();
//        var allEvaluations = await _context.AttackEvaluationRecords.ToListAsync();

//        var totalPayloads = allSessions.Count;

//        // Count sessions that have at least one evaluation
//        var evaluatedSessions = allSessions.Count(s => s.EvaluatedAt != null);
//        var pendingSessions = totalPayloads - evaluatedSessions;

//        // ✅ VERIFIED: Error rate calculations
//        // Detection errors = count where DetectionCorrect is FALSE
//        var detectionErrors = allEvaluations.Count(e => !e.DetectionCorrect);
//        var detectionCorrect = allEvaluations.Count(e => e.DetectionCorrect);

//        // Classification errors = count where ClassificationCorrect is FALSE
//        var classificationErrors = allEvaluations.Count(e => !e.ClassificationCorrect);
//        var classificationCorrect = allEvaluations.Count(e => e.ClassificationCorrect);

//        var totalEvaluations = allEvaluations.Count;

//        return new EvaluationStats
//        {
//            TotalPayloadsTested = totalPayloads,
//            TotalEvaluated = evaluatedSessions,
//            TotalPending = pendingSessions,

//            // ✅ Detection stats
//            DetectionCorrectCount = detectionCorrect,
//            DetectionIncorrectCount = detectionErrors,
//            DetectionAccuracyPercent = totalEvaluations > 0
//                ? Math.Round((double)detectionCorrect / totalEvaluations * 100, 2)
//                : 0,

//            // ✅ Classification stats
//            ClassificationCorrectCount = classificationCorrect,
//            ClassificationIncorrectCount = classificationErrors,
//            ClassificationAccuracyPercent = totalEvaluations > 0
//                ? Math.Round((double)classificationCorrect / totalEvaluations * 100, 2)
//                : 0
//        };
//    }

//    public async Task<List<FailedPayloadEntry>> GetFailedAsync(string type)
//    {
//        // Get all sessions that have at least one evaluation
//        var evaluatedSessions = await _context.EvaluationSessions
//            .Include(s => s.AttackEvaluations)
//            .Where(s => s.EvaluatedAt != null)
//            .ToListAsync();

//        var failed = new List<FailedPayloadEntry>();

//        foreach (var session in evaluatedSessions)
//        {
//            foreach (var eval in session.AttackEvaluations)
//            {
//                bool isFailed = type.ToLower() switch
//                {
//                    "detection" => !eval.DetectionCorrect,
//                    "classification" => !eval.ClassificationCorrect,
//                    "all" => !eval.DetectionCorrect || !eval.ClassificationCorrect,
//                    _ => false
//                };

//                if (isFailed)
//                {
//                    string failureType = (!eval.DetectionCorrect, !eval.ClassificationCorrect) switch
//                    {
//                        (true, true) => "MissedDetection + WrongClassification",
//                        (true, false) => "MissedDetection",
//                        (false, true) => "WrongClassification",
//                        _ => ""
//                    };

//                    failed.Add(new FailedPayloadEntry
//                    {
//                        SessionId = session.Id,
//                        Payload = session.Payload,
//                        AttackName = eval.AttackName,
//                        Pattern = eval.Pattern,
//                        FailureType = failureType,
//                        Notes = eval.Notes,
//                        CreatedAt = session.CreatedAt
//                    });
//                }
//            }
//        }

//        return failed;
//    }

//    // ─────────────────────────────────────────────────────────────
//    // Attack Summary with Error Rates
//    // ─────────────────────────────────────────────────────────────

//    public async Task<List<AttackSummary>> GetAttackSummaryAsync()
//    {
//        // Get all evaluated attacks with session info
//        var evaluatedAttacks = await _context.AttackEvaluationRecords
//            .Include(e => e.EvaluationSession)
//            .ToListAsync();

//        // Get all sessions without evaluations to count pending attacks
//        var pendingSessions = await _context.EvaluationSessions
//            .Where(s => s.EvaluatedAt == null)
//            .ToListAsync();

//        // Count pending attacks by name
//        var pendingAttackCounts = new Dictionary<string, int>();
//        foreach (var session in pendingSessions)
//        {
//            var matches = ParseMatchDetails(session.MatchedAttacksJson);
//            foreach (var match in matches)
//            {
//                if (!pendingAttackCounts.ContainsKey(match.AttackName))
//                    pendingAttackCounts[match.AttackName] = 0;
//                pendingAttackCounts[match.AttackName]++;
//            }
//        }

//        // Group evaluated attacks by attack name
//        var attackGroups = evaluatedAttacks
//            .GroupBy(e => e.AttackName)
//            .Select(g => new
//            {
//                AttackName = g.Key,
//                Evaluations = g.ToList()
//            })
//            .ToList();

//        // Build summary for each attack
//        var summaries = new List<AttackSummary>();

//        foreach (var group in attackGroups)
//        {
//            var evaluatedCount = group.Evaluations.Count;
//            var pendingCount = pendingAttackCounts.GetValueOrDefault(group.AttackName, 0);
//            var totalOccurrences = evaluatedCount + pendingCount;

//            // ✅ VERIFIED ERROR RATE CALCULATIONS:
//            // Error = count where Correct is FALSE
//            var detectionErrors = group.Evaluations.Count(e => !e.DetectionCorrect);
//            var classificationErrors = group.Evaluations.Count(e => !e.ClassificationCorrect);

//            // Error Rate = (errors / total evaluated) × 100
//            var detectionErrorRate = evaluatedCount > 0
//                ? Math.Round((double)detectionErrors / evaluatedCount * 100, 2)
//                : 0;

//            var classificationErrorRate = evaluatedCount > 0
//                ? Math.Round((double)classificationErrors / evaluatedCount * 100, 2)
//                : 0;

//            // Get failed payloads (where detection OR classification is wrong)
//            var failedPayloads = group.Evaluations
//                .Where(e => !e.DetectionCorrect || !e.ClassificationCorrect)
//                .Select(e => new FailedPayloadInfo
//                {
//                    SessionId = e.EvaluationSession.Id,
//                    Payload = e.EvaluationSession.Payload,
//                    Pattern = e.Pattern,
//                    DetectionCorrect = e.DetectionCorrect,
//                    ClassificationCorrect = e.ClassificationCorrect,
//                    AssignedTo = e.EvaluationSession.AssignedTo,
//                    Notes = e.Notes,
//                    CreatedAt = e.EvaluationSession.CreatedAt
//                })
//                .OrderByDescending(p => p.CreatedAt)
//                .ToList();

//            summaries.Add(new AttackSummary
//            {
//                AttackName = group.AttackName,
//                TotalOccurrences = totalOccurrences,
//                EvaluatedCount = evaluatedCount,
//                PendingCount = pendingCount,
//                DetectionErrorRate = detectionErrorRate,
//                ClassificationErrorRate = classificationErrorRate,
//                FailedPayloads = failedPayloads
//            });
//        }

//        // Add attacks that only exist in pending sessions
//        foreach (var attackName in pendingAttackCounts.Keys)
//        {
//            if (!summaries.Any(s => s.AttackName == attackName))
//            {
//                summaries.Add(new AttackSummary
//                {
//                    AttackName = attackName,
//                    TotalOccurrences = pendingAttackCounts[attackName],
//                    EvaluatedCount = 0,
//                    PendingCount = pendingAttackCounts[attackName],
//                    DetectionErrorRate = 0,
//                    ClassificationErrorRate = 0,
//                    FailedPayloads = new List<FailedPayloadInfo>()
//                });
//            }
//        }

//        return summaries.OrderByDescending(s => s.TotalOccurrences).ToList();
//    }

//    // ─────────────────────────────────────────────────────────────
//    // Helper Methods
//    // ─────────────────────────────────────────────────────────────

//    private static List<MatchDetail> ParseMatchDetails(string? json)
//    {
//        if (string.IsNullOrWhiteSpace(json))
//            return new();

//        try
//        {
//            return JsonSerializer.Deserialize<List<MatchDetail>>(json) ?? new();
//        }
//        catch
//        {
//            return new();
//        }
//    }
//}

//// ─────────────────────────────────────────────────────────────
//// Helper class for deserializing match details
//// ─────────────────────────────────────────────────────────────
//public class MatchDetail
//{
//    public string AttackName { get; set; }
//    public string Pattern { get; set; }
//    public int Score { get; set; }
//}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////
///// /////////
///

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

    // ─────────────────────────────────────────────────────────────
    // Session Management
    // ─────────────────────────────────────────────────────────────

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

    // ─────────────────────────────────────────────────────────────
    // Evaluation Submission
    // ─────────────────────────────────────────────────────────────

    public async Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request)
    {
        var session = await _context.EvaluationSessions
            .Include(s => s.AttackEvaluations)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId);

        if (session == null)
            return null;

        // Pentester evaluates each attack: DetectionCorrect, ClassificationCorrect, Notes
        foreach (var eval in request.AttackEvaluations)
        {
            // Check if this specific attack+pattern combo was already evaluated
            var existingEval = session.AttackEvaluations
                .FirstOrDefault(e => e.AttackName == eval.AttackName
                                  && e.Pattern == eval.Pattern);

            if (existingEval != null)
            {
                // Update existing evaluation
                existingEval.DetectionCorrect = eval.DetectionCorrect;
                existingEval.ClassificationCorrect = eval.ClassificationCorrect;
                existingEval.Notes = eval.Notes;
                existingEval.EvaluatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new evaluation record
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

        // Update session timestamp
        session.EvaluatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return session;
    }

    // ─────────────────────────────────────────────────────────────
    // Statistics & Reporting
    // ─────────────────────────────────────────────────────────────

    public async Task<EvaluationStats> GetStatsAsync()
    {
        var allSessions = await _context.EvaluationSessions.ToListAsync();
        var allEvaluations = await _context.AttackEvaluationRecords.ToListAsync();

        var totalPayloads = allSessions.Count;

        // Detection stats: count correct/incorrect
        var detectionCorrect = allEvaluations.Count(e => e.DetectionCorrect);
        var detectionIncorrect = allEvaluations.Count(e => !e.DetectionCorrect);

        // Classification stats: count correct/incorrect
        var classificationCorrect = allEvaluations.Count(e => e.ClassificationCorrect);
        var classificationIncorrect = allEvaluations.Count(e => !e.ClassificationCorrect);

        var totalEvaluations = allEvaluations.Count;

        return new EvaluationStats
        {
            TotalPayloadsTested = totalPayloads,

            // Detection stats
            DetectionCorrectCount = detectionCorrect,
            DetectionIncorrectCount = detectionIncorrect,
            DetectionAccuracyPercent = totalEvaluations > 0
                ? Math.Round((double)detectionCorrect / totalEvaluations * 100, 2)
                : 0,

            // Classification stats
            ClassificationCorrectCount = classificationCorrect,
            ClassificationIncorrectCount = classificationIncorrect,
            ClassificationAccuracyPercent = totalEvaluations > 0
                ? Math.Round((double)classificationCorrect / totalEvaluations * 100, 2)
                : 0
        };
    }

    public async Task<List<FailedPayloadEntry>> GetFailedAsync(string type)
    {
        // Get all sessions that have at least one evaluation
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

    // ─────────────────────────────────────────────────────────────
    // ✅ SIMPLIFIED: Attack Summary - ONLY Evaluated Attacks
    // No pending count, no totalOccurrences - just what's been evaluated
    // ─────────────────────────────────────────────────────────────

    public async Task<List<AttackSummary>> GetAttackSummaryAsync()
    {
        // Get all evaluated attacks with session info
        // ✅ This is a SINGLE database query with JOIN
        var evaluatedAttacks = await _context.AttackEvaluationRecords
            .Include(e => e.EvaluationSession)
            .ToListAsync();

        // ✅ Group by attack name (in-memory grouping - fast!)
        var attackGroups = evaluatedAttacks
            .GroupBy(e => e.AttackName)
            .Select(g => new
            {
                AttackName = g.Key,
                Evaluations = g.ToList()
            })
            .ToList();

        // Build summary for each attack
        var summaries = new List<AttackSummary>();

        foreach (var group in attackGroups)
        {
            var evaluatedCount = group.Evaluations.Count;

            // ✅ Calculate error rates (count where Correct = FALSE)
            var detectionErrors = group.Evaluations.Count(e => !e.DetectionCorrect);
            var classificationErrors = group.Evaluations.Count(e => !e.ClassificationCorrect);

            var detectionErrorRate = evaluatedCount > 0
                ? Math.Round((double)detectionErrors / evaluatedCount * 100, 2)
                : 0;

            var classificationErrorRate = evaluatedCount > 0
                ? Math.Round((double)classificationErrors / evaluatedCount * 100, 2)
                : 0;

            // Get failed payloads (where detection OR classification is wrong)
            var failedPayloads = group.Evaluations
                .Where(e => !e.DetectionCorrect || !e.ClassificationCorrect)
                .Select(e => new FailedPayloadInfo
                {
                    SessionId = e.EvaluationSession.Id,  // ✅ Kept for navigation if needed
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