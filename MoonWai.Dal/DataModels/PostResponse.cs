using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    [Table("post_response")]
    public class PostResponse
    {
        [Column("post_id"),             PrimaryKey(1), NotNull] public int PostId             { get; set; }
        [Column("thread_id"),           PrimaryKey(2), NotNull] public int ThreadId           { get; set; }
        [Column("board_id"),            PrimaryKey(3), NotNull] public int BoardId            { get; set; }
        [Column("respondent_post_id"),  PrimaryKey(4), NotNull] public int RespondentPostId   { get; set; }
        [Column("respondent_threadId"), PrimaryKey(5), NotNull] public int RespondentThreadId { get; set; }
        [Column("respondent_board_id"), PrimaryKey(6), NotNull] public int RespondentBoardId  { get; set; }

        #region Associations

        [Association(ThisKey="", OtherKey="", CanBeNull=false)]
        public Post Post { get; set; }

        [Association(ThisKey="", OtherKey="", CanBeNull=false)]
        public Post RespondentPost { get; set; }

        #endregion
    }
}
