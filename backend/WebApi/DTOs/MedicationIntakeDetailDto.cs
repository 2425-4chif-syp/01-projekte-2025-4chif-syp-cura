namespace WebApi.DTOs
{
    /// <summary>
    /// Represents a single drawer opening event
    /// System can only track: Date + Time of Day (Morning/Noon/Afternoon/Evening)
    /// </summary>
    public class MedicationIntakeDetailDto
    {
        /// <summary>
        /// Date when drawer was opened (e.g., 2026-01-15)
        /// </summary>
        public DateOnly IntakeDate { get; set; }
        
        /// <summary>
        /// Time of day when drawer was opened
        /// 1 = Morning, 2 = Noon, 4 = Afternoon, 8 = Evening
        /// </summary>
        public int DayTimeFlag { get; set; }
        
        /// <summary>
        /// Human-readable time of day (Morning, Noon, Afternoon, Evening)
        /// </summary>
        public string TimeLabel { get; set; } = string.Empty;
        
        /// <summary>
        /// Exact timestamp when drawer was opened
        /// </summary>
        public DateTime OpenedAt { get; set; }
        
        /// <summary>
        /// RFID tag used to open the drawer
        /// </summary>
        public string? RfidTag { get; set; }
        
        public string? Notes { get; set; }
    }
}
