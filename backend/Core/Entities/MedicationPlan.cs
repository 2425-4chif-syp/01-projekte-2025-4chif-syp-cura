using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class MedicationPlan : EntityObject
    {
        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        
        [Required]
        [ForeignKey(nameof(Medication))]
        public int MedicationId { get; set; }
        
        [ForeignKey(nameof(Caregiver))]
        public int? CaregiverId { get; set; }
        
        // Binary weekdays: Sun=1, Mon=2, Tue=4, Wed=8, Thu=16, Fri=32, Sat=64
        [Range(0, 127)]
        public int WeekdayFlags { get; set; } = 0;
        
        // Binary day times: Morning=1, Noon=2, Afternoon=4, Evening=8
        [Range(0, 15)]
        public int DayTimeFlags { get; set; } = 0;
        
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        public DateTime ValidFrom { get; set; }
        
        public DateTime? ValidTo { get; set; }
        
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Patient Patient { get; set; } = null!;
        public Medication Medication { get; set; } = null!;
        public Caregiver? Caregiver { get; set; }

        public override string ToString()
        {
            return $"{Patient?.Name} - {Medication?.Name} ({Quantity}x)";
        }
    }
}
