namespace CuraApi.Dtos;

// Response DTOs
public record PatientDto(int Id, string Name, int? Age, string? PhoneNumber, string? Email);

public record MedicationDto(int Id, string Name, string? ActiveIngredient);

public record MedicationPlanDto(
    int Id,
    int PatientId,
    string PatientName,
    int MedicationId,
    string MedicationName,
    int WeekdayFlags,
    int DayTimeFlags,
    int Quantity,
    DateTime ValidFrom,
    DateTime? ValidTo,
    string? Notes,
    bool IsActive
);

public record MedicationIntakeDto(
    int Id,
    int PatientId,
    string PatientName,
    int MedicationPlanId,
    string MedicationName,
    DateTime IntakeTime,
    int Quantity,
    string? Notes,
    string? RfidTag
);

public record DailyStatusDto(
    DateTime Date,
    int Scheduled,
    int Taken,
    string Status  // "empty", "checked", "missed"
);

// Request DTOs
public record LogIntakeRequest(
    int PatientId,
    int MedicationPlanId,
    DateTime? IntakeTime,
    int Quantity = 1,
    string? Notes = null,
    string? RfidTag = null
);
