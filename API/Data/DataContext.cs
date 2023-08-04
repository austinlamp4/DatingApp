using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    /* This is used as a service for the rest of the app */
    public class DataContext : DbContext
    {
        /*
        Auto generated constructor that takes in DbContextOptions.
        */
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; } //Represents tables inside the DB
    }
}