using Decopia.API.DTOs;

namespace Decopia.API.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
        Task<UserResponseDto> UpdateUserAsync(Guid publicId, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(Guid publicId);
        Task<UserResponseDto> GetUserAsync(Guid publicId);
        Task<List<UserResponseDto>> GetUsersAsync();               // ⭐ الجديد الصح
        Task<List<UserResponseDto>> SearchUsersByNameAsync(string name);
        Task<bool> ChangeStatusAsync(Guid publicId, bool isActive);
    }
}
