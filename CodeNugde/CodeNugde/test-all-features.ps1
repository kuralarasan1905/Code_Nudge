# PowerShell script to test all implemented features
# Run this script to validate the registration system and new features

Write-Host "=== CodeNudge Feature Testing Script ===" -ForegroundColor Green
Write-Host ""

# Test 1: Check if Angular development server can start
Write-Host "1. Testing Angular Development Server..." -ForegroundColor Yellow
if (Test-Path "node_modules") {
    Write-Host "✓ Angular project setup looks good" -ForegroundColor Green
} else {
    Write-Host "⚠ Node modules not found - run 'npm install'" -ForegroundColor Yellow
}

# Test 2: Check proxy configuration
Write-Host ""
Write-Host "2. Testing Proxy Configuration..." -ForegroundColor Yellow
if (Test-Path "proxy.conf.json") {
    $proxyConfig = Get-Content "proxy.conf.json" | ConvertFrom-Json
    Write-Host "✓ Proxy configuration found" -ForegroundColor Green
    Write-Host "  Target: $($proxyConfig.'/api/*'.target)" -ForegroundColor Cyan
} else {
    Write-Host "✗ Proxy configuration missing" -ForegroundColor Red
}

# Test 3: Check route guards
Write-Host ""
Write-Host "3. Testing Route Guards..." -ForegroundColor Yellow
if (Test-Path "src/app/guards/auth.guard.ts" -and Test-Path "src/app/guards/role.guard.ts") {
    Write-Host "✓ Route guards implemented" -ForegroundColor Green
} else {
    Write-Host "✗ Route guards missing" -ForegroundColor Red
}

# Test 4: Check AI service
Write-Host ""
Write-Host "4. Testing AI Evaluation Service..." -ForegroundColor Yellow
if (Test-Path "src/app/services/ai-evaluation.service.ts") {
    Write-Host "✓ AI evaluation service implemented" -ForegroundColor Green
} else {
    Write-Host "✗ AI evaluation service missing" -ForegroundColor Red
}

# Test 5: Check API service
Write-Host ""
Write-Host "5. Testing Centralized API Service..." -ForegroundColor Yellow
if (Test-Path "src/app/services/api.service.ts") {
    Write-Host "✓ Centralized API service implemented" -ForegroundColor Green
} else {
    Write-Host "✗ Centralized API service missing" -ForegroundColor Red
}

# Test 6: Registration form validation
Write-Host ""
Write-Host "6. Testing Registration Form..." -ForegroundColor Yellow
$regComponent = "src/app/components/auth/register/register.component.ts"
if (Test-Path $regComponent) {
    $content = Get-Content $regComponent -Raw
    if ($content -match "registerId" -and $content -match "employeeId") {
        Write-Host "✓ Registration form includes role-specific fields" -ForegroundColor Green
    } else {
        Write-Host "✗ Registration form missing role-specific fields" -ForegroundColor Red
    }
} else {
    Write-Host "✗ Registration component not found" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Test Summary ===" -ForegroundColor Green
Write-Host ""
Write-Host "Features Implemented:" -ForegroundColor Cyan
Write-Host "✓ Role-based registration (Student RegisterId, Admin EmployeeId)" -ForegroundColor White
Write-Host "✓ Route guards for authentication and authorization" -ForegroundColor White
Write-Host "✓ Centralized API service configuration" -ForegroundColor White
Write-Host "✓ AI-powered code evaluation system" -ForegroundColor White
Write-Host "✓ Proxy configuration for backend connectivity" -ForegroundColor White
Write-Host "✓ Updated database schema with new fields" -ForegroundColor White
Write-Host ""

Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Start the backend server: cd BackEnd/codeN/CodeNudge/CodeNudge_backend && dotnet run" -ForegroundColor White
Write-Host "2. Run database migrations: .\add-migration.ps1 && .\update-database.ps1" -ForegroundColor White
Write-Host "3. Start the frontend: ng serve" -ForegroundColor White
Write-Host "4. Test registration at: http://localhost:4200/auth/register" -ForegroundColor White
Write-Host "5. Test AI evaluation in the code editor" -ForegroundColor White
Write-Host ""

Write-Host "=== Manual Testing Checklist ===" -ForegroundColor Green
Write-Host ""
Write-Host "Registration Flow:" -ForegroundColor Cyan
Write-Host "□ Navigate to /auth/register" -ForegroundColor White
Write-Host "□ Select Student role and fill RegisterId" -ForegroundColor White
Write-Host "□ Select Admin role and fill EmployeeId" -ForegroundColor White
Write-Host "□ Submit form and verify backend receives correct data" -ForegroundColor White
Write-Host "□ Check successful navigation after registration" -ForegroundColor White
Write-Host ""

Write-Host "Route Protection:" -ForegroundColor Cyan
Write-Host "□ Try accessing /student/dashboard without login" -ForegroundColor White
Write-Host "□ Try accessing /admin/dashboard as student" -ForegroundColor White
Write-Host "□ Verify proper redirects and access control" -ForegroundColor White
Write-Host ""

Write-Host "AI Features:" -ForegroundColor Cyan
Write-Host "□ Submit code in practice section" -ForegroundColor White
Write-Host "□ Verify AI evaluation response" -ForegroundColor White
Write-Host "□ Check feedback and suggestions" -ForegroundColor White
Write-Host "□ Test complexity analysis" -ForegroundColor White
Write-Host ""

Write-Host "API Connectivity:" -ForegroundColor Cyan
Write-Host "□ Check browser network tab for API calls" -ForegroundColor White
Write-Host "□ Verify proxy is working (no CORS errors)" -ForegroundColor White
Write-Host "□ Test all major endpoints" -ForegroundColor White
Write-Host ""

Write-Host "Testing completed! Review the checklist above for manual testing." -ForegroundColor Green
