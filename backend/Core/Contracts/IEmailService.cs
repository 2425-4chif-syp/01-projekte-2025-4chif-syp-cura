namespace Core.Contracts;

public interface IEmailService
{
    /// <summary>
    /// Sendet eine E-Mail an den angegebenen Empfänger
    /// </summary>
    /// <param name="toEmail">Empfänger E-Mail-Adresse</param>
    /// <param name="subject">Betreff der E-Mail</param>
    /// <param name="body">Inhalt der E-Mail (HTML oder Plain Text)</param>
    Task SendEmailAsync(string toEmail, string subject, string body);

    /// <summary>
    /// Sendet eine Alert-E-Mail für verpasste Medikamenteneinnahme
    /// </summary>
    /// <param name="patientEmail">E-Mail-Adresse des Patienten</param>
    /// <param name="patientName">Name des Patienten</param>
    /// <param name="medicationName">Name des Medikaments</param>
    /// <param name="scheduledTime">Geplante Einnahmezeit</param>
    /// <param name="currentTime">Aktuelle Zeit (CET)</param>
    Task SendMissedMedicationAlertAsync(string patientEmail, string patientName, string medicationName, TimeSpan scheduledTime, DateTime currentTime);
}
