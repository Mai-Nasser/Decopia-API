namespace Rules_Engine_API.Models;

public class SecurityEvent
{
    // General
    public string EventType { get; set; } = "web_attack";
    public DateTime Timestamp { get; set; }

    // Attack context
    public string AttackType { get; set; }
    public List<string> MatchedAttacks { get; set; }

    // Request context
    public string SourceIp { get; set; }
    public string Payload { get; set; }

    // Honeypot context
    public string Decoy { get; set; }
}