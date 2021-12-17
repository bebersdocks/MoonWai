using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Dal;
using MoonWai.Dal.DataModels;
using MoonWai.Shared.Definitions;
using MoonWai.Shared.Models;

namespace MoonWai.Api.Controllers
{
    public class BaseController : Controller
    {
        protected Task<User> GetUser(Dc dc)
        {
            if (HttpContext.User.Identity.IsAuthenticated && HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userIdStr = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

                if (int.TryParse(userIdStr, out var userId))
                {
                    return dc.Users
                        .LoadWith(i => i.Settings)
                        .FirstOrDefaultAsync(i => i.UserId == userId);
                }
            }

            return Task.FromResult<User>(null);
        }

        /// <summary>
        /// Failed responses for 4xx and 5xx status codes.
        /// </summary>
        protected IActionResult Failed(int statusCode, ErrorId errorId, params object[] args)
        {
            var errorMsg = Program.Translations.GetErrorMsg(errorId, args);
            var error = new ErrorDto { ErrorId = errorId, Message = errorMsg };

            return StatusCode(statusCode, error);
        }

        protected IActionResult BadRequest(ErrorId errorId, params object[] args) =>
            Failed(400, errorId, args);

        protected IActionResult LogonFailed(ErrorId errorId, params object[] args) =>
            Failed(401, errorId, args);

        protected IActionResult Forbidden(ErrorId errorId, params object[] args) =>
            Failed(403, errorId, args);
              
        protected IActionResult NotFound(ErrorId errorId, params object[] args) =>
            Failed(404, errorId, args);

        protected IActionResult Conflict(ErrorId errorId, params object[] args) =>
            Failed(409, errorId, args);

        protected IActionResult ServerError(ErrorId errorId, params object[] args) =>
            Failed(500, errorId, args);
    }
}
