using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class RfidChip : EntityObject
    {
        [Required, MaxLength(50)]
        public string ChipId { get; set; } = string.Empty;
        
        [Required]
        public int PatientId { get; set; }
        
        [Required, MaxLength(20)]
        public string Weekday { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;

        // Navigation property
        public Patient? Patient { get; set; }

        public override string ToString()
        {
            return $"{ChipId} - {Weekday} (Patient: {PatientId})";
        }
    }
}