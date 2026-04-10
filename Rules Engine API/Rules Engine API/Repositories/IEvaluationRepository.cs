
//using Rules_Engine_API.Models;

//namespace Rules_Engine_API.Repositories;

//public interface IEvaluationRepository
//{
//    /// <summary>Save a new evaluation session (after analyze is called)</summary>
//    Task<EvaluationSession> CreateSessionAsync(EvaluationSession session);

//    /// <summary>Submit evaluation by pentest engineer</summary>
//    Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request);

//    /// <summary>Get a single session by ID</summary>
//    Task<EvaluationSession?> GetSessionByIdAsync(int id);

//    /// <summary>Get all sessions (paginated, optional filter by evaluated status)</summary>
//    Task<(List<EvaluationSession> Items, int TotalCount)> GetSessionsAsync(
//        int page,
//        int pageSize,
//        bool? evaluatedOnly = null
//    );

//    /// <summary>Aggregated statistics only (no payload arrays)</summary>
//    Task<EvaluationStats> GetStatsAsync();

//    /// <summary>Failed payloads – all returned at once, no pagination</summary>
//    Task<List<FailedPayloadEntry>> GetFailedAsync(string type);  // "detection" | "classification" | "all"
//}
////////////////////////////////////////////////////
/////

//using Rules_Engine_API.Models;

//namespace Rules_Engine_API.Repositories;

//public interface IEvaluationRepository
//{
//    Task<EvaluationSession> CreateSessionAsync(EvaluationSession session);
//    Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request);
//    Task<EvaluationSession?> GetSessionByIdAsync(int id);
//    Task<(List<EvaluationSession> Items, int TotalCount)> GetSessionsAsync(
//        int page, int pageSize, bool? evaluatedOnly = null);
//    Task<EvaluationStats> GetStatsAsync();
//    Task<List<FailedPayloadEntry>> GetFailedAsync(string type);
//}


///////////////////
///


//using Rules_Engine_API.Models;

//namespace Rules_Engine_API.Repositories;

//public interface IEvaluationRepository
//{
//    Task<EvaluationSession> CreateSessionAsync(EvaluationSession session);
//    Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request);
//    Task<EvaluationSession?> GetSessionByIdAsync(int id);
//    Task<(List<EvaluationSession> Items, int TotalCount)> GetSessionsAsync(int page, int pageSize);
//    Task<EvaluationStats> GetStatsAsync();
//    Task<List<FailedPayloadEntry>> GetFailedAsync(string type);
//}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using Rules_Engine_API.Models;

namespace Rules_Engine_API.Repositories;

public interface IEvaluationRepository
{
    // ─── Session Management ──────────────────────────────────────
    Task<EvaluationSession> CreateSessionAsync(EvaluationSession session);
    Task<EvaluationSession?> GetSessionByIdAsync(int id);
    Task<(List<EvaluationSession> items, int total)> GetSessionsAsync(int page, int pageSize);

    // ─── Evaluation Submission ───────────────────────────────────
    Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request);

    // ─── Statistics & Reporting ──────────────────────────────────
    Task<EvaluationStats> GetStatsAsync();
    Task<List<FailedPayloadEntry>> GetFailedAsync(string type);

    // ✅ NEW: Attack-level summary with error rates
    Task<List<AttackSummary>> GetAttackSummaryAsync();
}