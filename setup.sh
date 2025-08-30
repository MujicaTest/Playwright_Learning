#!/bin/bash

# Playwright C# AI Test Generation - Quick Setup Script
# This script helps new developers set up the environment quickly

echo "🚀 Playwright C# AI Test Generation - Setup Script"
echo "=================================================="

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 9.0 SDK or later from:"
    echo "   https://dotnet.microsoft.com/download"
    exit 1
fi

echo "✅ .NET SDK found: $(dotnet --version)"

# Navigate to C# project directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CSHARP_DIR="$SCRIPT_DIR/playwright-multilang/csharp-playwright"

if [ ! -d "$CSHARP_DIR" ]; then
    echo "❌ C# project directory not found at: $CSHARP_DIR"
    exit 1
fi

cd "$CSHARP_DIR"
echo "📁 Working in: $(pwd)"

# Check for OpenAI API key
if [ -z "$OPENAI_API_KEY" ]; then
    echo "⚠️  OPENAI_API_KEY environment variable not set."
    echo "   The AI test generation will use fallback tests instead."
    echo "   To enable AI generation, set your API key:"
    echo "   export OPENAI_API_KEY=\"sk-your-api-key-here\""
    echo ""
else
    echo "✅ OPENAI_API_KEY environment variable is set"
fi

# Restore and build project
echo "📦 Restoring NuGet packages..."
dotnet restore

if [ $? -ne 0 ]; then
    echo "❌ Failed to restore packages"
    exit 1
fi

echo "🔨 Building project..."
dotnet build

if [ $? -ne 0 ]; then
    echo "❌ Build failed"
    exit 1
fi

echo "✅ Build successful"

# Install Playwright browsers
echo "🌐 Installing Playwright browsers..."
if [ -f "bin/Debug/net9.0/playwright.ps1" ]; then
    pwsh bin/Debug/net9.0/playwright.ps1 install
elif [ -f "bin/Debug/net9.0/playwright.dll" ]; then
    dotnet exec bin/Debug/net9.0/playwright.dll install
else
    echo "⚠️  Playwright installation files not found. You may need to install browsers manually."
fi

# Run tests to verify setup
echo "🧪 Running tests to verify setup..."
dotnet test

if [ $? -eq 0 ]; then
    echo ""
    echo "🎉 Setup completed successfully!"
    echo ""
    echo "Next steps:"
    echo "1. Set your OpenAI API key (if not already set):"
    echo "   export OPENAI_API_KEY=\"sk-your-api-key-here\""
    echo ""
    echo "2. Generate a test:"
    echo "   dotnet run -- \"https://jsonplaceholder.typicode.com\" \"JSONPlaceholder API\" \"Test POST endpoint\""
    echo ""
    echo "3. Run tests:"
    echo "   dotnet test"
    echo ""
else
    echo "⚠️  Tests failed. Check the output above for issues."
fi
