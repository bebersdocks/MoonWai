using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class BoardSection
    {
        [PrimaryKey, Identity] public int    BoardSectionId { get; set; }
        [Column,      NotNull] public string Name           { get; set; }
    }
}
