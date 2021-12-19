using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Api.Definitions;
using MoonWai.Api.Models.Post;
using MoonWai.Api.Models.Thread;
using MoonWai.Api.Models.User;
using MoonWai.Dal;
using MoonWai.Dal.DataModels;

namespace MoonWai.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class UserController : BaseController
    {
        [NonAction]
        private Task<List<ThreadPreviewDto>> GetUserThreads(Dc dc, int userId)
        {   
            var query = dc.Threads
                .LoadWith(i => i.Post)
                .Where(i => i.UserId == userId)
                .Select(i => new ThreadPreviewDto
                {
                    ThreadId = i.ThreadId,
                    ParentId = i.ParentId,
                    Title = i.Title,
                    Message = i.Post.Message,
                    CreateDt = i.CreateDt,
                    LastEditDt = i.LastEditDt
                });

            return query.ToListAsync(); 
        }

        [HttpGet]
        [Route("user/threads")]
        public async Task<IActionResult> GetUserThreads()
        {
            using var dc = new Dc();

            var user = await GetUser(dc);    
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
            using var dc = new Dc();

            var user = await GetUser(dc);     
            var posts = await GetUserPosts(dc, user.UserId);

            return Ok(posts);
        }

        [HttpPut]
        [Route("user/language/{languageId:int}")]
        public async Task<IActionResult> UpdateLanguage(LanguageId languageId)
        {
            using var dc = new Dc();

            var user = await GetUser(dc);
            
            user.Settings.LanguageId = languageId;

            if (await dc.UpdateAsync(user.Settings) < 1)
                return ServerError(ErrorId.FailedToUpdateSettings);

            return Ok();
        }

        [HttpPut]
        [Route("user/settings")]
        public async Task<IActionResult> UpdateSettings(UserSettingsDto userSettingsDto)
        {
            using var dc = new Dc();

            var user = await GetUser(dc);
            var settings = user.Settings;

            settings.LanguageId = userSettingsDto.LanguageId;
            settings.DefaultBoardId = userSettingsDto.DefaultBoardId;

            if (await dc.UpdateAsync(settings) < 1)
                return ServerError(ErrorId.FailedToUpdateSettings);

            return Ok();
        }
    }
}
