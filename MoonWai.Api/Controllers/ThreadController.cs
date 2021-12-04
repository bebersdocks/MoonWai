using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Dal;
using MoonWai.Dal.DataModels;
using MoonWai.Shared.Definitions;
using MoonWai.Shared.Models;

namespace MoonWai.Api.Controllers
{
    [ApiController]
    public class ThreadController : BaseController
    {
        [NonAction]
        private Task<List<PostDto>> GetThreadPosts(Dc dc, int threadId)
        {   
            var query = dc.Posts
                .Where(i => i.ThreadId == threadId)
                .Select(i => new PostDto
                {
                    PostId = i.PostId,
                    Message = i.Message,
                    CreateDt = i.CreateDt
                });

            return query.ToListAsync();
        }

        [HttpGet]
        [Route("api/threads/{threadId:int}")]
        public async Task<IActionResult> GetThreadPosts(int threadId)
        {
            using var dc = new Dc();

            var thread = await dc.Threads.FirstOrDefaultAsync(i => i.ThreadId == threadId);

            if (thread == null)
                return NotFound(TranslationId.ThreadNotFound);

            var posts = await GetThreadPosts(dc, threadId);

            return Ok(posts);
        }

        [HttpPost]
        [Route("api/threads")]
        public async Task<IActionResult> InsertThread(InsertThreadDto insertThreadDto)
        {
            using var dc = new Dc();

            var newThread = new Thread();

            newThread.Title = insertThreadDto.Title;
            newThread.Message = insertThreadDto.Message;
            newThread.BoardId = insertThreadDto.BoardId;
            newThread.UserId = insertThreadDto.UserId;
            newThread.CreateDt = DateTime.UtcNow;

            var threadId = await dc.InsertWithInt32IdentityAsync(newThread);

            if (threadId <= 0)
                return ServerError(TranslationId.FailedToCreateNewThread);

            return Ok();
        }

        [HttpPut]
        [Route("api/threads")]
        public async Task<IActionResult> UpdateThread(UpdateThreadDto updateThreadDto)
        {
            using var dc = new Dc();

            var thread = await dc.Threads.FirstOrDefaultAsync(i => i.ThreadId == updateThreadDto.ThreadId);

            if (thread == null)
                return NotFound(TranslationId.ThreadNotFound);

            if (thread.CreateDt.Add(Common.allowedEditTime) > DateTime.UtcNow)
                return Forbidden(TranslationId.AllowedEditTime, Common.allowedEditTime.Minutes);

            thread.Title = updateThreadDto.Title;
            thread.Message = updateThreadDto.Message;

            if (await dc.UpdateAsync(thread) < 1)
                return ServerError(TranslationId.FailedToUpdateThread);

            return Ok();
        }
    }
}
