# API Test Automation - Workflow

## How to Generate API Test Cases from Thunderclient

### The Process

```
1. Identify collection → 2. Read Thunderclient JSON → 3. Generate test case MD → 4. Review → 5. Implement test
```

---

## Step-by-Step Workflow

### Step 1: Identify Thunderclient Collection
**You provide:**
- Collection path: `client-site-v2.1/02-dashboard`
- OR specific request: "Process the login.json request"
- OR domain: "Generate test cases for all ClientAPI login tests"

### Step 2: I Read Thunderclient JSON
**I will:**
- Read the collection folder structure
- Parse request JSON files
- Extract: method, endpoint, body, headers, tests, scripts
- Identify what's being tested (status codes, response validation)
- Note any gaps (missing assertions, no DB checks)

**Example:**
```
You: "Process client-site-v2.1/01-login collection"
Me: [Reads all JSON files in folder]
Me: "Found 6 requests:
     - login.json ✅
     - login-aonly-user.json ✅ 
     - invalid-reset-password... (password reset endpoint)
     - valid-reset-password... (password reset endpoint)
     - update-permissions... (permissions endpoint)
     Should I create test cases for all login-related ones?"
```

### Step 3: I Generate Test Case MD
**I will create:**
- Test case markdown file following template
- Include Thunderclient location and status
- Add database verification checklist
- Note any gaps or missing coverage
- High-level, implementation-agnostic

**Example output:**
```
Created:
- .docs/test-cases/API/ClientAPI/Login-25146/01-TC25057-ValidCredentialsLogin.md

Test case includes:
- What it tests (1-2 sentences)
- Request/response checklist
- Database verification points
- Gap analysis (what's missing from Thunderclient)
```

### Step 4: You Review
**You verify:**
- Test case accurately reflects what should be tested
- Database verification is appropriate
- Nothing over-specified (no hardcoded values like user IDs)
- Gap analysis is accurate

**Then tell me:**
- ✅ "Looks good, ready to implement" → Move to implementation
- ⚠️ "Update X in the test case" → I'll adjust
- ❓ "Is this actually API-appropriate?" → We discuss

### Step 5: Implement Test Code
**I will create:**
- Test class file (e.g., `ValidCredentialsLogin.cs`)
- Request/Response models if needed
- Database helper methods if needed

**Example:**
```
Created:
- Tests/Login/ValidCredentialsLogin.cs
- Models/Responses/LoginResponse.cs

Ready to run: dotnet test --filter "Feature=Login"
```

---

## Processing Thunderclient Collections

Thunderclient organizes tests as JSON files in folders. Each request file contains method, endpoint, body, headers, tests (assertions), and post-request scripts.

### Request JSON Format
Each request file contains:
- `name` - Request name
- `method` - HTTP method (GET, POST, PUT, DELETE)
- `url` - Endpoint with {{variables}}
- `body` - Request body (if applicable)
- `headers` - Custom headers
- `tests` - Assertions (status code, schema validation)
- `postReq` - Scripts run after request (variable setting)

### What to Extract

**For test case documentation:**
1. **Endpoint & Method** - What API is being called
2. **Request structure** - Body/headers/params
3. **Expected response** - Status code, fields
4. **Tests/Assertions** - What Thunderclient validates
5. **Post-request actions** - What variables are set (indicates dependencies)

**What to add (not in Thunderclient):**
- Database verification requirements
- Proper test user allocation
- Gap analysis

### Common Patterns

**Pattern 1: Simple CRUD endpoint**
```
login.json → TC25057-ValidCredentialsLogin.md
- POST to /user/login
- Checks status 200
- Missing: token validation, DB verification
- Add: UserLoginDetail table check
```

**Pattern 2: Invalid scenario (often missing)**
```
No file found → TC25058-InvalidCredentialsLogin.md
- Thunderclient doesn't test this
- Document what SHOULD be tested
- Mark as critical gap
- Implement from scratch
```

**Pattern 3: Complex workflow (multiple requests)**
```
Multiple JSONs for one flow → Single comprehensive test case
- Document entire flow
- Note dependencies between requests
- May need multiple test methods
```

---

## Common Workflows

### Generate Test Cases for Entire Collection
```
You: "Generate test cases for client-site-v2.1/01-login"
Me: [Reads all request files]
Me: [Creates test case MD for each relevant request]
Me: [Identifies gaps in coverage]
Me: "Created 3 test cases, identified 2 critical gaps"
```

### Generate Test Case for Single Request
```
You: "Create test case for login.json"
Me: [Reads login.json]
Me: [Generates TC25057-ValidCredentialsLogin.md]
Me: "Test case created with DB verification added"
```

### Identify Coverage Gaps
```
You: "What's missing from the login collection?"
Me: [Analyzes all requests]
Me: "Found gaps:
     - No invalid password tests ❌ Critical
     - No inactive user tests ❌ Critical
     - No token refresh tests ❌ Critical"
```

### Implement Test from Test Case
```
You: "Implement TC25057"
Me: [Reads test case MD]
Me: [Generates C# test class]
Me: [Creates response models]
Me: [Adds DatabaseHelper methods if needed]
```

---

## Test Case → Code Workflow

### From Test Case MD to C# Test

**Test case says:**
```markdown
### Response Checks
- [ ] Status code is 200
- [ ] Response contains: token, refreshToken
- [ ] User ID matches expected user
```

**I generate:**
```csharp
var response = await _apiHelper.LoginAsync(request);
Assert.Equal(200, response.StatusCode);
Assert.NotNull(response.Token);
Assert.NotNull(response.RefreshToken);
Assert.Equal(expectedUserId, response.User.Id);
```

**Test case says:**
```markdown
### Database Checks
- [ ] UserLoginDetail:
  - Login record created
  - RefreshToken matches response
```

**I generate:**
```csharp
var dbRecord = await _dbHelper.GetLatestLoginDetail(userId);
Assert.NotNull(dbRecord);
Assert.Equal(response.RefreshToken, dbRecord.RefreshToken);
```

---

## Key Differences from UI Workflow

| Aspect | UI Workflow | API Workflow |
|--------|-------------|--------------|
| **Source** | Azure DevOps test cases | Thunderclient collections + Azure |
| **Step 1** | Fetch Azure test case | Read Thunderclient JSON |
| **Generation** | Page objects + tests | Request models + tests |
| **Verification** | UI elements visible | Response + Database |
| **Dependencies** | Browser automation | HTTP + Database |

---

## Tips for Success

### 1. Start with Gaps
- Implement critical missing tests first (invalid login, etc.)
- Don't just convert what exists in Thunderclient
- Build comprehensive coverage

### 2. Database Verification
- Always add DB checks for state changes
- Thunderclient can't do this - it's our value-add
- Keep SQL queries simple and focused

### 3. Test User Discipline  
- Use 9200-9299 range consistently
- Document user configurations
- Avoid conflicts with UI tests

### 4. Group Related Tests
- Multiple scenarios in Thunderclient → One test case with sub-scenarios
- Example: 3 invalid login scenarios in one test case

### 5. Leverage Existing Infrastructure
- Reuse TokenHelper from UI project
- Same appsettings structure
- Same database recreation scripts

---

## What I Can Help With

✅ Read and parse Thunderclient collections  
✅ Generate test case MD files  
✅ Identify coverage gaps  
✅ Create test code (classes, models, helpers)  
✅ Add database verification  
✅ Suggest SQL queries for verification

---

## What I Cannot Do

❌ Run tests (you run them)  
❌ Install NuGet packages (you install via dotnet)  
❌ Execute SQL queries (you verify via tests)  
❌ Access actual APIs (you describe behavior)

---

## Ready to Start

**Your first command:**
```
"Generate test cases for client-site-v2.1/01-login collection"
```

Or:
```
"Implement TC25057 (Valid Login API)"
```

And we're off! 🚀
