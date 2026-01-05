# Client API Test Cases Documentation

This directory contains comprehensive test case specifications for the **Client API** (user-facing API endpoints for the ControliQ application).

## Overview

Client API tests verify backend endpoints used by the web/mobile applications:
- **Authentication:** Login, logout, token refresh
- **User Management:** Account activation, password reset, preferences
- **Data Operations:** CRUD operations for business entities
- **Business Logic:** Capacity planning, resource management, etc.

These tests complement UI tests by testing the backend layer directly.

## Current Status

### Login Suite (Login-25146)

| Test Case | Thunderclient | Documentation | Implementation | Priority |
|-----------|---------------|---------------|----------------|----------|
| TC25057 - Valid Login | ✅ Exists | 📝 Complete | ⏳ Not Started | 🟢 Medium |
| TC25058 - Invalid Login | ❌ **Missing** | 📝 Complete | ⏳ Not Started | 🔴 **Critical** |
| TC25060 - Token Refresh | ❌ **Missing** | 📝 Complete | ⏳ Not Started | 🔴 **Critical** |
| TC25059 - Email Link Expiry | ❌ Missing (Azure only) | 📝 Complete | ⏳ Not Started | 🟡 Medium |

**Thunderclient Coverage:** 25% (1/4 exists)  
**Documentation:** 100% (4/4 documented)  
**Implementation:** 0% (0/4 implemented)

**Note:** Each test case includes a "Gap Analysis" section showing what's missing from Thunderclient and priority for implementation.

## Documentation Approach

Each test case MD file contains:

1. **What This Tests**
   - Clear 1-2 sentence description

2. **Test Checklist**
   - Request details (method, endpoint, body)
   - Response checks (status, fields, validations)
   - Database checks (tables, what to verify)
   - Edge cases (if applicable)

3. **Gap Analysis**
   - Thunderclient coverage status
   - What's missing or needs enhancement
   - Priority and action items

4. **Notes**
   - Important context only

## Key Differences from UI Test Cases

**API Test Cases Focus On:**
- Backend endpoint behavior
- Database state verification
- Request/response validation
- Business logic at API layer

**UI Test Cases Focus On:**
- User-visible behavior
- Page interactions and workflows
- Element locators
- Browser-based validations

**Both are needed for complete coverage!**

## Test User Ranges

- **9000-9199:** UI test users
- **9200-9299:** API test users (separate to avoid conflicts)

See: `.docs/llm/SEEDING-REFERENCE.md` for complete user list

## Next Steps

### Phase 1: Complete Documentation ✅ (IN PROGRESS)
- [x] Create API test case template
- [x] Document Login tests (TC25057, TC25058, TC25060)
- [x] Document TODO cases (TC25059)
- [ ] Review and refine test cases
- [ ] Decide on time manipulation approach for TC25059

### Phase 2: Build Framework
- [ ] Create AO.Automation.API.Client project
- [ ] Implement DatabaseHelper (with Dapper)
- [ ] Implement ApiHelper (HTTP client wrapper)
- [ ] Create response/request DTOs
- [ ] Configure appsettings for local/test environments

### Phase 3: Implement Tests
- [ ] Implement TC25057 - Valid Login
- [ ] Implement TC25058 - Invalid Login (3 scenarios)
- [ ] Implement TC25060 - Token Refresh
- [ ] Implement TC25059 - Email Link Expiry (after approach decided)

### Phase 4: Seed Data
- [ ] Create ApiTestUsers.sql seeding script
- [ ] Seed users 9200-9205
- [ ] Configure special user states (inactive, no roles, invited)
- [ ] Document security stamps for token generation

### Phase 5: Expand Coverage
- [ ] Convert more Thunderclient tests to test cases
- [ ] Identify gaps in Thunderclient coverage
- [ ] Document and implement additional API tests
- [ ] Move relevant "NOT-UI" tests from Cypress to API suite

## Resources

- **Test Case Template:** `.docs/llm/TESTCASE_INSTRUCTIONS_API.md`
- **Seeding Reference:** `.docs/llm/SEEDING-REFERENCE.md`
- **Thunderclient Collections:** `WW7/ww7-api/thunder-tests/collections/`
- **UI Test Cases (for comparison):** `.docs/test-cases/UI/ClientApp/Login-25146/`

## Success Criteria

API test automation is successful when:

✅ **Coverage**
- All API endpoints have corresponding test cases
- NOT-UI tests migrated from Cypress/UI suite
- Critical authentication and data persistence flows covered

✅ **Quality**
- Each test verifies both API response AND database state
- Tests use seeded data (no production dependencies)
- Tests are fast, reliable, and independent

✅ **Maintainability**
- Test cases are well-documented before implementation
- Clear separation between UI and API test users
- Database queries are documented in test cases
- SQL is kept simple (no stored procedures for test verification)

✅ **Integration**
- API tests run in CI/CD pipeline
- Can run in parallel with UI tests (separate data)
- Database can be easily seeded and reset
- Tests work in local dev and test environments

## Questions or Issues?

Refer to:
- **API Test Case Instructions:** `.docs/llm/TESTCASE_INSTRUCTIONS_API.md`
- **UI Test Case Instructions:** `.docs/llm/TESTCASE_INSTRUCTIONS.md` (for comparison)
- **Project Patterns:** `.docs/llm/PATTERNS.md`
- **Rationale:** `.docs/llm/RATIONALE.md`
