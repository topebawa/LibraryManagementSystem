using Microsoft.EntityFrameworkCore;
using LibraryManagement.API.DTOs;

namespace LibraryManagement.API.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        string GenerateJwtToken(string username, int userId);
    }
}