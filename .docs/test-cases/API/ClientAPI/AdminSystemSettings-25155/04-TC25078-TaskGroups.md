# TC25078: Task Groups Management (API)

**Azure Test Case:** 25078
**Suite:** AdminSystemSettings-25155
**Thunderclient:** ❌ Not yet implemented
**Test Users:** 9200-9299 range (API test users with admin roles)

---

## What This Tests

Task Groups can be created, read, updated, and deleted via API with proper database persistence. Core Tasks can be designated as Outcome Volume for Core Task Groups with correct relationship storage.

---

## Test Checklist

### Create Task Group
#### Request
- Method: POST
- Endpoint: `/api/admin/taskgroups`
- Body:
  ```json
  {
    "name": "Test Task Group",
    "description": "Test description",
    "taskGroupType": "Core" // if types exist
  }
  ```

#### Response Checks
- [ ] Status code is 201 Created
- [ ] Response contains entity with ID
- [ ] Name matches request
- [ ] Type matches request (if applicable)

#### Database Checks
- [ ] **TaskGroup table:**
  - Record exists with returned ID
  - Name matches request
  - Description matches request
  - TaskGroupType correct (if applicable)
  - CreatedDate is recent
  - IsActive = 1

---

### Get All Task Groups
#### Request
- Method: GET
- Endpoint: `/api/admin/taskgroups`

#### Response Checks
- [ ] Status code is 200 OK
- [ ] Response is array of Task Groups
- [ ] Created entity is in the list

---

### Get Single Task Group
#### Request
- Method: GET
- Endpoint: `/api/admin/taskgroups/{id}`

#### Response Checks
- [ ] Status code is 200 OK
- [ ] Response contains entity with correct ID
- [ ] All fields populated correctly

---

### Update Task Group
#### Request
- Method: PUT
- Endpoint: `/api/admin/taskgroups/{id}`
- Body:
  ```json
  {
    "id": "{entityId}",
    "name": "Updated Task Group Name",
    "description": "Updated description"
  }
  ```

#### Response Checks
- [ ] Status code is 200 OK
- [ ] Name updated correctly

#### Database Checks
- [ ] **TaskGroup table:**
  - Name updated to new value
  - Description updated
  - ModifiedDate is recent

---

### Set Outcome Volume for Core Task Group
#### Request
- Method: POST or PUT
- Endpoint: `/api/admin/taskgroups/{groupId}/outcomevolume` (or similar)
- Body:
  ```json
  {
    "coreTaskId": "guid"
  }
  ```

#### Response Checks
- [ ] Status code is 200 OK
- [ ] Response confirms Outcome Volume assignment

#### Database Checks
- [ ] **TaskGroup table OR TaskGroupOutcomeVolume table:**
  - OutcomeVolumeTaskId = specified Core Task ID
  - OR relationship record exists in join table
  - Link is active

---

### Delete Task Group
#### Request
- Method: DELETE
- Endpoint: `/api/admin/taskgroups/{id}`

#### Response Checks
- [ ] Status code is 200 OK or 204 No Content

#### Database Checks
- [ ] **TaskGroup table:**
  - Soft-deleted: IsDeleted = 1 OR DeletedDate set
  - OR hard-deleted: Record removed

---

### Edge Cases
- [ ] Create with duplicate name (validation error or allowed)
- [ ] Update non-existent Task Group (404)
- [ ] Delete non-existent Task Group (404 or idempotent 200)
- [ ] Set Outcome Volume with non-existent Core Task (404)
- [ ] Set Outcome Volume for non-Core Task Group (validation error if types enforced)

---

## Data Requirements

### Pre-Seeded Core Task
**Core Task for Outcome Volume:**
- **Id:** 9100
- **Name:** "Pre-seeded Core Task A"
- **Purpose:** For testing Outcome Volume assignment

### Test Data Strategy
- **Task Group Name:** `$"API Test TaskGroup {Guid.NewGuid()}"`
- **Self-Sufficient:** Tests create own Task Groups
- **Category:** Repeatable

---

## Notes

### Database Schema Assumptions
Table names and structure need confirmation:
- **Task Group:** `TaskGroup` table
- **Outcome Volume:** Either field in TaskGroup table or separate relationship table

**TODO:** Verify actual schema when implementing.

### Task Group Types
If TaskGroupType field exists (Core, Diverted, etc.):
- Verify type is stored correctly
- Verify Outcome Volume only applies to Core type
- Test validation when setting Outcome Volume on non-Core groups

### Outcome Volume Relationship
The "Outcome Volume" designation could be:
- **Simple field:** TaskGroup.OutcomeVolumeTaskId
- **Separate table:** TaskGroupOutcomeVolume with relationship records
- **Configuration table:** More complex relationship structure

This affects how we query and verify the relationship.

### Complements UI Test
This API test verifies backend operations and database persistence for Task Groups. The UI test (04-TC25078-TaskGroups.md) verifies user-visible workflows and Outcome Volume configuration interface.
