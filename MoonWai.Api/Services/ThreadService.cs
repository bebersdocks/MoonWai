using System;
using System.Linq;
using System.Threading.Tasks;

using LinqToDB;

using MoonWai.Api.Models;
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

            var query = dc.Threads
                .LoadWith(t => t.Posts.Take(postsCount + 1))
                .Select(t => new ThreadDto
                {
                    ThreadId = t.ThreadId,
                    ParentId = t.ParentId,
                    Title = t.Title,
                    Posts = t.Posts
                        .Select(p => new PostDto
                        {
                            PostId = p.PostId,
                            Message = p.Message,
                            Media = p.Media 
                                .Select(m => new MediaDto
                                {
                                    Name = m.Name,
                                    Path = m.Path,
                                    Thumbnail = m.Thumbnail
                                })
                                .ToList(),
                            RespondentPostIds = p.Responses.Select(r => r.RespondentPostId).ToList(),
                            CreateDt = p.CreateDt,
                        })
                        .ToList(),
                    PostsCount = t.Posts.Count(),
                    CreateDt = t.CreateDt
                });

            return query;
        }
        
        public Task<ThreadDto> GetThread(Dc dc, int threadId)
        {   
            var query = GetThreads(dc, false);

            return query.FirstOrDefaultAsync(t => t.ThreadId == threadId);
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
