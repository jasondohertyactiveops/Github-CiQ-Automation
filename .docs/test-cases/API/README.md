# API Test Cases

This directory contains comprehensive API test case specifications organized by API domain.

## Directory Structure

```
.docs/test-cases/API/
├── ClientAPI/              # User-facing application API
│   └── Login-25146/       # Login & authentication tests
├── InternalAPI/           # Internal/admin API (future)
├── IntegrationAPI/        # Third-party integration API (future)
└── README.md              # This file
```

## API Domains

### ClientAPI
**Thunderclient Collection:** `client-site-v2.1/`, `client-api-*/`  
**Purpose:** User-facing endpoints for web/mobile applications  
**Examples:** Login, workgroups, tasks, capacity planning  
**Status:** ✅ In Progress (Login suite documented)

### InternalAPI (Future)
**Thunderclient Collection:** `internal-api/`  
**Purpose:** Internal/admin endpoints  
**Examples:** System configuration, bulk operations  
**Status:** ⏳ Not Started

### IntegrationAPI (Future)
**Thunderclient Collection:** `integration-api/`  
**Purpose:** Third-party integration endpoints  
**Examples:** External system connectors, webhooks  
**Status:** ⏳ Not Started

## Getting Started

1. **Choose API Domain:** Navigate to the appropriate API directory (e.g., `ClientAPI/`)
2. **Review Test Suites:** Each suite corresponds to an Azure DevOps test suite
3. **Read Test Cases:** Each MD file contains complete test specification
4. **Implement Tests:** Follow the automation approach documented in each test case

## Documentation Standards

All API test cases follow the template in:  
`.docs/llm/TESTCASE_INSTRUCTIONS_API.md`

Key elements:
- **Source Analysis:** Azure + Thunderclient + API exploration
- **Request/Response Specs:** Complete API contract documentation
- **Database Verification:** SQL queries to verify data persistence
- **Test Steps:** Arrange-Act-Assert-Cleanup format
- **Automation Approach:** Technology stack and patterns

## Test User Ranges

- **9000-9199:** UI test users
- **9200-9299:** API test users (separate to avoid conflicts)

See: `.docs/llm/SEEDING-REFERENCE.md` for complete user list

## Naming Conventions

### Directories
- `{APIDomain}/` - API category (ClientAPI, InternalAPI, etc.)
- `{SuiteName}-{SuiteID}/` - Azure test suite

### Files
- `{Order}-TC{ID}-{Name}.md` - Regular test case
- `{Order}-TC{ID}-{Name}-NOT-API.md` - Not suitable for API testing
- `{Order}-TC{ID}-{Name}-TODO.md` - Needs discussion/resolution

## Resources

- **API Test Case Template:** `.docs/llm/TESTCASE_INSTRUCTIONS_API.md`
- **UI Test Case Template:** `.docs/llm/TESTCASE_INSTRUCTIONS.md`
- **Seeding Reference:** `.docs/llm/SEEDING-REFERENCE.md`
- **Thunderclient Collections:** `WW7/ww7-api/thunder-tests/collections/`

## Migration from Thunderclient

Process for converting Thunderclient tests to documented test cases:

1. **Identify Collection:** Find relevant Thunderclient collection
2. **Create Directory:** Create appropriate domain/suite directories
3. **Document Test Cases:** Use API test case template
4. **Include DB Verification:** Add SQL queries for data validation
5. **Review:** Ensure completeness before implementation

## Progress Tracking

### ClientAPI
- [x] Login Suite (Login-25146) - 75% documented
  - [x] TC25057 - Valid Login
  - [x] TC25058 - Invalid Login
  - [x] TC25060 - Token Refresh
  - [ ] TC25059 - Email Link Expiry (TODO)

### InternalAPI
- [ ] Not started

### IntegrationAPI
- [ ] Not started

## Next Steps

1. **Complete Login Suite:** Resolve TODO cases, implement tests
2. **Expand ClientAPI:** Document more suites from Thunderclient
3. **Add Other Domains:** Start InternalAPI, IntegrationAPI as needed
4. **Build Framework:** Create AO.Automation.API.Client project
5. **Seed Data:** Create API test users (9200-9299)
