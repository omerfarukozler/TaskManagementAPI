using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;

        public UserController(TaskManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new 
            { 
                Id = user.Id, 
                Name = user.Name, 
                CreatedAt = user.CreatedAtUnix,
                Tasks = user.Tasks.Select(t => new 
                {
                    t.Id,
                    t.Title,
                    t.Content,
                    t.Status
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDto)
        {
            if (userDto == null)
            {
                return BadRequest();
            }

            var user = new User
            {
                Name = userDto.Name,
                Password = userDto.Password,
                CreatedAtUnix = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var createdUserDto = new 
            {
                Id = user.Id,
                Name = user.Name
            };

            return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, createdUserDto);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO userDto)
        {
            if (userDto == null || userDto.Id == 0)
            {
                return BadRequest("Invalid user data.");
            }

            var user = await _context.Users.FindAsync(userDto.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = userDto.Name;
            user.Password = userDto.Password;

            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Name });
        }
    }
}
