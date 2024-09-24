using Microsoft.EntityFrameworkCore;

namespace DAL.DbModels
{
    public partial class DefaultDbContext : DbContext
    {
        public DefaultDbContext() { }
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Login);

                entity.HasIndex(e => e.Login)
                    .HasDatabaseName("Unique_Users_Login")
                    .IsUnique();

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Sault).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
