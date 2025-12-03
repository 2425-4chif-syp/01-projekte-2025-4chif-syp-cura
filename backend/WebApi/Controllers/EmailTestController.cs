using Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailTestController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailTestController> _logger;

    public EmailTestController(IEmailService emailService, ILogger<EmailTestController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Sendet eine Test-Alert-E-Mail für verpasste Medikamente
    /// </summary>
    /// <param name="email">Empfänger E-Mail-Adresse (default: timon.schmalzer@gmail.com)</param>
    [HttpPost("send-missed-medication-alert")]
    public async Task<IActionResult> SendMissedMedicationAlert([FromQuery] string? email = null)
    {
        try
        {
            var recipientEmail = email ?? "timon.schmalzer@gmail.com";
            
            await _emailService.SendMissedMedicationAlertAsync(
                recipientEmail,
                "Timon Schmalzer",
                "Aspirin 500mg",
                TimeSpan.FromHours(8)
            );
            
            _logger.LogInformation("Test-Alert-E-Mail erfolgreich gesendet an {Email}", recipientEmail);
            
            return Ok(new 
            { 
                success = true,
                message = $"Test-Alert-E-Mail wurde gesendet an {recipientEmail}",
                timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Senden der Test-E-Mail");
            return StatusCode(500, new 
            { 
                success = false,
                message = $"Fehler: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Sendet eine einfache Test-E-Mail
    /// </summary>
    [HttpPost("send-simple-test")]
    public async Task<IActionResult> SendSimpleTest([FromQuery] string? email = null)
    {
        try
        {
            var recipientEmail = email ?? "timon.schmalzer@gmail.com";
            
            var subject = "🧪 Cura System Test-E-Mail";
            var body = @"
                <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #5cb85c;'>✅ E-Mail-System funktioniert!</h2>
                        <p>Hallo Timon,</p>
                        <p>Diese Test-E-Mail bestätigt, dass das Cura E-Mail-Alert-System erfolgreich konfiguriert ist.</p>
                        <hr>
                        <p style='font-size: 12px; color: #666;'>
                            Gesendet: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + @"<br>
                            Von: Cura Medication System
                        </p>
                    </body>
                </html>
            ";
            
            await _emailService.SendEmailAsync(recipientEmail, subject, body);
            
            _logger.LogInformation("Einfache Test-E-Mail erfolgreich gesendet an {Email}", recipientEmail);
            
            return Ok(new 
            { 
                success = true,
                message = $"Einfache Test-E-Mail wurde gesendet an {recipientEmail}",
                timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Senden der einfachen Test-E-Mail");
            return StatusCode(500, new 
            { 
                success = false,
                message = $"Fehler: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Gibt die aktuelle E-Mail-Konfiguration zurück (ohne Passwort)
    /// </summary>
    [HttpGet("config")]
    public IActionResult GetEmailConfig([FromServices] IConfiguration configuration)
    {
        return Ok(new
        {
            smtpServer = configuration["Email:SmtpServer"],
            smtpPort = configuration["Email:SmtpPort"],
            fromName = configuration["Email:FromName"],
            fromAddress = configuration["Email:FromAddress"],
            username = configuration["Email:Username"],
            passwordConfigured = !string.IsNullOrEmpty(configuration["Email:Password"])
        });
    }
}
