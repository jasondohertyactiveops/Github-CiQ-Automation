# CiQ Test Automation

Automated Playwright .NET tests generated from Azure DevOps manual test cases.

## Overview

This project converts manual test cases from Azure Test Plans into fully automated Playwright .NET (C#) end-to-end tests.

## Prerequisites

### For Running Tests
- **.NET 10 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/10.0)
- **PowerShell** - For running Playwright browser install script (included with Windows)

### For Development
- **Azure DevOps MCP server** - Access to test plans, suites, and test case details
- **Playwright MCP server** (optional) - Live browser interaction for exploring the app
- **Git** - Version control
- **Visual Studio Code** or **Visual Studio** - Code editor

## Workflow

1. Query Azure DevOps for manual test case details (by ID, suite, or plan name)
2. Explore the live app with Playwright tools for accurate locators
3. Generate clean, reliable Playwright .NET C# tests following manual steps
4. Apply best practices:
   - Auto-waits
   - Strong locators
   - Expect assertions
   - Reusable authentication

## Test Structure

- One test class per test case (or as appropriate)
- Readable, maintainable code
- Iterative approach: generate → run → heal failures

## Documentation

### For LLMs (AI Assistants)
**Start here when working on this project:**
- **[LLM-GUIDE.md](.docs/llm/LLM-GUIDE.md)** - Your capabilities and limitations on this Windows machine
- **[PROJECT_INSTRUCTIONS.md](.docs/llm/PROJECT_INSTRUCTIONS.md)** - Complete architectural decisions (Q1-Q16)
- **[WORKFLOW.md](.docs/llm/WORKFLOW.md)** - How to work with the user (fetch → generate → run → fix → commit)
- **[PATTERNS.md](.docs/llm/PATTERNS.md)** - Code examples and reference implementations
- **[RATIONALE.md](.docs/llm/RATIONALE.md)** - Why decisions were made (for consistency)

### For Developers
Coming soon:
- Getting Started Guide
- Local Environment Setup
- Contributing Guidelines

## Project Status

**Current Phase:** Foundation and Architecture
- ✅ Project structure defined
- ✅ Architectural decisions documented
- ✅ Workflow established
- ⏳ Awaiting project setup

## Quick Start

### Initial Setup (One-Time)

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/Github-CiQ-Automation.git
   cd Github-CiQ-Automation
   ```

2. **Restore NuGet packages**
   ```bash
   cd src/PlaywrightTests
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Install Playwright browsers** (one-time, ~300MB download)
   ```bash
   pwsh bin/Debug/net10.0/playwright.ps1 install
   ```
   This downloads Chromium, Firefox, and WebKit browsers that Playwright manages.

### Running Tests

```bash
cd src/PlaywrightTests

# Run all tests
dotnet test

# Run specific feature
dotnet test --filter "Feature=Login"

# Run smoke tests only
dotnet test --filter "Suite=Smoke"
```

### First Time Setup Complete!
You're ready to run tests or start automating new ones.

## Technology Stack

- **.NET 10**
- **xUnit** - Test framework
- **Playwright for .NET** - Browser automation
- **C#** - Programming language
- **Azure DevOps** - Test case management

## Key Principles

1. **Pre-seeded database** - Fresh, deterministic test data
2. **Reusable auth state** - Fast authentication
3. **Page Object Model** - Maintainable test structure
4. **Feature-based organization** - Tests grouped by functionality
5. **Semantic locators first** - Resilient, accessible selectors
6. **No database interrogation** - UI tests via UI only
7. **Independent tests** - Can run in any order, in parallel

## Contributing

*Guidelines coming soon*

## License

*To be determined*
