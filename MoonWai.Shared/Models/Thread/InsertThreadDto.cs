namespace MoonWai.Shared.Models.Thread 
{
    public class InsertThreadDto
    {
        public string Title   { get; set; }
        public string Message { get; set; }
        public int    BoardId { get; set; }
        public int?   UserId  { get; set; }
    }
}
