using System.ComponentModel.DataAnnotations.Schema;

namespace Rules_Engine_API.Models;

 
public class AttackEvaluationRecord
{
    public int Id { get; set; }

     public int EvaluationSessionId { get; set; }
    public EvaluationSession EvaluationSession { get; set; }

     public string AttackName { get; set; }


     [Column(TypeName = "nvarchar(max)")]
    public string Pattern { get; set; }

     public bool DetectionCorrect { get; set; }
    public bool ClassificationCorrect { get; set; }
    public string? Notes { get; set; }

    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}
