package org.example;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.SQLException;

import org.eclipse.paho.client.mqttv3.*;
import java.sql.*;

public class Main {
    private static String broker = "tcp://192.168.1.104:1883";  // Deine MQTT-Broker-Adresse
    private static String topic = "rc522/tag";  // Dein MQTT-Topic
    private static String topics = "rfid/+/data";
    private static String dbUrl = "jdbc:mariadb://192.168.1.104:3306/cura-test";  // Deine MySQL-Datenbank
    private static String dbUser = "poldi";  // Dein MySQL-Benutzername
    private static String dbPassword = "passme";  // Dein MySQL-Passwort

    public static void main(String[] args) {
        try {
            // MQTT-Client konfigurieren
            MqttClient client = new MqttClient(broker, MqttClient.generateClientId());
            MqttConnectOptions options = new MqttConnectOptions();
            options.setCleanSession(true);
            client.connect(options);

            // MQTT-Nachricht empfangen
            client.subscribe(topic);
            client.setCallback(new MqttCallback() {
                @Override
                public void connectionLost(Throwable cause) {
                    System.out.println("Verbindung verloren: " + cause.getMessage());
                }

                @Override
                public void messageArrived(String topic, MqttMessage message) throws Exception {
                    String messageContent = new String(message.getPayload());
                    System.out.println("Nachricht empfangen: " + messageContent);

                    // In die MySQL-Datenbank speichern
                    try (Connection conn = DriverManager.getConnection(dbUrl, dbUser, dbPassword)) {
                        String sql = "INSERT INTO logs (rfid) VALUES (?);";
                        try (PreparedStatement stmt = conn.prepareStatement(sql)) {
                            stmt.setString(1, messageContent);  // RFID-Tag
                            stmt.executeUpdate();
                            System.out.println("Daten in die Datenbank eingefügt.");
                        }
                    } catch (SQLException e) {
                        e.printStackTrace();
                    }
                }

                @Override
                public void deliveryComplete(IMqttDeliveryToken token) {
                    // Nicht benötigt, wenn du nur Nachrichten empfängst
                }
            });

            // MQTT-Client starten
            System.out.println("Warte auf Nachrichten...");
            while (true) {
                // MQTT-Client im Hintergrund laufen lassen
                Thread.sleep(1000);
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
