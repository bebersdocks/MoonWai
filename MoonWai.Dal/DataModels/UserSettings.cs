using System;

using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public enum LanguageId 
    {
        English,
        Russian
    }

    public class UserSettings
    {
        [PrimaryKey, NotNull] public int        UserId         { get; set; }
        [Column,     NotNull] public LanguageId LanguageId     { get; set; }
        [Column,     NotNull] public int        DefaultBoardId { get; set; }
        [Column,    Nullable] public DateTime?  LastEditDt     { get; set; }

        #region Associations

        [Association(ThisKey=nameof(UserId), OtherKey=nameof(UserId), Relationship=Relationship.OneToOne, CanBeNull=false)]
        public User User { get; set; }

        [Association(ThisKey=nameof(DefaultBoardId), OtherKey=nameof(Board.BoardId), Relationship=Relationship.OneToOne, CanBeNull=false)]
        public Board DefaultBoard { get; set; }

        #endregion
    }
}
