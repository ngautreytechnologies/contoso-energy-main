using ContosoEnergy.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContosoEnergy.Infrastructure.Data
{
    public class ContosoDbContext(DbContextOptions<ContosoDbContext> options) : DbContext(options)
    {
        public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(e => e.CreatedAt)
                      .IsRequired();
            });
        }
    }
}
