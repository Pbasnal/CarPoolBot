using Bot.Common;
using Bot.Data.Models;
using Bot.Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Bot.Data.EfDatastores;

namespace Bot.Data.EfModels
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=DbConnectionString")
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
