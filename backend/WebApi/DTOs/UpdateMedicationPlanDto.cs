using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class UpdateMedicationPlanDto
    {
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int MedicationId { get; set; }
        
        public int? CaregiverId { get; set; }
        
        [Range(0, 127)]
        public int WeekdayFlags { get; set; } = 0;
        
        [Range(0, 15)]
        public int DayTimeFlags { get; set; } = 0;
        
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        public DateTime ValidFrom { get; set; }
        
        public DateTime? ValidTo { get; set; }
        
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
