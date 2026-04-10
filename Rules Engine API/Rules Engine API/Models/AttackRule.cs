namespace Rules_Engine_API.Models
{
    public class AttackRule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AttackPattern> Patterns { get; set; }
    }
}
