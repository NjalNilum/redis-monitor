# Redis Monitor - Beispielkonfigurationen

## Typische Anwendungsfälle

### 1. Lokale Entwicklung

```
Host: localhost
Port: 6379
Include Filter: (leer)
Exclude Filter: debug, health
Max. Nachrichten: 1000
```

**Verwendung**: Überwachung aller Channels außer Debug und Health-Checks

---

### 2. Produktionsüberwachung (nur Business Events)

```
Host: redis.production.com
Port: 6379
Include Filter: order, payment, user, notification
Exclude Filter: (leer)
Max. Nachrichten: 500
```

**Verwendung**: Fokus auf geschäftskritische Events

---

### 3. Debugging spezifischer Channels

```
Host: 192.168.1.100
Port: 6379
Include Filter: error, exception, failed
Exclude Filter: (leer)
Max. Nachrichten: 2000
```

**Verwendung**: Fehleranalyse und Debugging

---

### 4. Performance-Testing (hohe Last)

```
Host: localhost
Port: 6379
Include Filter: (leer)
Exclude Filter: (leer)
Max. Nachrichten: 100
```

**Verwendung**: Bei sehr hohem Nachrichtenaufkommen, kleine Buffer-Größe für Performance

---

### 5. Multi-Tenant Überwachung

```
Host: redis.tenant1.com
Port: 6379
Include Filter: tenant:123
Exclude Filter: internal, system
Max. Nachrichten: 1000
```

**Verwendung**: Überwachung eines spezifischen Mandanten

---

## Filter-Syntax

### Include Filter

Der Include Filter ist eine **ODER**-Verknüpfung:

```
user, order, payment
```

Zeigt Nachrichten an, die **mindestens einen** dieser Begriffe enthalten:
- ✅ `user:login` → enthält "user"
- ✅ `order:created` → enthält "order"
- ✅ `payment:success` → enthält "payment"
- ✅ `user-order:123` → enthält "user" UND "order"
- ❌ `product:update` → enthält keinen der Begriffe

### Exclude Filter

Der Exclude Filter ist ebenfalls eine **ODER**-Verknüpfung:

```
debug, test, internal
```

Blendet Nachrichten aus, die **mindestens einen** dieser Begriffe enthalten:
- ❌ `debug:info` → enthält "debug"
- ❌ `test:scenario` → enthält "test"
- ❌ `internal:event` → enthält "internal"
- ✅ `user:login` → enthält keinen der Begriffe

### Kombinierte Filter

Include und Exclude können kombiniert werden:

```
Include: user, order
Exclude: test, debug
```

Ergebnis:
- ✅ `user:login` → Include matched, kein Exclude
- ✅ `order:created` → Include matched, kein Exclude
- ❌ `user:test` → Include matched, aber Exclude auch
- ❌ `product:update` → Include nicht matched
- ❌ `order:debug` → Include matched, aber Exclude auch

---

## Retention Policy

### Konfigurierbare Grenzen

- **Minimum**: 10 Nachrichten
- **Maximum**: 10.000 Nachrichten
- **Standard**: 1.000 Nachrichten

### Funktionsweise

Die Retention Policy arbeitet **pro Channel**:

```
Channel: user:events → Max. 1000 Nachrichten
Channel: order:created → Max. 1000 Nachrichten
Channel: payment:success → Max. 1000 Nachrichten
```

Wenn die maximale Anzahl erreicht ist:
1. Älteste Nachricht wird entfernt
2. Neue Nachricht wird hinzugefügt
3. Gesamtzahl bleibt konstant

### Empfohlene Werte

| Szenario | Empfohlener Wert | Begründung |
|----------|------------------|------------|
| Niedrige Last (<10 msg/s) | 1000-2000 | Standard, gute Balance |
| Mittlere Last (10-100 msg/s) | 500-1000 | Performance-Optimierung |
| Hohe Last (>100 msg/s) | 100-500 | Kritisch für Performance |
| Debugging/Analyse | 2000-5000 | Mehr Historie verfügbar |
| Produktion-Monitoring | 500-1000 | Balance zwischen Historie und Performance |

---

## Redis-Server Konfiguration

### Voraussetzungen

Redis Server sollte für Pub/Sub optimiert sein:

```conf
# redis.conf

# Erhöhe Client Output Buffer für Pub/Sub
client-output-buffer-limit pubsub 32mb 8mb 60

# Erhöhe maximale Anzahl Clients
maxclients 10000

# Deaktiviere Persistence für bessere Performance (optional)
save ""
appendonly no
```

### Sicherheit

Für Produktionsumgebungen:

```conf
# Binde an spezifische IP
bind 192.168.1.100

# Setze Passwort
requirepass your_strong_password_here

# Benenne gefährliche Befehle um
rename-command FLUSHDB ""
rename-command FLUSHALL ""
rename-command CONFIG ""
```

**Wichtig**: Bei Verwendung von `requirepass` muss die Connection-String angepasst werden:

```
Host: redis.production.com
Port: 6379
Connection String: redis.production.com:6379,password=your_password
```

---

## Netzwerk-Konfiguration

### Firewall-Regeln

Stelle sicher, dass Port 6379 erreichbar ist:

```bash
# Windows Firewall
netsh advfirewall firewall add rule name="Redis" dir=in action=allow protocol=TCP localport=6379

# Linux (ufw)
sudo ufw allow 6379/tcp

# Linux (iptables)
sudo iptables -A INPUT -p tcp --dport 6379 -j ACCEPT
```

### Docker Setup

Redis im Docker Container:

```yaml
# docker-compose.yml
version: '3.8'
services:
  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    command: redis-server --appendonly yes

volumes:
  redis-data:
```

---

## Performance-Tuning

### Browser-Optimierung

Bei vielen Nachrichten:
1. Verwende **Exclude Filter** für unwichtige Channels
2. Reduziere **Max. Nachrichten** auf 100-500
3. Schließe nicht benötigte Browser-Tabs

### Server-Optimierung

Bei hoher Last:
1. Erhöhe Redis Server RAM
2. Verwende Redis Cluster für Skalierung
3. Implementiere Rate-Limiting im Publisher

### Netzwerk-Optimierung

Bei langsamer Verbindung:
1. Verwende lokales Redis-Caching
2. Komprimiere Payloads
3. Verwende Redis im gleichen Netzwerk

---

## Beispiel-Szenarien

### Szenario 1: E-Commerce Order Tracking

```json
{
  "host": "redis.shop.com",
  "port": 6379,
  "includeFilter": "order:created, order:shipped, order:delivered",
  "excludeFilter": "",
  "maxMessages": 1000,
  "darkMode": true
}
```

### Szenario 2: User Activity Monitoring

```json
{
  "host": "localhost",
  "port": 6379,
  "includeFilter": "user:login, user:logout, user:action",
  "excludeFilter": "bot, crawler",
  "maxMessages": 2000,
  "darkMode": true
}
```

### Szenario 3: System Health Check

```json
{
  "host": "192.168.1.50",
  "port": 6379,
  "includeFilter": "health, status",
  "excludeFilter": "",
  "maxMessages": 500,
  "darkMode": false
}
```

---

Weitere Informationen finden Sie in der [README.md](README.md) und [QUICKSTART.md](QUICKSTART.md).
