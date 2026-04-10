//namespace Rules_Engine_API.Models;

///// <summary>
///// Request body sent by pentest engineer to submit evaluation for a session
///// </summary>
//public class SubmitEvaluationRequest
//{
//    /// <summary>ID of the EvaluationSession to evaluate</summary>
//    public int SessionId { get; set; }

//    /// <summary>Did the system correctly detect an attack? true = yes, false = missed / false positive</summary>
//    public bool DetectionCorrect { get; set; }

//    /// <summary>Did the system correctly classify the attack type?</summary>
//    public bool ClassificationCorrect { get; set; }

//    /// <summary>Optional notes from the engineer</summary>
//    public string? Notes { get; set; }
//}

///////////////////////////////////////////////////////
///

namespace Rules_Engine_API.Models;

/// <summary>
/// Request body sent by pentest engineer to submit evaluation
/// Each attack in the session is evaluated separately
/// </summary>
public class SubmitEvaluationRequest
{
    public int SessionId { get; set; }
    public List<AttackEvaluation> AttackEvaluations { get; set; } = new();
}

public class AttackEvaluation
{
    public string AttackName { get; set; }

    // ✅ NEW: Pattern to differentiate between same attack names
    public string Pattern { get; set; }
    public bool DetectionCorrect { get; set; }
    public bool ClassificationCorrect { get; set; }
    public string? Notes { get; set; }
}