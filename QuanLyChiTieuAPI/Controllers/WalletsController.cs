using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Services;

namespace QuanLyChiTieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Token không hợp lệ");
            }
            return userId;
        }

        [HttpGet]
        public async Task<ActionResult<List<WalletDto>>> GetAllWallets()
        {
            try
            {
                var userId = GetUserId();
                var wallets = await _walletService.GetAllWalletsAsync(userId);
                return Ok(wallets);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WalletDto>> GetWallet(int id)
        {
            try
            {
                var userId = GetUserId();
                var wallet = await _walletService.GetWalletByIdAsync(userId, id);
                
                if (wallet == null)
                    return NotFound(new { message = "Không tìm thấy ví" });

                return Ok(wallet);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<WalletDto>> CreateWallet([FromBody] CreateWalletDto createDto)
        {
            try
            {
                var userId = GetUserId();
                var wallet = await _walletService.CreateWalletAsync(userId, createDto);
                return CreatedAtAction(nameof(GetWallet), new { id = wallet.Id }, wallet);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo ví", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WalletDto>> UpdateWallet(int id, [FromBody] UpdateWalletDto updateDto)
        {
            try
            {
                var userId = GetUserId();
                var wallet = await _walletService.UpdateWalletAsync(userId, id, updateDto);
                
                if (wallet == null)
                    return NotFound(new { message = "Không tìm thấy ví" });

                return Ok(wallet);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật ví", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWallet(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _walletService.DeleteWalletAsync(userId, id);
                
                if (!result)
                    return NotFound(new { message = "Không tìm thấy ví" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa ví", error = ex.Message });
            }
        }

        [HttpPut("{id}/set-default")]
        public async Task<ActionResult> SetDefaultWallet(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _walletService.SetDefaultWalletAsync(userId, id);
                
                if (!result)
                    return NotFound(new { message = "Không tìm thấy ví" });

                return Ok(new { message = "Đã đặt làm ví mặc định" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpGet("{id}/balance")]
        public async Task<ActionResult<WalletBalanceDto>> GetWalletBalance(int id)
        {
            try
            {
                var userId = GetUserId();
                var balance = await _walletService.GetWalletBalanceAsync(userId, id);
                
                if (balance == null)
                    return NotFound(new { message = "Không tìm thấy ví" });

                return Ok(balance);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpPost("transfer")]
        public async Task<ActionResult> Transfer([FromBody] TransferDto transferDto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _walletService.TransferAsync(userId, transferDto);
                
                if (!result)
                    return BadRequest(new { message = "Chuyển tiền thất bại. Kiểm tra số dư và ví." });

                return Ok(new { message = "Chuyển tiền thành công" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi chuyển tiền", error = ex.Message });
            }
        }
    }
}
