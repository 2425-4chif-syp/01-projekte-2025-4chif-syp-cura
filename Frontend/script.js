const clientId = 'mqttjs_' + Math.random().toString(16).substr(2, 8);
const host = 'wss://test.mosquitto.org:8081/mqtt';
const topic = 'example/topic';

const client = new Paho.MQTT.Client(host, clientId);

client.onConnectionLost = onConnectionLost;
client.onMessageArrived = onMessageArrived;

client.connect({
    onSuccess: onConnect,
    useSSL: true
});

function onConnect() {
    console.log('Connected to MQTT broker');
    client.subscribe(topic);
    updateConnectionStatus(true);
}

function onConnectionLost(responseObject) {
    if (responseObject.errorCode !== 0) {
        console.log('Connection lost: ' + responseObject.errorMessage);
        updateConnectionStatus(false);
    }
}

function onMessageArrived(message) {
    console.log('Message arrived: ' + message.payloadString);
    const mqttMessage = document.getElementById('mqtt-message');
    mqttMessage.textContent = message.payloadString;
    mqttMessage.classList.add('updated');
    setTimeout(() => mqttMessage.classList.remove('updated'), 300);
}

function updateConnectionStatus(isConnected) {
    const statusIndicator = document.querySelector('.status-indicator');
    const connectionStatus = document.getElementById('connection-status');
    
    if (isConnected) {
        statusIndicator.classList.add('connected');
        statusIndicator.classList.remove('disconnected');
        connectionStatus.textContent = 'Connected';
    } else {
        statusIndicator.classList.add('disconnected');
        statusIndicator.classList.remove('connected');
        connectionStatus.textContent = 'Disconnected';
    }
}
