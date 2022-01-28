using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Api.Services;
using MoonWai.Api.Definitions;
using MoonWai.Api.Models.Board;
using MoonWai.Dal;

namespace MoonWai.Api.Controllers
{
    [ApiController]
    public class BoardController : BaseController
    {
        private readonly ThreadService threadService;

        public BoardController(Dc dc, ThreadService threadService) : base(dc)
        {
            this.threadService = threadService;
        }

        [NonAction]
        private Task<List<BoardDto>> GetBoardsTask()
        {
            var query = _dc.Boards
                .LoadWith(b => b.BoardSection)
                .Select(b => new BoardDto
                {
                    BoardId = b.BoardId,
                    BoardSection = new BoardSectionDto
                    {
                        BoardSectionId = b.BoardSectionId,
                        Name = b.BoardSection.Name
                    },
                    Path = b.Path,
                    Name = b.Name
                });
            
            return query.ToListAsync();
        }

        [HttpGet]
        [Route("api/boards")]
        public async Task<IActionResult> GetBoards()
        {
            var boards = await GetBoardsTask();

            return Ok(boards);
        }

        [HttpGet]
        [Route("api/boards/{boardPath}")]
        public async Task<IActionResult> GetBoardThreads(string boardPath, bool preview = true, int? page = null, int? pageSize = null)
        {
            var board = await _dc.Boards.FirstOrDefaultAsync(b => b.Path.Equals(boardPath));

            if (board == null)
                return NotFound(ErrorId.BoardNotFound);

            var query = threadService.GetThreads(_dc, board.BoardId, preview);
            
            return await PagedQueryResult(query, page, pageSize);
        }
    }
}
