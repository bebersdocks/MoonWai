using System;
using System.Collections.Generic;

using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    [Table("thread")]
    public class Thread
    {
        [Column("thread_id"),   PrimaryKey(1), NotNull] public int       ThreadId   { get; set; }
        [Column("board_id"),    PrimaryKey(2), NotNull] public int       BoardId    { get; set; }
        [Column("user_id"),                   Nullable] public int?      UserId     { get; set; }
        [Column("title"),                      NotNull] public string    Title      { get; set; }
        [Column("message"),                    NotNull] public string    Message    { get; set; }
        [Column("create_date"),                NotNull] public DateTime  CreateDt   { get; set; }
        [Column("last_edit_date"),            Nullable] public DateTime? LastEditDt { get; set; }

        #region Associations

        [Association(ThisKey=nameof(BoardId), OtherKey=nameof(BoardId), CanBeNull=false)]
        public Board Board { get; set; }

        [Association(ThisKey=nameof(UserId), OtherKey=nameof(UserId), CanBeNull=true)]
        public User User { get; set; }

        [Association(ThisKey="", OtherKey="", CanBeNull=false)]
        public IEnumerable<Post> Posts { get; set; }

        #endregion
    }
}
