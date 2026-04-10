using Decopia.API.DTOs;
using Decopia.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Decopia.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // 1) Add User
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            await _userService.CreateUserAsync(dto);
            return Ok(new { message = "User created successfully" });
        }

        // 2) Get All Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // 3) Search users
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string name)
        {
            var users = await _userService.SearchUsersByNameAsync(name);
            return Ok(users);
        }

        // 4) Get User by publicId
        [HttpGet("{publicId}")]
        public async Task<IActionResult> GetUser(Guid publicId)
        {
            var user = await _userService.GetUserAsync(publicId);
            return Ok(user);
        }

        // 5) Update User
        [HttpPut("{publicId}")]
        public async Task<IActionResult> UpdateUser(Guid publicId, [FromBody] UpdateUserDto dto)
        {
            await _userService.UpdateUserAsync(publicId, dto);
            return Ok(new { message = "User updated successfully" });
        }

        // 6) Activate/deactivate user
        [HttpPatch("{publicId}/activate")]
        public async Task<IActionResult> ChangeStatus(Guid publicId, [FromQuery] bool isActive)
        {
            await _userService.ChangeStatusAsync(publicId, isActive);
            return Ok(new { message = "Status updated successfully" });
        }

        // 7) Delete user (soft delete)
        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteUser(Guid publicId)
        {
            var success = await _userService.DeleteUserAsync(publicId);
            if (!success)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User deleted successfully" });
        }
    }
}



//using Microsoft.AspNetCore.Mvc;
//using Decopia.API.Services;
//using Microsoft.AspNetCore.Authorization;
//using Decopia.API.DTOs;
//using Decopia.API.Interfaces;

//namespace Decopia.API.Controllers
//{
//    [ApiController]
//    [Route("api/users")]
//    [Authorize(Roles = "Admin")] // كل ال endpoints هنا للأدمن فقط
//    public class UsersController : ControllerBase
//    {
//        private readonly IUserService _userService;

//        public UsersController(IUserService userService)
//        {
//            _userService = userService;
//        }

//        // 1) Add new user (Admin only)
//        [HttpPost]
//        public async Task<IActionResult> AddUser([FromBody] CreateUserDto dto)
//        {
//            await _userService.CreateUserAsync(dto);
//            return Ok(new { message = "User created successfully" });
//        }

//        // 2) Get list of all users
//        [HttpGet]
//        public async Task<IActionResult> GetUsers()
//        {
//            var users = await _userService.GetUsersAsync();
//            return Ok(users);
//        }

//        // 3) Search user by name
//        [HttpGet("search")]
//        public async Task<IActionResult> Search([FromQuery] string name)
//        {
//            var users = await _userService.SearchUsersByNameAsync(name);
//            return Ok(users);
//        }

//        // 4) Get user by publicId
//        [HttpGet("{publicId}")]
//        public async Task<IActionResult> GetUser(Guid publicId)
//        {
//            var user = await _userService.GetUserAsync(publicId);
//            return Ok(user);
//        }

//        // 5) Update user info
//        [HttpPut("{publicId}")]
//        public async Task<IActionResult> UpdateUser(Guid publicId, UpdateUserDto dto)
//        {
//            await _userService.UpdateUserAsync(publicId, dto);
//            return Ok(new { message = "User updated successfully" });
//        }

//        // 6) Activate / Deactivate user
//        [HttpPatch("{publicId}/activate")]
//        public async Task<IActionResult> ChangeStatus(Guid publicId, [FromQuery] bool isActive)
//        {
//            await _userService.ChangeStatusAsync(publicId, isActive);
//            return Ok(new { message = "Status updated successfully" });
//        }
//    }
//}
