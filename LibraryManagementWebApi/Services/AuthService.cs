using LibraryManagementWebApi.Data;
using LibraryManagementWebApi.DTOs;
using LibraryManagementWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagementWebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task Register(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var existingUser =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Email == email);

            if (existingUser != null)
            {
                throw new Exception(
                    "Email already registered.");
            }

            var user = new User
            {
                Name = dto.Name.Trim(),

                Email = email,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        dto.Password),

                Role = "User",

                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
        }

        public async Task<LoginResponseDto> Login(
            LoginDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var user =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Email == email);

            if (user == null)
            {
                throw new Exception(
                    "Invalid email or password.");
            }

            var passwordValid =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    user.PasswordHash);

            if (!passwordValid)
            {
                throw new Exception(
                    "Invalid email or password.");
            }

            var claims =
                new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(
                        "UserId",
                        user.Id.ToString()),

                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Name,
                        user.Name),

                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Email,
                        user.Email),

                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Role,
                        user.Role)
                };

            var jwtKey =
                _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new Exception(
                    "JWT key is not configured.");
            }

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var token =
                new JwtSecurityToken(
                    issuer:
                        _configuration["Jwt:Issuer"],

                    audience:
                        _configuration["Jwt:Audience"],

                    claims:
                        claims,

                    expires:
                        DateTime.UtcNow.AddHours(2),

                    signingCredentials:
                        credentials
                );

            var tokenString =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            return new LoginResponseDto
            {
                Token = tokenString,
                UserId = user.Id,
                Name = user.Name,
                Role = user.Role
            };
        }
    }
}