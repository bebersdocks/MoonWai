namespace MoonWai.Api.Models.Board
{
    public class BoardDto
    {        
        public int             BoardId      { get; set; }
        public BoardSectionDto BoardSection { get; set; }
        public string          Path         { get; set; }
        public string          Name         { get; set; }
    }
}
