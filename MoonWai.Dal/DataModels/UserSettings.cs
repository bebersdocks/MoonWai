using System;

using LinqToDB.Mapping;

using MoonWai.Shared.Definitions;

namespace MoonWai.Dal.DataModels
{
    [Table("user_settings")]
    public class UserSettings
    {
        [Column("user_id"), PrimaryKey, NotNull] public int        UserId         { get; set; }
        [Column("language_id"),         NotNull] public LanguageId LanguageId     { get; set; }
        [Column("default_board_id"),    NotNull] public int        DefaultBoardId { get; set; }
        [Column("last_edit_date"),     Nullable] public DateTime?  LastEditDt     { get; set; }

        #region Associations

        [Association(ThisKey=nameof(UserId), OtherKey=nameof(UserId), CanBeNull=false)]
        public User User { get; set; }

        [Association(ThisKey=nameof(DefaultBoardId), OtherKey=nameof(Board.BoardId), CanBeNull=false)]
        public Board DefaultBoard { get; set; }

        #endregion
    }
}
