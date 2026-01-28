# TC25078: Task Groups Management

**Azure Test Case ID:** 25078
**Suite:** AdminSystemSettings-25155
**Category:** Pure-UI-Appropriate

---

## Source Analysis

### Azure Test Case
- **Title:** [Smoke Test] User is able to view, create, update and delete Task Groups
- **Steps:**
  1. User is able to view, create, update and delete Task Groups
  2. User is able to set a Core Task as Outcome Volume for a Core Task Group
- **Expected Results:** (Not specified in Azure)
- **Issues:**
  - Reasonable scope (single entity type)
  - "Outcome Volume" concept needs exploration
  - Core Task Group vs general Task Group distinction unclear

### Cypress Implementation
- **File:** ❌ Not found
- **Test Name:** N/A
- **What It Actually Tests:** N/A

### Playwright MCP Exploration
**TODO:** Need to explore:
- `/admin/taskgroups` - Task Groups page
- How "Outcome Volume" setting works
- Difference between Core Task Groups and other Task Groups (if any)

---

## Final Test Specification

### What This Test Should Verify

This test verifies users can perform CRUD operations on Task Groups and configure Outcome Volume settings:
- Navigate to Task Groups admin page
- Create new Task Group
- View Task Group details
- Edit Task Group
- Set a Core Task as Outcome Volume for a Core Task Group
- Delete Task Group

### Prerequisites
- **User:** User from 9000-9199 range (UI tests)
- **Permissions:** Admin access to /admin/taskgroups
- **Pre-seeded Core Tasks:** For Outcome Volume assignment

### Test Steps

#### Create Task Group
1. Navigate to `/admin/taskgroups`
   - **Expected:** Task Groups grid displays
2. Click "Add New"
   - **Expected:** Create form/dialog appears
3. Fill form:
   - **Name:** `Test Task Group ${Guid}`
   - **Description:** Test description
   - **Other fields as required**
4. Submit
   - **Expected:** Success message
   - **Expected:** Entity appears in grid

#### View Task Group
5. Verify created Task Group visible in grid
   - **Expected:** Row shows all key details
6. Click to view details (if separate detail view exists)
   - **Expected:** Full Task Group details displayed

#### Edit Task Group
7. Click Edit for created Task Group
   - **Expected:** Edit form appears with current values
8. Modify Name and Description
9. Submit changes
   - **Expected:** Success message
   - **Expected:** Grid updates with new values

#### Set Outcome Volume for Core Task Group
10. Navigate to or identify a Core Task Group
11. Access Outcome Volume setting interface
12. Select a Core Task to set as Outcome Volume
13. Confirm selection
    - **Expected:** Success message
    - **Expected:** Core Task marked as Outcome Volume for the group

#### Delete Task Group
14. Click Delete for test Task Group
    - **Expected:** Confirmation dialog
15. Confirm deletion
    - **Expected:** Success message
    - **Expected:** Entity removed from grid

### Expected Results
- User can successfully create Task Groups via UI
- Task Groups visible in grid with all details
- Edit changes persist correctly
- Core Tasks can be designated as Outcome Volume for Core Task Groups
- Delete removes Task Group from system
- All operations show appropriate success/error messages

### Automation Approach
- **Pattern:** Pattern A (Workflow) - Complete CRUD workflow
- **Page Objects Needed:**
  - `TaskGroupsPage`
  - `CreateTaskGroupDialog`
  - `EditTaskGroupDialog`
- **Locators:** TODO - Requires UI exploration
  - Add New button: `Page.GetByRole(AriaRole.Button, new() { Name = "Add New" })`
  - Grid: `Page.GetByRole(AriaRole.Grid)`
  - Edit/Delete buttons per row
- **Waits/Timing:**
  - Wait for success toast after operations
  - Wait for grid refresh

---

## Data Requirements

### Pre-Seeded Core Tasks
**Core Task for Outcome Volume:**
- **Name:** "Pre-seeded Core Task A" (Id: 9100)
- **Purpose:** For testing Outcome Volume assignment

### Pre-Seeded Core Task Group
**Core Task Group:**
- **Name:** "Pre-seeded Core Task Group" (Id: 9050)
- **Purpose:** For testing Outcome Volume assignment (if group must exist first)

### Test Data Strategy
- **Task Group Name:** `$"AutoTest TaskGroup {Guid.NewGuid()}"`
- **Why GUID:** Uniqueness, parallel execution
- **Self-Sufficient:** Test creates, uses, deletes own Task Group
- **Category:** Repeatable

---

## Notes

### Outcome Volume Concept
"Outcome Volume" appears to be a specific designation for Core Tasks within Core Task Groups. This likely affects:
- Reporting and metrics
- Performance calculations
- Volume tracking

**TODO:** Requires UI exploration to understand exact mechanism and where this setting is configured.

### Task Group Types
The Azure test case mentions "Core Task Group" specifically for Outcome Volume setting. This suggests:
- Multiple types of Task Groups may exist
- Outcome Volume only applies to Core Task Groups
- Need to understand type distinctions

### UI Exploration Required
Need to explore actual Task Groups page to:
- Document exact form fields
- Understand Outcome Volume configuration interface
- Identify semantic locators
- Verify if Task Group types exist and how they're managed
