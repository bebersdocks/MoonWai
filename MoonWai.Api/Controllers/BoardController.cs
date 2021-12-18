using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Dal;
using MoonWai.Shared;
using MoonWai.Shared.Definitions;
using MoonWai.Shared.Models.Board;
using MoonWai.Shared.Models.Post;
using MoonWai.Shared.Models.Thread;

namespace MoonWai.Api.Controllers
{
    [ApiController]
    public class BoardController : BaseController
    {
        [NonAction]
        private Task<List<BoardDto>> GetBoards(Dc dc)
        {
            var query = dc.Boards 
                .LoadWith(i => i.BoardSection)
                .Select(i => new BoardDto
                {
                    BoardId = i.BoardId,
                    BoardSection = new BoardSectionDto
                    {
                        BoardSectionId = i.BoardSectionId,
                        Name = i.BoardSection.Name
                    },
                    Path = i.Path,
                    Name = i.Name
                });
            
            return query.ToListAsync();
        }

        [HttpGet]
        [Route("api/boards")]
        public async Task<IActionResult> GetBoards()
        {
            using var dc = new Dc();

            var boards = await GetBoards(dc);

            return Ok(boards);
        }

        [NonAction]
        private IQueryable<ThreadDto> GetThreads(Dc dc, int boardId, bool preview = true)
        {   
            var postsCount = preview ? Constants.PostsInPreview : Constants.MaxPostsPerThread;

            var query = dc.Threads
                .LoadWith(i => i.Posts)
                .Where(i => i.BoardId == boardId)
                .Select(i => new ThreadDto
                {
                    ThreadId = i.ThreadId,
                    ParentId = i.ParentId,
                    Title = i.Title,
                    Message = i.Message,
                    Posts = i.Posts
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

            return query;
        }

        [HttpGet]
        [Route("api/boards/{boardPath}")]
        public async Task<IActionResult> GetBoardThreads(string boardPath, bool preview = true, int? page = null, int? pageSize = null)
        {
            using var dc = new Dc();

            var board = await dc.Boards.FirstOrDefaultAsync(i => i.Path.Equals(boardPath));

            if (board == null)
                return NotFound(ErrorId.BoardNotFound);

            var query = GetThreads(dc, board.BoardId, preview);
            
            return await PagedQueryResult(query, page, pageSize);
        }
    }
}
