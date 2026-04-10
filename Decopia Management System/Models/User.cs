namespace Decopia.API.Models
{
    public class User
    {
        public int Id { get; set; }                 // Primary Key
        public Guid PublicId { get; set; } = Guid.NewGuid(); // PublicId مخفي لليوزر
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }    // نخزن الـ password مش text
        public string Role { get; set; }            // Admin / User
        public bool IsActive { get; set; } = true;  // لتفعيل/تعطيل الحساب

        public bool IsDeleted { get; set; } = false;// حذف الحساب
        public DateTime? DeletedAt { get; set; }

        // لخاصية Forgot/Reset Password
        public string? ResetCode { get; set; }
        public DateTime? ResetCodeExpiry { get; set; }
    }
}
