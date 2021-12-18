using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    public class PostResponse
    {
        [PrimaryKey(1), NotNull] public int PostId           { get; set; }
        [PrimaryKey(2), NotNull] public int ThreadId         { get; set; }
        [PrimaryKey(3), NotNull] public int RespondentPostId { get; set; }

        #region Associations

        [Association(ThisKey=nameof(PostId), OtherKey=nameof(PostId), Relationship=Relationship.OneToOne, CanBeNull=false)]
        public Post Post { get; set; }

        [Association(ThisKey=nameof(ThreadId), OtherKey=nameof(ThreadId), Relationship=Relationship.OneToOne, CanBeNull=false)]
        public Thread Thread { get; set; }

        [Association(ThisKey=nameof(RespondentPostId), OtherKey=nameof(PostId), Relationship=Relationship.OneToOne, CanBeNull=false)]
        public Post RespondentPost { get; set; }

        #endregion
    }
}
