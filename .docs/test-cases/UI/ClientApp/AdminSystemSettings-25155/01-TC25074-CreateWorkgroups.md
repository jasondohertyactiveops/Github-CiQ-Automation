# TC25074: User should be able to create workgroups

**Azure Test Case ID:** 25074
**Suite:** AdminSystemSettings-25155
**Category:** Pure-UI-Appropriate

---

## Source Analysis

### Azure Test Case
- **Title:** [Smoke Test] User should be able to create workgroups
- **Steps:**
  1. User is able to search and filter all Admin Add/View screens
  2. User is able to view, create, update, delete Departments, Groups and Teams and action is recorded in the Audit log
  3. User is able to export Departments, Groups and Teams
  4. Check the translated content in French
- **Expected Results:** (Not specified in Azure)
- **Issues:** 
  - Title says "workgroups" but steps mention "Departments, Groups and Teams"
  - Very broad scope - covers CRUD + search + export
  - No specific expected results
  - Audit log checking (separate initiative - ignore for now)
  - French translations (separate initiative - ignore for now)

### Cypress Implementation
- **File:** ❌ Not found
- **Test Name:** N/A
- **What It Actually Tests:** N/A

### Playwright MCP Exploration
- **Page URL:** `http://ww7client.localhost/admin/departmentsandgroups`
- **Elements Found:**
  - Heading: "Departments and Groups"
  - Button: "Add New" (to create new entity)
  - Button: "Search"
  - Button: "Export"
  - Button: "Filter"
  - Grid with columns: Name, Short Name, Description, Type, First Day of Week
  - Edit/Delete buttons per row
- **Interactions:** 
  - Can create, edit, delete departments and groups
  - Can search/filter grid
  - Can export data
  - Type dropdown shows "Department" and "Group" options

**Clarification:** "Workgroups" in Azure test case = "Departments and Groups" in the application UI.

---

## Final Test Specification

### What This Test Should Verify

This test verifies users can perform complete CRUD operations on Departments and Groups through the UI:
- Navigate to the Departments and Groups admin page
- Create a new entity (Department or Group)
- See the entity in the grid
- Search/filter to find the entity
- Edit the entity and see updated values
- Export the grid data
- Delete the entity and verify removal

### Prerequisites
- **User:** User from 9000-9199 range (UI tests)
- **Permissions:** Admin access to /admin/departmentsandgroups
- **Test User:** TestUser1 (has admin permissions)

### Test Steps

#### Create Department/Group
1. Navigate to `/admin/departmentsandgroups`
   - **Expected:** Departments and Groups page displays with grid and toolbar
2. Click "Add New" button
   - **Expected:** Create form/dialog appears
3. Fill form:
   - **Name:** `Test Dept ${Guid}`
   - **Short Name:** Unique abbreviation
   - **Description:** Test description
   - **Type:** Select "Department" or "Group"
   - **First Day of Week:** Select day
4. Submit form
   - **Expected:** Success message/toast appears
   - **Expected:** Form closes, grid refreshes
5. Verify entity appears in grid
   - **Expected:** New row visible with entered values

#### Search/Filter
6. Use Search functionality to find created entity
   - **Expected:** Grid filters to show matching entity
7. Clear search
   - **Expected:** Grid shows all entities again

#### Edit
8. Click Edit button for created entity
   - **Expected:** Edit form/dialog appears with current values
9. Modify Name and Description
10. Submit changes
    - **Expected:** Success message/toast appears
    - **Expected:** Grid refreshes showing updated values

#### Export
11. Click "Export" button
    - **Expected:** File download triggered (CSV/Excel)
    - **Expected:** File contains grid data

#### Delete
12. Click Delete button for created entity
    - **Expected:** Confirmation dialog appears
13. Confirm deletion
    - **Expected:** Success message/toast appears
    - **Expected:** Entity removed from grid

### Expected Results
- User can successfully create departments and groups via UI
- Created entities immediately visible in grid
- Search/filter functionality works correctly
- Edit changes persist and display correctly
- Export functionality downloads data file
- Delete removes entity from grid display

### Automation Approach
- **Pattern:** Pattern A (Workflow) - Complete CRUD workflow with multiple steps
- **Page Objects Needed:** 
  - `DepartmentsAndGroupsPage`
  - `CreateDepartmentGroupDialog`
  - `EditDepartmentGroupDialog`
- **Locators:**
  - Add New button: `Page.GetByRole(AriaRole.Button, new() { Name = "Add New" })`
  - Search button: `Page.GetByRole(AriaRole.Button, new() { Name = "Search" })`
  - Export button: `Page.GetByRole(AriaRole.Button, new() { Name = "Export" })`
  - Filter button: `Page.GetByRole(AriaRole.Button, new() { Name = "Filter" })`
  - Grid: `Page.GetByRole(AriaRole.Grid, new() { Name = "Departments and Groups" })`
  - Edit buttons: `Page.GetByRole(AriaRole.Button, new() { Name = "Edit" })`
  - Delete buttons: `Page.GetByRole(AriaRole.Button, new() { Name = "Delete" })`
- **Waits/Timing:** 
  - Wait for success toast after create/edit/delete
  - Wait for grid refresh after operations
  - Wait for form dialog to appear/close

---

## Data Requirements

### User
- **User 9000-9099:** automation.admin1@activeops.com (or similar from UI test user range)
- **Permissions:** Admin access to Admin section

### Test Data Strategy
- **Entity Name:** `$"AutoTest Dept {Guid.NewGuid()}"`
- **Why GUID:** Ensures uniqueness, supports parallel execution, no cleanup required
- **Self-Sufficient:** Test creates its own entity, modifies it, then deletes it
- **Category:** Repeatable (creates and cleans up own data)

---

## Notes

### Out of Scope (Per Workflow Instructions)
- **Audit Logging:** Separate initiative - not testing audit log entries
- **French Translations:** Separate initiative - not testing translated content

### "Workgroups" Terminology
Azure test case uses "workgroups" but the UI shows "Departments and Groups". These are the same thing:
- Type = "Department" or "Group"
- Both managed on the same page
- Organizational hierarchy entities

### Test Coverage
This UI test verifies user-visible CRUD operations only. The API test will verify:
- Database persistence
- HTTP status codes
- Response structure
- Business rule enforcement
