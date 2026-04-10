using Rules_Engine_API.Models;

namespace Rules_Engine_API.Services

{
    public static class EventMapper
    {
        public static SecurityEvent Map(
            string payload,
            string sourceIp,
            int totalScore,
            List<string> matches
        )
        {
            return new SecurityEvent
            {
                AttackType = string.Join(" + ", matches.Distinct()),
                SourceIp = sourceIp,
                Payload = payload,
                //TotalScore = totalScore,
                MatchedAttacks = matches,
                Decoy = "webtrap-login",
                //Severity = totalScore switch
                //{
                //    >= 30 => "critical",
                //    >= 20 => "high",
                //    >= 10 => "medium",
                //    _ => "low"
                //},
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
