@echo off
REM Playwright C# AI Test Generation - Quick Setup Script for Windows
REM This script helps new developers set up the environment quickly

echo 🚀 Playwright C# AI Test Generation - Setup Script
echo ==================================================

REM Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET SDK not found. Please install .NET 9.0 SDK or later from:
    echo    https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo ✅ .NET SDK found: %DOTNET_VERSION%

REM Navigate to C# project directory
set SCRIPT_DIR=%~dp0
set CSHARP_DIR=%SCRIPT_DIR%playwright-multilang\csharp-playwright

if not exist "%CSHARP_DIR%" (
    echo ❌ C# project directory not found at: %CSHARP_DIR%
    pause
    exit /b 1
)

cd /d "%CSHARP_DIR%"
echo 📁 Working in: %CD%

REM Check for OpenAI API key
if "%OPENAI_API_KEY%"=="" (
    echo ⚠️  OPENAI_API_KEY environment variable not set.
    echo    The AI test generation will use fallback tests instead.
    echo    To enable AI generation, set your API key:
    echo    setx OPENAI_API_KEY "sk-your-api-key-here"
    echo.
) else (
    echo ✅ OPENAI_API_KEY environment variable is set
)

REM Restore and build project
echo 📦 Restoring NuGet packages...
dotnet restore

if %errorlevel% neq 0 (
    echo ❌ Failed to restore packages
    pause
    exit /b 1
)

echo 🔨 Building project...
dotnet build

if %errorlevel% neq 0 (
    echo ❌ Build failed
    pause
    exit /b 1
)

echo ✅ Build successful

REM Install Playwright browsers
echo 🌐 Installing Playwright browsers...
if exist "bin\Debug\net9.0\playwright.ps1" (
    powershell -ExecutionPolicy Bypass -File "bin\Debug\net9.0\playwright.ps1" install
) else if exist "bin\Debug\net9.0\playwright.dll" (
    dotnet exec "bin\Debug\net9.0\playwright.dll" install
) else (
    echo ⚠️  Playwright installation files not found. You may need to install browsers manually.
)

REM Run tests to verify setup
echo 🧪 Running tests to verify setup...
dotnet test

if %errorlevel% equ 0 (
    echo.
    echo 🎉 Setup completed successfully!
    echo.
    echo Next steps:
    echo 1. Set your OpenAI API key (if not already set^):
    echo    setx OPENAI_API_KEY "sk-your-api-key-here"
    echo.
    echo 2. Generate a test:
    echo    dotnet run -- "https://jsonplaceholder.typicode.com" "JSONPlaceholder API" "Test POST endpoint"
    echo.
    echo 3. Run tests:
    echo    dotnet test
    echo.
) else (
    echo ⚠️  Tests failed. Check the output above for issues.
)

pause
