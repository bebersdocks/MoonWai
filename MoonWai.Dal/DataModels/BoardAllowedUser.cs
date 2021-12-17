using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class BoardAllowedUser
    {
        [PrimaryKey(0), NotNull] public int BoardId { get; set; }
        [PrimaryKey(1), NotNull] public int UserId  { get; set; }

        #region Associations

        [Association(ThisKey=nameof(BoardId), OtherKey=nameof(BoardId), CanBeNull=false)]
        public Board Board { get; set; }

        [Association(ThisKey=nameof(UserId), OtherKey=nameof(UserId), CanBeNull=false)]
        public User User { get; set; }

        #endregion
    }
}
