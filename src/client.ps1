$ip = "127.0.0.1"
$port = 8888
$udp = New-Object System.Net.Sockets.UdpClient
$udp.Connect($ip, $port)

while ($true) {
    $msg = Read-Host ">"

    $bytes = [System.Text.Encoding]::UTF8.GetBytes($msg)
    $udp.Send($bytes, $bytes.Length)
}

$udp.Close()
