using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Api.Definitions;
using MoonWai.Api.Models;
using MoonWai.Api.Utils;
using MoonWai.Dal;
using MoonWai.Dal.DataModels;

namespace MoonWai.Api.Controllers
{
    public class BaseController : Controller
    {
        protected readonly Dc _dc;

        public BaseController(Dc dc)
        {
            _dc = dc;
        }

        protected Task<User> GetUser()
        {
            if (HttpContext.User.Identity.IsAuthenticated && HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userIdStr = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

                if (int.TryParse(userIdStr, out var userId))
                {
                    return _dc.Users
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
            var error = new ErrorDto { ErrorId = errorId, ErrorIdStr = errorId.ToString(), Message = errorMsg };

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

        protected async Task<IActionResult> PagedQueryResult<T>(IQueryable<T> query, int? page = null, int? pageSize = null)
        {
            if ((page ?? 1) < 1)
                return BadRequest(ErrorId.PageCantBeSmallerThanOne);

            if ((pageSize ?? 1) < 1)
                return BadRequest(ErrorId.PageSizeCantZeroOrNegative);

            page ??= 1; // first page by default
            pageSize ??= Constants.DefaultThreadsPerPage;
            
            var totalCount = await query.CountAsync();
            var pages = ((totalCount - 1) / pageSize.Value) + 1;

            var items = await query
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync();

            return Ok(new { Items = items, TotalCount = totalCount, Pages = pages }); 
        }
    }
}
