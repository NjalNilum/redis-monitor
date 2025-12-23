# Redis Monitor

Eine Blazor Server Anwendung zum Überwachen von Redis Pub/Sub Nachrichten in Echtzeit.

## Features

✅ **Echtzeit-Überwachung**: Empfängt alle Redis Pub/Sub Nachrichten über `PSUBSCRIBE *`
✅ **Channel-Liste**: Zeigt alle aktiven Channels an (Chat-Style, neueste unten)
✅ **JSON-Formatierung**: Automatische Formatierung von JSON-Payloads
✅ **Verbindungskonfiguration**: Flexible IP/Port-Einstellung
✅ **Include-Filter**: Zeigt nur Nachrichten mit bestimmten Keywords an
✅ **Exclude-Filter**: Blendet Nachrichten mit bestimmten Keywords aus
✅ **Retention Policy**: Konfigurierbare maximale Nachrichtenanzahl pro Channel (10-10000)
✅ **Dark/Light Theme**: Umschaltbar mit Default Dark Mode
✅ **Copy-to-Clipboard**: Einzelne Payloads oder alle Nachrichten eines Channels kopieren

## Technologie-Stack

- .NET 10
- Blazor Server mit Interactive Server Components
- StackExchange.Redis 2.10.1
- Custom CSS für Dark/Light Theme

## Schnellstart

### Voraussetzungen

- .NET 10 SDK installiert
- Redis Server läuft (lokal oder remote)

### Starten der Anwendung

```bash
# Im Projektverzeichnis
dotnet run
```

Die Anwendung startet standardmäßig auf:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

### Redis-Verbindung einrichten

1. **Host eingeben**: IP-Adresse oder Hostname des Redis-Servers (Standard: localhost)
2. **Port eingeben**: Port des Redis-Servers (Standard: 6379)
3. **Verbinden klicken**: Startet die Überwachung

## Verwendung

### Filter

**Include Filter** (Komma-getrennt):
```
channel1, event, user
```
Zeigt nur Nachrichten an, die einen dieser Begriffe enthalten.

**Exclude Filter** (Komma-getrennt):
```
debug, test, health
```
Blendet Nachrichten mit diesen Begriffen aus.

### Retention Policy

- **Min**: 10 Nachrichten
- **Max**: 10000 Nachrichten
- **Standard**: 1000 Nachrichten

Wenn die maximale Anzahl erreicht ist, werden älteste Nachrichten automatisch entfernt.

### Copy-to-Clipboard

- **Einzelne Nachricht**: Klick auf den JSON-Payload
- **Alle Nachrichten**: Button "Alles kopieren" rechts oben

### Theme-Wechsel

Checkbox "Dark Mode" in der oberen rechten Ecke aktivieren/deaktivieren.

## Projektstruktur

```
RedisMonitor/
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor          # Minimales Layout
│   └── Pages/
│       └── Home.razor                 # Haupt-UI-Komponente
├── Models/
│   ├── RedisMessage.cs                # Nachrichtenmodell
│   └── RedisSettings.cs               # Konfigurationsmodell
├── Services/
│   └── RedisMonitorService.cs         # Redis Pub/Sub Service
├── wwwroot/
│   ├── css/
│   │   └── redis-monitor.css          # Custom Styling
│   └── js/
│       └── redis-monitor.js           # JavaScript Interop
└── Program.cs                         # App-Konfiguration
```

## Architektur

### RedisMonitorService

- Verwaltet Redis-Verbindung mit `ConnectionMultiplexer`
- Abonniert alle Channels via `PSUBSCRIBE *`
- Filtert Nachrichten basierend auf Include/Exclude-Filtern
- Wendet Retention Policy an
- Thread-safe mit `ConcurrentDictionary` und Locks
- Event-basierte Benachrichtigung der UI

### UI-Komponenten

**Channel-Liste (Links)**:
- Zeigt alle aktiven Channels
- Anzahl der Nachrichten pro Channel
- Auswählbar für Detailansicht

**Message-Display (Rechts)**:
- Chronologische Anzeige aller Nachrichten
- Formatierte JSON-Ausgabe
- Timestamp und Channel-Name
- Click-to-copy Funktionalität

## Entwicklung

### Build

```bash
dotnet build
```

### Run (Development)

```bash
dotnet run --environment Development
```

### Publish

```bash
dotnet publish -c Release -o ./publish
```

## Troubleshooting

### Verbindung schlägt fehl

- Prüfen Sie, ob Redis-Server läuft: `redis-cli ping`
- Prüfen Sie Firewall-Einstellungen
- Verifizieren Sie IP/Port-Konfiguration

### Keine Nachrichten sichtbar

- Prüfen Sie, ob tatsächlich Pub/Sub-Nachrichten gesendet werden
- Testen Sie mit: `redis-cli PUBLISH testchannel "Hello World"`
- Überprüfen Sie Include/Exclude-Filter

### Performance-Probleme

- Reduzieren Sie die maximale Nachrichtenanzahl
- Verwenden Sie Exclude-Filter für unwichtige Channels
- Schließen Sie Debug-Channels aus

## Lizenz

Dieses Projekt ist für den persönlichen und kommerziellen Gebrauch frei verfügbar.

## Support

Bei Fragen oder Problemen erstellen Sie bitte ein Issue im Repository.
