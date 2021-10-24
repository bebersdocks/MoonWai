using System;

using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class User
    {
        [PrimaryKey, Identity] public int       UserId       { get; set; }
        [Column,      NotNull] public string    Username     { get; set; }
        [Column,      NotNull] public byte[]    PasswordSalt { get; set; }
        [Column,      NotNull] public byte[]    PasswordHash { get; set; }
        [Column,      NotNull] public DateTime  CreateDt     { get; set; }
        [Column,     Nullable] public DateTime? LastAccessDt { get; set; }

        #region Associations

        [Association(ThisKey="UserId", OtherKey="UserId", CanBeNull=false)]
        public UserSettings Settings { get; set; }

        #endregion
    }
}
