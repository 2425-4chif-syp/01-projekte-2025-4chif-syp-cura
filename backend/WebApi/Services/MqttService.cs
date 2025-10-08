using Core.Contracts;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System.Text;

namespace WebApi.Services;

public class MqttService : IMqttService, IDisposable
{
    private readonly IManagedMqttClient _mqttClient;
    private readonly ILogger<MqttService> _logger;
    private readonly IConfiguration _configuration;
    private bool _disposed;

    public bool IsConnected => _mqttClient.IsConnected;
    public event EventHandler<(string Topic, string Message)>? MessageReceived;

    public MqttService(ILogger<MqttService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        var factory = new MqttFactory();
        _mqttClient = factory.CreateManagedMqttClient();
        
        // Setup event handlers
        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
        _mqttClient.ConnectedAsync += OnConnected;
        _mqttClient.DisconnectedAsync += OnDisconnected;
    }

    public async Task StartAsync()
    {
        try
        {
            var server = _configuration["Mqtt:Server"] ?? "localhost";
            var port = int.Parse(_configuration["Mqtt:Port"] ?? "1883");
            var clientId = _configuration["Mqtt:ClientId"] ?? "cura-backend";

            _logger.LogInformation("Starting MQTT client. Server: {Server}, Port: {Port}, ClientId: {ClientId}", 
                server, port, clientId);

            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithTcpServer(server, port)
                    .WithClientId(clientId)
                    .WithCleanSession()
                    .Build())
                .Build();

            await _mqttClient.StartAsync(options);
            _logger.LogInformation("MQTT client started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start MQTT client");
            throw;
        }
    }

    public async Task StopAsync()
    {
        try
        {
            _logger.LogInformation("Stopping MQTT client");
            await _mqttClient.StopAsync();
            _logger.LogInformation("MQTT client stopped successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping MQTT client");
            throw;
        }
    }

    public async Task PublishAsync(string topic, string message)
    {
        try
        {
            if (!IsConnected)
            {
                _logger.LogWarning("MQTT client is not connected. Cannot publish message to topic: {Topic}", topic);
                return;
            }

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(message))
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.EnqueueAsync(mqttMessage);
            _logger.LogInformation("Published message to topic '{Topic}': {Message}", topic, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to topic: {Topic}", topic);
            throw;
        }
    }

    public async Task SubscribeAsync(string topic)
    {
        try
        {
            await _mqttClient.SubscribeAsync(topic);
            _logger.LogInformation("Subscribed to topic: {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to topic: {Topic}", topic);
            throw;
        }
    }

    public async Task UnsubscribeAsync(string topic)
    {
        try
        {
            await _mqttClient.UnsubscribeAsync(topic);
            _logger.LogInformation("Unsubscribed from topic: {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unsubscribe from topic: {Topic}", topic);
            throw;
        }
    }

    private Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        try
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            
            _logger.LogInformation("Received message from topic '{Topic}': {Message}", topic, message);
            
            // Special handling for RC522 RFID tag messages from ESP32
            var rc522Topic = _configuration["Mqtt:Topics:Rc522Tag"] ?? "rc522/tag";
            if (topic == rc522Topic)
            {
                _logger.LogInformation("🏷️ ESP32 RFID Tag detected: {RfidTag}", message);
                
                // Forward to internal CURA topic for further processing
                var curaRfidTopic = _configuration["Mqtt:Topics:RfidScanned"] ?? "cura/rfid/scanned";
                var forwardMessage = message.Trim();
                
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await PublishAsync(curaRfidTopic, forwardMessage);
                        _logger.LogInformation("Forwarded ESP32 RFID tag to internal topic: {Topic}", curaRfidTopic);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to forward ESP32 RFID tag");
                    }
                });
            }
            
            // Fire the event for subscribers
            MessageReceived?.Invoke(this, (topic, message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing received MQTT message");
        }
        
        return Task.CompletedTask;
    }

    private Task OnConnected(MqttClientConnectedEventArgs e)
    {
        _logger.LogInformation("MQTT client connected successfully");
        
        // Auto-subscribe to ESP32 RC522 RFID tag topic after connection
        _ = Task.Run(async () =>
        {
            try
            {
                var rc522Topic = _configuration["Mqtt:Topics:Rc522Tag"] ?? "rc522/tag";
                await SubscribeAsync(rc522Topic);
                _logger.LogInformation("🏷️ Auto-subscribed to ESP32 RFID topic: {Topic}", rc522Topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to auto-subscribe to ESP32 RFID topic");
            }
        });
        
        return Task.CompletedTask;
    }

    private Task OnDisconnected(MqttClientDisconnectedEventArgs e)
    {
        _logger.LogWarning("MQTT client disconnected. Reason: {Reason}", e.Reason);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _mqttClient?.Dispose();
            _disposed = true;
        }
    }
}