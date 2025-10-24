# WindowManagerCL

.NET библиотека для программного управления окнами Windows.

## Что это

WindowManagerCL позволяет программно находить и управлять окнами Windows Desktop через Win32 API:
- Поиск окон по заголовку, классу, Process ID, regex
- Управление состоянием (активировать, свернуть, развернуть, закрыть)
- Изменение позиции и размера
- Навигация по иерархии окон (родители/дети)

## Подключение

```bash
dotnet add reference путь/к/WindowManagerCL/WindowManagerCL.csproj
```

```csharp
using WindowManagerCL.API;
```

## Быстрый старт

```csharp
// Найти окно
var notepad = Window.FindByTitle("Notepad", exact: false);

// Управление
notepad.Activate();      // активировать
notepad.Maximize();      // развернуть
notepad.MoveTo(100, 100); // переместить
notepad.Resize(800, 600); // изменить размер
notepad.Close();         // закрыть

// Свойства
string title = notepad.Title;
WindowBounds bounds = notepad.Bounds;
WindowState state = notepad.State;
bool exists = notepad.IsValid;
```

## Основные возможности

### Поиск окон

```csharp
// По заголовку
var window = Window.FindByTitle("My App");
Window.TryFindByTitle("Calculator", exact: false, out var calc);

// По regex
var chromeWindows = Window.FindByTitleRegex(@".*Chrome$");

// По классу/PID
var notepadWindows = Window.FindByClassName("Notepad");
var processWindows = Window.FindByProcessId(1234);

// Все окна / активное окно
var allWindows = Window.FindAll();
var activeWindow = Window.GetForegroundWindow();
```

### Управление окном

```csharp
window.Activate();   // вывести на передний план
window.Minimize();   // свернуть
window.Maximize();   // развернуть
window.Restore();    // восстановить
window.Close();      // закрыть

window.MoveTo(x, y);        // переместить
window.Resize(w, h);        // изменить размер
window.SetBounds(bounds);   // установить позицию и размер
```

### Иерархия

```csharp
var parent = window.Parent;
var children = window.GetChildren();
var button = window.FindChild("Button", "OK");
```

### Обработка ошибок

```csharp
try
{
    var window = Window.FindByTitle("My App");
    window.Maximize();
}
catch (WindowNotFoundException ex)
{
    Console.WriteLine($"Окно не найдено: {ex.SearchCriteria}");
}
catch (InvalidWindowHandleException ex)
{
    Console.WriteLine($"Окно закрыто: {ex.Reason}");
}
catch (WindowOperationException ex)
{
    Console.WriteLine($"Операция не удалась: {ex.Operation}");
}
```

## API

### Класс Window (статический)

- `FindAll()` - все окна
- `FindByTitle(title, exact)` - по заголовку
- `TryFindByTitle(title, exact, out window)` - безопасный поиск
- `FindByTitleRegex(pattern)` - по regex
- `FindByClassName(className)` - по классу
- `FindByProcessId(processId)` - по PID
- `FromHandle(handle)` - из HWND
- `GetForegroundWindow()` - активное окно

### Класс WindowControl

**Свойства:**
- `Handle` - HWND
- `Title` - заголовок
- `ClassName` - класс окна
- `ProcessId` - PID
- `IsVisible` - видимость
- `IsValid` - существование
- `Bounds` - позиция и размер
- `State` - состояние (Normal/Minimized/Maximized)
- `Parent` - родительское окно

**Методы:**
- `Activate()` - активировать
- `Minimize()` - свернуть
- `Maximize()` - развернуть
- `Restore()` - восстановить
- `Close()` - закрыть
- `MoveTo(x, y)` - переместить
- `Resize(width, height)` - изменить размер
- `SetBounds(bounds)` - установить границы
- `GetChildren()` - получить дочерние окна
- `FindChild(className, text)` - найти дочернее окно

### Типы

**WindowBounds** - структура позиции/размера:
- `WindowBounds(x, y, width, height)`
- Свойства: `X`, `Y`, `Width`, `Height`, `Right`, `Bottom`

**WindowState** - enum состояния:
- `Normal`, `Minimized`, `Maximized`

### Исключения

- `WindowNotFoundException` - окно не найдено
- `InvalidWindowHandleException` - невалидный handle
- `WindowOperationException` - операция не удалась
- `WindowManagerException` - базовое исключение

## Требования

- .NET 6.0+
- Windows 10 1809+
- Без сторонних зависимостей

## Документация

- **[Docs/README.md](WindowManagerCL/Docs/README.md)** - краткое руководство
- **[Docs/API_REFERENCE.md](WindowManagerCL/Docs/API_REFERENCE.md)** - полный справочник API

## Архитектура

```
WindowManagerCL/
├── API/               # Публичный API
│   ├── Window.cs      # Статические методы поиска
│   ├── WindowControl.cs  # Управление окном
│   ├── WindowBounds.cs   # Структура границ
│   ├── WindowState.cs    # Enum состояний
│   └── Exceptions.cs     # Исключения
├── Core/              # Внутренняя логика
│   └── WindowFinder.cs   # Поиск и фильтрация
└── Infrastructure/    # Win32 API
    ├── WinApi.cs         # P/Invoke
    └── WindowEnumerator.cs  # Перечисление
```

## Производительность

- Поиск окон: < 100ms для ~100 окон
- Операции: < 50ms
- Нулевые аллокации в hot paths
- Regex кэшируются

## Лицензия

MIT License - see [LICENSE](LICENSE)
