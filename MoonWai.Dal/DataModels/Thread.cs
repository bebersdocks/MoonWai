using System;
using System.Collections.Generic;

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

        #region Associations

        [Association(ThisKey=nameof(ParentId), OtherKey=nameof(ThreadId), CanBeNull = true)]
        public Thread ParentThread { get; set; }

        [Association(ThisKey=nameof(BoardId), OtherKey=nameof(BoardId), CanBeNull=false)]
        public Board Board { get; set; }

        [Association(ThisKey=nameof(UserId), OtherKey=nameof(UserId), CanBeNull=true)]
        public User User { get; set; }

        [Association(ThisKey=nameof(ThreadId), OtherKey=nameof(ThreadId), CanBeNull=false)]
        public IEnumerable<Post> Posts { get; set; }

        #endregion
    }
}
