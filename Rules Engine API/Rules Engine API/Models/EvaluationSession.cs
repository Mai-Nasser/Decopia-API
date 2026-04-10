//namespace Rules_Engine_API.Models;

///// <summary>
///// Represents a single payload test + pentest engineer evaluation
///// </summary>
//public class EvaluationSession
//{
//    public int Id { get; set; }

//    // ─── Payload & Detection ───────────────────────────────────────
//    public string Payload { get; set; }
//    public string SourceIp { get; set; }
//    public string? SessionId { get; set; }
//    public string? UserAgent { get; set; }

//    // Attack info returned by the engine
//    public string DetectedAttackType { get; set; }   // e.g. "SQL Injection + XSS"
//    public string MatchedAttacksJson { get; set; }   // JSON array stored as string

//    // ─── Pentest Engineer Evaluation ──────────────────────────────
//    /// <summary>Did the system correctly DETECT that there was an attack?</summary>
//    public bool? DetectionCorrect { get; set; }

//    /// <summary>Did the system correctly CLASSIFY the attack type?</summary>
//    public bool? ClassificationCorrect { get; set; }

//    /// <summary>Free-text notes from the pentest engineer</summary>
//    public string? Notes { get; set; }

//    /// <summary>True = evaluation has been submitted by engineer</summary>
//    public bool IsEvaluated { get; set; } = false;

//    // ─── Timestamps ───────────────────────────────────────────────
//    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//    public DateTime? EvaluatedAt { get; set; }
//}
///////////////////////////////////
///


namespace Rules_Engine_API.Models;

public class EvaluationSession
{
    public int Id { get; set; }

    // ─── Payload & Detection ───────────────────────────────────────
    public string Payload { get; set; }
    public string SourceIp { get; set; }
    public string? AssignedTo { get; set; }   // email of the pentest engineer
    public string? UserAgent { get; set; }

    // Attack info returned by the engine
    public string DetectedAttackType { get; set; }
    public string MatchedAttacksJson { get; set; }

    // ─── Evaluation Status ────────────────────────────────────────
    //public bool IsEvaluated { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EvaluatedAt { get; set; }

    // ─── Per-Attack Evaluations ───────────────────────────────────
    public List<AttackEvaluationRecord> AttackEvaluations { get; set; } = new();
}