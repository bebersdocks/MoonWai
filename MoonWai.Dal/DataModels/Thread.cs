using System;

using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class Thread
    {
        [PrimaryKey, Identity] public int       ThreadId   { get; set; }
        [Column,      NotNull] public int       BoardId    { get; set; }
        [Column,     Nullable] public int?      UserId     { get; set; }
        [Column,      NotNull] public string    Title      { get; set; }
        [Column,      NotNull] public string    Message    { get; set; }
        [Column,      NotNull] public DateTime  CreateDt   { get; set; }
        [Column,     Nullable] public DateTime? LastEditDt { get; set; }

        #region Associations

        [Association(ThisKey="BoardId", OtherKey="BoardId", CanBeNull=false)]
        public Board Board { get; set; }

        [Association(ThisKey="UserId", OtherKey="UserId", CanBeNull=true)]
        public User User { get; set; }

        #endregion
    }
}
