# Playwright Multi-language Demo

This project demonstrates how to write and run the same Playwright test using:
- JavaScript
- Python
- C#

## 📁 Folder Structure
```
playwright-multilang/
├── js-playwright/       → Playwright with JavaScript
├── python-playwright/   → Playwright with Python
├── csharp-playwright/   → Playwright with C#
└── README.md
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
