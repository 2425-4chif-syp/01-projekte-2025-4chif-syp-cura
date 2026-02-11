using System.ComponentModel.DataAnnotations.Schema;

namespace CuraApi.Entities;

[Table("medications")]
public class Medication
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Column("active_ingredient")]
    public string? ActiveIngredient { get; set; }
    
    public ICollection<MedicationPlan> MedicationPlans { get; set; } = new List<MedicationPlan>();
}
