using MoonWai.Api.Definitions;

namespace MoonWai.Api.Models
{
    public class ErrorDto
    {        
        public ErrorId ErrorId { get; set; }
        public string  Message { get; set; }
    }
}
