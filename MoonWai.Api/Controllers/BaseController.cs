using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Api.Resources;
using MoonWai.Dal;
using MoonWai.Dal.DataModels;
using MoonWai.Shared.Definitions;
using MoonWai.Shared.Models;

namespace MoonWai.Api.Controllers
{
    public class BaseController : Controller
    {
        protected Task<User> GetUser()
        {
            if (HttpContext.User.Identity.IsAuthenticated && HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userIdStr = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

                if (int.TryParse(userIdStr, out var userId))
                {
                    using var dc = new Dc();

                    return dc.Users
                        .LoadWith(i => i.Settings)
                        .FirstOrDefaultAsync(i => i.UserId == userId);
                }
            }

            return Task.FromResult<User>(null);
        }

        protected string GetTranslation(TranslationId translationId, params object[] args)
        {
            var languageId = LanguageId.English;// TODO

            return Translations.GetTranslation(languageId, translationId, args);
        }

        /// <summary>
        /// Failed responses for 4xx and 5xx status codes.
        /// </summary>
        protected IActionResult Failed(int statusCode, TranslationId translationId, params object[] args)
        {
            var translation = GetTranslation(translationId, args);

            var respObj = new ErrorDto(translation);

            return StatusCode(statusCode, respObj);
        }

        protected IActionResult BadRequest(TranslationId translationId, params object[] args) =>
            Failed(400, translationId, args);

        protected IActionResult LogonFailed(TranslationId translationId, params object[] args) =>
            Failed(401, translationId, args);

        protected IActionResult Forbidden(TranslationId translationId, params object[] args) =>
            Failed(403, translationId, args);
              
        protected IActionResult NotFound(TranslationId translationId, params object[] args) =>
            Failed(404, translationId, args);

        protected IActionResult Conflict(TranslationId translationId, params object[] args) =>
            Failed(409, translationId, args);

        protected IActionResult ServerError(TranslationId translationId, params object[] args) =>
            Failed(500, translationId, args);
    }
}
