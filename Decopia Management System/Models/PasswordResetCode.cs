namespace Decopia.API.Models
{
    public class PasswordResetCode
    {
        public int Id { get; set; }
        public Guid UserPublicId { get; set; }
        public string Code { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}
