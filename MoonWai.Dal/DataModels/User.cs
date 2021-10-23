using System;

using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    [Table("user")]
    public class User
    {
        [Column("user_id"), PrimaryKey, Identity] public int       UserId       { get; set; }
        [Column("username"),             NotNull] public string    Username     { get; set; }
        [Column("password_salt"),        NotNull] public byte[]    PasswordSalt { get; set; }
        [Column("password_hash"),        NotNull] public byte[]    PasswordHash { get; set; }
        [Column("create_date"),          NotNull] public DateTime  CreateDt     { get; set; }
        [Column("last_access_date"),    Nullable] public DateTime? LastAccessDt { get; set; }

        #region Associations

        [Association(ThisKey=nameof(UserId), OtherKey=nameof(UserId), CanBeNull=false)]
        public UserSettings Settings { get; set; }

        #endregion
    }
}
