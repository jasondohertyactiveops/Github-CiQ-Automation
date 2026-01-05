# LLM Documentation Structure

This directory contains guidance for LLMs working on the automation project.

## Organization

**Shared/** - Applies to both UI and API automation  
**UI/** - UI test automation (Playwright browser tests)  
**API/** - API test automation (Playwright API tests)

## Shared Documentation

**LLM-GUIDE.md** - Environment setup, capabilities, limitations  
**SEEDING-REFERENCE.md** - Test user IDs for both UI (9000-9199) and API (9200-9299)

## UI Documentation

**TESTCASE_INSTRUCTIONS.md** - How to create UI test case documentation  
**PROJECT_INSTRUCTIONS.md** - Architecture decisions (Q1-Q16)  
**WORKFLOW.md** - How to work with LLM to automate UI tests  
**PATTERNS.md** - Code examples and patterns for UI tests  
**RATIONALE.md** - Why decisions were made

## API Documentation

**TESTCASE_INSTRUCTIONS.md** - How to create API test case documentation  
**PROJECT_INSTRUCTIONS.md** - Architecture decisions for API framework  
**WORKFLOW.md** - How to create API test cases from Thunderclient  
**PATTERNS.md** - Code examples and patterns for API tests

## Documentation Parity

Both UI and API have the same documentation structure to ensure equal attention and completeness:
- Test case instructions
- Project architecture decisions  
- Collaboration workflow
- Code patterns and examples

This makes it easy to apply consistent standards across both automation approaches.
