 namespace Rules_Engine_API.Models;

public class EvaluationSession
{
    public int Id { get; set; }

     public string Payload { get; set; }
    public string SourceIp { get; set; }
    public string? AssignedTo { get; set; }    
    public string? UserAgent { get; set; }

     public string DetectedAttackType { get; set; }
    public string MatchedAttacksJson { get; set; }

     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EvaluatedAt { get; set; }

     public List<AttackEvaluationRecord> AttackEvaluations { get; set; } = new();
}
