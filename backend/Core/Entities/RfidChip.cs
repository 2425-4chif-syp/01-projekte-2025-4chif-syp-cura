using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class RfidChip : EntityObject
    {
        [Required, MaxLength(50)]
        public string ChipId { get; set; } = string.Empty;
        
        [Required, MaxLength(20)]
        public string Weekday { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            return $"{ChipId} - {Weekday}";
        }
    }
}