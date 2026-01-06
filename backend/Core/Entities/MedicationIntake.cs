using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    /// <summary>
    /// Logs when the medication drawer was opened (by day and time of day)
    /// System can only detect: which day (via RFID) + time of day (morning/noon/afternoon/evening)
    /// NOT which specific medication was taken
    /// </summary>
    public class MedicationIntake : EntityObject
    {
        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        
        /// <summary>
        /// Date when the drawer was opened
        /// </summary>
        [Required]
        public DateOnly IntakeDate { get; set; }
        
        /// <summary>
        /// Time of day when drawer was opened
        /// Binary flags: Morning=1, Noon=2, Afternoon=4, Evening=8
        /// Only ONE value per entry (not combined)
        /// </summary>
        [Required, Range(1, 8)]
        public int DayTimeFlag { get; set; }
        
        /// <summary>
        /// Exact timestamp when the drawer was opened
        /// </summary>
        [Required]
        public DateTime OpenedAt { get; set; }
        
        /// <summary>
        /// RFID Tag that was used (weekday chip)
        /// </summary>
        public string? RfidTag { get; set; }
        
        public string? Notes { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; } = null!;

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
