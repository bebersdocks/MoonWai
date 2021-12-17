using System;

namespace MoonWai.Shared.Models.Post
{
    public class PostDto
    {
        public int      PostId   { get; set; }
        public string   Message  { get; set; }
        public DateTime CreateDt { get; set; }
    }
}
