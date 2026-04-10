
////namespace Rules_Engine_API.Models;

////public class EvaluationStats
////{
////    // ─── Overall Counts ───────────────────────────────────────────
////    public int TotalPayloadsTested { get; set; }
////    public int TotalEvaluated { get; set; }
////    public int TotalPending { get; set; }

////    // ─── Detection Stats (per attack level) ──────────────────────
////    public int DetectionCorrectCount { get; set; }
////    public int DetectionIncorrectCount { get; set; }
////    public double DetectionAccuracyPercent { get; set; }

////    // ─── Classification Stats (per attack level) ──────────────────
////    public int ClassificationCorrectCount { get; set; }
////    public int ClassificationIncorrectCount { get; set; }
////    public double ClassificationAccuracyPercent { get; set; }
////}

/////// <summary>
/////// One failed attack entry – used in /api/evaluation/failed
/////// </summary>
////public class FailedPayloadEntry
////{
////    public int SessionId { get; set; }
////    public string Payload { get; set; }
////    public string AttackName { get; set; }
////    public string FailureType { get; set; }  // "MissedDetection" | "WrongClassification" | "MissedDetection + WrongClassification"
////    public string? Notes { get; set; }
////    public DateTime CreatedAt { get; set; }
////}
/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
/////
//namespace Rules_Engine_API.Models;

//public class EvaluationStats
//{
//    // ─── Overall Counts ───────────────────────────────────────────
//    public int TotalPayloadsTested { get; set; }
//    public int TotalEvaluated { get; set; }
//    public int TotalPending { get; set; }

//    // ─── Detection Stats (per attack level) ──────────────────────
//    public int DetectionCorrectCount { get; set; }
//    public int DetectionIncorrectCount { get; set; }
//    public double DetectionAccuracyPercent { get; set; }

//    // ─── Classification Stats (per attack level) ──────────────────
//    public int ClassificationCorrectCount { get; set; }
//    public int ClassificationIncorrectCount { get; set; }
//    public double ClassificationAccuracyPercent { get; set; }
//}

///// <summary>
///// One failed attack entry – used in /api/evaluation/failed
///// </summary>
//public class FailedPayloadEntry
//{
//    public int SessionId { get; set; }
//    public string Payload { get; set; }
//    public string AttackName { get; set; }
//    public string Pattern { get; set; }           // ✅ NEW: To differentiate same attack names
//    public string FailureType { get; set; }       // "MissedDetection" | "WrongClassification" | "Both"
//    public string? Notes { get; set; }
//    public DateTime CreatedAt { get; set; }
//}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///

namespace Rules_Engine_API.Models;

public class EvaluationStats
{
    // ─── Overall Count ────────────────────────────────────────────
    /// <summary>Total number of payloads tested (sessions created)</summary>
    public int TotalPayloadsTested { get; set; }

    // ─── Detection Stats (per attack level) ──────────────────────
    public int DetectionCorrectCount { get; set; }
    public int DetectionIncorrectCount { get; set; }
    public double DetectionAccuracyPercent { get; set; }

    // ─── Classification Stats (per attack level) ──────────────────
    public int ClassificationCorrectCount { get; set; }
    public int ClassificationIncorrectCount { get; set; }
    public double ClassificationAccuracyPercent { get; set; }
}

/// <summary>
/// One failed attack entry – used in /api/evaluation/failed
/// </summary>
public class FailedPayloadEntry
{
    public int SessionId { get; set; }
    public string Payload { get; set; }
    public string AttackName { get; set; }
    public string Pattern { get; set; }
    public string FailureType { get; set; }  // "MissedDetection" | "WrongClassification" | "Both"
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}