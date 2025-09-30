# PowerShell script to update the database with new migrations
# Run this script from the project root directory

Write-Host "Updating database with new migrations..." -ForegroundColor Green

# Update database
dotnet ef database update --project Infrastructure --startup-project Presentation

Write-Host "Database updated successfully!" -ForegroundColor Green
