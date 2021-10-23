using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    [Table("board")]
    public class Board
    {
        [Column("board_id"), PrimaryKey, Identity] public int    BoardId { get; set; }
        [Column("path"),                  NotNull] public string Path    { get; set; }
        [Column("name"),                  NotNull] public string Name    { get; set; }
    }
}
