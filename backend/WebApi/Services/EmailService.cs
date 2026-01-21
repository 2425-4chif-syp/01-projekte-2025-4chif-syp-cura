using Core.Contracts;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace WebApi.Services;
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            // E-Mail-Nachricht erstellen
            var message = new MimeMessage();
            
            // Absender (aus appsettings.json)
            var fromName = _configuration["Email:FromName"] ?? "Cura Medication System";
            var fromAddress = _configuration["Email:FromAddress"];
            
            if (string.IsNullOrEmpty(fromAddress))
            {
                _logger.LogError("Email:FromAddress ist nicht konfiguriert");
                throw new InvalidOperationException("Email:FromAddress ist nicht konfiguriert");
            }
            
            message.From.Add(new MailboxAddress(fromName, fromAddress));
            
            // Empfänger
            message.To.Add(new MailboxAddress("", toEmail));
            
            // Betreff
            message.Subject = subject;
            
            // Body (HTML und Plain Text)
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body,
                TextBody = StripHtmlTags(body) // Fallback für Plain Text
            };
            message.Body = bodyBuilder.ToMessageBody();

            // SMTP-Client erstellen und verbinden
            using var client = new SmtpClient();
            
            var smtpServer = _configuration["Email:SmtpServer"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];
            
            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogError("Email-Konfiguration ist unvollständig");
                throw new InvalidOperationException("Email-Konfiguration ist unvollständig");
            }
            
            // Verbindung zum SMTP-Server
            await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            
            // Authentifizierung
            await client.AuthenticateAsync(username, password);
            
            // E-Mail senden
            await client.SendAsync(message);
            
            // Verbindung trennen
            await client.DisconnectAsync(true);
            
            _logger.LogInformation("E-Mail erfolgreich gesendet an {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Senden der E-Mail an {ToEmail}", toEmail);
            throw;
        }
    }

    public async Task SendMissedMedicationAlertAsync(
        string patientEmail, 
        string patientName, 
        string medicationName, 
        TimeSpan scheduledTime)
    {
        var subject = "⚠️ Medikamenten-Erinnerung verpasst";
        
        var body = $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #d9534f;'>⚠️ Medikamenten-Einnahme verpasst!</h2>
                    <p>Hallo {patientName},</p>
                    <p>Sie haben Ihre Medikamente heute nicht zur geplanten Zeit eingenommen:</p>
                    <ul>
                        <li><strong>Medikament:</strong> {medicationName}</li>
                        <li><strong>Geplante Zeit:</strong> {scheduledTime.ToString(@"hh\:mm")} Uhr</li>
                        <li><strong>Aktuell:</strong> {DateTime.Now:HH:mm} Uhr</li>
                    </ul>
                    <p style='color: #d9534f; font-weight: bold;'>Bitte nehmen Sie Ihre Medikamente umgehend ein!</p>
                    <hr style='margin-top: 30px; border: none; border-top: 1px solid #ccc;'>
                    <p style='font-size: 12px; color: #666;'>
                        Dies ist eine automatische Nachricht vom Cura Medikamentenverwaltungssystem.<br>
                        Bei Fragen wenden Sie sich bitte an Ihren Betreuer.
                    </p>
                </body>
            </html>
        ";

        await SendEmailAsync(patientEmail, subject, body);
        
        _logger.LogWarning(
            "Missed medication alert gesendet an {PatientName} ({PatientEmail}) für {MedicationName} um {ScheduledTime}",
            patientName, patientEmail, medicationName, scheduledTime);
    }

    /// <summary>
    /// Entfernt HTML-Tags für Plain-Text-Fallback
    /// </summary>
    private string StripHtmlTags(string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        // Einfache HTML-Tag-Entfernung
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty)
            .Replace("&nbsp;", " ")
            .Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace("&amp;", "&")
            .Trim();
    }
}
