//namespace Rules_Engine_API.Models;

///// <summary>
///// Summary statistics for a specific attack type
///// </summary>
//public class AttackSummary
//{
//    public string AttackName { get; set; }

//    /// <summary>Total times this attack appeared across all payloads</summary>
//    public int TotalOccurrences { get; set; }

//    /// <summary>How many of these occurrences have been evaluated</summary>
//    public int EvaluatedCount { get; set; }

//    /// <summary>How many are still pending evaluation</summary>
//    public int PendingCount { get; set; }

//    /// <summary>Percentage of evaluations where DetectionCorrect = false</summary>
//    public double DetectionErrorRate { get; set; }

//    /// <summary>Percentage of evaluations where ClassificationCorrect = false</summary>
//    public double ClassificationErrorRate { get; set; }

//    /// <summary>List of payloads where this attack had detection or classification errors</summary>
//    public List<FailedPayloadInfo> FailedPayloads { get; set; } = new();
//}

///// <summary>
///// Details about a specific payload that had errors for this attack
///// </summary>
//public class FailedPayloadInfo
//{
//    public int SessionId { get; set; }
//    public string Payload { get; set; }
//    public string Pattern { get; set; }
//    public bool DetectionCorrect { get; set; }
//    public bool ClassificationCorrect { get; set; }
//    public string? AssignedTo { get; set; }
//    public string? Notes { get; set; }
//    public DateTime CreatedAt { get; set; }
//}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///

namespace Rules_Engine_API.Models;

/// <summary>
/// Summary statistics for a specific attack type
/// Shows only EVALUATED attacks (no pending tracking)
/// </summary>
public class AttackSummary
{
    public string AttackName { get; set; }

    /// <summary>Total number of times this attack was evaluated by pentesters</summary>
    public int EvaluatedCount { get; set; }

    /// <summary>Percentage of evaluations where DetectionCorrect = false</summary>
    public double DetectionErrorRate { get; set; }

    /// <summary>Percentage of evaluations where ClassificationCorrect = false</summary>
    public double ClassificationErrorRate { get; set; }

    /// <summary>List of payloads where this attack had detection or classification errors</summary>
    public List<FailedPayloadInfo> FailedPayloads { get; set; } = new();
}

/// <summary>
/// Details about a specific payload that had errors for this attack
/// </summary>
public class FailedPayloadInfo
{
    /// <summary>Session ID - can be used to navigate to full session details if needed</summary>
    public int SessionId { get; set; }

    public string Payload { get; set; }
    public string Pattern { get; set; }
    public bool DetectionCorrect { get; set; }
    public bool ClassificationCorrect { get; set; }
    public string? AssignedTo { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}