using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Api.Definitions;
using MoonWai.Api.Models;
using MoonWai.Api.Models.Post;
using MoonWai.Dal;

namespace MoonWai.Api.Controllers
{
    [ApiController]
    public class PostController : BaseController
    {
        public PostController(Dc dc) : base(dc) {}

        [HttpGet]
        [Route("api/posts/{postId:int}")]
        public async Task<IActionResult> GetPost(int postId)
        {
            var post = await _dc.Posts
                .LoadWith(p => p.Media)
                .LoadWith(p => p.Responses)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
                return NotFound(ErrorId.PostNotFound);

            var postDto = new PostDto
            {
                PostId = post.PostId,
                Message = post.Message,
                Media = post.Media 
                    .Select(m => new MediaDto
                    {
                        Name = m.Name,
                        Path = m.Path,
                        Thumbnail = m.Thumbnail
                    })
                    .ToList(),
                RespondentPostIds = post.Responses.Select(r => r.RespondentPostId).ToList(),
                CreateDt = post.CreateDt,
            };

            return Ok(postDto);
        }
    }
}
