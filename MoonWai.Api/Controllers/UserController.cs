using System.Collections.Generic;
using System.Linq;
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
    public class UserController : BaseController
    {
        [NonAction]
        private Task<List<ThreadDto>> GetUserThreads(Dc dc, int userId)
        {   
            var query = dc.Threads
                .Where(i => i.UserId == userId)
                .Select(i => new ThreadDto
                {
                    ThreadId = i.ThreadId,
                    Title = i.Title,
                    Message = i.Message,
                    CreateDt = i.CreateDt
                });

            return query.ToListAsync(); 
        }

        [HttpGet]
        [Route("user/threads")]
        public async Task<IActionResult> GetUserThreads()
        {
            var user = await GetUser();
            
            using var dc = new Dc();

            var threads = await GetUserThreads(dc, user.UserId);

            return Ok(threads);
        }

        [NonAction]
        private Task<List<PostDto>> GetUserPosts(Dc dc, int userId)
        {   
            var query = dc.Posts
                .Where(i => i.UserId == userId)
                .Select(i => new PostDto
                {
                    PostId = i.PostId,
                    Message = i.Message,
                    CreateDt = i.CreateDt
                });

            return query.ToListAsync(); 
        }

        [HttpGet]
        [Route("user/posts")]
        public async Task<IActionResult> GetUserPosts()
        {
            var user = await GetUser();
            
            using var dc = new Dc();

            var posts = await GetUserPosts(dc, user.UserId);

            return Ok(posts);
        }

        [HttpPut]
        [Route("user/language/{languageId:int}")]
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
        [Route("user/settings")]
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
