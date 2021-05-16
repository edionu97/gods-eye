using Microsoft.EntityFrameworkCore;
using GodsEye.Application.Persistence.Models;

namespace GodsEye.Application.Persistence.DatabaseContext
{
    public partial class GodsEyeDatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Create the constructor of the db context, and pass the connection string to it
        /// </summary>
        /// <param name="connectionString">the connection string</param>
        public GodsEyeDatabaseContext(string connectionString)
            : base(GetConnectionOptions(connectionString))
        {

        }

        /// <summary>
        /// This method it is called before the model to be created
        /// </summary>
        /// <param name="modelBuilder">the model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //ensure that the username is unique
            modelBuilder
                .Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }

    }
}
