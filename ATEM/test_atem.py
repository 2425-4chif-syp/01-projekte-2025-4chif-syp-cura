from PyATEMMax.ATEMMax import ATEMMax
import time

# IP Ihres ATEM
ATEM_IP = "192.168.0.226"

# ATEM-Objekt erstellen und verbinden
atem = ATEMMax()
atem.connect(ATEM_IP)
atem.waitForConnection()

# Eingang 2 auf Program schalten
print("Schalte auf HDMI 2...")
atem.setProgramInputVideoSource(0, 2)

# 5 Sekunden warten
time.sleep(5)

# Zurück auf Eingang 1
print("Schalte zurück auf HDMI 1...")
atem.setProgramInputVideoSource(0, 1)

# Verbindung trennen
atem.disconnect()
print("Fertig!")