using System;
using System.Collections.Generic;

namespace MoonWai.Shared.Models.Post
{
    public class PostDto
    {
        public int            PostId   { get; set; }
        public string         Message  { get; set; }
        public List<MediaDto> Media    { get; set; }
        public DateTime       CreateDt { get; set; }
    }
}
