using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Dal;
using MoonWai.Shared.Definitions;
using MoonWai.Shared.Models;

namespace MoonWai.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class SettingsController : BaseController
    {
        [HttpPut]
        [Route("settings/language/{languageId:int}")]
        public async Task<IActionResult> UpdateLanguage(LanguageId languageId)
        {
            var user = await GetUser();
            
            user.Settings.LanguageId = languageId;

            using var dc = new Dc();

            if (await dc.UpdateAsync(user.Settings) < 1)
                return ServerError(TranslationId.FailedToUpdateSettings);

            return Ok();
        }

        [HttpPut]
        [Route("settings")]
        public async Task<IActionResult> UpdateSettings(UserSettingsDto userSettingsDto)
        {
            var user = await GetUser();

            var settings = user.Settings;

            settings.LanguageId = userSettingsDto.LanguageId;
            settings.DefaultBoardId = userSettingsDto.DefaultBoardId;

            using var dc = new Dc();

            if (await dc.UpdateAsync(settings) < 1)
                return ServerError(TranslationId.FailedToUpdateSettings);

            return Ok();
        }
    }
}
