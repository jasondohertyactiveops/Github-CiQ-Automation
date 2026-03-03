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
