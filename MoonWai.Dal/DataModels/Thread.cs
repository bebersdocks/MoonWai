using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB;
using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class Thread
    {
        [PrimaryKey, Identity] public int       ThreadId   { get; set; }
        [Column,     Nullable] public int       ParentId   { get; set; }
        [Column,      NotNull] public int       BoardId    { get; set; }
        [Column,     Nullable] public int?      UserId     { get; set; }
        [Column,      NotNull] public string    Title      { get; set; }
        [Column,      NotNull] public string    Message    { get; set; }
        [Column,      NotNull] public DateTime  CreateDt   { get; set; }
        [Column,     Nullable] public DateTime? LastEditDt { get; set; }

        #region QueryExpressions

        private static Expression<Func<Thread, Dc, IQueryable<Media>>> MediaJoin =>
            (i, dc) => dc.Media.Where(j => j.SourceId == i.ThreadId && j.SourceType == MediaSourceType.Thread);

        #endregion

        #region Associations

        [Association(ThisKey=nameof(ParentId), OtherKey=nameof(ThreadId), Relationship=Relationship.OneToOne, CanBeNull=true)]
        public Thread ParentThread { get; set; }

        [Association(ThisKey=nameof(BoardId), OtherKey=nameof(BoardId), Relationship=Relationship.OneToOne, CanBeNull=false)]
        public Board Board { get; set; }

        [Association(ThisKey=nameof(UserId), OtherKey=nameof(UserId), Relationship=Relationship.OneToOne, CanBeNull=true)]
        public User User { get; set; }

        [Association(QueryExpressionMethod=nameof(MediaJoin), Relationship=Relationship.OneToMany)]
        public IEnumerable<Media> Media { get; set; }

        [Association(ThisKey=nameof(ThreadId), OtherKey=nameof(ThreadId), Relationship=Relationship.OneToMany)]
        public IEnumerable<Post> Posts { get; set; }

        #endregion
    }
}
