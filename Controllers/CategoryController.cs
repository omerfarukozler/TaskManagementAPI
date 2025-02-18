using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;

        public CategoryController(TaskManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (category == null || string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Geçersiz kategori verisi.");
            }

            var newCategory = new Category  
            {
                Name = category.Name
            };

            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { id = newCategory.Id }, newCategory);
        }
    }
}
