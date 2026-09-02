using LibraryManagementWebApi.DTOs;

namespace LibraryManagementWebApi.Services
{
    public interface IAuthService
    {
        Task Register(RegisterDto dto);

        Task<LoginResponseDto> Login(LoginDto dto);
    }
}