$composeFile = "docker-compose.testing.yml"
$services = @(
  "productservice-api-tests",
  "productservice-application-tests",
  "productservice-infrastructure-tests",
  "orderservice-api-tests",
  "orderservice-application-tests",
  "orderservice-infrastructure-tests"
)

# Levantar dependencias
docker compose -f $composeFile up -d db redis

$results = @{}

foreach ($svc in $services) {
    Write-Host "▶️ Ejecutando $svc..."
    $logFile = "logs_$svc.txt"
    docker compose -f $composeFile run --rm $svc *> $logFile
    $exitCode = $LASTEXITCODE
    $results[$svc] = $exitCode

    Write-Host ""
    Write-Host "===== 📊 Resultados: $svc (exit code $exitCode) ====="
    Select-String -Path $logFile -Pattern "Test Run|Total tests|Passed|Failed|Error"
    Select-String -Path $logFile -Pattern "Failed" -Context 0,3
    Write-Host "==============================================="
}

docker compose -f $composeFile down -v

Write-Host ""
Write-Host "===== 🏁 Resumen Final ====="
foreach ($svc in $results.Keys) {
    if ($results[$svc] -eq 0) {
        Write-Host "$svc ✅ PASSED" -ForegroundColor Green
    } else {
        Write-Host "$svc ❌ FAILED" -ForegroundColor Red
    }
}
Write-Host "============================"

# Salir con error si alguno falló
if ($results.Values -contains 1) { exit 1 } else { exit 0 }