namespace Core.Contracts;

public interface IMqttService
{
    /// <summary>
    /// Starts the MQTT client and connects to the broker
    /// </summary>
    Task StartAsync();

    /// <summary>
    /// Stops the MQTT client
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Publishes a message to the specified topic
    /// </summary>
    /// <param name="topic">The MQTT topic to publish to</param>
    /// <param name="message">The message payload</param>
    Task PublishAsync(string topic, string message);

    /// <summary>
    /// Subscribes to a specific topic
    /// </summary>
    /// <param name="topic">The MQTT topic to subscribe to</param>
    Task SubscribeAsync(string topic);

    /// <summary>
    /// Unsubscribes from a specific topic
    /// </summary>
    /// <param name="topic">The MQTT topic to unsubscribe from</param>
    Task UnsubscribeAsync(string topic);

    /// <summary>
    /// Checks if the MQTT client is connected
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Event that fires when a message is received
    /// </summary>
    event EventHandler<(string Topic, string Message)> MessageReceived;
}