using System.ComponentModel.DataAnnotations;

namespace Decopia.API.DTOs
{

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
        public string Role { get; set; }  

    }

   
    public class UpdateUserDto
    {
        [Required]
        public string FullName { get; set; }

         
        public string Role { get; set; }

        public bool IsActive { get; set; }
    }

     public class UserResponseDto
    {
        public Guid PublicId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }

     public class ChangeUserStatusDto
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
