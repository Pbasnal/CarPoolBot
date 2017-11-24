using System.Data.Entity;

namespace Bot.Data.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=ConnectionString")
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<DatabaseContext>());
        }

        public DbSet<Commuter> Commuters { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripRequest> TripRequests { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
    }
}
