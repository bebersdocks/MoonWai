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

        public BoardController(ThreadService threadService)
        {
            this.threadService = threadService;
        }

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

        [HttpGet]
        [Route("api/boards/{boardPath}")]
        public async Task<IActionResult> GetBoardThreads(string boardPath, bool preview = true, int? page = null, int? pageSize = null)
        {
            using var dc = new Dc();

            var board = await dc.Boards.FirstOrDefaultAsync(i => i.Path.Equals(boardPath));

            if (board == null)
                return NotFound(ErrorId.BoardNotFound);

            var query = threadService.GetThreads(dc, board.BoardId, preview);
            
            return await PagedQueryResult(query, page, pageSize);
        }
    }
}
