using CoreUserService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreUserService.Repositories
{
    /// <summary>
    /// The DomainDbContext implementation
    /// </summary>
    public sealed class DomainDbContext : DbContext
    {
        /// <summary>
        /// User db set
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// UserGuid db set
        /// </summary>
        public DbSet<TemporaryPasscode> TemporaryPasscodes { get; set; }

        /// <summary>
        /// UserRepository constructor
        /// </summary>
        public DomainDbContext(DbContextOptions options) : base(options)
        {
            //Run migrations on instance creation
            Database.Migrate();
        }

        /// <summary>
        /// Repository method to persist the context changes to the database
        /// </summary>
        /// <returns>bool, true if successful</returns>
        public bool Save()
        {
            return SaveChanges() >= 0;
        }

        /// <summary>
        /// Creates unique constraints on desired columns
        /// </summary>
        /// <param name="modelBuilder">ModelBuilder instance</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Build User.Username Unique Index
            modelBuilder.Entity<User>().HasIndex(c => c.Username).IsUnique();

            //Build User.Email Unique Index
            modelBuilder.Entity<User>().HasIndex(c => c.Email).IsUnique();

            //Build UserGuid.Guid Unique Index
            modelBuilder.Entity<TemporaryPasscode>().HasIndex(c => c.Passcode).IsUnique();

            //Define User/Address Relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Address)
                .WithOne(a => a.User)
                .HasForeignKey<Address>(u => u.UserId);
        }
    }
}
