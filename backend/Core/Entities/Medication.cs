using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Medication : EntityObject
    {
        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string? ActiveIngredient { get; set; }

        // Navigation Properties
        public List<MedicationPlan> MedicationPlans { get; set; } = new List<MedicationPlan>();

        public override string ToString()
        {
            return Name;
        }
    }
}
