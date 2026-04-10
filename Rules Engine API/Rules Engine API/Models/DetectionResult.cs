namespace Rules_Engine_API.Models
{
    public class DetectionResult
    {
        public int AttackId { get; set; }
        public string AttackName { get; set; }
        public string Pattern { get; set; }
        public int Score { get; set; }
    }
}
