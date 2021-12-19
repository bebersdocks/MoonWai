using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Api.Definitions;
using MoonWai.Api.Models.Thread;
using MoonWai.Api.Services;
using MoonWai.Api.Utils;
using MoonWai.Dal;

namespace MoonWai.Api.Controllers
{
    [ApiController]
    public class ThreadController : BaseController
    {
        private readonly ThreadService threadService;

        public ThreadController(ThreadService threadService)
        {
            this.threadService = threadService;
        }

        [HttpGet]
        [Route("api/threads/{threadId:int}")]
        public async Task<IActionResult> GetThread(int threadId)
        {
            using var dc = new Dc();

            var thread = await threadService.GetThread(dc, threadId);

            if (thread == null)
                return NotFound(ErrorId.ThreadNotFound);

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
                    return Forbidden(ErrorId.NotAllowedToPostInThisBoard);
                }
            }

            if (await threadService.InsertThread(dc, insertThreadDto) <= 0)
                return ServerError(ErrorId.FailedToCreateNewThread);

            return Ok();
        }

        [HttpPut]
        [Route("api/threads")]
        public async Task<IActionResult> UpdateThread(UpdateThreadDto updateThreadDto)
        {
            using var dc = new Dc();

            var thread = await dc.Threads
                .LoadWith(i => i.Post)
                .FirstOrDefaultAsync(i => i.ThreadId == updateThreadDto.ThreadId);

            if (thread == null)
                return NotFound(ErrorId.ThreadNotFound);

            if (thread.CreateDt.Add(Constants.AllowedEditTime) > DateTime.UtcNow)
                return Forbidden(ErrorId.AllowedEditTime, Constants.AllowedEditTime.Minutes);

            if (await threadService.UpdateThread(dc, thread, thread.Post, updateThreadDto) < 2)
                return ServerError(ErrorId.FailedToUpdateThread);

            return Ok();
        }
    }
}
