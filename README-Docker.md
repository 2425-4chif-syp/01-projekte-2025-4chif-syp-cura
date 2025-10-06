# CURA - Containerized Setup

## 🚀 Quick Start

### Alles mit einem Befehl starten:
```bash
cd database
docker compose up --build
```

### Services nach dem Start:
- **Backend API**: http://localhost:5257
- **Swagger UI**: http://localhost:5257/swagger/index.html
- **pgAdmin**: http://localhost:8080
- **PostgreSQL**: localhost:5434

## 📋 Login-Daten

### pgAdmin (http://localhost:8080)
- **Email**: admin@cura.at
- **Password**: passme

### PostgreSQL Verbindung in pgAdmin
- **Host**: cura_postgres
- **Port**: 5432
- **Database**: cura
- **Username**: admin
- **Password**: passme

## 🔧 Entwicklung

### Services stoppen:
```bash
docker compose down
```

### Services mit Volume-Reset (neue DB):
```bash
docker compose down -v
docker compose up --build
```

### Nur Backend neu bauen:
```bash
docker compose up --build backend
```

## 📁 Projekt-Struktur
```
├── backend/
│   ├── Dockerfile
│   ├── WebApi/
│   │   ├── appsettings.json         # Lokale Entwicklung
│   │   └── appsettings.Docker.json  # Container-Umgebung
│   ├── Core/
│   └── Persistence/
├── database/
│   ├── docker-compose.yaml
│   ├── init.sql
│   └── testdata.sql
└── README-Docker.md
```

## 🐳 Container-Architektur
- **cura_postgres**: PostgreSQL 16 Datenbank
- **cura_pgadmin**: pgAdmin4 Web-Interface
- **cura_backend**: .NET 8 WebAPI Backend
- **cura-network**: Isoliertes Docker-Netzwerk