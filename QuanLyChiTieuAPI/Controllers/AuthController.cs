using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Services;

namespace QuanLyChiTieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi đăng ký", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi đăng nhập", error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Token không hợp lệ" });
            }

            // In a real app, you'd get user from database
            // For now, return basic info from claims
            var emailClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Email);
            var nameClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Name);

            return Ok(new UserDto
            {
                Id = userId,
                Email = emailClaim?.Value ?? "",
                FullName = nameClaim?.Value ?? "",
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
