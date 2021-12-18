using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class Post
    {
        [PrimaryKey, Identity] public int       PostId     { get; set; }
        [Column,     Nullable] public int?      UserId     { get; set; }
        [Column,      NotNull] public int       ThreadId   { get; set; }
        [Column,      NotNull] public string    Message    { get; set; }
        [Column,      NotNull] public DateTime  CreateDt   { get; set; }
        [Column,     Nullable] public DateTime? LastEditDt { get; set; }

        #region QueryExpressions

        private static Expression<Func<Post, Dc, IQueryable<Media>>> MediaJoin =>
            (i, dc) => dc.Media.Where(j => j.SourceId == i.PostId && j.SourceType == MediaSourceType.Post);

        #endregion

        #region Associations

        [Association(ThisKey=nameof(UserId), OtherKey=nameof(UserId), Relationship=Relationship.OneToOne, CanBeNull=true)]
        public User User { get; set; }

        [Association(ThisKey=nameof(ThreadId), OtherKey=nameof(ThreadId), Relationship=Relationship.OneToOne, CanBeNull=false)]
        public Thread Thread { get; set; }

        [Association(QueryExpressionMethod=nameof(MediaJoin), Relationship=Relationship.OneToMany)]
        public IEnumerable<Media> Media { get; set; }

        #endregion
    }
}
