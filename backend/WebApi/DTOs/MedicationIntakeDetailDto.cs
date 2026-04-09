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
        /// Time of day flag (1=Morning, 2=Noon, 4=Afternoon, 8=Evening)
        /// Calculated from IntakeTime hour
        /// </summary>
        public int DayTimeFlag => GetDayTimeFlag(IntakeTime, Notes);
        
        /// <summary>
        /// Human-readable time of day label
        /// </summary>
        public string TimeLabel => GetTimeLabel(IntakeTime, Notes);
        
        private static int GetDayTimeFlag(DateTime intakeTime, string? notes)
        {
            if (!string.IsNullOrWhiteSpace(notes))
            {
                if (notes.Contains("MORNING", StringComparison.OrdinalIgnoreCase)) return 1;
                if (notes.Contains("NOON", StringComparison.OrdinalIgnoreCase)) return 2;
                if (notes.Contains("AFTERNOON", StringComparison.OrdinalIgnoreCase)) return 4;
                if (notes.Contains("EVENING", StringComparison.OrdinalIgnoreCase)) return 8;
            }

            var localHour = GetAustriaLocalHour(intakeTime);

            return localHour switch
            {
                >= 6 and < 11 => 1,   // Morning
                >= 11 and < 14 => 2,  // Noon
                >= 14 and < 18 => 4,  // Afternoon
                >= 18 and < 22 => 8,  // Evening
                _ => 1                // Default to Morning
            };
        }
        
        private static string GetTimeLabel(DateTime intakeTime, string? notes) => GetDayTimeFlag(intakeTime, notes) switch
        {
            1 => "Morning",
            2 => "Noon",
            4 => "Afternoon",
            8 => "Evening",
            _ => "Unknown"
        };

        private static int GetAustriaLocalHour(DateTime intakeTime)
        {
            var austriaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Vienna");

            var localTime = intakeTime.Kind switch
            {
                DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(intakeTime, austriaTimeZone),
                DateTimeKind.Local => TimeZoneInfo.ConvertTime(intakeTime, austriaTimeZone),
                // Legacy values without kind are treated as UTC in this API.
                _ => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(intakeTime, DateTimeKind.Utc), austriaTimeZone)
            };

            return localTime.Hour;
        }
    }
}
