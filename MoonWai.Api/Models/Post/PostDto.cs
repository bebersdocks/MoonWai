using System;
using System.Collections.Generic;
using System.Linq;

using MoonWai.Dal.DataModels;

using DalPost = MoonWai.Dal.DataModels.Post;

namespace MoonWai.Api.Models.Post
{
    public class PostDto
    {
        public int            PostId            { get; set; }
        public string         Message           { get; set; }
        public List<MediaDto> Media             { get; set; }
        public List<int>      RespondentPostIds { get; set; }
        public DateTime       CreateDt          { get; set; }

        public PostDto() {}

        public PostDto(DalPost post, IEnumerable<Media> media = null, IEnumerable<PostResponse> responses = null)
        {
            PostId  = post.PostId;
            Message = post.Message;

            Media = media?
                .Select(m => new MediaDto(m))
                .ToList();

            RespondentPostIds = responses?
                .Select(r => r.RespondentPostId)
                .ToList();

            CreateDt = post.CreateDt;
        }
    }
}
