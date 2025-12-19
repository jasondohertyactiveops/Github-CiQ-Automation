# CiQ Test Automation

Automated Playwright .NET tests generated from Azure DevOps manual test cases.

## Overview

This project converts manual test cases from Azure Test Plans into fully automated Playwright .NET (C#) end-to-end tests.

## Prerequisites

- Azure DevOps MCP server (access to test plans, suites, and test case details)
- Playwright MCP server (live browser interaction and code generation)
- .NET 10
- Playwright for .NET

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

## Getting Started

Documentation coming soon.
