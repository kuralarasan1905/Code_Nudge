# PowerShell script to add Entity Framework migration for new user fields
# Run this script from the project root directory

Write-Host "Adding Entity Framework migration for RegisterId and EmployeeId fields..." -ForegroundColor Green

# Add migration
dotnet ef migrations add AddUserRoleSpecificFields --project Infrastructure --startup-project Presentation

Write-Host "Migration added successfully!" -ForegroundColor Green
Write-Host "To update the database, run: dotnet ef database update --project Infrastructure --startup-project Presentation" -ForegroundColor Yellow
