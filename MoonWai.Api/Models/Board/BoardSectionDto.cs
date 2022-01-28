using MoonWai.Dal.DataModels;

namespace MoonWai.Api.Models.Board
{
    public class BoardSectionDto
    {
        public int    BoardSectionId { get; set; }
        public string Name           { get; set; }

        public BoardSectionDto(BoardSection boardSection)
        {
            BoardSectionId = boardSection.BoardSectionId;
            Name           = boardSection.Name;
        }
    }
}
