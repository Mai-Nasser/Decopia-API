using Decopia.API.DTOs;


namespace Decopia.API.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto dto);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto dto);
    }
}
