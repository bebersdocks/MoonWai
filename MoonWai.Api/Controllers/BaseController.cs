using Microsoft.AspNetCore.Mvc;

using MoonWai.Api.Resources;
using MoonWai.Shared.Definitions;
using MoonWai.Shared.Models;

namespace MoonWai.Api.Controllers
{
    public class BaseController : Controller
    {
        protected string GetTranslation(TranslationId translationId, params object[] args)
        {
            var languageId = LanguageId.English;// TODO

            return Translations.GetTranslation(languageId, translationId, args);
        }

        /// <summary>
        /// Failed response for 4xx and 5xx status codes.
        /// </summary>
        protected IActionResult Failed(int statusCode, TranslationId translationId, params object[] args)
        {
            var translation = GetTranslation(translationId, args);

            var respObj = new ErrorResponse(translation);

            return StatusCode(statusCode, respObj);
        }

        protected IActionResult BadRequest(TranslationId translationId, params object[] args) =>
            Failed(400, translationId, args);

        protected IActionResult LogonFailed(TranslationId translationId, params object[] args) =>
            Failed(401, translationId, args);
            
        protected IActionResult NotFound(TranslationId translationId, params object[] args) => 
            Failed(404, translationId, args);

        protected IActionResult Conflict(TranslationId translationId, params object[] args) =>
            Failed(409, translationId, args);

        protected IActionResult ServerError(TranslationId translationId, params object[] args) =>
            Failed(500, translationId, args);
    }
}
