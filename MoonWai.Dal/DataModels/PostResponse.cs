using LinqToDB.Mapping;

namespace MoonWai.Dal.DataModels
{
    [Table(Schema="dbo", Name="Post")]
    public class PostResponse
    {
        [PrimaryKey(1), NotNull] public int PostId           { get; set; }
        [PrimaryKey(2), NotNull] public int ThreadId         { get; set; }
        [PrimaryKey(3), NotNull] public int RespondentPostId { get; set; }

        #region Associations

		[Association(ThisKey="PostId", OtherKey="PostId", CanBeNull=false)]
		public Post Post { get; set; }

        [Association(ThisKey="ThreadId", OtherKey="ThreadId", CanBeNull=false)]
		public Thread Thread { get; set; }

        [Association(ThisKey="RespondentPostId", OtherKey="PostId", CanBeNull=false)]
		public Post RespondentPost { get; set; }

		#endregion
    }
}
