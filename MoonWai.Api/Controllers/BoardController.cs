using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using LinqToDB;

using MoonWai.Dal;

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
        [Route("board/{boardId:int}/threads")]
        public async Task<IActionResult> GetBoardThreads(int boardId)
        {
            using var dc = new Dc();

            var threads = await dc.Threads.Where(i => i.BoardId == boardId).ToListAsync();

            return Ok(threads);
        }
    }
}
