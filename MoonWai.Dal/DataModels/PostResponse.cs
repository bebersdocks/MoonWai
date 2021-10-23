using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class PostResponse
    {
        [PrimaryKey(1), NotNull] public int PostId             { get; set; }
        [PrimaryKey(2), NotNull] public int ThreadId           { get; set; }
        [PrimaryKey(3), NotNull] public int BoardId            { get; set; }
        [PrimaryKey(4), NotNull] public int RespondentPostId   { get; set; }
        [PrimaryKey(5), NotNull] public int RespondentThreadId { get; set; }
        [PrimaryKey(6), NotNull] public int RespondentBoardId  { get; set; }

        #region Associations

        [Association(ThisKey="PostId", OtherKey="PostId", CanBeNull=false)]
        public Post Post { get; set; }

        [Association(ThisKey="ThreadId", OtherKey="ThreadId", CanBeNull=false)]
        public Thread Thread { get; set; }

        [Association(ThisKey="BoardId", OtherKey="BoardId", CanBeNull=false)]
        public Board Board { get; set; }

        [Association(ThisKey="RespondentPostId", OtherKey="PostId", CanBeNull=false)]
        public Post RespondentPost { get; set; }

        [Association(ThisKey="RespondentThreadId", OtherKey="RespondentThreadId", CanBeNull=false)]
        public Thread RespondentThread { get; set; }

        [Association(ThisKey="RespondentBoardId", OtherKey="RespondentBoardId", CanBeNull=false)]
        public Board RespondentBoard { get; set; }

        #endregion
    }
}
