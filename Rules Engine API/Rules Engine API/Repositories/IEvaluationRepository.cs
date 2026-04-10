 

using Rules_Engine_API.Models;

namespace Rules_Engine_API.Repositories;

public interface IEvaluationRepository
{
     Task<EvaluationSession> CreateSessionAsync(EvaluationSession session);
    Task<EvaluationSession?> GetSessionByIdAsync(int id);
    Task<(List<EvaluationSession> items, int total)> GetSessionsAsync(int page, int pageSize);

     Task<EvaluationSession?> SubmitEvaluationAsync(SubmitEvaluationRequest request);

     Task<EvaluationStats> GetStatsAsync();
    Task<List<FailedPayloadEntry>> GetFailedAsync(string type);

     Task<List<AttackSummary>> GetAttackSummaryAsync();
}
