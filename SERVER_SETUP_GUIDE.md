# 🚀 Cura Server Setup Guide

## 📋 **Server Informationen**
- **Server:** vm12.htl-leonding.ac.at
- **User:** curaadm
- **Password:** C4LM@htl.mqtt25
- **Port:** 22 (SSH), 80 (HTTP), 5432 (PostgreSQL)

---

## **Phase 1: Grundverbindung testen** 🔌

### **Schritt 1: SSH-Verbindung**
```bash
# Mit PowerShell oder Command Prompt
ssh curaadm@vm12.htl-leonding.ac.at
# Password eingeben: C4LM@htl.mqtt25
```

### **Schritt 2: Server Status prüfen**
```bash
# Nach erfolgreicher Anmeldung:
pwd                    # Aktueller Pfad
ls -la                 # Dateien anzeigen
whoami                 # User bestätigen
```

---

## **Phase 2: Projekt Setup** 📁

### **Schritt 3: Repository klonen**
```bash
# Falls noch nicht vorhanden:
cd ~
git clone https://github.com/2425-4chif-syp/01-projekte-2025-4chif-syp-cura.git cura-project

# In Projekt-Verzeichnis wechseln:
cd cura-project
```

### **Schritt 4: Berechtigungen setzen**
```bash
# Git-Berechtigungen korrigieren:
sudo chown -R curaadm:curaadm .git/
sudo chown -R curaadm:curaadm .

# Aktuelle Branch prüfen:
git status
git branch
```

---

## **Phase 3: Webserver Setup** 🌐

### **Schritt 5: Nginx installieren/prüfen**
```bash
# Nginx Status prüfen:
sudo systemctl status nginx

# Falls nicht installiert:
sudo apt update
sudo apt install nginx

# Nginx starten:
sudo systemctl start nginx
sudo systemctl enable nginx
```

### **Schritt 6: Nginx konfigurieren**
```bash
# Backup der Standard-Config:
sudo cp /etc/nginx/sites-available/default /etc/nginx/sites-available/default.backup

# Neue Config erstellen:
sudo nano /etc/nginx/sites-available/default
```

**Nginx Config Inhalt:**
```nginx
server {
    listen 80;
    server_name vm12.htl-leonding.ac.at;
    
    root /home/curaadm/cura-project/web;
    index index.html;
    
    location / {
        try_files $uri $uri/ =404;
    }
    
    # Für Angular (später):
    location /app {
        root /home/curaadm/cura-project/web/cura-frontend/dist;
        try_files $uri $uri/ /index.html;
    }
}
```

### **Schritt 7: Nginx aktivieren**
```bash
# Config testen:
sudo nginx -t

# Nginx neustarten:
sudo systemctl restart nginx

# Status prüfen:
sudo systemctl status nginx
```

---

## **Phase 4: Datenbank Setup** 🗄️

### **Schritt 8: Docker installieren**
```bash
# Docker Status prüfen:
docker --version

# Falls nicht installiert:
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# User zu docker Gruppe hinzufügen:
sudo usermod -aG docker curaadm
newgrp docker

# Docker starten:
sudo systemctl start docker
sudo systemctl enable docker
```

### **Schritt 9: PostgreSQL Container starten**
```bash
# In database-Verzeichnis wechseln:
cd ~/cura-project/database

# Container starten:
docker-compose up -d

# Status prüfen:
docker ps
docker logs database_postgres_1
```

### **Schritt 10: Datenbank testen**
```bash
# Verbindung testen:
docker exec -it database_postgres_1 psql -U cura_user -d cura_db

# In PostgreSQL:
\l                     # Datenbanken anzeigen
\dt                    # Tabellen anzeigen
SELECT * FROM rfid_chips LIMIT 5;
\q                     # Beenden
```

---

## **Phase 5: Backend Vorbereitung** ⚙️

### **Schritt 11: .NET Runtime installieren**
```bash
# .NET Status prüfen:
dotnet --version

# Falls nicht installiert:
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

### **Schritt 12: Backend testen**
```bash
# Backend-Verzeichnis:
cd ~/cura-project/backend

# .NET Projekt prüfen:
ls -la *.sln *.csproj

# Build testen (falls vorhanden):
dotnet restore
dotnet build
```

---

## **Phase 6: Testen & Validierung** ✅

### **Schritt 13: Webseite testen**
```bash
# Lokal testen:
curl http://localhost

# Von außen testen:
curl http://vm12.htl-leonding.ac.at
```

### **Schritt 14: Services Status**
```bash
# Alle Services prüfen:
sudo systemctl status nginx
docker ps
sudo netstat -tlnp | grep :80
sudo netstat -tlnp | grep :5432

# Logs anzeigen:
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log
```

### **Schritt 15: Firewall prüfen**
```bash
# Firewall Status:
sudo ufw status

# Falls aktiv, Ports öffnen:
sudo ufw allow 22    # SSH
sudo ufw allow 80    # HTTP
sudo ufw allow 5432  # PostgreSQL
sudo ufw reload
```

---

## **Phase 7: GitHub Actions vorbereiten** 🔄

### **Schritt 16: SSH Key für GitHub**
```bash
# SSH Key generieren (falls nicht vorhanden):
ssh-keygen -t rsa -b 4096 -C "github-actions@cura-project"

# Public Key anzeigen:
cat ~/.ssh/id_rsa.pub

# Zu authorized_keys hinzufügen:
cat ~/.ssh/id_rsa.pub >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys

# Private Key für GitHub Secret:
cat ~/.ssh/id_rsa
# Diesen Inhalt als VM12_SSH_KEY Secret in GitHub eintragen
```

---

## **🎯 Troubleshooting Checkliste**

### **Häufige Probleme:**
- [ ] SSH-Verbindung funktioniert
- [ ] Git-Berechtigungen korrekt
- [ ] Nginx läuft und zeigt Webseite
- [ ] Docker Container läuft
- [ ] Datenbank erreichbar
- [ ] .NET installiert
- [ ] Firewall-Ports offen
- [ ] GitHub SSH Key konfiguriert

### **Wichtige Befehle für Debugging:**
```bash
# System Info:
free -h                # RAM
df -h                  # Speicher
ps aux | grep nginx    # Prozesse
journalctl -u nginx    # Service Logs

# Netzwerk:
ss -tlnp              # Offene Ports
ping google.com       # Internet-Verbindung

# Docker:
docker system df      # Docker Speicher
docker logs <container>  # Container Logs
```

---

## **✅ Erfolg-Indikatoren:**

1. **Webseite erreichbar:** http://vm12.htl-leonding.ac.at
2. **SSH funktioniert:** Keine Passwort-Abfrage bei Pipeline
3. **Datenbank läuft:** `docker ps` zeigt postgres Container
4. **Git works:** `git pull` ohne Fehler

**🚀 Ready for Pipeline Deployment!**