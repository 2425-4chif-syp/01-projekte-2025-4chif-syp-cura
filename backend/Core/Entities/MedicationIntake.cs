using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    /// <summary>
    /// Logs when a medication was actually taken by a patient
    /// </summary>
    public class MedicationIntake : EntityObject
    {
        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        
        [Required]
        [ForeignKey(nameof(MedicationPlan))]
        public int MedicationPlanId { get; set; }
        
        [Required]
        public DateTime IntakeTime { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        public string? Notes { get; set; }
        
        /// <summary>
        /// RFID Tag that was used to log this intake (if applicable)
        /// </summary>
        public string? RfidTag { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; } = null!;
        public MedicationPlan MedicationPlan { get; set; } = null!;

        public override string ToString()
        {
            return $"{Patient?.Name} - {MedicationPlan?.Medication?.Name} at {IntakeTime:yyyy-MM-dd HH:mm}";
        }
    }
}
