# TC25075: User is able to move workgroups (API)

**Azure Test Case:** 25075
**Suite:** AdminSystemSettings-25155
**Thunderclient:** ❌ Not yet implemented
**Test Users:** 9200-9299 range (API test users with admin roles)

---

## What This Tests

Workgroups (Teams, Departments, Groups) can be assigned and unassigned from organizational hierarchies via API, with correct database persistence of parent-child relationships and handling of complex scenarios.

---

## Test Checklist

### View Organizational Structure
#### Request
- Method: GET
- Endpoint: `/api/admin/workgroups/hierarchy` (or similar)

#### Response Checks
- [ ] Status code is 200 OK
- [ ] Response contains hierarchical structure
- [ ] Parent-child relationships are correct
- [ ] All entity types represented (Department, Group, Team)

---

### Assign Workgroup to Parent
#### Request
- Method: POST
- Endpoint: `/api/admin/workgroups/assign` (or similar)
- Body:
  ```json
  {
    "workgroupId": "guid-or-id",
    "parentId": "guid-or-id",
    "assignmentType": "Department" // or "Group"
  }
  ```

#### Response Checks
- [ ] Status code is 200 OK or 201 Created
- [ ] Response confirms assignment

#### Database Checks
- [ ] **WorkgroupHierarchy (or similar) table:**
  - Record created linking workgroup to parent
  - ParentId matches request
  - ChildId matches request
  - EffectiveDate is set
  - IsActive = 1

---

### Unassign Workgroup from Parent
#### Request
- Method: DELETE or POST
- Endpoint: `/api/admin/workgroups/unassign/{workgroupId}` (or similar)

#### Response Checks
- [ ] Status code is 200 OK or 204 No Content

#### Database Checks
- [ ] **WorkgroupHierarchy table:**
  - Assignment record soft-deleted: IsActive = 0 OR EndDate set
  - OR hard-deleted: Record removed

---

### Complex Assignment (Team with Department Parent → Group)
#### Request
- Method: POST
- Endpoint: `/api/admin/workgroups/assign`
- Body: Assign Team (that has Department parent) directly to Group

#### Response Checks
- [ ] Status code is 200 OK
- [ ] Assignment succeeds

#### Database Checks
- [ ] **WorkgroupHierarchy table:**
  - New assignment: Team → Group exists
  - Original assignment: Team → Department still exists (or modified)
  - Parent Department → Original Parent unchanged

---

### Cancel Pending Assignment
#### Request
- Method: DELETE or POST
- Endpoint: `/api/admin/workgroups/assignments/pending/{id}/cancel`

#### Response Checks
- [ ] Status code is 200 OK

#### Database Checks
- [ ] **PendingWorkgroupAssignments (or similar) table:**
  - Pending record removed or marked as canceled
  - No changes applied to actual WorkgroupHierarchy

---

### Edge Cases
- [ ] Assign to non-existent parent (404 or validation error)
- [ ] Create circular reference (Parent A → Child B → Parent A)
- [ ] Unassign non-existent assignment (404 or idempotent 200)
- [ ] Assign already assigned workgroup (conflict or idempotent)

---

## Data Requirements

### Pre-Seeded Organizational Structure

**Departments:**
- Department A (Id: 9001, Parent: Organization)
- Department B (Id: 9002, Parent: Organization)

**Groups:**
- Group X (Id: 9003, Parent: Organization)

**Teams:**
- Team A1 (Id: 9011, Parent: Department A / 9001)
- Team A2 (Id: 9012, Parent: Department A / 9001)
- Team B1 (Id: 9021, Parent: Department B / 9002)

**Purpose:** Enables testing various assignment/unassignment scenarios and complex parent-child relationships.

### Test Data Strategy
- **Pre-seeded:** Organizational structure with known relationships
- **Self-Sufficient:** Tests modify assignments using pre-seeded entities
- **Category:** Repeatable (can reset assignments between test runs)

---

## Notes

### Database Schema Assumptions
The actual table names and structure need confirmation. Possible names:
- `WorkgroupHierarchy`
- `OrganizationalStructure`
- `WorkgroupAssignments`
- Separate parent-child relationship tables per entity type

**TODO:** Verify actual schema when implementing.

### Assignment Timing
Assignments may be:
- **Immediate:** Applied instantly
- **Pending:** Require approval or scheduled activation

Need to confirm workflow and database representation.

### Soft vs Hard Delete
Unassignments likely use soft delete (EndDate, IsActive=0) to preserve history, but this needs verification.

### Complements UI Test
This API test verifies backend operations and database state for organizational hierarchy management. The UI test (02-TC25075-MoveWorkgroups.md) verifies user-visible workflows and assignment interfaces.
