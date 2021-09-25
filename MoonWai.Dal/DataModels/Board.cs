using System;

using LinqToDB;
using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    [Table(Schema="dbo", Name="Board")]
    public class Board
    {
        [PrimaryKey, Identity] public int    BoardId { get; set; }
        [Column,      NotNull] public string Name    { get; set; }
    }
}
