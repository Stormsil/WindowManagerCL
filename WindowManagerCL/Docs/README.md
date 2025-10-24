# WindowManagerCL

.NET библиотека для программного управления окнами Windows.

## Что это

WindowManagerCL - это библиотека для поиска, управления и навигации по окнам Windows Desktop. Позволяет программно находить окна по различным критериям и управлять их состоянием, позицией и размером через Win32 API.

## Подключение к проекту

```bash
# Добавить ссылку на проект
dotnet add reference путь/к/WindowManagerCL/WindowManagerCL.csproj
```

Или в `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="путь\к\WindowManagerCL\WindowManagerCL.csproj" />
</ItemGroup>
```

## Using директива

```csharp
using WindowManagerCL.API;
```

## Основное использование

### Поиск окна

```csharp
// По точному заголовку
var notepad = Window.FindByTitle("Untitled - Notepad");

// По частичному заголовку
var notepad = Window.FindByTitle("Notepad", exact: false);

// Безопасный поиск (не бросает исключение)
if (Window.TryFindByTitle("Calculator", exact: false, out var calc))
{
    // окно найдено
}

// По регулярному выражению
var chromeWindows = Window.FindByTitleRegex(@".*Chrome$");

// По классу окна
var notepadWindows = Window.FindByClassName("Notepad");

// По Process ID
var windows = Window.FindByProcessId(1234);

// Все окна
var allWindows = Window.FindAll();

// Текущее активное окно
var activeWindow = Window.GetForegroundWindow();
```

### Управление окном

```csharp
var window = Window.FindByTitle("My App");

// Состояние
window.Activate();      // активировать (на передний план)
window.Minimize();      // свернуть
window.Maximize();      // развернуть
window.Restore();       // восстановить
window.Close();         // закрыть

// Позиция и размер
window.MoveTo(100, 100);           // переместить
window.Resize(800, 600);           // изменить размер
window.SetBounds(new WindowBounds(100, 100, 800, 600)); // установить всё сразу

// Иерархия
var parent = window.Parent;        // родительское окно
var children = window.GetChildren(); // дочерние окна
var button = window.FindChild("Button", "OK"); // найти дочернее окно
```

### Свойства окна

```csharp
var window = Window.FindByTitle("My App");

IntPtr handle = window.Handle;      // HWND
string title = window.Title;        // заголовок
string className = window.ClassName; // класс
uint processId = window.ProcessId;   // ID процесса
bool isVisible = window.IsVisible;   // видимо ли
bool isValid = window.IsValid;       // существует ли
WindowBounds bounds = window.Bounds; // позиция и размер
WindowState state = window.State;    // состояние (Normal/Minimized/Maximized)
WindowControl parent = window.Parent; // родитель
```

## Обработка ошибок

```csharp
try
{
    var window = Window.FindByTitle("My App");
    window.Maximize();
}
catch (WindowNotFoundException ex)
{
    // окно не найдено
    Console.WriteLine($"Не найдено: {ex.SearchCriteria}");
}
catch (InvalidWindowHandleException ex)
{
    // окно закрыто или handle невалиден
    Console.WriteLine($"Невалидное окно: {ex.Reason}");
}
catch (WindowOperationException ex)
{
    // операция не удалась
    Console.WriteLine($"Операция {ex.Operation} не удалась");
    Console.WriteLine($"Win32 код: {ex.Win32ErrorCode}");
}
catch (WindowManagerException ex)
{
    // базовое исключение (ловит всё)
    Console.WriteLine($"Ошибка: {ex.Message}");
}
```

## Требования

- .NET 6.0+
- Windows 10 1809+
- Без сторонних зависимостей

## Полная документация

См. [API_REFERENCE.md](API_REFERENCE.md) для полного описания всех методов и свойств.
