using MoonWai.Shared.Definitions;

namespace MoonWai.Shared.Models
{
    public class ErrorDto
    {        
        public TranslationId ErrorId { get; set; }
        public string        Message { get; set; }
    }
}
