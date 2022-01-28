using MoonWai.Dal.DataModels;

using DalBoard = MoonWai.Dal.DataModels.Board;

namespace MoonWai.Api.Models.Board
{
    public class BoardDto
    {        
        public int             BoardId      { get; set; }
        public BoardSectionDto BoardSection { get; set; }
        public string          Path         { get; set; }
        public string          Name         { get; set; }

        public BoardDto(DalBoard board, BoardSection boardSection)
        {
            BoardId      = board.BoardId;
            BoardSection = new BoardSectionDto(boardSection);
            Path         = board.Path;
            Name         = board.Name;
        }
    }
}
