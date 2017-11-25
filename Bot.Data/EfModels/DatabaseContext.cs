using System.Data.Entity;

namespace Bot.Data.EfModels
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=ConnectionString")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DatabaseContext>());
            Database.CommandTimeout = 300;
        }

        public DbSet<EfCommuter> EfCommuters { get; set; }
        public DbSet<EfTrip> EfTrips { get; set; }
        public DbSet<EfTripRequest> EfTripRequests { get; set; }
        public DbSet<EfVehicle> EfVehicles { get; set; }
    }
}
