using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        protected int CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(userIdClaim, out int userId) ? userId : throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
            }
        }

    }
}