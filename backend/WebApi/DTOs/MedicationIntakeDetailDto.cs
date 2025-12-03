namespace WebApi.DTOs
{
    public class MedicationIntakeDetailDto
    {
        public int MedicationPlanId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string TimeLabel { get; set; } = string.Empty;
        public DateTime ScheduledTime { get; set; }
        public bool IsTaken { get; set; }
        public DateTime? TakenAt { get; set; }
    }
}
