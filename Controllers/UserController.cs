using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(TaskManagementDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO userLogin)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Name == userLogin.Name && u.Password == userLogin.Password);

        if (user == null)
            return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre!" });

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
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
                    Status = ((TaskDTO.StatusMessage)t.Status).ToString(),
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
