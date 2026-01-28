# TC25076: Tasks and National Holidays Management (API)

**Azure Test Case:** 25076
**Suite:** AdminSystemSettings-25155
**Thunderclient:** ❌ Not yet implemented
**Test Users:** 9200-9299 range (API test users with admin roles)
**CRITICAL:** This test case has extremely broad scope and should be split into multiple focused test cases

---

## What This Tests

Core Tasks, Diverted Tasks, Downtime, and National Holidays can be managed via API with full CRUD operations, workgroup linking, and proper database persistence.

**RECOMMENDED SPLIT:** Create 4 separate API test files:
- `03a-TC25076-CoreTasks.md`
- `03b-TC25076-DivertedTasks.md`
- `03c-TC25076-Downtime.md`
- `03d-TC25076-NationalHolidays.md`

---

## Test Checklist

### Core Tasks

#### Create Core Task
##### Request
- Method: POST
- Endpoint: `/api/admin/coretasks`
- Body:
  ```json
  {
    "name": "Test Core Task",
    "description": "Description",
    "standardTime": 15.5,
    "taskType": "Processing" // or similar
  }
  ```

##### Response Checks
- [ ] Status code is 201 Created
- [ ] Response contains entity with ID
- [ ] Name, StandardTime match request

##### Database Checks
- [ ] **CoreTask table:**
  - Record exists with returned ID
  - All fields match request
  - CreatedDate is recent
  - IsActive = 1

#### Update Core Task
##### Request
- Method: PUT
- Endpoint: `/api/admin/coretasks/{id}`
- Body: Updated task details

##### Database Checks
- [ ] **CoreTask table:**
  - StandardTime updated correctly
  - ModifiedDate is recent

#### Delete Core Task
##### Request
- Method: DELETE
- Endpoint: `/api/admin/coretasks/{id}`

##### Database Checks
- [ ] **CoreTask table:**
  - Soft-deleted: IsDeleted = 1 OR DeletedDate set
  - OR hard-deleted: Record removed

#### Link Core Task to Workgroup
##### Request
- Method: POST
- Endpoint: `/api/admin/coretaskstoworkgroup`
- Body:
  ```json
  {
    "coreTaskId": "guid",
    "workgroupId": "guid"
  }
  ```

##### Database Checks
- [ ] **CoreTaskWorkgroup (or similar) table:**
  - Link record exists
  - CoreTaskId and WorkgroupId match
  - IsActive = 1

#### Unlink Core Task from Workgroup
##### Request
- Method: DELETE
- Endpoint: `/api/admin/coretaskstoworkgroup/{taskId}/{workgroupId}`

##### Database Checks
- [ ] **CoreTaskWorkgroup table:**
  - Link removed or soft-deleted

---

### Diverted Tasks

**Similar structure to Core Tasks:**
- POST `/api/admin/divertedtasks`
- PUT `/api/admin/divertedtasks/{id}`
- DELETE `/api/admin/divertedtasks/{id}`
- Link: POST `/api/admin/divertedtaskstoworkgroup`
- Unlink: DELETE `/api/admin/divertedtaskstoworkgroup/{taskId}/{workgroupId}`

**Database table:** `DivertedTask`, `DivertedTaskWorkgroup` (or similar)

---

### Downtime

**Similar structure with special constraints:**
- POST `/api/admin/downtime`
- PUT `/api/admin/downtime/{id}`
- DELETE `/api/admin/downtime/{id}` - **Cannot delete system types (Annual Leave, National Holidays)**
- Link: POST `/api/admin/downtimetoworkgroup`
- Unlink: DELETE `/api/admin/downtimetoworkgroup/{downtimeId}/{workgroupId}`

**Database table:** `Downtime`, `DowntimeWorkgroup` (or similar)

##### Edge Case - Protected Types
- [ ] Attempting to DELETE Annual Leave type returns error
- [ ] Attempting to DELETE National Holidays type returns error

---

### National Holidays

**Similar structure:**
- POST `/api/admin/nationalholidays`
- PUT `/api/admin/nationalholidays/{id}`
- DELETE `/api/admin/nationalholidays/{id}`
- Link: POST `/api/admin/nationalholidaystoworkgroup`
- Unlink: DELETE `/api/admin/nationalholidaystoworkgroup/{holidayId}/{workgroupId}`

**Database table:** `NationalHoliday`, `NationalHolidayWorkgroup` (or similar)

**Additional fields:** Date, IsRecurring, etc.

---

### Common Edge Cases (All Entity Types)
- [ ] Create with duplicate name (validation error or allowed)
- [ ] Update non-existent entity (404)
- [ ] Delete non-existent entity (404 or idempotent 200)
- [ ] Link to non-existent workgroup (404)
- [ ] Link same task to same workgroup twice (conflict or idempotent)

---

## Data Requirements

### Pre-Seeded Workgroups
- **Workgroup A:** For linking tests (Id: 9001)
- **Workgroup B:** For additional linking scenarios (Id: 9002)

### Test Data Strategy
**For each entity type:**
- **Entity Names:** `$"API Test {EntityType} {Guid.NewGuid()}"`
- **Self-Sufficient:** Each test creates own entities
- **Category:** Repeatable

---

## Notes

### Scope Concern - CRITICAL
This single Azure test case covers **4 distinct entity types** each with full CRUD and linking operations. This represents **25+ distinct API operations** that should be tested.

**Strong recommendation:** Split into 4 separate API test implementations during automation.

### Database Schema Assumptions
Table names and structures need confirmation:
- **Core Tasks:** `CoreTask`, `CoreTaskWorkgroup`
- **Diverted Tasks:** `DivertedTask`, `DivertedTaskWorkgroup`
- **Downtime:** `Downtime`, `DowntimeWorkgroup`
- **National Holidays:** `NationalHoliday`, `NationalHolidayWorkgroup`

**TODO:** Verify actual schema when implementing.

### Protected Downtime Types
System-protected downtime types (Annual Leave, National Holidays) cannot be deleted. This should be enforced at API level and return appropriate error response.

### Standard Time Field
Core Tasks have a "StandardTime" field (numeric, likely hours or minutes). This is a key field that should be verified in database checks.

### Task Type Changes
Azure mentions "User can change core task type and core task will appear correctly in MD and Reports" - this suggests:
- Task Type is a field that can be modified
- Type changes may affect how tasks appear in other modules
- API should validate type changes and update correctly

### Complements UI Test
This API test verifies backend operations and database persistence for all task entity types. The UI test (03-TC25076-TasksNationalHolidays.md) verifies user-visible workflows and interfaces.
