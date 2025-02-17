using Microsoft.AspNetCore.Mvc;

namespace TaskManagementAPI.Controllers{
    [ApiController]
    [Route("controller")]
    public class UserController : ControllerBase {
        private readonly TaskManagementDbContext _context;
        public UserController(TaskManagementDbContext context) {
            _context = context;
        }
    }
}