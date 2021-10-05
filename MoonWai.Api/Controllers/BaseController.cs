using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MoonWai.Api.Resources.Translations;
using MoonWai.Shared.Definitions;

namespace MoonWai.Api.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger<BaseController> logger;

        protected BaseController(ILogger<BaseController> logger) => this.logger = logger;

        protected string GetTranslation(TranslationId translationId, params object[] args)
        {
            var languageId = LanguageId.English; // TODO

            return Translations.GetTranslation(languageId, translationId, args);
        }

        protected IActionResult BadRequest(TranslationId translationId, params object[] args)
        {
            var translation = GetTranslation(translationId, args);

            return BadRequest(translation);
        }

        protected IActionResult NotFound(TranslationId translationId, params object[] args)
        {
            var translation = GetTranslation(translationId, args);

            return NotFound(translation);
        }
    }
}
