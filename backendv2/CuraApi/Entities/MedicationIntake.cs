using System.ComponentModel.DataAnnotations.Schema;

namespace CuraApi.Entities;

[Table("medication_intakes")]
public class MedicationIntake
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("patient_id")]
    public int PatientId { get; set; }
    
    [Column("medication_plan_id")]
    public int MedicationPlanId { get; set; }
    
    [Column("intake_time")]
    public DateTime IntakeTime { get; set; }
    
    [Column("quantity")]
    public int Quantity { get; set; }
    
    [Column("notes")]
    public string? Notes { get; set; }
    
    [Column("rfid_tag")]
    public string? RfidTag { get; set; }
    
    public Patient Patient { get; set; } = null!;
    public MedicationPlan MedicationPlan { get; set; } = null!;
}
