using System;
using System.Linq;
using System.Threading.Tasks;

using LinqToDB;

using MoonWai.Dal;
using MoonWai.Dal.DataModels;
using MoonWai.Shared;
using MoonWai.Shared.Models.Post;
using MoonWai.Shared.Models.Thread;

namespace MoonWai.Api.Services
{
    public class ThreadService
    {
        private IQueryable<ThreadDto> GetThreads(Dc dc, bool preview = true)
        {
            var postsCount = preview ? Constants.PostsInPreview : Constants.MaxPostsPerThread;

            var query = dc.Threads
                .LoadWith(i => i.Posts.Take(postsCount))
                .Select(i => new ThreadDto
                {
                    ThreadId = i.ThreadId,
                    ParentId = i.ParentId,
                    Title = i.Title,
                    Message = i.Message,
                    Posts = i.Posts
                        .Select(j => new PostDto
                        {
                            PostId = j.PostId,
                            Message = j.Message,
                            CreateDt = j.CreateDt
                        })
                        .ToList(),
                    PostsCount = i.Posts.Count(),
                    CreateDt = i.CreateDt
                });

            return query;
        }
        
        public Task<ThreadDto> GetThread(Dc dc, int threadId)
        {   
            var query = GetThreads(dc, false);

            return query.FirstOrDefaultAsync(i => i.ThreadId == threadId);
        }

        public IQueryable<ThreadDto> GetThreads(Dc dc, int boardId, bool preview = true)
        {   
            var query = GetThreads(dc, preview);

            return query;
        }

        public Task<int> InsertThread(Dc dc, InsertThreadDto insertThreadDto)
        {
            var newThread = new Thread();

            newThread.Title    = insertThreadDto.Title;
            newThread.Message  = insertThreadDto.Message;
            newThread.BoardId  = insertThreadDto.BoardId;
            newThread.UserId   = insertThreadDto.UserId;
            newThread.CreateDt = DateTime.UtcNow;

            return dc.InsertWithInt32IdentityAsync(newThread);
        }

        public Task<int> UpdateThread(Dc dc, Thread thread, UpdateThreadDto updateThreadDto)
        {
            thread.Title   = updateThreadDto.Title;
            thread.Message = updateThreadDto.Message;

            return dc.UpdateAsync(thread);
        }
    }
}
