using System.ComponentModel.DataAnnotations.Schema;

namespace CuraApi.Entities;

[Table("patients")]
public class Patient
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Column("age")]
    public int? Age { get; set; }
    
    [Column("phone_number")]
    public string? PhoneNumber { get; set; }
    
    [Column("email")]
    public string? Email { get; set; }
    
    [Column("location_id")]
    public int? LocationId { get; set; }
    
    public ICollection<MedicationPlan> MedicationPlans { get; set; } = new List<MedicationPlan>();
    public ICollection<MedicationIntake> MedicationIntakes { get; set; } = new List<MedicationIntake>();
}
