using System;

using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public enum LanguageId
    {
        English,
        Russian
    }

    public class User
    {
        [PrimaryKey, Identity] public int        UserId       { get; set; }
        [Column,      NotNull] public string     Username     { get; set; }
        [Column,      NotNull] public byte[]     PasswordSalt { get; set; }
        [Column,      NotNull] public byte[]     PasswordHash { get; set; }
        [Column,      NotNull] public LanguageId LanguageId   { get; set; }
        [Column,      NotNull] public DateTime   CreateDt     { get; set; }
        [Column,     Nullable] public DateTime?  LastAccessDt { get; set; }
    }
}
