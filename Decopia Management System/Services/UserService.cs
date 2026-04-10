using Decopia.API.Data;
using Decopia.API.DTOs;
using Decopia.API.Interfaces;
using Decopia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Decopia.API.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            var user = new User
            {
                PublicId = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                PublicId = user.PublicId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task<UserResponseDto> UpdateUserAsync(Guid publicId, UpdateUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PublicId == publicId  
                                                               );

            if (user == null || user.IsDeleted)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;


           

            if (!string.IsNullOrWhiteSpace(dto.Role))
                user.Role = dto.Role;

            user.IsActive = dto.IsActive;


            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                PublicId = user.PublicId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task<bool> DeleteUserAsync(Guid publicId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PublicId == publicId);
            if (user == null) return false;

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.IsActive = false;          
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserResponseDto> GetUserAsync(Guid publicId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PublicId == publicId  
            );
            if (user == null) return null;

            return new UserResponseDto
            {
                PublicId = user.PublicId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task<List<UserResponseDto>> GetUsersAsync()
        {
            return await _context.Users
                .Where(x => !x.IsDeleted).Select(u => new UserResponseDto
                {
                    PublicId = u.PublicId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<List<UserResponseDto>> SearchUsersByNameAsync(string name)
        {
            return await _context.Users
                .Where(x => !x.IsDeleted).Where(x => x.FullName.Contains(name))
                .Select(u => new UserResponseDto
                {
                    PublicId = u.PublicId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<bool> ChangeStatusAsync(Guid publicId, bool isActive)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PublicId == publicId  
            );
            if (user == null) return false;

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
