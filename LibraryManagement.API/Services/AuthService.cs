using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryManagement.API.Data;
using LibraryManagement.API.DTOs;
using LibraryManagement.API.Models;

namespace LibraryManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email);

                if (existingUser != null)
                {
                    return new ApiResponse<AuthResponseDto>
                    {
                        Success = false,
                        Message = "User with this username or email already exists",
                        Errors = new List<string> { "Username or email already taken" }
                    };
                }

                // Create new user
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(user.Username, user.Id);
                var expiresAt = DateTime.UtcNow.AddHours(24);

                return new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = new AuthResponseDto
                    {
                        Token = token,
                        Username = user.Username,
                        ExpiresAt = expiresAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return new ApiResponse<AuthResponseDto>
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        Errors = new List<string> { "Authentication failed" }
                    };
                }

                var token = GenerateJwtToken(user.Username, user.Id);
                var expiresAt = DateTime.UtcNow.AddHours(24);

                return new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new AuthResponseDto
                    {
                        Token = token,
                        Username = user.Username,
                        ExpiresAt = expiresAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = "Login failed",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public string GenerateJwtToken(string username, int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}