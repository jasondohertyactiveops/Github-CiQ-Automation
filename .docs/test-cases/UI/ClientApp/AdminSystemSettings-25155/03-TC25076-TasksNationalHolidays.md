# TC25076: Tasks and National Holidays Management

**Azure Test Case ID:** 25076
**Suite:** AdminSystemSettings-25155
**Category:** Pure-UI-Appropriate
**CRITICAL:** This test case has extremely broad scope and should be split into multiple focused test cases

---

## Source Analysis

### Azure Test Case
- **Title:** [Smoke Test] User is able to view, create, update, assign, un-assign and delete Tasks/National Holidays
- **Steps:**
  1. User is able to view, create, update, delete Core Tasks and action is recorded in the Audit log
  2. User is able to export Core Tasks
  3. Core Tasks View permission doesn't allow editing data
  4. User can change standard time for Core Tasks and action is recorded in the Audit log
  5. User is able to link and unlink Core Tasks to workgroup in normal and switch view and action is recorded in the Audit log
  6. User can change core task type and core task will appear correctly in MD and Reports
  7. User is able to view, create, update, delete Diverted Tasks and action is recorded in the Audit log
  8. User is able to link and unlink Diverted Tasks to workgroup and action is recorded in the Audit log
  9. User is able to view, create, update, delete Downtime (except deleting Annual Leave & National Holidays), and action is recorded in the Audit log
  10. User is able to link and unlink Downtime to workgroup and action is recorded in the Audit log
  11. User is able to view, create, update, delete National Holidays and action is recorded in the Audit log
  12. User is able to link and unlink National Holidays to workgroup and action is recorded in the Audit log
  13. Check the translated content in French
- **Expected Results:** (Not specified in Azure)
- **Issues:**
  - **MASSIVE SCOPE:** Covers 4 distinct entity types (Core Tasks, Diverted Tasks, Downtime, National Holidays)
  - Each entity has CRUD + linking + special behaviors
  - Should be 4+ separate test cases
  - Permissions testing included (step 3)
  - Reports verification (step 6) - may be out of scope for UI test
  - Audit logging (separate initiative - ignore)
  - French translations (separate initiative - ignore)

### Cypress Implementation
- **File:** ❌ Not found
- **Test Name:** N/A
- **What It Actually Tests:** N/A

### Playwright MCP Exploration
**TODO:** Need to explore each admin area:
- `/admin/coretasks` - Core Tasks page
- `/admin/coretaskstoworkgroup` - Core Tasks to Workgroup linking
- `/admin/divertedtasks` - Diverted Tasks page
- `/admin/divertedtaskstoworkgroup` - Diverted Tasks to Workgroup linking
- `/admin/downtime` - Downtime page
- `/admin/downtimetoworkgroup` - Downtime to Workgroup linking
- `/admin/nationalholidays` - National Holidays page
- `/admin/nationalholidaystoworkgroup` - National Holidays to Workgroup linking

---

## Final Test Specification

### What This Test Should Verify

**RECOMMENDED SPLIT:** This Azure test case should be divided into 4 separate automated test cases:
1. **Core Tasks Management** - CRUD + workgroup linking + standard time changes
2. **Diverted Tasks Management** - CRUD + workgroup linking
3. **Downtime Management** - CRUD + workgroup linking (excluding system-protected items)
4. **National Holidays Management** - CRUD + workgroup linking

**For Initial Implementation:** Focus on Core Tasks as representative pattern, then replicate for other entity types.

### Prerequisites
- **User:** User from 9000-9199 range (UI tests)
- **Permissions:** Admin access to all task management pages
- **Pre-seeded workgroups:** For linking operations

### Test Steps (Core Tasks as Example)

#### Core Tasks CRUD
1. Navigate to `/admin/coretasks`
   - **Expected:** Core Tasks grid displays
2. Click "Add New"
   - **Expected:** Create form appears
3. Fill form with task details (Name, Description, Standard Time, Type)
4. Submit
   - **Expected:** Success message, entity in grid
5. Click Edit on created task
6. Modify Name and Standard Time
7. Submit
   - **Expected:** Success message, changes reflected in grid
8. Use Export button
   - **Expected:** File download with task data
9. Click Delete
   - **Expected:** Confirmation dialog, then success message and removal from grid

#### Link Core Tasks to Workgroup
10. Navigate to `/admin/coretaskstoworkgroup`
    - **Expected:** Assignment interface displays
11. Select a Core Task and a Workgroup
12. Confirm assignment
    - **Expected:** Success message, link visible
13. Unlink the task from workgroup
    - **Expected:** Success message, link removed

#### Standard Time Changes
14. Modify Standard Time for a Core Task
15. Verify change persists
    - **Expected:** New standard time saved and displayed

#### View Permission Testing
16. **TODO:** Requires user with View-only permission
    - Verify Add/Edit/Delete buttons are disabled or hidden

**Repeat similar patterns for:**
- Diverted Tasks
- Downtime (note: cannot delete Annual Leave & National Holidays types)
- National Holidays

### Expected Results
- All entity types support full CRUD operations
- Workgroup linking/unlinking works for all types
- Export functionality available
- Permission restrictions enforced (View vs Edit)
- System-protected entities (Annual Leave, National Holidays) cannot be deleted

### Automation Approach
- **Pattern:** Pattern A (Workflow) repeated per entity type
- **Page Objects Needed:**
  - `CoreTasksPage`, `CoreTasksToWorkgroupPage`
  - `DivertedTasksPage`, `DivertedTasksToWorkgroupPage`
  - `DowntimePage`, `DowntimeToWorkgroupPage`
  - `NationalHolidaysPage`, `NationalHolidaysToWorkgroupPage`
- **Locators:** TODO - Requires UI exploration
- **Implementation Strategy:**
  - Create base `TaskEntityPage` class with common CRUD operations
  - Each specific page inherits and adds entity-specific logic
  - Reusable linking component for workgroup assignments

---

## Data Requirements

### Pre-Seeded Workgroups
- **Workgroup A:** For linking tests (Id: 9001)
- **Workgroup B:** For unlinking tests (Id: 9002)

### Test Data Strategy (Per Entity Type)
- **Entity Names:** `$"AutoTest {EntityType} {Guid.NewGuid()}"`
- **Why GUID:** Uniqueness, parallel execution
- **Self-Sufficient:** Tests create, use, delete own entities
- **Category:** Repeatable

### Special Considerations
**Downtime:** Cannot delete system-protected types (Annual Leave, National Holidays)
**National Holidays:** May have date-specific constraints

---

## Notes

### Scope Concern - CRITICAL
This test case covers **4 distinct entity types** each with:
- CRUD operations (4 operations × 4 entity types = 16 operations)
- Workgroup linking (2 operations × 4 entity types = 8 operations)
- Special behaviors (standard time, permissions, type changes)
- Export functionality

**Total: 25+ distinct test scenarios** bundled into a single Azure test case.

**Recommendation:** Split into 4 separate automated test cases during implementation:
- `03a-TC25076-CoreTasks.md`
- `03b-TC25076-DivertedTasks.md`
- `03c-TC25076-Downtime.md`
- `03d-TC25076-NationalHolidays.md`

### Out of Scope
- **Audit Logging:** Separate initiative
- **French Translations:** Separate initiative
- **Reports Verification:** "appears correctly in MD and Reports" - MD = Manage Data (different module), Reports may be separate test

### Pattern Reuse
All 4 entity types likely follow similar UI patterns:
- Same grid layout
- Same Add/Edit dialogs
- Same linking mechanism
This allows creating reusable page object components.

### View Permission Testing
Step 3 requires a user with View-only permissions. This may need separate test user or permission management setup.

### UI Exploration Required
Each of the 8 pages needs exploration to document:
- Exact form fields
- Validation rules
- Semantic locators
- Linking interface mechanics
