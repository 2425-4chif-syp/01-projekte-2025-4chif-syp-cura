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
            var timeOfDay = DayTimeFlag switch
            {
                1 => "Morning",
                2 => "Noon",
                4 => "Afternoon",
                8 => "Evening",
                _ => "Unknown"
            };
            return $"{Patient?.Name} - {timeOfDay} on {IntakeDate:yyyy-MM-dd} at {OpenedAt:HH:mm}";
        }
    }
}
