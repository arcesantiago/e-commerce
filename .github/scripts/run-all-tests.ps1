$composeFiles = @("-f", "docker-compose.yml", "-f", "docker-compose.testing.yml")
$testServices = @(
  "productservice-api-tests",
  "productservice-application-tests",
  "productservice-infrastructure-tests",
  "orderservice-api-tests",
  "orderservice-application-tests",
  "orderservice-infrastructure-tests"
)

docker compose @composeFiles --profile infra down -v --remove-orphans

docker compose @composeFiles --profile infra build $testServices

$results = @{}

foreach ($svc in $testServices) {
    Write-Host "▶️ Ejecutando $svc..."
    $logFile = "logs_$svc.txt"

    & docker compose @composeFiles --profile infra run --rm $svc 1> $logFile 2>&1
    $exitCode = $LASTEXITCODE
    $results[$svc] = $exitCode

    Write-Host ""
    Write-Host "===== 📊 Resultados: $svc (exit code $exitCode) ====="
    Select-String -Path $logFile -Pattern "Test Run|Total tests|Passed|Failed|Error"
    Select-String -Path $logFile -Pattern "Failed" -Context 0,3
    Write-Host "==============================================="
}

docker compose @composeFiles --profile infra down -v --rmi all --remove-orphans
docker system prune -af --volumes

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

if ($results.Values -contains 1) { exit 1 } else { exit 0 }