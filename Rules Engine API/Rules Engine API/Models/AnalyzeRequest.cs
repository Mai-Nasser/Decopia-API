

namespace Rules_Engine_API.Models;

public class AnalyzeRequest
{
    public string Payload { get; set; }
    public string ClientId { get; set; }   // ✅ IP الحقيقي
    public string? AssignedTo { get; set; }   // email of the pentest engineer
    public string? UserAgent { get; set; }
}
