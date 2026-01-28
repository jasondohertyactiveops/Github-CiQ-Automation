# TC25075: User is able to move workgroups

**Azure Test Case ID:** 25075
**Suite:** AdminSystemSettings-25155
**Category:** Pure-UI-Appropriate

---

## Source Analysis

### Azure Test Case
- **Title:** [Smoke Test] User is able to move workgroups and action is recorded in the Audit log
- **Steps:**
  1. User is able to view all Groups at Org level on Workgroups to Main Workgroup
  2. User is able to move workgroups and action is recorded in the Audit log
  3. User is able to assign, unassign Workgroups to, from Department or Group
  4. User is able to assign a single Team that has a Parent Department (not Organisation) to a Group without assigning it's parent to the Group
  5. User is able to cancel pending Assignment, Unassignment for Org hierarchy
  6. Check the translated content in French
- **Expected Results:** (Not specified in Azure)
- **Issues:**
  - Title says "move" but steps cover viewing, assigning, unassigning, canceling
  - "Move" likely means reassigning workgroups in organizational hierarchy
  - Audit log recording (separate initiative - ignore)
  - French translations (separate initiative - ignore)
  - Complex organizational hierarchy management

### Cypress Implementation
- **File:** ❌ Not found
- **Test Name:** N/A
- **What It Actually Tests:** N/A

### Playwright MCP Exploration
**TODO:** Need to explore:
- Workgroups to Main Workgroup assignment page
- How "move" operations work in UI
- Pending assignments/cancellation workflow

---

## Final Test Specification

### What This Test Should Verify

This test verifies users can manage organizational hierarchy through workgroup assignments:
- View organizational structure and workgroup relationships
- Assign workgroups (Teams, Departments, Groups) to parent organizational units
- Unassign workgroups from organizational units
- Handle complex scenarios (Team with Department parent assigned to Group)
- Cancel pending assignment/unassignment operations

### Prerequisites
- **User:** User from 9000-9199 range (UI tests)
- **Permissions:** Admin access to organizational hierarchy management
- **Pre-seeded entities:**
  - Departments with child Teams
  - Groups
  - Various workgroup assignments

### Test Steps

#### View Organizational Structure
1. Navigate to workgroups/organizational hierarchy management page
   - **Expected:** Tree or list view shows org structure
2. View Groups at Organization level
   - **Expected:** All top-level groups visible

#### Assign Workgroup
3. Select a workgroup (Team/Department) to assign
4. Choose target parent (Department or Group)
5. Confirm assignment
   - **Expected:** Success message appears
   - **Expected:** Workgroup appears under new parent in structure

#### Unassign Workgroup
6. Select an assigned workgroup
7. Choose to unassign/remove from parent
8. Confirm unassignment
   - **Expected:** Success message appears
   - **Expected:** Workgroup removed from parent in structure

#### Complex Assignment (Team with Department Parent)
9. Find a Team that has a Department as parent (not Organization)
10. Assign this Team to a Group WITHOUT assigning the parent Department
11. Confirm assignment
    - **Expected:** Assignment succeeds
    - **Expected:** Team appears under Group
    - **Expected:** Parent Department remains in original location

#### Cancel Pending Operations
12. Initiate an assignment operation
13. Cancel before confirming
    - **Expected:** Operation canceled
    - **Expected:** No changes to org structure
14. If pending assignments exist, cancel them
    - **Expected:** Pending operations removed/cleared

### Expected Results
- User can view complete organizational hierarchy
- Workgroups can be assigned to different organizational units
- Workgroups can be unassigned/removed
- Complex parent-child scenarios work correctly
- Pending operations can be canceled
- UI updates reflect changes immediately

### Automation Approach
- **Pattern:** Pattern A (Workflow) - Multi-step organizational management
- **Page Objects Needed:**
  - `WorkgroupsHierarchyPage` or `OrganizationalStructurePage`
  - Assignment/unassignment dialogs
- **Locators:** TODO - Requires UI exploration
- **Waits/Timing:**
  - Wait for org structure tree to load
  - Wait for assignment confirmations
  - Wait for UI refresh after operations

---

## Data Requirements

### Pre-Seeded Organizational Structure

**Department with Teams:**
- **Department:** "Test Department A" (Id: 9001)
- **Child Team 1:** "Test Team A1" (Id: 9011, Parent: 9001)
- **Child Team 2:** "Test Team A2" (Id: 9012, Parent: 9001)

**Group:**
- **Group:** "Test Group X" (Id: 9002)

**Purpose:** Enables testing assignment, unassignment, and complex parent-child scenarios.

### Test Data Strategy
- **Self-Sufficient:** Uses pre-seeded organizational entities
- **Category:** Repeatable (tests modify assignments but entities persist)

---

## Notes

### Terminology Clarification
- **"Move workgroups"** = Reassigning organizational units in hierarchy
- **"Workgroups"** = Collective term for Departments, Groups, and Teams

### Out of Scope
- **Audit Logging:** Separate initiative
- **French Translations:** Separate initiative

### UI Exploration Required
This test case requires exploring the actual workgroups/organizational hierarchy management UI to:
- Understand exact page structure and navigation
- Identify semantic locators for assignment operations
- Understand pending operations workflow
- Document actual "move" mechanism

### Complexity Note
This test involves complex organizational hierarchy management. May need to split into multiple focused test cases during implementation if workflows are significantly different.
