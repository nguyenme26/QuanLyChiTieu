using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLyChiTieuAPI.Data;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Models;

namespace QuanLyChiTieuAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new InvalidOperationException("Email đã được sử dụng");
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create user
            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PhoneNumber = registerDto.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create default wallet for user
            var defaultWallet = new Wallet
            {
                Name = "Ví chính",
                Description = "Ví mặc định",
                Balance = 0,
                Currency = "VND",
                IsDefault = true,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Wallets.Add(defaultWallet);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpiryInMinutes", 60));

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không chính xác");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Tài khoản đã bị khóa");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpiryInMinutes", 60));

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHereThatIsAtLeast32CharactersLong!";
            var issuer = jwtSettings["Issuer"] ?? "QuanLyChiTieuAPI";
            var audience = jwtSettings["Audience"] ?? "QuanLyChiTieuClient";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpiryInMinutes", 60)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
