namespace WebApi.DTOs
{
    public class DayDetailDto
    {
        public int MedicationPlanId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string TimeLabel { get; set; } = string.Empty;
        public int TimeSlotFlag { get; set; }
        public int DayTimeFlag { get; set; }  // Same as TimeSlotFlag
        public int Quantity { get; set; }
        public bool WasTaken { get; set; }
        public DateTime? IntakeTime { get; set; }
        public int? ActualQuantity { get; set; }
        public string? Notes { get; set; }
    }
}
