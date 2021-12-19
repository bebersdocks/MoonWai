using System;
using System.Collections.Generic;

namespace MoonWai.Api.Models.Post
{
    public class PostDto
    {
        public int            PostId            { get; set; }
        public string         Message           { get; set; }
        public List<MediaDto> Media             { get; set; }
        public List<int>      RespondentPostIds { get; set; }
        public DateTime       CreateDt          { get; set; }
    }
}
