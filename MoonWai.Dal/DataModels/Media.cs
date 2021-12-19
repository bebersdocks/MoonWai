using System;

using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public enum MediaSourceType
    {
        System = 0,
        Post = 1
    }

    public class Media
    {
        [PrimaryKey, Identity] public int             MediaId    { get; set; }
        [Column,     Nullable] public int             SourceId   { get; set; }
        [Column,      NotNull] public MediaSourceType SourceType { get; set; }
        [Column,      NotNull] public string          Name       { get; set; }
        [Column,      NotNull] public string          Path       { get; set; }
        [Column,      NotNull] public string          Thumbnail  { get; set; }
        [Column,      NotNull] public DateTime        CreateDt   { get; set; }
    }
}
