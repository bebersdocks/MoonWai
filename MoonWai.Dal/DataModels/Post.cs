using System;

using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class Post
    {
        [PrimaryKey(1), NotNull] public int       PostId     { get; set; }
        [PrimaryKey(2), NotNull] public int       ThreadId   { get; set; }
        [PrimaryKey(3), NotNull] public int       BoardId    { get; set; }
        [Column,       Nullable] public int?      UserId     { get; set; }
        [Column,        NotNull] public string    Message    { get; set; }
        [Column,        NotNull] public DateTime  CreateDt   { get; set; }
        [Column,       Nullable] public DateTime? LastEditDt { get; set; }

        #region Associations

        [Association(ThisKey="UserId", OtherKey="UserId", CanBeNull=true)]
        public User User { get; set; }

        [Association(ThisKey="ThreadId", OtherKey="ThreadId", CanBeNull=false)]
        public Thread Thread { get; set; }

        [Association(ThisKey="BoardId", OtherKey="BoardId", CanBeNull=false)]
        public Board Board { get; set; }

        #endregion
    }
}
