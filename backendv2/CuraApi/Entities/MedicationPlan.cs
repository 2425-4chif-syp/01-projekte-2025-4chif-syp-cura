using System.ComponentModel.DataAnnotations.Schema;

namespace CuraApi.Entities;

[Table("medication_plans")]
public class MedicationPlan
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("patient_id")]
    public int PatientId { get; set; }
    
    [Column("medication_id")]
    public int MedicationId { get; set; }
    
    [Column("caregiver_id")]
    public int? CaregiverId { get; set; }
    
    // Binary weekdays: Sun=1, Mon=2, Tue=4, Wed=8, Thu=16, Fri=32, Sat=64
    [Column("weekday_flags")]
    public int WeekdayFlags { get; set; }
    
    // Binary day times: Morning=1, Noon=2, Afternoon=4, Evening=8
    [Column("day_time_flags")]
    public int DayTimeFlags { get; set; }
    
    [Column("quantity")]
    public int Quantity { get; set; }
    
    [Column("valid_from")]
    public DateTime ValidFrom { get; set; }
    
    [Column("valid_to")]
    public DateTime? ValidTo { get; set; }
    
    [Column("notes")]
    public string? Notes { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
    
    public Patient Patient { get; set; } = null!;
    public Medication Medication { get; set; } = null!;
    public ICollection<MedicationIntake> MedicationIntakes { get; set; } = new List<MedicationIntake>();
}
