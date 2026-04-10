using System.ComponentModel.DataAnnotations;

//كل حاجة تخص Users(CRUD + Activate)
namespace Decopia.API.DTOs
{

    // لما Admin يضيف موظف جديد
    public class CreateUserDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Admin / User

    }

    // لما Admin
    // يحدث بيانات الموظف

    public class UpdateUserDto
    {
        [Required]
        public string FullName { get; set; }

        //[EmailAddress]
        //public string Email { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }
    }

    // لما نرجع بيانات الموظف للادمن
    public class UserResponseDto
    {
        public Guid PublicId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }

    // لتغيير حالة الموظف (Activate / Deactivate)
    public class ChangeUserStatusDto
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
