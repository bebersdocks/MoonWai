using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Dal;
using MoonWai.Dal.DataModels;
using MoonWai.Shared;
using MoonWai.Shared.Definitions;
using MoonWai.Shared.Models.Post;
using MoonWai.Shared.Models.Thread;

namespace MoonWai.Api.Controllers
{
    [ApiController]
    public class ThreadController : BaseController
    {
        [NonAction]
        private Task<ThreadDto> GetThread(Dc dc, int threadId)
        {   
            var query = dc.Threads
                .LoadWith(i => i.Posts)
                .Select(i => new ThreadDto
                {
                    ThreadId = i.ThreadId,
                    ParentId = i.ParentId,
                    Title = i.Title,
                    Message = i.Message,
                    Posts = i.Posts
                        .Select(j => new PostDto
                        {
                            PostId = j.PostId,
                            Message = j.Message,
                            CreateDt = j.CreateDt
                        })
                        .ToList(),
                    PostsCount = i.Posts.Count(),
                    CreateDt = i.CreateDt
                });

            return query.FirstOrDefaultAsync(i => i.ThreadId == threadId);
        }

        [HttpGet]
        [Route("api/threads/{threadId:int}")]
        public async Task<IActionResult> GetThread(int threadId)
        {
            using var dc = new Dc();

            var thread = await GetThread(dc, threadId);

            if (thread == null)
                return NotFound(TranslationId.ThreadNotFound);

            return Ok(thread);
        }

        [HttpPost]
        [Route("api/threads")]
        public async Task<IActionResult> InsertThread(InsertThreadDto insertThreadDto)
        {
            using var dc = new Dc();

            var allowedUserIds = await dc.BoardAllowedUsers
                .Where(i => i.BoardId == insertThreadDto.BoardId)
                .Select(i => i.UserId)
                .ToListAsync();

            if (allowedUserIds.Any())
            {
                var user = await GetUser(dc);
                if (user == null || !allowedUserIds.Contains(user.UserId))
                {
                    return Forbidden(TranslationId.NotAllowedToPostInThisBoard);
                }
            }

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

            if (thread.CreateDt.Add(Constants.AllowedEditTime) > DateTime.UtcNow)
                return Forbidden(TranslationId.AllowedEditTime, Constants.AllowedEditTime.Minutes);

            thread.Title = updateThreadDto.Title;
            thread.Message = updateThreadDto.Message;

            if (await dc.UpdateAsync(thread) < 1)
                return ServerError(TranslationId.FailedToUpdateThread);

            return Ok();
        }
    }
}
