# Redis Monitor - Quick Start Guide

## Voraussetzungen

1. **.NET 10 SDK** muss installiert sein
2. **Redis Server** muss laufen

## Installation & Start

### 1. Projekt starten

```bash
cd c:\_workDir\2025\redis-monitor
dotnet run
```

### 2. Browser öffnen

Die Anwendung öffnet sich automatisch unter:
- **HTTP**: http://localhost:5108
- **HTTPS**: https://localhost:7215

## Erste Schritte

### Redis-Verbindung herstellen

1. **Host eingeben**: z.B. `localhost` oder `192.168.1.100`
2. **Port eingeben**: Standard ist `6379`
3. Auf **"Verbinden"** klicken

### Test mit Redis CLI

Um die Funktionalität zu testen, öffnen Sie ein Terminal und senden Sie Test-Nachrichten:

```bash
# Einfache Nachricht
redis-cli PUBLISH test-channel "Hello World"

# JSON-Nachricht
redis-cli PUBLISH user:events '{"event":"login","user":"john","timestamp":"2025-12-22T10:00:00"}'

# Mehrere Channels
redis-cli PUBLISH orders:created '{"orderId":123,"amount":99.99}'
redis-cli PUBLISH notifications:sent '{"userId":456,"type":"email"}'
```

## Features nutzen

### Filter verwenden

**Include Filter** (nur diese anzeigen):
```
Eingabe: user, order, payment
```
→ Zeigt nur Channels/Nachrichten mit diesen Begriffen

**Exclude Filter** (diese ausblenden):
```
Eingabe: debug, test, health
```
→ Blendet diese Channels/Nachrichten aus

### Nachrichten kopieren

- **Einzelne Nachricht**: Klick auf JSON-Payload
- **Alle Nachrichten**: Button "Alles kopieren" oben rechts

### Retention Policy anpassen

Standard: 1000 Nachrichten pro Channel

Ändern Sie die Zahl im Feld **"Max. Nachrichten pro Channel"** (10-10000)

### Theme wechseln

Checkbox **"Dark Mode"** oben rechts aktivieren/deaktivieren

## Entwicklung

### Projekt bauen

```bash
dotnet build
```

### Mit HTTPS starten

```bash
dotnet run --launch-profile https
```

### Nur HTTP (ohne SSL)

```bash
dotnet run --launch-profile http
```

## Troubleshooting

### "Verbindung fehlgeschlagen"

**Lösung 1**: Redis Server starten
```bash
# Windows (mit Redis als Service)
redis-server

# Linux/Mac
sudo service redis-server start
```

**Lösung 2**: Firewall prüfen
- Port 6379 muss offen sein

**Lösung 3**: Redis-Konfiguration prüfen
```bash
redis-cli ping
# Sollte "PONG" zurückgeben
```

### Keine Nachrichten sichtbar

**Test ob Redis empfängt**:
```bash
# Terminal 1 - Abonnieren
redis-cli PSUBSCRIBE "*"

# Terminal 2 - Senden
redis-cli PUBLISH test "Hello"
```

Falls Terminal 1 die Nachricht empfängt, aber die App nicht:
- Filter prüfen (Include/Exclude)
- Browser-Konsole auf Fehler prüfen (F12)

### Performance-Probleme

Bei vielen Nachrichten pro Sekunde:
1. **Max. Nachrichten reduzieren**: z.B. auf 100
2. **Exclude-Filter nutzen**: Unwichtige Channels ausblenden
3. **Include-Filter nutzen**: Nur relevante Channels anzeigen

## VS Code Integration

### Empfohlene Extensions

- **C# Dev Kit** (ms-dotnettools.csdevkit)
- **C#** (ms-dotnettools.csharp)

### Debug-Konfiguration

Drücken Sie `F5` um das Projekt im Debug-Modus zu starten.

## Deployment

### Publish für Produktion

```bash
dotnet publish -c Release -o ./publish
```

Die kompilierten Dateien befinden sich dann in `./publish/`

### Als Windows Service

```bash
# Service installieren
sc.exe create RedisMonitor binPath="C:\path\to\publish\RedisMonitor.exe"

# Service starten
sc.exe start RedisMonitor
```

## Support

Bei Fragen oder Problemen:
1. Siehe [README.md](README.md) für detaillierte Dokumentation
2. Prüfen Sie die Browser-Konsole (F12) auf JavaScript-Fehler
3. Prüfen Sie die Anwendungs-Logs im Terminal

---

**Viel Erfolg mit dem Redis Monitor! 🚀**
