

namespace Rules_Engine_API.Models;

public class AnalyzeRequest
{
    public string Payload { get; set; }
    public string ClientId { get; set; }     
    public string? AssignedTo { get; set; }   
    public string? UserAgent { get; set; }
}
