using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Services;

namespace QuanLyChiTieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
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
        public async Task<ActionResult<List<CategoryDto>>> GetAllCategories([FromQuery] bool? isIncome)
        {
            try
            {
                var userId = GetUserId();
                var categories = await _categoryService.GetAllCategoriesAsync(userId, isIncome);
                return Ok(categories);
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
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            try
            {
                var userId = GetUserId();
                var category = await _categoryService.GetCategoryByIdAsync(userId, id);
                
                if (category == null)
                    return NotFound(new { message = "Không tìm thấy danh mục" });

                return Ok(category);
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
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            try
            {
                var userId = GetUserId();
                var category = await _categoryService.CreateCategoryAsync(userId, createDto);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo danh mục", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateDto)
        {
            try
            {
                var userId = GetUserId();
                var category = await _categoryService.UpdateCategoryAsync(userId, id, updateDto);
                
                if (category == null)
                    return NotFound(new { message = "Không tìm thấy danh mục hoặc không thể sửa danh mục hệ thống" });

                return Ok(category);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật danh mục", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _categoryService.DeleteCategoryAsync(userId, id);
                
                if (!result)
                    return NotFound(new { message = "Không tìm thấy danh mục hoặc không thể xóa danh mục hệ thống" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa danh mục", error = ex.Message });
            }
        }
    }
}
