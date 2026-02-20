# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**WindowManagerCL** is a .NET library for programmatic control of Windows desktop windows through Win32 P/Invoke APIs. It provides window discovery, state management, positioning, and hierarchy navigation.

- **Language:** C# 10.0
- **Framework:** .NET 8.0-windows
- **Dependencies:** None (pure .NET + Win32 interop)
- **Documentation:** Russian language (README.md, Docs/)

## Build Commands

```bash
# Build the library
dotnet build

# Clean build artifacts
dotnet clean

# Build for Release
dotnet build -c Release

# Create NuGet package
dotnet pack

# Build with MSBuild
msbuild WindowManagerCL.sln
```

**Note:** No test project exists in this solution. Tests would need to be added separately.

## Code Architecture

### Layered Architecture (API → Core → Infrastructure)

The codebase follows a strict 3-layer architecture:

```
WindowManagerCL/
├── API/                    # Public API surface
│   ├── Window.cs           # Static facade for window discovery
│   ├── WindowControl.cs    # Instance wrapper for window operations
│   ├── WindowBounds.cs     # Immutable value object for position/size
│   ├── WindowState.cs      # Enum for window states
│   └── Exceptions.cs       # Custom exception hierarchy
├── Core/                   # Business logic layer
│   └── WindowFinder.cs     # Window filtering and search logic
└── Infrastructure/         # Low-level Win32 interop
    ├── WinApi.cs           # P/Invoke declarations (user32.dll, kernel32.dll)
    └── WindowEnumerator.cs # Window enumeration helpers
```

**Layer Rules:**
- API layer depends on Core
- Core layer depends on Infrastructure
- Infrastructure has no internal dependencies (only Win32)
- Never bypass layers (e.g., API should not call Infrastructure directly)

### Key Design Patterns

**1. Static Facade Pattern (`Window` class)**
- Single static entry point for window discovery
- Methods: `FindAll()`, `FindByTitle()`, `FindByTitleRegex()`, `FindByClassName()`, `FindByProcessId()`, `GetForegroundWindow()`
- Delegates to `WindowFinder` in Core layer

**2. Instance Wrapper Pattern (`WindowControl` class)**
- Wraps native HWND with object-oriented interface
- Properties query Win32 on every access (no caching)
- Provides methods for state management, positioning, hierarchy navigation

**3. Immutable Value Objects**
- `WindowBounds`: readonly struct with validation
- No setters, construct with new values

**4. Exception Translation**
- Win32 errors are caught and wrapped in custom exceptions
- Hierarchy: `WindowManagerException` (base) → `WindowNotFoundException`, `InvalidWindowHandleException`, `WindowOperationException`
- Always capture Win32 error codes and messages

### Critical Implementation Details

**No Caching Philosophy**
- `WindowControl` properties (Title, Bounds, State, etc.) always query Win32 APIs
- Rationale: Window state changes externally; fresh data prevents stale values
- Exception: Regex patterns are cached in `WindowFinder._regexCache` for performance

**P/Invoke Patterns**
- All Win32 declarations in `Infrastructure/WinApi.cs`
- Use `[DllImport("user32.dll", SetLastError = true)]` and check errors with `Marshal.GetLastWin32Error()`
- Callbacks use `GCHandle` to prevent garbage collection (see `WindowEnumerator`)
- Structures use `[StructLayout(LayoutKind.Sequential)]`

**Error Handling**
- Window enumeration gracefully skips invalid handles (try/catch in `WindowFinder.FindAll()`)
- Operations throw custom exceptions with context (handle, operation, search criteria)
- Never expose Win32 error codes directly to API consumers

**Performance Considerations**
- Window enumeration: < 100ms for ~100 windows
- Regex patterns cached in `ConcurrentDictionary`
- No allocations in hot paths where possible
- Use `yield return` for lazy enumeration

## Development Guidelines

### Code Style (.editorconfig enforced)

- **Indentation:** 4 spaces for C#, 2 spaces for XML/JSON
- **Braces:** Always on new lines (`csharp_new_line_before_open_brace = all`)
- **Naming:** Interfaces must start with 'I' (warning enforced)
- **Usings:** `System.*` directives first, no grouping
- **Line endings:** CRLF (Windows standard)
- **Charset:** UTF-8

### Nullable Reference Types

- **Enabled:** All code uses nullable reference types (`<Nullable>enable</Nullable>`)
- **Strict Enforcement:** CS8600-8604 treated as errors (not warnings)
- Always handle null cases explicitly
- Use `?` for nullable parameters and return types
- Use null-forgiving operator `!` only when absolutely certain

### XML Documentation

- **Required:** `GenerateDocumentationFile` is enabled
- Document all public APIs with `<summary>`, `<param>`, `<returns>`, `<exception>`
- Include `<remarks>` for non-obvious behavior (e.g., performance notes, Win32 quirks)
- Internal classes can have lighter documentation

### Dependencies Policy

**Zero External Dependencies**
- This library uses only built-in .NET and Win32 APIs
- Do not add NuGet packages unless absolutely necessary and discussed
- Rationale: Simplicity, reliability, minimal footprint

### Documentation Language

- **Russian:** Primary documentation is in Russian (README.md, Docs/)
- Code comments and XML docs are in English
- When updating docs, maintain Russian language

## Common Pitfalls

1. **Don't cache window properties** - They must query Win32 each time for fresh data
2. **Don't call Infrastructure from API** - Always go through Core layer
3. **Don't expose Win32 types in API** - Use wrapper types (e.g., `WindowBounds` instead of `RECT`)
4. **Don't forget error translation** - Wrap Win32 errors in custom exceptions
5. **Don't skip XML docs** - Build will generate warnings for missing documentation
6. **Don't violate nullable contracts** - Compiler errors will block build

## Project Structure Context

- **Solution file:** `WindowManagerCL.sln` (at repository root)
- **Project file:** `WindowManagerCL/WindowManagerCL.csproj`
- **Source code:** `WindowManagerCL/` subdirectory
- **Documentation:** `Docs/README.md` and `Docs/API_REFERENCE.md` (Russian)
- **Main branch:** `001-window-management-library`

## Platform Requirements

- **OS:** Windows 10 1809+ (Win32 API level)
- **Framework:** .NET 6.0+ (targeting .NET 8.0-windows)
- **Architecture:** Any CPU (x86/x64/ARM64 via P/Invoke)
