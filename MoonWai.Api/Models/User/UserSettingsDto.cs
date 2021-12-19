using MoonWai.Dal.DataModels;

namespace MoonWai.Api.Models.User
{
    public class UserSettingsDto
    {
        public LanguageId LanguageId     { get; set; }
        public int        DefaultBoardId { get; set; }
    }
}
