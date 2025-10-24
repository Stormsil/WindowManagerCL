# WindowManagerCL

A .NET library for programmatic window management on Windows - find, control, and navigate desktop windows with ease.

[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Windows-blue)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

## Features

- 🔍 **Find Windows** - Search by title (exact/partial/regex), class name, or process ID
- 🎛️ **Control Windows** - Activate, minimize, maximize, resize, move, and close windows
- 🌲 **Navigate Hierarchy** - Access parent and child windows, enumerate controls
- ⚡ **High Performance** - Efficient Win32 API usage with < 100ms searches
- 🛡️ **Predictable Errors** - Clear exception hierarchy with detailed error information
- 📝 **Full Documentation** - Complete XML docs and IntelliSense support

## Installation

```bash
dotnet add package WindowManagerCL
```

## Quick Start

### Find and activate a window

```csharp
using WindowManagerCL.API;

// Find window by exact title
var notepad = Window.FindByTitle("Untitled - Notepad");

// Bring to foreground
notepad.Activate();
```

### Control window state and position

```csharp
using WindowManagerCL.API;

var window = Window.FindByTitle("My Application");

// Maximize window
window.Maximize();

// Move and resize
window.MoveTo(100, 100);
window.Resize(800, 600);

// Or set both at once (more efficient)
window.SetBounds(new WindowBounds(100, 100, 1024, 768));
```

### Safe window search with Try-pattern

```csharp
using WindowManagerCL.API;

if (Window.TryFindByTitle("Calculator", exact: false, out var calc))
{
    Console.WriteLine("Calculator found!");
    calc.Maximize();
}
else
{
    Console.WriteLine("Calculator is not running");
}
```

### Find windows with regex

```csharp
using WindowManagerCL.API;

// Find all Chrome windows
var chromeWindows = Window.FindByTitleRegex(@".*Google Chrome$");

foreach (var window in chromeWindows)
{
    Console.WriteLine($"Chrome: {window.Title}");
    Console.WriteLine($"  Position: {window.Bounds}");
    Console.WriteLine($"  Process ID: {window.ProcessId}");
}
```

### Navigate window hierarchy

```csharp
using WindowManagerCL.API;

var mainWindow = Window.FindByTitle("My Application");

// Get all child windows
var children = mainWindow.GetChildren();

foreach (var child in children)
{
    Console.WriteLine($"  Child: {child.ClassName}");
}

// Find specific child by class name
var button = mainWindow.FindChild("Button", "OK");
if (button != null)
{
    Console.WriteLine($"Found OK button at {button.Bounds}");
}
```

### Handle errors gracefully

```csharp
using WindowManagerCL.API;
using WindowManagerCL.Exceptions;

try
{
    var window = Window.FindByTitle("Temporary Window");
    window.Maximize();
    window.Close();
}
catch (WindowNotFoundException ex)
{
    Console.WriteLine($"Window not found: {ex.SearchCriteria}");
}
catch (InvalidWindowHandleException ex)
{
    Console.WriteLine($"Window closed: {ex.Reason}");
}
catch (WindowOperationException ex)
{
    Console.WriteLine($"Operation '{ex.Operation}' failed");
    Console.WriteLine($"Win32 Error: {ex.Win32ErrorCode}");
}
```

## API Overview

### Static Facade (`Window`)

- `FindAll()` - Enumerate all top-level windows
- `FindByTitle(title, exact)` - Find by exact or partial title
- `TryFindByTitle(title, exact, out window)` - Non-throwing variant
- `FindByTitleRegex(pattern)` - Find using regex pattern
- `FindByClassName(className)` - Find by window class
- `FindByProcessId(processId)` - Find windows owned by process
- `FromHandle(handle)` - Wrap existing HWND

### Instance Methods (`WindowControl`)

**Properties:**
- `Handle`, `Title`, `ClassName`, `ProcessId`
- `Bounds`, `State`, `IsVisible`, `IsValid`, `Parent`

**State Management:**
- `Activate()` - Bring to foreground
- `Minimize()` - Minimize to taskbar
- `Maximize()` - Maximize window
- `Restore()` - Restore to normal state
- `Close()` - Close window

**Position/Size:**
- `MoveTo(x, y)` - Move window
- `Resize(width, height)` - Resize window
- `SetBounds(bounds)` - Set position and size

**Hierarchy:**
- `GetChildren()` - Enumerate child windows
- `FindChild(className, text)` - Find child by class/text

### Exceptions

- `WindowNotFoundException` - Window not found by search criteria
- `WindowOperationException` - Operation failed (with Win32 error code)
- `InvalidWindowHandleException` - Handle invalid or window closed
- `WindowManagerException` - Base class (catch-all)

## Architecture

WindowManagerCL follows a clean, layered architecture:

```
WindowManagerCL/
├── API/
│   ├── Window.cs              # Static facade for search
│   ├── WindowControl.cs       # Instance-based window control
│   └── WindowFinder.cs        # Internal search logic
├── Infrastructure/
│   ├── WinApi.cs              # P/Invoke declarations
│   ├── Exceptions.cs          # Exception hierarchy
│   └── WindowEnumerator.cs    # Enumeration helpers
└── Models/
    ├── WindowState.cs         # Enum: Normal, Minimized, Maximized
    └── WindowBounds.cs        # Struct: X, Y, Width, Height
```

## Requirements

- **.NET 6.0 or later** (LTS version recommended)
- **Windows 10 version 1809 or later**
- **No third-party dependencies** - uses only System libraries and Win32 APIs

## Performance

- Window searches complete in **< 100ms** for typical scenarios (< 100 windows)
- Window manipulation operations complete in **< 50ms**
- **Zero allocations** in hot paths (enumeration uses structs and spans)
- Regex patterns are **compiled and cached** for repeated use

## Design Principles

1. **Simple and Intuitive API** - Static facade for finding, instance methods for controlling
2. **Single Responsibility** - Window management only, no UI frameworks or process management
3. **Predictable Error Handling** - Clear exception hierarchy with Try-pattern methods
4. **Platform-Native Integration** - Proper Win32 API usage with encapsulated HWNDs
5. **Performance and Minimal Overhead** - Efficient operations with async variants

## Interactive Demo

Try out all library features with our interactive console application:

```bash
cd InteractiveDemo/WindowManagerCL.InteractiveDemo
dotnet run
```

**Features:**
- Search windows by title, class, regex, or process ID
- Control window state (activate, minimize, maximize, restore, close)
- Manage window position and size
- Explore window hierarchy (parent/child windows)
- View detailed window information
- Ready-to-use examples (Notepad, Chrome, Calculator)

Perfect for learning the API and testing functionality!

## Contributing

Contributions are welcome! Please read our contributing guidelines and code of conduct.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with ❤️ for the .NET automation and testing community
- Uses Win32 User32.dll APIs for reliable window management
- Inspired by the need for simple, predictable window control in .NET

## Support

For questions, issues, or feature requests, please open an issue on GitHub.
