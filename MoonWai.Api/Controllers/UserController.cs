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
        public UserController(Dc dc) : base(dc) {}

        [NonAction]
        private Task<List<ThreadPreviewDto>> GetUserThreads(int userId)
        {   
            var query = _dc.Threads
                .LoadWith(t => t.Post)
                .Where(t => t.UserId == userId)
                .Select(t => new ThreadPreviewDto
                {
                    ThreadId = t.ThreadId,
                    ParentId = t.ParentId,
                    Title = t.Title,
                    Message = t.Post.Message,
                    CreateDt = t.CreateDt,
                    LastEditDt = t.LastEditDt
                });

            return query.ToListAsync(); 
        }

        [HttpGet]
        [Route("user/threads")]
        public async Task<IActionResult> GetUserThreads()
        {
            var user = await GetUser();    
            var threads = await GetUserThreads(user.UserId);

            return Ok(threads);
        }

        [NonAction]
        private Task<List<PostDto>> GetUserPosts(int userId)
        {   
            var query = _dc.Posts
                .Where(p => p.UserId == userId)
                .Select(p => new PostDto
                {
                    PostId = p.PostId,
                    Message = p.Message,
                    CreateDt = p.CreateDt
                });

            return query.ToListAsync();
        }

        [HttpGet]
        [Route("user/posts")]
        public async Task<IActionResult> GetUserPosts()
        {
            var user = await GetUser();     
            var posts = await GetUserPosts(user.UserId);

            return Ok(posts);
        }

        [HttpPut]
        [Route("user/language/{languageId:int}")]
        public async Task<IActionResult> UpdateLanguage(LanguageId languageId)
        {
            var user = await GetUser();
            
            user.Settings.LanguageId = languageId;

            if (await _dc.UpdateAsync(user.Settings) < 1)
                return ServerError(ErrorId.FailedToUpdateSettings);

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

            if (await _dc.UpdateAsync(settings) < 1)
                return ServerError(ErrorId.FailedToUpdateSettings);

            return Ok();
        }
    }
}
