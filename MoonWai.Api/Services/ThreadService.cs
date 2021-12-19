using System;
using System.Linq;
using System.Threading.Tasks;

using LinqToDB;

using MoonWai.Dal;
using MoonWai.Dal.DataModels;
using MoonWai.Shared;
using MoonWai.Shared.Models;
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
                .LoadWith(i => i.Posts.Take(postsCount + 1))
                .Select(i => new ThreadDto
                {
                    ThreadId = i.ThreadId,
                    ParentId = i.ParentId,
                    Title = i.Title,
                    Posts = i.Posts
                        .Select(j => new PostDto
                        {
                            PostId = j.PostId,
                            Message = j.Message,
                            Media = j.Media 
                                .Select(k => new MediaDto
                                {
                                    Name = k.Name,
                                    Path = k.Path,
                                    Thumbnail = k.Thumbnail
                                })
                                .ToList(),
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
                .ContinueWith(i => tr.Commit());

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
                .ContinueWith(i => i.Result + dc.Update(post));

            await tr.CommitAsync();

            return updatedCount;
        }
    }
}
