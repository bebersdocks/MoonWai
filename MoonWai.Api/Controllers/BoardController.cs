using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using LinqToDB;

using MoonWai.Api.Resources.Translations;
using MoonWai.Dal;
using MoonWai.Shared.Definitions;

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

        [HttpGet]
        [Route("board/{boardPath}/threads")]
        public async Task<IActionResult> GetBoardThreads(string boardPath)
        {
            using var dc = new Dc();

            var board = await dc.Boards.FirstOrDefaultAsync(i => i.Path.Equals(boardPath));

            if (board == null)
                return NotFound(TranslationId.BoardNotFound);

            var threads = await dc.Threads.Where(i => i.BoardId == board.BoardId).ToListAsync();

            return Ok(threads);
        }

        [HttpGet]
        [Route("board/{boardId:int}/threads")]
        public async Task<IActionResult> GetBoardThreads(int boardId)
        {
            using var dc = new Dc();

            var threads = await dc.Threads.Where(i => i.BoardId == boardId).ToListAsync();

            return Ok(threads);
        }
    }
}
