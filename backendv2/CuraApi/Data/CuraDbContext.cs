using CuraApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuraApi.Data;

public class CuraDbContext : DbContext
{
    public CuraDbContext(DbContextOptions<CuraDbContext> options) : base(options) { }
    
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<MedicationPlan> MedicationPlans => Set<MedicationPlan>();
    public DbSet<MedicationIntake> MedicationIntakes => Set<MedicationIntake>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Patient relationships
        modelBuilder.Entity<Patient>()
            .HasMany(p => p.MedicationPlans)
            .WithOne(mp => mp.Patient)
            .HasForeignKey(mp => mp.PatientId);
            
        modelBuilder.Entity<Patient>()
            .HasMany(p => p.MedicationIntakes)
            .WithOne(mi => mi.Patient)
            .HasForeignKey(mi => mi.PatientId);
        
        // Medication relationships
        modelBuilder.Entity<Medication>()
            .HasMany(m => m.MedicationPlans)
            .WithOne(mp => mp.Medication)
            .HasForeignKey(mp => mp.MedicationId);
        
        // MedicationPlan relationships
        modelBuilder.Entity<MedicationPlan>()
            .HasMany(mp => mp.MedicationIntakes)
            .WithOne(mi => mi.MedicationPlan)
            .HasForeignKey(mi => mi.MedicationPlanId);
    }
}
