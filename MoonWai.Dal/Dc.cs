using System;
using System.Linq;

using LinqToDB;

using MoonWai.Dal.DataModels;

namespace MoonWai.Dal
{
    public partial class Dc : LinqToDB.Data.DataConnection
    {
        public ITable<Board>        Boards        => this.GetTable<Board>();
        public ITable<BoardSection> BoardSections => this.GetTable<BoardSection>();
        public ITable<Media>        Media         => this.GetTable<Media>();
        public ITable<Post>         Posts         => this.GetTable<Post>();
        public ITable<PostResponse> PostResponses => this.GetTable<PostResponse>();
        public ITable<Thread>       Threads       => this.GetTable<Thread>();
        public ITable<User>         Users         => this.GetTable<User>();
        public ITable<UserSettings> UserSettings  => this.GetTable<UserSettings>();
   
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

            createTable(BoardSections);
            createTable(Boards);
            createTable(Media);
            createTable(Posts);
            createTable(PostResponses);
            createTable(Threads);
            createTable(Users);
            createTable(UserSettings);
        }
    }
}
