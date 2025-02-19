using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;
using System.Security.Claims;

namespace TaskManagementAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : BaseController
    {
        private readonly TaskManagementDbContext _context;

        public TaskController(TaskManagementDbContext context)
        {
            _context = context;
        }


        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetUserTasks(int taskId)
        {
            var taskOwner = await _context.Tasks
            .Where(t => t.Id == taskId)
            .Select(t => t.UserId)
            .FirstOrDefaultAsync();

            if (taskOwner == 0)
            {
                return NotFound("Görev bulunamadı.");
            }

            if (CurrentUserId != taskOwner)
            {
                return Forbid();
            }

            var tasks = await _context.Tasks
                .Where(t => t.Id == taskId)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Content,
                    Status = ((TaskDTO.StatusMessage)t.Status).ToString(),
                    User = t.User != null
                            ? new { t.User.Id, t.User.Name }
                            : null,
                    Categories = t.Categories.Select(c => new { c.Id, c.Name }).ToList()
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskDTO taskDto)
        {
            if (CurrentUserId != taskDto.UserId)
            {
                return Forbid();
            }
            if (taskDto == null)
            {
                return BadRequest(new { Message = "Görev Bilgileri Eksik Girildi." });

            }

            var existingCategories = await _context.Categories.Where(c => taskDto.Categories.Contains(c.Id)).ToListAsync();
            if (existingCategories.Count != taskDto.Categories.Count)
            {
                return BadRequest(new { Message = "Geçersiz kategori ID'leri verildi." });
            }

            var task = new TaskManagementAPI.Models.Task
            {
                UserId = taskDto.UserId,
                Title = taskDto.Title,
                Content = taskDto.Content,
                Status = (int)taskDto.Status,
                Categories = existingCategories
            };

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserTasks), new { taskId = task.Id }, new
            {
                task.Id,
                task.UserId,
                task.Title,
                task.Content,
                Status = ((TaskDTO.StatusMessage)task.Status).ToString(),
                Categories = task.Categories.Select(c => new { c.Id, c.Name }).ToList()
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTask([FromBody] TaskDTO taskDto)
        {
            if (CurrentUserId != taskDto.UserId)
            {
                return Forbid();
            }

            if (taskDto == null)
            {
                return BadRequest("Görev bilgileri eksik.");
            }

            var task = await _context.Tasks
                .Include(t => t.Categories)
                .FirstOrDefaultAsync(t => t.Id == taskDto.Id);

            if (task == null)
            {
                return NotFound("Görev bulunamadı.");
            }

            task.Title = taskDto.Title;
            task.Content = taskDto.Content;
            task.Status = (int)taskDto.Status;
            task.Categories = await _context.Categories.Where(c => taskDto.Categories.Contains(c.Id)).ToListAsync();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                task.Id,
                task.UserId,
                task.Title,
                task.Content,
                Status = ((TaskDTO.StatusMessage)task.Status).ToString(),
                Categories = task.Categories.Select(c => new { c.Id, c.Name }).ToList()
            });
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {

            var taskOwner = await _context.Tasks
            .Where(t => t.Id == taskId)
            .Select(t=>t.UserId)
            .FirstOrDefaultAsync();

            if(CurrentUserId != taskOwner) 
            {
                return Forbid();
            }

            var task = await _context.Tasks.FindAsync(taskId);

            if (task == null)
            {
                return NotFound("Görev bulunamadı.");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(new { task.Id });
        }
    }
}