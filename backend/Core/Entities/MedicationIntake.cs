using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    /// <summary>
    /// Tracks medication intake events from the database
    /// Records when medications were taken via RFID drawer opening
    /// </summary>
    public class MedicationIntake : EntityObject
    {
        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        
        /// <summary>
        /// Medication plan that was followed (null if drawer opening tracked all meds at once)
        /// </summary>
        [ForeignKey(nameof(MedicationPlan))]
        public int? MedicationPlanId { get; set; }
        
        /// <summary>
        /// Exact timestamp when medication was taken
        /// </summary>
        [Required]
        public DateTime IntakeTime { get; set; }
        
        /// <summary>
        /// Quantity of medication taken
        /// </summary>
        [Required, Range(1, 100)]
        public int Quantity { get; set; }
        
        /// <summary>
        /// RFID Tag that was used (weekday chip)
        /// </summary>
        public string? RfidTag { get; set; }
        
        public string? Notes { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; } = null!;
        public MedicationPlan? MedicationPlan { get; set; }

        public override string ToString()
        {
            var hour = IntakeTime.Hour;
            var timeOfDay = hour switch
            {
                >= 6 and < 11 => "Morning",
                >= 11 and < 14 => "Noon",
                >= 18 and < 22 => "Evening",
                >= 22 or < 6 => "Night",
                _ => "Unknown"
            };
            
            var planInfo = MedicationPlanId.HasValue ? $"Plan {MedicationPlanId}" : "No Plan";
            return $"{Patient?.Name} - {timeOfDay} on {IntakeTime:yyyy-MM-dd} at {IntakeTime:HH:mm} ({Quantity}x) [{planInfo}]";
        }
    }
}
