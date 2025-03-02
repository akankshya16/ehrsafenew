using MedicationData;
using Microsoft.EntityFrameworkCore;
using UserData;

public class MasterDB : DbContext
{
    public MasterDB(DbContextOptions<MasterDB> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Medication> Medications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<User>()
            .ToTable("Users")
            .HasKey(u => u.Id);

       
        modelBuilder.Entity<Medication>()
            .ToTable("Medications")
            .HasKey(m => m.Id);

      
       modelBuilder.Entity<User>()
                .HasMany(u => u.Medication)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade); // configured a single user can have multi medications
    }
}
