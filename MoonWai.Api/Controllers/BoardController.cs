using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using LinqToDB;

using MoonWai.Api.Resources;
using MoonWai.Dal;
using MoonWai.Shared.Definitions;
using MoonWai.Shared.Models;

namespace MoonWai.Api.Controllers
{
    [ApiController]
    public class BoardController : BaseController
    {
        public BoardController(ILogger<AuthController> logger) : base(logger) { }

        [HttpGet]
        [Route("boards")]
        public async Task<IActionResult> GetBoards()
        {
            using var dc = new Dc();

            var boards = await dc.Boards.ToListAsync();

            return Ok(boards);
        }

        [HttpGet]
        [Route("board/{boardId:int}/path")]
        public async Task<IActionResult> GetBoardPath(int boardId)
        {
            using var dc = new Dc();
      
            var board = await dc.Boards.FirstOrDefaultAsync(i => i.BoardId == boardId);

            if (board == null)
                return NotFound(TranslationId.BoardNotFound);

            return Ok(board.Path);
        }

        [NonAction]
        private Task<List<ThreadDto>> GetThreads(Dc dc, int boardId, bool preview = true, int? page = null, int? pageSize = null)
        {   
            page ??= 1; // first page by default
            pageSize ??= Common.defaultThreadsPerPage;

            var postsCount = preview ? Common.postsInPreviewCount : Common.maxPostsPerThread;

            var query = dc.Threads
                .LoadWith(i => i.Posts)
                .Where(i => i.BoardId == boardId)
                .Select(i => new ThreadDto
                {
                    ThreadId = i.ThreadId,
                    Title = i.Title,
                    Message = i.Message,
                    Posts = i.Posts
                        .OrderByDescending(j => j.CreateDt)
                        .Take(postsCount)
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

            return query
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync(); 
        }

        private async Task<IActionResult> GetBoardThreads(Dc dc, int boardId, bool preview = true, int? page = null, int? pageSize = null)
        {
            if ((page ?? 1) < 1)
                return BadRequest(TranslationId.PageCantBeSmallerThanOne);

            if ((pageSize ?? 1) < 1)
                return BadRequest(TranslationId.PageSizeCantZeroOrNegative);

            var threads = await GetThreads(dc, boardId, preview, page, pageSize);

            return Ok(threads);
        }

        [HttpGet]
        [Route("board/{boardPath}/threads")]
        public async Task<IActionResult> GetBoardThreads(string boardPath, bool preview = true, int? page = null, int? pageSize = null)
        {
            using var dc = new Dc();

            var board = await dc.Boards.FirstOrDefaultAsync(i => i.Path.Equals(boardPath));

            if (board == null)
                return NotFound(TranslationId.BoardNotFound);

            return await GetBoardThreads(dc, board.BoardId, preview, page, pageSize);
        }

        [HttpGet]
        [Route("board/{boardId:int}/threads")]
        public async Task<IActionResult> GetBoardThreads(int boardId, bool preview = true, int? page = null, int? pageSize = null)
        {
            using var dc = new Dc();
            
            var board = await dc.Boards.FirstOrDefaultAsync(i => i.BoardId == boardId);

            if (board == null)
                return NotFound(TranslationId.BoardNotFound);

            return await GetBoardThreads(dc, board.BoardId, preview, page, pageSize);
        }
    }
}
