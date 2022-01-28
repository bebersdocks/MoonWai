using System;

using DalThread = MoonWai.Dal.DataModels.Thread;

namespace MoonWai.Api.Models.Thread
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

        public ThreadPreviewDto(DalThread thread)
        {
            ThreadId   = thread.ThreadId;
            ParentId   = thread.ParentId;
            Title      = thread.Title;
            Message    = thread.Post.Message;
            CreateDt   = thread.CreateDt;
            LastEditDt = thread.LastEditDt;
        }
    }
}
