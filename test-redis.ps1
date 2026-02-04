#!/usr/bin/env pwsh
# Redis Monitor Test Script
# Sendet Test-Nachrichten an Redis für Demo-Zwecke

param(
    [string]$RedisHost = "localhost",
    [int]$Port = 6379,
    [int]$Count = 100
)

Write-Host "Redis Monitor Test Script" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Host: $RedisHost" -ForegroundColor Yellow
Write-Host "Port: $Port" -ForegroundColor Yellow
Write-Host "Anzahl Nachrichten: $Count" -ForegroundColor Yellow
Write-Host ""

$channels = @(
    "user:login",
    "user:logout",
    "order:created",
    "order:shipped",
    "order:delivered",
    "payment:success",
    "payment:failed",
    "notification:sent",
    "notification:error",
    "system:health"
)

$userNames = @("Alice", "Bob", "Charlie", "Diana", "Eve", "Frank")
$products = @("Laptop", "Smartphone", "Tablet", "Headphones", "Monitor")
$statuses = @("pending", "processing", "completed", "failed")

function Get-RandomUser {
    return $userNames | Get-Random
}

function Get-RandomProduct {
    return $products | Get-Random
}

function Get-RandomStatus {
    return $statuses | Get-Random
}

function Get-RandomId {
    return Get-Random -Minimum 1000 -Maximum 9999
}

function Get-RandomAmount {
    return [Math]::Round((Get-Random -Minimum 10 -Maximum 500) + (Get-Random) * 0.99, 2)
}

function Send-RedisMessageViaCli {
    param(
        [string]$Channel,
        [string]$Message
    )
    
    try {
        $result = redis-cli -h $RedisHost -p $Port PUBLISH $Channel $Message 2>&1
        return ($LASTEXITCODE -eq 0)
    }
    catch {
        return $false
    }
}

function Send-RedisMessageViaTcp {
    param(
        [string]$Channel,
        [string]$Message
    )
    
    try {
        $tcpClient = New-Object System.Net.Sockets.TcpClient
        $tcpClient.Connect($RedisHost, $Port)
        $stream = $tcpClient.GetStream()
        $writer = New-Object System.IO.StreamWriter($stream)
        $reader = New-Object System.IO.StreamReader($stream)
        
        # Redis PUBLISH command: *3\r\n$7\r\nPUBLISH\r\n$<len>\r\n<channel>\r\n$<len>\r\n<message>\r\n
        $channelBytes = [System.Text.Encoding]::UTF8.GetBytes($Channel)
        $messageBytes = [System.Text.Encoding]::UTF8.GetBytes($Message)
        
        $command = "*3`r`n"
        $command += "`$7`r`nPUBLISH`r`n"
        $command += "`$$($channelBytes.Length)`r`n$Channel`r`n"
        $command += "`$$($messageBytes.Length)`r`n$Message`r`n"
        
        $writer.Write($command)
        $writer.Flush()
        
        # Read response
        $response = $reader.ReadLine()
        
        $writer.Close()
        $reader.Close()
        $stream.Close()
        $tcpClient.Close()
        
        return ($response -match "^:\d+")
    }
    catch {
        return $false
    }
}

function Send-RedisMessage {
    param(
        [string]$Channel,
        [string]$Message
    )
    
    $success = $false
    
    if ($script:UseRedisCli) {
        $success = Send-RedisMessageViaCli -Channel $Channel -Message $Message
    }
    else {
        $success = Send-RedisMessageViaTcp -Channel $Channel -Message $Message
    }
    
    if ($success) {
        Write-Host "[✓] " -ForegroundColor Green -NoNewline
        Write-Host "$Channel" -ForegroundColor Cyan -NoNewline
        Write-Host " → " -NoNewline
        
        if ($Message.Length -gt 60) {
            Write-Host ($Message.Substring(0, 60) + "...") -ForegroundColor Gray
        } else {
            Write-Host $Message -ForegroundColor Gray
        }
    } else {
        Write-Host "[✗] Fehler bei $Channel" -ForegroundColor Red
    }
}

# Check if redis-cli is available
$script:UseRedisCli = $false
try {
    $null = Get-Command redis-cli -ErrorAction Stop
    $script:UseRedisCli = $true
    Write-Host "[✓] redis-cli gefunden" -ForegroundColor Green
}
catch {
    Write-Host "[!] redis-cli nicht gefunden - verwende TCP Verbindung" -ForegroundColor Yellow
}

# Test Redis Connection
Write-Host "Teste Redis Verbindung..." -ForegroundColor Yellow

try {
    $tcpClient = New-Object System.Net.Sockets.TcpClient
    $tcpClient.ReceiveTimeout = 3000
    $tcpClient.SendTimeout = 3000
    $tcpClient.Connect($RedisHost, $Port)
    
    if ($tcpClient.Connected) {
        $stream = $tcpClient.GetStream()
        $writer = New-Object System.IO.StreamWriter($stream)
        $reader = New-Object System.IO.StreamReader($stream)
        
        # Send PING command
        $writer.Write("*1`r`n`$4`r`nPING`r`n")
        $writer.Flush()
        
        # Read response
        $response = $reader.ReadLine()
        
        $writer.Close()
        $reader.Close()
        $stream.Close()
        $tcpClient.Close()
        
        if ($response -eq "+PONG") {
            Write-Host "[✓] Redis ist erreichbar auf ${RedisHost}:${Port}" -ForegroundColor Green
        }
        else {
            throw "Ungültige Antwort von Redis: $response"
        }
    }
    else {
        throw "Verbindung konnte nicht hergestellt werden"
    }
}
catch {
    Write-Host "[✗] Redis nicht erreichbar auf ${RedisHost}:${Port}" -ForegroundColor Red
    Write-Host "Fehler: $_" -ForegroundColor Red
    Write-Host "Bitte stellen Sie sicher, dass Redis läuft." -ForegroundColor Yellow
    exit 1
}
Write-Host ""
Write-Host "Sende Test-Nachrichten..." -ForegroundColor Yellow
Write-Host ""

for ($i = 1; $i -le $Count; $i++) {
    $channel = $channels | Get-Random
    $timestamp = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
    
    $message = switch -Wildcard ($channel) {
        "user:login" {
            @{
                event = "user_login"
                user = Get-RandomUser
                userId = Get-RandomId
                ip = "192.168.1.$((Get-Random -Minimum 1 -Maximum 254))"
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
        
        "user:logout" {
            @{
                event = "user_logout"
                user = Get-RandomUser
                userId = Get-RandomId
                sessionDuration = Get-Random -Minimum 60 -Maximum 3600
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
        
        "order:created" {
            @{
                event = "order_created"
                orderId = Get-RandomId
                customerId = Get-RandomId
                product = Get-RandomProduct
                quantity = Get-Random -Minimum 1 -Maximum 5
                amount = Get-RandomAmount
                status = "pending"
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
        
        "order:shipped" {
            @{
                event = "order_shipped"
                orderId = Get-RandomId
                carrier = @("DHL", "UPS", "FedEx") | Get-Random
                trackingNumber = "TRK$(Get-Random -Minimum 100000 -Maximum 999999)"
                estimatedDelivery = (Get-Date).AddDays((Get-Random -Minimum 2 -Maximum 7)).ToString("yyyy-MM-dd")
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
        
        "order:delivered" {
            @{
                event = "order_delivered"
                orderId = Get-RandomId
                deliveredAt = $timestamp
                signedBy = Get-RandomUser
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
        
        "payment:success" {
            @{
                event = "payment_success"
                paymentId = Get-RandomId
                orderId = Get-RandomId
                amount = Get-RandomAmount
                method = @("credit_card", "paypal", "bank_transfer") | Get-Random
                transactionId = "TXN$(Get-Random -Minimum 100000 -Maximum 999999)"
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
        
        "payment:failed" {
            @{
                event = "payment_failed"
                paymentId = Get-RandomId
                orderId = Get-RandomId
                amount = Get-RandomAmount
                reason = @("insufficient_funds", "card_declined", "expired_card", "fraud_suspected") | Get-Random
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
        
        "notification:sent" {
            @{
                event = "notification_sent"
                notificationId = Get-RandomId
                userId = Get-RandomId
                type = @("email", "sms", "push") | Get-Random
                subject = "Your order update"
                status = "sent"
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
        
        "notification:error" {
            @{
                event = "notification_error"
                notificationId = Get-RandomId
                userId = Get-RandomId
                type = @("email", "sms", "push") | Get-Random
                error = @("invalid_recipient", "service_unavailable", "rate_limit_exceeded") | Get-Random
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
        
        "system:health" {
            @{
                event = "health_check"
                service = @("api", "database", "cache", "queue") | Get-Random
                status = @("healthy", "degraded", "unhealthy") | Get-Random
                responseTime = Get-Random -Minimum 10 -Maximum 500
                memoryUsage = Get-Random -Minimum 30 -Maximum 90
                cpuUsage = Get-Random -Minimum 10 -Maximum 80
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
        
        default {
            @{
                event = "generic_event"
                data = "Test data"
                timestamp = $timestamp
            } | ConvertTo-Json -Compress
        }
    }
    
    Send-RedisMessage -Channel $channel -Message $message
    
    # Kleine Pause zwischen Nachrichten
    Start-Sleep -Milliseconds (Get-Random -Minimum 100 -Maximum 500)
}

Write-Host ""
Write-Host "[✓] Alle Test-Nachrichten gesendet!" -ForegroundColor Green
Write-Host ""
Write-Host "Überprüfen Sie den Redis Monitor im Browser." -ForegroundColor Cyan
