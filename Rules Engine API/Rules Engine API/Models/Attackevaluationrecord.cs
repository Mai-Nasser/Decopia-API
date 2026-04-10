using System.ComponentModel.DataAnnotations.Schema;

namespace Rules_Engine_API.Models;

/// <summary>
/// Stores the evaluation result for a single attack within a session
/// </summary>
public class AttackEvaluationRecord
{
    public int Id { get; set; }

    // Link to parent session
    public int EvaluationSessionId { get; set; }
    public EvaluationSession EvaluationSession { get; set; }

    // Which attack was evaluated
    public string AttackName { get; set; }


    // ✅ NEW: Pattern that matched (to differentiate between same attack names)
    [Column(TypeName = "nvarchar(max)")]
    public string Pattern { get; set; }

    // Pentest engineer verdict
    public bool DetectionCorrect { get; set; }
    public bool ClassificationCorrect { get; set; }
    public string? Notes { get; set; }

    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}