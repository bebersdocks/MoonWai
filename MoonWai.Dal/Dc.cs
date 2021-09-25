using System;
using System.Linq;

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

        private const string defaultConfigurationStr = "MoonWai";

        public static void CreateDefaultConfiguration(DbSettings dbSettings)
        {
            var connStr = dbSettings.GetConnStr();

            var dataProvider = GetDataProvider(ProviderName.PostgreSQL, connStr);
            
            if (dataProvider == null) 
                throw new Exception("Unable to get data provider");

            AddConfiguration(defaultConfigurationStr, connStr, dataProvider);
        }

        public Dc() : base(defaultConfigurationStr) { }

		public Dc(string configurationStr) : base(configurationStr) { }

        public void CreateTables()
        {
            var schemaProvider = DataProvider.GetSchemaProvider();
            var dbSchema = schemaProvider.GetSchema(this);

            void createTable<T>(ITable<T> table)
            {
                if (!dbSchema.Tables.Any(i => i.TableName == table.TableName))
                {
                    this.CreateTable<T>();
                }
            }

            createTable(Boards);
            createTable(Media);
            createTable(Posts);
            createTable(PostResponses);
            createTable(Threads);
            createTable(Users);
        }
    }
}
