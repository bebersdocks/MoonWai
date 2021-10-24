using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class Board
    {
        [PrimaryKey, Identity] public int    BoardId { get; set; }
        [Column,      NotNull] public string Path    { get; set; }
        [Column,      NotNull] public string Name    { get; set; }
    }
}
