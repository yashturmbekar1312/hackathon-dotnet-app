@echo off
echo Setting environment variables for development...

REM Email Service Configuration
set BREVO_API_KEY=your_brevo_api_key_here

REM JWT Configuration
set JWT_SECRET_KEY=DevelopmentSecretKeyThatIsAtLeast32CharactersLongForDevelopment2025

REM Development Environment
set ASPNETCORE_ENVIRONMENT=Development
set ASPNETCORE_URLS=http://localhost:5000;https://localhost:5001

echo Environment variables set successfully!
echo You can now run: dotnet run
