using System;
using System.Collections.Generic;

using MoonWai.Api.Models.Post;

namespace MoonWai.Api.Models.Thread 
{
    public class ThreadDto
    {
        public int            ThreadId   { get; set; }
        public int            ParentId   { get; set; }
        public string         Title      { get; set; }
        public List<PostDto>  Posts      { get; set; }
        public int            PostsCount { get; set; }
        public DateTime       CreateDt   { get; set; }
    }
}
