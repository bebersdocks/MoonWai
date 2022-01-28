using System;
using System.Linq;
using System.Threading.Tasks;

using LinqToDB;

using MoonWai.Api.Models.Post;
using MoonWai.Api.Models.Thread;
using MoonWai.Api.Utils;
using MoonWai.Dal;
using MoonWai.Dal.DataModels;

namespace MoonWai.Api.Services
{
    public class ThreadService
    {
        private IQueryable<ThreadDto> GetThreads(Dc dc, bool preview = true)
        {
            var postsCount = preview ? Constants.PostsInPreview : Constants.MaxPostsPerThread;

            return dc.Threads
                .Select(t => new ThreadDto
                {
                    ThreadId = t.ThreadId,
                    ParentId = t.ParentId,
                    Title = t.Title,
                    Posts = t.Posts
                        .Select(p => new PostDto(p, p.Media, p.Responses))
                        .Take(postsCount + 1)
                        .ToList(),
                    PostsCount = t.Posts.Count(),
                    CreateDt = t.CreateDt
                });
        }
        
        public Task<ThreadDto> GetThread(Dc dc, int threadId)
        {   
            return GetThreads(dc, false).FirstOrDefaultAsync(t => t.ThreadId == threadId);
        }

        public IQueryable<ThreadDto> GetThreads(Dc dc, int boardId, bool preview = true)
        {   
            return GetThreads(dc, preview);
        }

        public async Task<int> InsertThread(Dc dc, InsertThreadDto insertThreadDto)
        {
            var utcNow = DateTime.UtcNow;

            var newThread = new Thread();

            newThread.Title    = insertThreadDto.Title;
            newThread.BoardId  = insertThreadDto.BoardId;
            newThread.UserId   = insertThreadDto.UserId;
            newThread.CreateDt = utcNow;

            using var tr = await dc.BeginTransactionAsync();

            var newThreadId = await dc.InsertWithInt32IdentityAsync(newThread);

            if (newThreadId < 0)
            {
                await tr.RollbackAsync();
                return newThreadId;
            }

            var newPost = new Post();

            newPost.ThreadId = newThreadId;
            newPost.Message  = insertThreadDto.Message;
            newPost.UserId   = insertThreadDto.UserId;
            newPost.CreateDt = utcNow;

            await dc
                .InsertWithInt32IdentityAsync(newPost)
                .ContinueWith(t => tr.Commit());

            return newThreadId;
        }

        public async Task<int> UpdateThread(Dc dc, Thread thread, Post post, UpdateThreadDto updateThreadDto)
        {
            var utcNow = DateTime.UtcNow;

            thread.Title      = updateThreadDto.Title;
            thread.LastEditDt = utcNow;

            post.Message    = updateThreadDto.Message;
            post.LastEditDt = utcNow;

            using var tr = await dc.BeginTransactionAsync();
            
            var updatedCount = await dc
                .UpdateAsync(thread)
                .ContinueWith(t => t.Result + dc.Update(post));

            await tr.CommitAsync();

            return updatedCount;
        }
    }
}
