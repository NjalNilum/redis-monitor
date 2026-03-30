# Work Log: Message Copy Render

## Umgesetzt
- Nachrichtenmodell um eine getrennte Display-Darstellung erweitert.
- JSON-Stringwerte fuer die Anzeige auf 150 Zeichen begrenzt, ohne den Volltext fuer Kopierfunktionen zu verlieren.
- Sammelkopie auf maximal 250 neueste sichtbare Nachrichten begrenzt.
- Einzelkopie pro Nachricht auf komplette Nachricht umgestellt: Zeitstempel, Channel und voller Payload.
- Chronologische Ansicht ebenfalls mit Sammelkopie fuer die neuesten sichtbaren Nachrichten ausgestattet.

## Validierung
- Razor- und C#-Fehlerpruefung ohne Befunde.
- `dotnet build monitor/RedisMonitor.sln` erfolgreich.

## Hinweise
- Fuer nicht-JSON Payloads wird die Anzeige ebenfalls auf 150 Zeichen gekuerzt, die Kopie bleibt vollstaendig.