using System;

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

        #region Associations

        [Association(ThisKey="UserId", OtherKey="UserId", CanBeNull=true)]
        public User User { get; set; }

        [Association(ThisKey="ThreadId", OtherKey="ThreadId", CanBeNull=false)]
        public Thread Thread { get; set; }

        #endregion
    }
}
