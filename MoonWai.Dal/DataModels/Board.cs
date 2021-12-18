using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class Board
    {
        [PrimaryKey, Identity] public int    BoardId        { get; set; }
        [Column,      NotNull] public int    BoardSectionId { get; set; }
        [Column,      NotNull] public string Path           { get; set; }
        [Column,      NotNull] public string Name           { get; set; }

        #region Associations

        [Association(ThisKey=nameof(BoardSectionId), OtherKey=nameof(BoardSectionId), Relationship=Relationship.OneToOne, CanBeNull=true)]
        public BoardSection BoardSection { get; set; }

        #endregion
    }
}
