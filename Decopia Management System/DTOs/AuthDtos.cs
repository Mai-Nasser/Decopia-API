using System.ComponentModel.DataAnnotations;

//كل حاجة تخص Authentication(Login, Forgot/Reset Password)
namespace Decopia.API.DTOs
{

    // Login Request
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    // Login Response
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Role { get; set; }
    }

    // Forgot Password Request
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    // Reset Password Request
    public class ResetPasswordRequestDto
    {

        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }

    }
}
