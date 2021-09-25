using System;

using MoonWai.Dal.DataModels;

using LinqToDB;

namespace MoonWai.Dal
{
    public partial class Dc : LinqToDB.Data.DataConnection
	{
		public ITable<Board>        Boards        { get { return this.GetTable<Board>(); } }
		public ITable<Media>        Media         { get { return this.GetTable<Media>(); } }
        public ITable<Post>         Posts         { get { return this.GetTable<Post>(); } }
        public ITable<PostResponse> PostResponses { get { return this.GetTable<PostResponse>(); } }
        public ITable<Thread>       Threads       { get { return this.GetTable<Thread>(); } }
        public ITable<User>         Users         { get { return this.GetTable<User>(); } }

		public Dc(string configuration)
			: base(configuration)
		{
		}
    }
}
