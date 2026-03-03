# LLM Instructions

## Quick Start

**Essential reading:**
1. **WORKFLOW-QUICK.md** - Test generation workflow (Azure DevOps → test code)
2. **DATA-RULES.md** - Test data self-sufficiency rules
3. **MCP-TOOLS.md** - Azure DevOps & Playwright MCP commands

---

## Environment
- Windows machine
- All paths relative to repo root

## Local Test Environment
**Docker URLs:**
- Client: http://ww7client.localhost
- API: http://localhost:8080

**Working Login Credentials:**
- Username: `TestUser1`
- Password: `Workware@1`
- (Has Admin permissions for all Admin/System Settings pages)

## Your Capabilities
**DO:**
- Use `Filesystem:*` tools for all file operations (read, write, edit, create, list, search)
- Generate and edit code files
- Suggest concise commit messages at logical points (message text only, no commands)

**DON'T:**
- Use `bash_tool` for file operations on D:\ paths (causes sync issues)
- Delete files (no delete capability available)
- Run any commands (dotnet, git, playwright, npm, etc.)
- Execute tests or builds
- Perform git operations

## User Responsibilities
- Installing packages
- Running tests/builds
- All git operations (you suggest messages)
- Executing Playwright tests

## File Operations
Always use `Filesystem:write_file`, `Filesystem:edit_file`, `Filesystem:read_text_file` for Windows paths.
Never use bash commands like `cat >`, `mkdir -p`, etc. on `/mnt/d/` paths.

## Operational Notes

### Infrastructure Gotchas (Solved)
- **Nginx SPA routing:** Direct URL navigation (e.g., `/rtm`) returns 404 without Nginx rewrite rules. Already fixed in container config.
- **FakeTime clock sync:** `FAKETIME=%` in Dockerfile causes container clock to freeze. Start scripts auto-fix with `+0`. If containers show wrong time, check `/tmp/faketime/timedefault.rc`.
- **RTM "Select Your Activity" dialog:** Appears on first RTM page load for some users. Tests must handle or dismiss this.

### Daily Development Cycle
1. **Morning:** `recreate-databases.ps1` → `dotnet test` (verify all passing) → start work
2. **During dev:** `dotnet test --filter "Category!=OneShot"` for quick iterations
3. **Before commit:** `recreate-databases.ps1` → `dotnet test` (full suite including OneShot)

### Two-Repo Coordination
Seeding lives in WW7, tests live in CiQ-Automation. When seeding changes:
1. Edit SQL in `WW7/ww7-api/.../Automation/`
2. Commit in WW7 repo
3. Run `recreate-databases.ps1`
4. Run tests in CiQ-Automation to verify
5. Commit in CiQ-Automation repo

Separate commits — different repos, different concerns.

### appsettings Files
- `appsettings.json` — Base/shared config (all environments)
- `appsettings.Local.json` — Local containerized Docker environment
- `appsettings.Development.json` — Dev environment (Azure hosted)
- `appsettings.Test.json` — Test environment (Azure hosted)
