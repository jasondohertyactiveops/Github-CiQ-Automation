# TC25074: User should be able to create workgroups (API)

**Azure Test Case:** 25074
**Suite:** AdminSystemSettings-25155
**Thunderclient:** ❌ Not yet implemented
**Test Users:** 9200-9299 range (API test users with admin roles)

---

## What This Tests

Departments and Groups (workgroups) can be created, read, updated, and deleted via API with proper database persistence, correct HTTP responses, and valid data structure.

---

## Test Checklist

### Create Department/Group
#### Request
- Method: POST
- Endpoint: `/api/admin/departmentsandgroups` (or similar)
- Body: 
  ```json
  {
    "name": "Test Department",
    "shortName": "TD",
    "description": "Test department for automation",
    "type": "Department", // or "Group"
    "firstDayOfWeek": "Monday"
  }
  ```

#### Response Checks
- [ ] Status code is 201 Created (or 200 OK)
- [ ] Response contains created entity with ID
- [ ] Name matches request
- [ ] Type matches request
- [ ] FirstDayOfWeek matches request
- [ ] Timestamps (CreatedDate, etc.) are present and recent

#### Database Checks
- [ ] **WorkGroup (or DepartmentGroup) table:**
  - Record exists with returned ID
  - Name matches request
  - ShortName matches request
  - Description matches request
  - Type matches request ('Department' or 'Group')
  - FirstDayOfWeek matches request
  - CreatedDate is recent
  - IsActive/IsDeleted flags are correct

---

### Get All Departments/Groups
#### Request
- Method: GET
- Endpoint: `/api/admin/departmentsandgroups`

#### Response Checks
- [ ] Status code is 200 OK
- [ ] Response is array of entities
- [ ] Created entity is in the list
- [ ] Each entity has required fields (Id, Name, Type, etc.)

---

### Get Single Department/Group
#### Request
- Method: GET
- Endpoint: `/api/admin/departmentsandgroups/{id}`

#### Response Checks
- [ ] Status code is 200 OK
- [ ] Response contains entity with correct ID
- [ ] All fields are populated correctly

---

### Update Department/Group
#### Request
- Method: PUT
- Endpoint: `/api/admin/departmentsandgroups/{id}`
- Body:
  ```json
  {
    "id": "{entityId}",
    "name": "Updated Name",
    "shortName": "UN",
    "description": "Updated description",
    "type": "Department",
    "firstDayOfWeek": "Monday"
  }
  ```

#### Response Checks
- [ ] Status code is 200 OK (or 204 No Content)
- [ ] Response contains updated entity (if 200)
- [ ] Name reflects update
- [ ] Description reflects update

#### Database Checks
- [ ] **WorkGroup table:**
  - Record exists with same ID
  - Name updated to new value
  - Description updated to new value
  - ModifiedDate is recent (later than CreatedDate)
  - Other fields unchanged

---

### Delete Department/Group
#### Request
- Method: DELETE
- Endpoint: `/api/admin/departmentsandgroups/{id}`

#### Response Checks
- [ ] Status code is 200 OK or 204 No Content

#### Database Checks
- [ ] **WorkGroup table:**
  - Record soft-deleted: `IsDeleted = 1` OR `DeletedDate IS NOT NULL`
  - OR hard-deleted: Record does not exist
- [ ] GET endpoint returns 404 for deleted entity

---

### Edge Cases
- [ ] Create with duplicate name (should fail or warn)
- [ ] Update non-existent entity (404)
- [ ] Delete non-existent entity (404 or 200)
- [ ] Create with invalid Type value (validation error)
- [ ] Create with missing required fields (validation error)

---

## Data Requirements

### Pre-Seeded Entities
**No pre-seeding required** - API tests create their own entities with unique names.

### Test Data Strategy
- **Entity Name:** `$"API Test Dept {Guid.NewGuid()}"`
- **Why GUID:** Uniqueness, no conflicts with other tests
- **Self-Sufficient:** Each test creates, uses, and optionally deletes its own data
- **Category:** Repeatable

---

## Notes

### Database Schema Assumptions
The actual table name and schema need to be confirmed. Possible names:
- `WorkGroup`
- `DepartmentGroup`
- `Workgroups`
- Separate `Department` and `Group` tables

**TODO:** Verify actual table name and schema when implementing.

### Soft vs Hard Delete
Need to confirm whether deletion is:
- **Soft delete:** Sets `IsDeleted=1` or `DeletedDate`
- **Hard delete:** Physically removes record

This affects the database verification queries.

### Type Values
Confirmed type values from UI:
- "Department"
- "Group"

Need to verify API accepts these exact string values or uses enum/ID.

### Complements UI Test
This API test verifies backend behavior and database persistence. The UI test (01-TC25074-CreateWorkgroups.md) verifies user-visible UI workflows. Together they provide complete coverage.
