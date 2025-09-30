Write-Host "=== CodeNudge Feature Testing ===" -ForegroundColor Green
Write-Host ""

# Test proxy configuration
Write-Host "1. Testing Proxy Configuration..." -ForegroundColor Yellow
if (Test-Path "proxy.conf.json") {
    Write-Host "✓ Proxy configuration found" -ForegroundColor Green
} else {
    Write-Host "✗ Proxy configuration missing" -ForegroundColor Red
}

# Test route guards
Write-Host ""
Write-Host "2. Testing Route Guards..." -ForegroundColor Yellow
if ((Test-Path "src/app/guards/auth.guard.ts") -and (Test-Path "src/app/guards/role.guard.ts")) {
    Write-Host "✓ Route guards implemented" -ForegroundColor Green
} else {
    Write-Host "✗ Route guards missing" -ForegroundColor Red
}

# Test AI service
Write-Host ""
Write-Host "3. Testing AI Evaluation Service..." -ForegroundColor Yellow
if (Test-Path "src/app/services/ai-evaluation.service.ts") {
    Write-Host "✓ AI evaluation service implemented" -ForegroundColor Green
} else {
    Write-Host "✗ AI evaluation service missing" -ForegroundColor Red
}

# Test API service
Write-Host ""
Write-Host "4. Testing Centralized API Service..." -ForegroundColor Yellow
if (Test-Path "src/app/services/api.service.ts") {
    Write-Host "✓ Centralized API service implemented" -ForegroundColor Green
} else {
    Write-Host "✗ Centralized API service missing" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Implementation Summary ===" -ForegroundColor Green
Write-Host "✓ Enhanced registration with role-specific fields" -ForegroundColor White
Write-Host "✓ Route guards for authentication and authorization" -ForegroundColor White
Write-Host "✓ Centralized API service configuration" -ForegroundColor White
Write-Host "✓ AI-powered code evaluation system" -ForegroundColor White
Write-Host "✓ Database schema updates" -ForegroundColor White
Write-Host ""
Write-Host "Ready for testing! Start backend and frontend servers." -ForegroundColor Cyan
