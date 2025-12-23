# Redis Monitor - Deployment Guide

## 🚀 Quick Start Scripts

Diese Skripte ermöglichen das einfache Starten und Bereitstellen der Anwendung per Doppelklick:

### Für die Entwicklung

#### `start-redis-monitor.bat`
**Verwendung:** Doppelklick zum Starten der Anwendung im Debug-Modus
- Startet die Anwendung direkt aus dem Quellcode
- Verwendet die Development-Konfiguration
- Ideal für Entwicklung und Testing

### Für die Bereitstellung

#### `publish-release.bat` (oder `publish-release.ps1`)
**Verwendung:** Doppelklick zum Erstellen einer Release-Version
- Erstellt eine optimierte Release-Build
- Veröffentlicht in den `publish`-Ordner
- Die veröffentlichte Version kann direkt ausgeführt werden

#### `deploy.bat` (oder `deploy.ps1`)
**Verwendung:** Doppelklick zum Erstellen eines vollständigen Deployment-Pakets
- Erstellt ein komplettes Deployment-Paket im `deployment`-Ordner
- Enthält:
  - Alle Anwendungsdateien
  - Konfigurationsvorlagen
  - Start-Skript (`run-release.bat`)
  - Dokumentation (`README.txt`)
- Bereit zum Zippen und Verteilen

#### `run-release.bat`
**Verwendung:** Wird automatisch in den Deployment-Ordner kopiert
- Startet die veröffentlichte Anwendung
- Für Endbenutzer gedacht

## 📋 Deployment-Workflow

### Schritt 1: Release Build erstellen
```
Doppelklick auf: publish-release.bat
```
Oder in PowerShell:
```powershell
.\publish-release.ps1
```

### Schritt 2: Deployment-Paket erstellen
```
Doppelklick auf: deploy.bat
```
Oder in PowerShell:
```powershell
.\deploy.ps1
```

### Schritt 3: Verteilen
1. Gehe zum `deployment`-Ordner
2. Zippe den `RedisMonitor`-Ordner
3. Verteile die ZIP-Datei

## 📦 Deployment-Paket Struktur

Nach dem Ausführen von `deploy.bat`:

```
deployment/
├── README.txt                    # Anleitung für Endbenutzer
└── RedisMonitor/
    ├── RedisMonitor.exe         # Hauptanwendung
    ├── run-release.bat          # Start-Skript
    ├── VERSION.txt              # Build-Informationen
    ├── appsettings.json         # Konfiguration
    ├── *.dll                    # Abhängigkeiten
    └── config/                  # Konfigurations-Templates
        ├── appsettings.json
        └── appsettings.Development.json
```

## 🔧 Anpassung der Skripte

### Publish-Optionen ändern
Bearbeite `publish-release.ps1`:
```powershell
# Eigenen Ausgabe-Ordner
.\publish-release.ps1 -OutputDir "C:\MyCustomPath"

# Andere Konfiguration
.\publish-release.ps1 -Configuration Debug
```

### Deployment-Optionen ändern
Bearbeite `deploy.ps1`:
```powershell
# Eigenen Deployment-Ordner
.\deploy.ps1 -DeploymentDir "C:\Releases\RedisMonitor-v1.0"
```

## 🎯 Szenarien

### Szenario 1: Entwicklung
- Verwende: `start-redis-monitor.bat`
- Code wird direkt ausgeführt
- Änderungen sofort verfügbar

### Szenario 2: Lokale Tests (Release)
- Verwende: `publish-release.bat`
- Navigiere zu: `publish\`
- Führe aus: `RedisMonitor.exe`

### Szenario 3: Deployment an Endbenutzer
- Verwende: `deploy.bat`
- Zippe: `deployment\RedisMonitor\`
- Verteile die ZIP-Datei
- Endbenutzer: Entpacken und `run-release.bat` ausführen

## ⚙️ Systemanforderungen

### Für Endbenutzer
- Windows 10/11 oder Windows Server
- .NET 10.0 Runtime (oder SDK)
- Redis Server (lokal oder remote)

### Für Entwickler
- Windows mit .NET 10.0 SDK
- PowerShell 5.1 oder höher
- Redis Server für Tests

## 🔐 Konfiguration vor Deployment

Vor dem Deployment sollten Sie:

1. **appsettings.json** prüfen:
   ```json
   {
     "RedisSettings": {
       "ConnectionString": "localhost:6379",
       "TopicPattern": "your-pattern-*"
     }
   }
   ```

2. **Verbindungseinstellungen** dokumentieren
3. **README.txt** im Deployment-Paket anpassen (optional)

## 🐛 Troubleshooting

### "dotnet nicht gefunden"
- .NET SDK installieren: https://dotnet.microsoft.com/download

### "Skript kann nicht ausgeführt werden"
- PowerShell als Administrator öffnen:
  ```powershell
  Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
  ```

### Port bereits belegt
- Bearbeite `appsettings.json` oder `Properties\launchSettings.json`
- Ändere den Port (Standard: 5000)

## 📝 Zusätzliche Hinweise

### Self-Contained Deployment
Wenn Benutzer kein .NET Runtime haben, ändere `deploy.ps1`:
```powershell
dotnet publish --configuration $Configuration --output $publishDir --self-contained true -r win-x64
```

### Automatisches Öffnen des Browsers
Die Anwendung öffnet automatisch den Browser beim Start.
URL: http://localhost:5000

### Logs und Fehlersuche
- Logs werden in der Konsole angezeigt
- Bei Problemen: Starte über Command Line für detaillierte Ausgabe

---

**Tipp:** Für schnelle Updates einfach `deploy.bat` ausführen und das neue Paket verteilen!
