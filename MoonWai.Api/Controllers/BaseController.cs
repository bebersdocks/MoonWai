using Microsoft.AspNetCore.Mvc;

using MoonWai.Api.Resources;
using MoonWai.Shared.Definitions;

namespace MoonWai.Api.Controllers
{
    public class BaseController : Controller
    {
        protected string GetTranslation(TranslationId translationId, params object[] args)
        {
            var languageId = LanguageId.English;// TODO

            return Translations.GetTranslation(languageId, translationId, args);
        }

        protected IActionResult StatusCode(int statusCode, TranslationId translationId, params object[] args)
        {
            var translation = GetTranslation(translationId, args);

            return StatusCode(statusCode, translation);
        }

        protected IActionResult BadRequest(TranslationId translationId, params object[] args) =>
            StatusCode(400, translationId, args);

        protected IActionResult LogonFailed(TranslationId translationId, params object[] args) =>
            StatusCode(401, translationId, args);
            
        protected IActionResult NotFound(TranslationId translationId, params object[] args) => 
            StatusCode(404, translationId, args);

        protected IActionResult Conflict(TranslationId translationId, params object[] args) =>
            StatusCode(409, translationId, args);

        protected IActionResult ServerError(TranslationId translationId, params object[] args) =>
            StatusCode(500, translationId, args);
    }
}
