using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MoonWai.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BaseController : Controller
    {
        protected readonly ILogger<BaseController> logger;

        public BaseController(ILogger<BaseController> logger) => this.logger = logger;
    }
}