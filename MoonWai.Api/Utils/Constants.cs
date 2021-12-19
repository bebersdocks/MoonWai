using System;

namespace MoonWai.Api.Utils
{
    public static class Constants
    {
        public static readonly TimeSpan AllowedEditTime = TimeSpan.FromMinutes(5.0);

        public const int DefaultBoardId        = 0;
        public const int DefaultThreadsPerPage = 15;
        public const int MaxPostsPerThread     = 1000;
        public const int MinPasswordLength     = 8;
        public const int PostsInPreview        = 5;
    }
}
