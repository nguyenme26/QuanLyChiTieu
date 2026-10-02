using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Services;

namespace QuanLyChiTieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetsController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
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
        public async Task<ActionResult<List<BudgetDto>>> GetAllBudgets()
        {
            try
            {
                var userId = GetUserId();
                var budgets = await _budgetService.GetAllBudgetsAsync(userId);
                return Ok(budgets);
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
        public async Task<ActionResult<BudgetDto>> GetBudget(int id)
        {
            try
            {
                var userId = GetUserId();
                var budget = await _budgetService.GetBudgetByIdAsync(userId, id);
                
                if (budget == null)
                    return NotFound(new { message = "Không tìm thấy ngân sách" });

                return Ok(budget);
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
        public async Task<ActionResult<BudgetDto>> CreateBudget([FromBody] CreateBudgetDto createDto)
        {
            try
            {
                var userId = GetUserId();
                var budget = await _budgetService.CreateBudgetAsync(userId, createDto);
                return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budget);
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
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo ngân sách", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BudgetDto>> UpdateBudget(int id, [FromBody] UpdateBudgetDto updateDto)
        {
            try
            {
                var userId = GetUserId();
                var budget = await _budgetService.UpdateBudgetAsync(userId, id, updateDto);
                
                if (budget == null)
                    return NotFound(new { message = "Không tìm thấy ngân sách" });

                return Ok(budget);
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
                return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật ngân sách", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBudget(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _budgetService.DeleteBudgetAsync(userId, id);
                
                if (!result)
                    return NotFound(new { message = "Không tìm thấy ngân sách" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa ngân sách", error = ex.Message });
            }
        }

        [HttpGet("alerts")]
        public async Task<ActionResult<List<BudgetAlertDto>>> GetBudgetAlerts()
        {
            try
            {
                var userId = GetUserId();
                var alerts = await _budgetService.GetBudgetAlertsAsync(userId);
                return Ok(alerts);
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

        [HttpPost("{id}/recalculate")]
        public async Task<ActionResult<BudgetDto>> RecalculateBudget(int id)
        {
            try
            {
                var userId = GetUserId();
                var budget = await _budgetService.RecalculateBudgetAsync(userId, id);
                
                if (budget == null)
                    return NotFound(new { message = "Không tìm thấy ngân sách" });

                return Ok(budget);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tính toán lại", error = ex.Message });
            }
        }
    }
}
