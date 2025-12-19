# LLM Instructions

## Environment
- Windows machine
- All paths use `D:\` format
- Repo root: `D:\ActiveOpsGit\Github-CiQ-Automation`

## Your Capabilities
**DO:**
- Use `Filesystem:*` tools for all file operations (read, write, edit, create, list, search)
- Generate and edit code files
- Suggest concise commit messages at logical points (message text only, no commands)

**DON'T:**
- Use `bash_tool` for file operations on D:\ paths (causes sync issues)
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
