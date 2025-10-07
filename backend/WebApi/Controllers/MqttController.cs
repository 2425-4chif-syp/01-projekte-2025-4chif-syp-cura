using Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MqttController : ControllerBase
{
    private readonly IMqttService _mqttService;
    private readonly ILogger<MqttController> _logger;
    private readonly IConfiguration _configuration;

    public MqttController(IMqttService mqttService, ILogger<MqttController> logger, IConfiguration configuration)
    {
        _mqttService = mqttService;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Get MQTT connection status
    /// </summary>
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new { 
            IsConnected = _mqttService.IsConnected,
            Timestamp = DateTime.UtcNow,
            Server = _configuration["Mqtt:Server"],
            Port = _configuration["Mqtt:Port"],
            ClientId = _configuration["Mqtt:ClientId"]
        });
    }

    /// <summary>
    /// Publish a test message to a specific topic
    /// </summary>
    [HttpPost("publish")]
    public async Task<IActionResult> PublishMessage([FromBody] PublishMessageRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Topic) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Topic and Message are required");
            }

            if (!_mqttService.IsConnected)
            {
                return BadRequest("MQTT client is not connected");
            }

            await _mqttService.PublishAsync(request.Topic, request.Message);
            
            _logger.LogInformation("Published message via API - Topic: {Topic}, Message: {Message}", 
                request.Topic, request.Message);

            return Ok(new { 
                Success = true, 
                Topic = request.Topic, 
                Message = request.Message,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing MQTT message");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Subscribe to a specific topic
    /// </summary>
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Topic))
            {
                return BadRequest("Topic is required");
            }

            if (!_mqttService.IsConnected)
            {
                return BadRequest("MQTT client is not connected");
            }

            await _mqttService.SubscribeAsync(request.Topic);
            
            _logger.LogInformation("Subscribed to topic via API: {Topic}", request.Topic);

            return Ok(new { 
                Success = true, 
                Topic = request.Topic,
                Message = "Successfully subscribed",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to MQTT topic");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Unsubscribe from a specific topic
    /// </summary>
    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromBody] SubscribeRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Topic))
            {
                return BadRequest("Topic is required");
            }

            await _mqttService.UnsubscribeAsync(request.Topic);
            
            _logger.LogInformation("Unsubscribed from topic via API: {Topic}", request.Topic);

            return Ok(new { 
                Success = true, 
                Topic = request.Topic,
                Message = "Successfully unsubscribed",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unsubscribing from MQTT topic");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Send a test message to the default test topic
    /// </summary>
    [HttpPost("test")]
    public async Task<IActionResult> SendTestMessage([FromBody] TestMessageRequest? request)
    {
        try
        {
            var testTopic = _configuration["Mqtt:Topics:Test"] ?? "cura/test";
            var testMessage = request?.Message ?? $"Test message from backend at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

            if (!_mqttService.IsConnected)
            {
                return BadRequest("MQTT client is not connected");
            }

            await _mqttService.PublishAsync(testTopic, testMessage);
            
            return Ok(new { 
                Success = true, 
                Topic = testTopic, 
                Message = testMessage,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending test MQTT message");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}

// DTOs for API requests
public class PublishMessageRequest
{
    public string Topic { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class SubscribeRequest
{
    public string Topic { get; set; } = string.Empty;
}

public class TestMessageRequest
{
    public string? Message { get; set; }
}