namespace WebApi.DTOs
{
    /// <summary>
    /// Represents a medication intake record from the database
    /// Tracks when medications were taken via RFID drawer opening
    /// </summary>
    public class MedicationIntakeDetailDto
    {
        /// <summary>
        /// Unique identifier for the intake record
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Patient who took the medication
        /// </summary>
        public int PatientId { get; set; }
        
        /// <summary>
        /// Medication plan that was followed (null if drawer opening tracked all meds at once)
        /// </summary>
        public int? MedicationPlanId { get; set; }
        
        /// <summary>
        /// Exact timestamp when medication was taken
        /// </summary>
        public DateTime IntakeTime { get; set; }
        
        /// <summary>
        /// Quantity of medication taken
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// RFID tag used to open the drawer
        /// </summary>
        public string? RfidTag { get; set; }
        
        /// <summary>
        /// Additional notes about the intake
        /// </summary>
        public string? Notes { get; set; }
        
        // Calculated properties for frontend compatibility
        
        /// <summary>
        /// Date when medication was taken (derived from IntakeTime)
        /// </summary>
        public DateOnly IntakeDate => DateOnly.FromDateTime(IntakeTime);
        
        /// <summary>
        /// Time of day flag (1=Morning, 2=Noon, 8=Evening, 16=Night)
        /// Calculated from IntakeTime hour
        /// </summary>
        public int DayTimeFlag => GetDayTimeFlag(IntakeTime.Hour);
        
        /// <summary>
        /// Human-readable time of day label
        /// </summary>
        public string TimeLabel => GetTimeLabel(IntakeTime.Hour);
        
        private static int GetDayTimeFlag(int hour) => hour switch
        {
            >= 6 and < 11 => 1,   // Morning
            >= 11 and < 14 => 2,  // Noon
            >= 18 and < 22 => 8,  // Evening
            >= 22 or < 6 => 16,   // Night
            _ => 1                // Default to Morning
        };
        
        private static string GetTimeLabel(int hour) => hour switch
        {
            >= 6 and < 11 => "Morning",
            >= 11 and < 14 => "Noon",
            >= 18 and < 22 => "Evening",
            >= 22 or < 6 => "Night",
            _ => "Unknown"
        };
    }
}
