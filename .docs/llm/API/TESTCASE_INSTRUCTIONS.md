# API Test Case Template (Simplified)

## Format

```markdown
# TC{ID}: {Title}

**Azure Test Case:** {ID}  
**Suite:** {SuiteName}-{SuiteID}  
**Thunderclient:** `collections/{path}` OR ❌ Not implemented  
**Test Users:** {Range description, e.g., "9200-9299 range (active user with roles)"}

---

## What This Tests

{Clear 1-2 sentence description of what backend behavior and database state is being verified}

---

## Test Checklist

### Request
- Method: POST/GET/PUT/DELETE
- Endpoint: `/api/path`
- Body: { key fields }

### Response Checks
- [ ] Status code is correct
- [ ] Response contains required fields
- [ ] Field values are valid/expected
- [ ] Data types are correct

### Database Checks
- [ ] **TableName:**
  - Record created/updated/deleted as expected
  - Key fields match response
  - Timestamps are recent
  - Related records updated correctly

### Edge Cases (if applicable)
- [ ] {High-level edge case description}

---

## Notes

{Any important context, gotchas, or related information}
```

---

## Example: Simplified Test Case

```markdown
# TC12345: Valid Login API

**Azure Test Case:** 12345  
**Suite:** Login-12300  
**Thunderclient:** `client-site-v2.1/01-login/login.json` ✅  
**Test Users:** 9200-9299 range (active user with roles)

---

## What This Tests

User with valid credentials can successfully login via API, receive a valid JWT token, and have correct login state recorded in database.

---

## Test Checklist

### Request
- Method: POST
- Endpoint: `/api/user/login`
- Body: { clientIdentifier, username, password }

### Response Checks
- [ ] Status code is 200
- [ ] Response contains: token, refreshToken, user object
- [ ] Token is valid JWT format
- [ ] Token expiry is reasonable
- [ ] RefreshToken is valid GUID
- [ ] User ID matches expected user
- [ ] Username matches request

### Database Checks
- [ ] **UserLoginDetail:**
  - Login record created for test user
  - LoginDateTime is recent
  - RefreshToken matches response
  - RefreshTokenExpiry is reasonable
  - SessionValidationToken exists

---

## Notes

- Token can be reused for authenticated API calls
- Complements UI login test (different layer)
```

---

## What to Remove

❌ **Remove:**
- Detailed "Source Analysis" sections
- Step-by-step "Arrange-Act-Assert" implementation
- Specific code examples (Assert.Equal, etc.)
- "Automation Approach" sections with tool choices
- Long explanations and rationale
- "Quality Checklist" sections

✅ **Keep:**
- What the test verifies (brief, clear)
- Thunderclient location
- Simple checklist of what to verify
- Database tables/fields to check with SQL hints
- Brief notes for important context only

---

## Rationale

**Test cases are specifications, not implementation guides.**

The developer writing the test will:
- Have access to Thunderclient collections for request/response examples
- Know how to use xUnit Assert
- Know how to use DatabaseHelper and ApiHelper
- Make implementation decisions based on the framework

The test case should answer:
- **What** needs to be tested?
- **Where** is the existing Thunderclient test (if any)?
- **Which** database tables to verify?

**Critical:** API tests are DEEP tests. The key value-add over UI tests is DATABASE VERIFICATION. Every API test must specify which tables/fields to check for persistence.
