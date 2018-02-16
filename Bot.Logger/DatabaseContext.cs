using System.Data.Entity;

namespace Bot.Logger
{
    public class LogDatabaseContext : DbContext
    {
        public LogDatabaseContext() : base("name=LogConnectionString")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<LogDatabaseContext>());
            Database.CommandTimeout = 300;
        }

        public DbSet<LogObject> Logs { get; set; }
        public DbSet<ExceptionLogObject> ExceptionLogs { get; set; }
    }
}
