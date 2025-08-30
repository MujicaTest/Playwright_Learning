# Playwright Multi-language Demo with AI Test Generation

This project demonstrates how to write and run Playwright tests using multiple languages, plus an **AI-powered test generation framework** that automatically creates tests from API exploration.

## 🌟 Features

- **Multi-language Playwright testing**: JavaScript, Python, C#
- **🤖 AI Test Generation**: Uses OpenAI GPT models to generate Playwright tests automatically
- **API Discovery**: Automatically explores APIs to understand their structure
- **Security-First**: No API keys stored in repository (uses environment variables)
- **Fallback Support**: Works without AI when API keys are unavailable

## 📁 Folder Structure
```
playwright-multilang/
├── js-playwright/       → Playwright with JavaScript
├── python-playwright/   → Playwright with Python  
├── csharp-playwright/   → Playwright with C# + AI Test Generation Framework
│   ├── Framework/AI/    → AI-powered test generation system
│   ├── GeneratedTests/  → AI-generated test output
│   └── Extensions/      → Helper utilities
└── README.md
```

## 🚀 Quick Start (AI Test Generation)

### Prerequisites
- .NET 9.0 SDK or later
- OpenAI API key (for AI test generation)

### Setup Options

#### Option 1: Automated Setup (Recommended)
```bash
# macOS/Linux
./setup.sh

# Windows
setup.bat
```

#### Option 2: Manual Setup
See detailed guide: [`SETUP_REQUIREMENTS.txt`](SETUP_REQUIREMENTS.txt)

### Basic Usage

1. **Set your OpenAI API key**:
   ```bash
   # macOS/Linux
   export OPENAI_API_KEY="sk-your-api-key-here"
   
   # Windows
   setx OPENAI_API_KEY "sk-your-api-key-here"
   ```

2. **Generate AI tests**:
   ```bash
   cd playwright-multilang/csharp-playwright
   dotnet run -- "https://jsonplaceholder.typicode.com" "JSONPlaceholder API" "Test POST endpoint"
   ```

3. **Run all tests**:
   ```bash
   dotnet test
   ```

## 📦 Prerequisites

Make sure you have these tools installed on your Mac:

- Node.js: `brew install node`
- Python 3: `brew install python`
- .NET SDK: `brew install --cask dotnet-sdk`
- Git: `brew install git`

---

## ▶️ How to Run Tests

### 🟩 JavaScript
```bash
cd js-playwright
npx playwright test
```

### 🟨 Python
```bash
cd python-playwright
source venv/bin/activate
pytest
```

### 🟦 C#
```bash
cd csharp-playwright
dotnet run
```

---

## 🧪 Test Description

All tests navigate to [https://playwright.dev](https://playwright.dev) and check that the page title contains "Playwright".

---

## 🔧 License

MIT
