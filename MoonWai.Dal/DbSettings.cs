namespace MoonWai.Dal
{
    public class DbSettings
    {
        public string UserId   { get; set; }   
        public string Password { get; set; }
        public string Server   { get; set; }
        public int    Port     { get; set; }
        public string Database { get; set; }

        public string GetConnStr()
        {
            var connStr = string.Format(
                "Server={0};Port={1};Database={2};User Id={3};Password={4};CommandTimeout=20",
                Server,
                Port,
                Database,
                UserId,
                Password);

            return connStr;
        }
    }
}
