using System;

namespace MoonWai.Shared.Models.Thread
{
    public class ThreadPreviewDto
    {
        public int       ThreadId   { get; set; }
        public int       ParentId   { get; set; }
        public string    Title      { get; set; }
        public string    Message    { get; set; }
        public int       PostsCount { get; set; }
        public DateTime  CreateDt   { get; set; }
        public DateTime? LastEditDt { get; set; }
    }
}
