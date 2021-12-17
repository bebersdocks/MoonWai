using MoonWai.Shared.Definitions;

namespace MoonWai.Shared.Models.User
{
    public class UserSettingsDto
    {
        public LanguageId LanguageId     { get; set; }
        public int        DefaultBoardId { get; set; }
    }
}
