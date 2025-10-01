using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<RfidChip> RfidChips { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Caregiver> Caregivers { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicationPlan> MedicationPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // RfidChip Configuration
            modelBuilder.Entity<RfidChip>(entity =>
            {
                entity.ToTable("rfid_chips");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ChipId).HasColumnName("chip_id").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Weekday).HasColumnName("weekday").IsRequired().HasMaxLength(20);
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
                
                entity.HasIndex(e => e.ChipId).IsUnique();
            });

            // Location Configuration
            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("locations");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Street).HasColumnName("street").IsRequired().HasMaxLength(255);
                entity.Property(e => e.HouseNumber).HasColumnName("house_number").IsRequired().HasMaxLength(10);
                entity.Property(e => e.PostalCode).HasColumnName("postal_code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.City).HasColumnName("city").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Floor).HasColumnName("floor").HasMaxLength(10);
            });

            // Medication Configuration
            modelBuilder.Entity<Medication>(entity =>
            {
                entity.ToTable("medications");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(255);
                entity.Property(e => e.ActiveIngredient).HasColumnName("active_ingredient").HasMaxLength(255);
            });

            // Caregiver Configuration
            modelBuilder.Entity<Caregiver>(entity =>
            {
                entity.ToTable("caregivers");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(50);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.HasOne(e => e.Location)
                    .WithMany(l => l.Caregivers)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Patient Configuration
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("patients");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(255);
                entity.Property(e => e.Age).HasColumnName("age");
                entity.Property(e => e.LocationId).HasColumnName("location_id");
                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(50);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);

                entity.HasOne(e => e.Location)
                    .WithMany(l => l.Patients)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // MedicationPlan Configuration
            modelBuilder.Entity<MedicationPlan>(entity =>
            {
                entity.ToTable("medication_plans");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PatientId).HasColumnName("patient_id").IsRequired();
                entity.Property(e => e.MedicationId).HasColumnName("medication_id").IsRequired();
                entity.Property(e => e.CaregiverId).HasColumnName("caregiver_id");
                entity.Property(e => e.WeekdayFlags).HasColumnName("weekday_flags").HasDefaultValue(0);
                entity.Property(e => e.DayTimeFlags).HasColumnName("day_time_flags").HasDefaultValue(0);
                entity.Property(e => e.Quantity).HasColumnName("quantity").IsRequired();
                entity.Property(e => e.ValidFrom).HasColumnName("valid_from").IsRequired();
                entity.Property(e => e.ValidTo).HasColumnName("valid_to");
                entity.Property(e => e.Notes).HasColumnName("notes");
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);

                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.MedicationPlans)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Medication)
                    .WithMany(m => m.MedicationPlans)
                    .HasForeignKey(e => e.MedicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Caregiver)
                    .WithMany(c => c.MedicationPlans)
                    .HasForeignKey(e => e.CaregiverId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}