using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Patient : EntityObject
    {
        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [Range(1, 150)]
        public int? Age { get; set; }
        
        [ForeignKey(nameof(Location))]
        public int? LocationId { get; set; }
        
        [MaxLength(50)]
        public string? PhoneNumber { get; set; }
        
        [MaxLength(255)]
        public string? Email { get; set; }

        // Navigation Properties
        public Location? Location { get; set; }
        public List<MedicationPlan> MedicationPlans { get; set; } = new List<MedicationPlan>();

        public override string ToString()
        {
            return Name;
        }
    }
}
