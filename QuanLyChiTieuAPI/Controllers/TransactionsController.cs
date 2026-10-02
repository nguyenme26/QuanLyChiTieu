using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Services;

namespace QuanLyChiTieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
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
        public async Task<ActionResult<PagedResultDto<TransactionDto>>> GetTransactions(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? walletId,
            [FromQuery] int? categoryId,
            [FromQuery] bool? isIncome,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetUserId();
                var filter = new TransactionFilterDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    WalletId = walletId,
                    CategoryId = categoryId,
                    IsIncome = isIncome,
                    Page = page,
                    PageSize = pageSize
                };
                var result = await _transactionService.GetTransactionsAsync(userId, filter);
                return Ok(result);
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
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            try
            {
                var userId = GetUserId();
                var transaction = await _transactionService.GetTransactionByIdAsync(userId, id);
                
                if (transaction == null)
                    return NotFound(new { message = "Không tìm thấy giao dịch" });

                return Ok(transaction);
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
        public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] CreateTransactionDto createDto)
        {
            try
            {
                var userId = GetUserId();
                var transaction = await _transactionService.CreateTransactionAsync(userId, createDto);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo giao dịch", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionDto>> UpdateTransaction(int id, [FromBody] UpdateTransactionDto updateDto)
        {
            try
            {
                var userId = GetUserId();
                var transaction = await _transactionService.UpdateTransactionAsync(userId, id, updateDto);
                
                if (transaction == null)
                    return NotFound(new { message = "Không tìm thấy giao dịch" });

                return Ok(transaction);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật giao dịch", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _transactionService.DeleteTransactionAsync(userId, id);
                
                if (!result)
                    return NotFound(new { message = "Không tìm thấy giao dịch" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa giao dịch", error = ex.Message });
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult> GetSummary(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? walletId)
        {
            try
            {
                var userId = GetUserId();
                var totalIncome = await _transactionService.GetTotalIncomeAsync(userId, startDate, endDate, walletId);
                var totalExpense = await _transactionService.GetTotalExpenseAsync(userId, startDate, endDate, walletId);
                
                return Ok(new
                {
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Balance = totalIncome - totalExpense
                });
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
    }
}
