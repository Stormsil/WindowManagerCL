# WindowManagerCL API Reference

Полный справочник по публичному API библиотеки WindowManagerCL.

## Namespace

```csharp
using WindowManagerCL.API;
```

Все публичные типы находятся в namespace `WindowManagerCL.API`.

---

## Класс `Window` (статический)

Главная точка входа для поиска окон. Все методы статические.

### Методы поиска

#### `FindAll()`

```csharp
public static IEnumerable<WindowControl> FindAll()
```

Возвращает все окна верхнего уровня (top-level windows).

**Возвращает:** Коллекция всех окон (включая невидимые).

**Примечания:**
- Использует Win32 `EnumWindows`
- Порядок окон не определен
- Для фильтрации видимых: `FindAll().Where(w => w.IsVisible)`

**Пример:**
```csharp
var allWindows = Window.FindAll();
var visibleWindows = Window.FindAll().Where(w => w.IsVisible);
```

---

#### `FindByTitle(string title, bool exact = true)`

```csharp
public static WindowControl FindByTitle(string title, bool exact = true)
```

Ищет окно по заголовку.

**Параметры:**
- `title` - заголовок окна для поиска
- `exact` - `true` для точного совпадения, `false` для частичного (регистр не учитывается)

**Возвращает:** Первое найденное окно.

**Исключения:**
- `WindowNotFoundException` - окно не найдено
- `ArgumentNullException` - `title` == null

**Примечания:**
- Поиск case-insensitive
- Если найдено несколько окон, возвращает первое
- Пустая строка ищет окна без заголовка

**Пример:**
```csharp
var notepad = Window.FindByTitle("Untitled - Notepad");
var anyNotepad = Window.FindByTitle("Notepad", exact: false);
```

---

#### `TryFindByTitle(string title, bool exact, out WindowControl? window)`

```csharp
public static bool TryFindByTitle(string title, bool exact, out WindowControl? window)
```

Безопасный поиск окна по заголовку (не бросает `WindowNotFoundException`).

**Параметры:**
- `title` - заголовок окна
- `exact` - точное/частичное совпадение
- `window` - out-параметр с результатом (null если не найдено)

**Возвращает:** `true` если окно найдено, иначе `false`.

**Исключения:**
- `ArgumentNullException` - `title` == null

**Пример:**
```csharp
if (Window.TryFindByTitle("Calculator", exact: false, out var calc))
{
    calc.Maximize();
}
else
{
    Console.WriteLine("Calculator не запущен");
}
```

---

#### `FindByTitleRegex(string pattern)`

```csharp
public static IEnumerable<WindowControl> FindByTitleRegex(string pattern)
```

Ищет окна по регулярному выражению.

**Параметры:**
- `pattern` - .NET regex паттерн (case-insensitive по умолчанию)

**Возвращает:** Коллекция окон с совпадающими заголовками (может быть пустой).

**Исключения:**
- `ArgumentNullException` - `pattern` == null
- `ArgumentException` - невалидное регулярное выражение

**Примечания:**
- Regex компилируется и кэшируется для производительности
- По умолчанию case-insensitive

**Примеры паттернов:**
- `".*Chrome$"` - заголовки заканчивающиеся на "Chrome"
- `"^Notepad"` - заголовки начинающиеся с "Notepad"
- `"File.*Explorer"` - содержит "File" затем "Explorer"

**Пример:**
```csharp
var chromeWindows = Window.FindByTitleRegex(@".*Google Chrome$");
foreach (var window in chromeWindows)
{
    Console.WriteLine(window.Title);
}
```

---

#### `FindByClassName(string className)`

```csharp
public static IEnumerable<WindowControl> FindByClassName(string className)
```

Ищет окна по имени класса окна.

**Параметры:**
- `className` - имя класса (например "Notepad", "Chrome_WidgetWin_1")

**Возвращает:** Коллекция окон с указанным классом (может быть пустой).

**Исключения:**
- `ArgumentNullException` - `className` == null

**Примечания:**
- Поиск case-insensitive
- Класс окна более стабилен чем заголовок
- Пустая строка ищет окна без класса (редко)

**Пример:**
```csharp
var notepadWindows = Window.FindByClassName("Notepad");
```

---

#### `FindByProcessId(uint processId)`

```csharp
public static IEnumerable<WindowControl> FindByProcessId(uint processId)
```

Ищет все окна принадлежащие процессу.

**Параметры:**
- `processId` - ID процесса (PID)

**Возвращает:** Коллекция окон процесса (может быть пустой).

**Примечания:**
- Возвращает все окна процесса (главные + диалоги)
- Если процесс не существует, возвращает пустую коллекцию (не бросает исключение)

**Пример:**
```csharp
uint pid = 12345;
var windows = Window.FindByProcessId(pid);
foreach (var window in windows)
{
    Console.WriteLine($"{window.Title} [PID: {window.ProcessId}]");
}
```

---

#### `FromHandle(IntPtr handle)`

```csharp
public static WindowControl FromHandle(IntPtr handle)
```

Создает `WindowControl` из существующего HWND.

**Параметры:**
- `handle` - нативный HWND (IntPtr)

**Возвращает:** `WindowControl` обертка над handle.

**Исключения:**
- `InvalidWindowHandleException` - handle == IntPtr.Zero или невалидное окно

**Примечания:**
- Валидирует handle через Win32 `IsWindow()`
- Используется для interop с другими Win32 API

**Пример:**
```csharp
IntPtr hwnd = GetSomeHwndFromSomewhere();
var window = Window.FromHandle(hwnd);
Console.WriteLine(window.Title);
```

---

#### `GetForegroundWindow()`

```csharp
public static WindowControl? GetForegroundWindow()
```

Получает текущее активное (foreground) окно.

**Возвращает:** Активное окно или `null` если нет активного окна.

**Примечания:**
- Возвращает окно с фокусом клавиатуры
- Может вернуть `null` (редко)
- Окно может принадлежать любому процессу

**Пример:**
```csharp
var activeWindow = Window.GetForegroundWindow();
if (activeWindow != null)
{
    Console.WriteLine($"Активно: {activeWindow.Title}");
}
```

---

## Класс `WindowControl`

Представляет одно окно Windows. Экземпляры создаются через методы класса `Window`.

### Свойства

#### `Handle`

```csharp
public IntPtr Handle { get; }
```

Нативный HWND (Window Handle).

**Тип:** `IntPtr` (read-only)

**Примечания:**
- Для advanced interop сценариев
- Handle может стать невалидным если окно закрылось

---

#### `Title`

```csharp
public string Title { get; }
```

Заголовок окна (caption).

**Тип:** `string` (read-only)

**Исключения:**
- `WindowOperationException` - не удалось получить заголовок

**Примечания:**
- Каждый доступ запрашивает Win32 (не кэшируется)
- Максимум 256 символов
- Пустая строка если окно без заголовка

---

#### `ClassName`

```csharp
public string ClassName { get; }
```

Имя класса окна.

**Тип:** `string` (read-only)

**Исключения:**
- `WindowOperationException` - не удалось получить класс

**Примечания:**
- Каждый доступ запрашивает Win32
- Класс окна более стабилен чем заголовок

---

#### `ProcessId`

```csharp
public uint ProcessId { get; }
```

ID процесса владельца окна.

**Тип:** `uint` (read-only)

**Исключения:**
- `WindowOperationException` - не удалось получить PID

---

#### `IsVisible`

```csharp
public bool IsVisible { get; }
```

Видимо ли окно.

**Тип:** `bool` (read-only)

**Примечания:**
- Не бросает исключений (возвращает `false` если окно закрыто)
- Каждый доступ запрашивает Win32

---

#### `IsValid`

```csharp
public bool IsValid { get; }
```

Существует ли окно (валиден ли handle).

**Тип:** `bool` (read-only)

**Примечания:**
- Безопасно вызывать на любом handle
- Не бросает исключений
- Используется для проверки существования окна

**Пример:**
```csharp
var window = Window.FindByTitle("Temp");
// ... время проходит ...
if (!window.IsValid)
{
    Console.WriteLine("Окно закрыто");
}
```

---

#### `Bounds`

```csharp
public WindowBounds Bounds { get; }
```

Позиция и размер окна.

**Тип:** `WindowBounds` (read-only)

**Исключения:**
- `WindowOperationException` - не удалось получить границы

**Примечания:**
- Координаты в screen space (могут быть отрицательными на multi-monitor)
- Каждый доступ запрашивает Win32

**Пример:**
```csharp
var bounds = window.Bounds;
Console.WriteLine($"X:{bounds.X}, Y:{bounds.Y}, W:{bounds.Width}, H:{bounds.Height}");
```

---

#### `State`

```csharp
public WindowState State { get; }
```

Состояние окна (Normal/Minimized/Maximized).

**Тип:** `WindowState` enum (read-only)

**Исключения:**
- `WindowOperationException` - не удалось получить состояние

**Примечания:**
- Каждый доступ запрашивает Win32

**Пример:**
```csharp
if (window.State == WindowState.Minimized)
{
    window.Restore();
}
```

---

#### `Parent`

```csharp
public WindowControl? Parent { get; }
```

Родительское окно или `null` если это top-level окно.

**Тип:** `WindowControl?` (read-only)

**Исключения:**
- `WindowOperationException` - не удалось получить родителя

**Примечания:**
- `null` для top-level окон
- Каждый доступ запрашивает Win32

---

### Методы управления состоянием

#### `Activate()`

```csharp
public void Activate()
```

Активирует окно (выводит на передний план, дает фокус клавиатуры).

**Исключения:**
- `InvalidWindowHandleException` - окно закрыто
- `WindowOperationException` - не удалось активировать

**Примечания:**
- Вызывает Win32 `SetForegroundWindow`
- Если окно свернуто, сначала восстанавливает
- Может не сработать если другое приложение не отпустило фокус

**Пример:**
```csharp
window.Activate();
```

---

#### `Minimize()`

```csharp
public void Minimize()
```

Сворачивает окно в taskbar.

**Исключения:**
- `InvalidWindowHandleException` - окно закрыто
- `WindowOperationException` - не удалось свернуть

**Примечания:**
- Вызывает Win32 `ShowWindow(SW_MINIMIZE)`

---

#### `Maximize()`

```csharp
public void Maximize()
```

Разворачивает окно на весь экран.

**Исключения:**
- `InvalidWindowHandleException` - окно закрыто
- `WindowOperationException` - не удалось развернуть

**Примечания:**
- Вызывает Win32 `ShowWindow(SW_MAXIMIZE)`

---

#### `Restore()`

```csharp
public void Restore()
```

Восстанавливает окно в нормальное состояние.

**Исключения:**
- `InvalidWindowHandleException` - окно закрыто
- `WindowOperationException` - не удалось восстановить

**Примечания:**
- Вызывает Win32 `ShowWindow(SW_RESTORE)`
- Возвращает размер/позицию до minimize/maximize

---

#### `Close()`

```csharp
public void Close()
```

Закрывает окно (отправляет WM_CLOSE).

**Исключения:**
- `InvalidWindowHandleException` - окно уже закрыто
- `WindowOperationException` - не удалось отправить сообщение

**Примечания:**
- Отправляет WM_CLOSE через `SendMessage`
- Приложение может проигнорировать (например, запросить сохранение)
- Метод возвращается сразу, не ждет закрытия
- Используйте `IsValid` для проверки закрытия

**Пример:**
```csharp
window.Close();
Thread.Sleep(500);
if (!window.IsValid)
{
    Console.WriteLine("Окно закрыто");
}
```

---

### Методы позиции и размера

#### `MoveTo(int x, int y)`

```csharp
public void MoveTo(int x, int y)
```

Перемещает окно в указанные координаты.

**Параметры:**
- `x` - координата левого края (screen space)
- `y` - координата верхнего края (screen space)

**Исключения:**
- `InvalidWindowHandleException` - окно закрыто
- `WindowOperationException` - не удалось переместить

**Примечания:**
- Координаты могут быть отрицательными (multi-monitor)
- Размер окна не меняется
- Вызывает Win32 `SetWindowPos` с флагом `SWP_NOSIZE`

**Пример:**
```csharp
window.MoveTo(100, 100);
```

---

#### `Resize(int width, int height)`

```csharp
public void Resize(int width, int height)
```

Изменяет размер окна.

**Параметры:**
- `width` - новая ширина (> 0)
- `height` - новая высота (> 0)

**Исключения:**
- `ArgumentOutOfRangeException` - width или height <= 0
- `InvalidWindowHandleException` - окно закрыто
- `WindowOperationException` - не удалось изменить размер

**Примечания:**
- Позиция окна не меняется
- Вызывает Win32 `SetWindowPos` с флагом `SWP_NOMOVE`

**Пример:**
```csharp
window.Resize(800, 600);
```

---

#### `SetBounds(WindowBounds bounds)`

```csharp
public void SetBounds(WindowBounds bounds)
```

Устанавливает позицию и размер окна одновременно.

**Параметры:**
- `bounds` - новые границы окна

**Исключения:**
- `InvalidWindowHandleException` - окно закрыто
- `WindowOperationException` - не удалось установить границы

**Примечания:**
- Эффективнее чем `MoveTo` + `Resize` отдельно
- Вызывает Win32 `SetWindowPos` один раз

**Пример:**
```csharp
var bounds = new WindowBounds(100, 100, 800, 600);
window.SetBounds(bounds);
```

---

### Методы иерархии

#### `GetChildren()`

```csharp
public IEnumerable<WindowControl> GetChildren()
```

Возвращает все прямые дочерние окна (контролы).

**Возвращает:** Коллекция дочерних окон (пустая если нет детей).

**Исключения:**
- `WindowOperationException` - окно закрыто во время перечисления

**Примечания:**
- Использует Win32 `EnumChildWindows`
- Возвращает только прямых детей (не рекурсивно)
- Для рекурсии: вызовите `GetChildren()` на каждом дочернем окне

**Пример:**
```csharp
var children = window.GetChildren();
foreach (var child in children)
{
    Console.WriteLine($"Дочернее: {child.ClassName}");
}
```

---

#### `FindChild(string className, string? text = null)`

```csharp
public WindowControl? FindChild(string className, string? text = null)
```

Ищет дочернее окно по классу и опционально тексту.

**Параметры:**
- `className` - класс дочернего окна (пустая строка = любой класс)
- `text` - текст/caption (null = любой текст)

**Возвращает:** Первое найденное дочернее окно или `null`.

**Исключения:**
- `ArgumentNullException` - `className` == null
- `WindowOperationException` - окно закрыто

**Примечания:**
- Использует Win32 `FindWindowEx`
- Поиск case-insensitive
- Если несколько совпадений, возвращает первое

**Пример:**
```csharp
var button = window.FindChild("Button", "OK");
if (button != null)
{
    Console.WriteLine($"Найдена кнопка: {button.Bounds}");
}
```

---

## Struct `WindowBounds`

Immutable структура для позиции и размера окна.

### Конструктор

```csharp
public WindowBounds(int x, int y, int width, int height)
```

**Параметры:**
- `x` - координата левого края
- `y` - координата верхнего края
- `width` - ширина (> 0)
- `height` - высота (> 0)

**Исключения:**
- `ArgumentOutOfRangeException` - width или height <= 0

### Свойства

```csharp
public int X { get; }          // левый край
public int Y { get; }          // верхний край
public int Width { get; }      // ширина
public int Height { get; }     // высота
public int Right { get; }      // правый край (X + Width)
public int Bottom { get; }     // нижний край (Y + Height)
```

### Методы

```csharp
public override string ToString()  // возвращает "(X, Y, Width, Height)"
public override bool Equals(object? obj)
public bool Equals(WindowBounds other)
public override int GetHashCode()
public static bool operator ==(WindowBounds left, WindowBounds right)
public static bool operator !=(WindowBounds left, WindowBounds right)
```

**Пример:**
```csharp
var bounds = new WindowBounds(100, 100, 800, 600);
Console.WriteLine(bounds);  // "(100, 100, 800, 600)"
Console.WriteLine($"Right: {bounds.Right}, Bottom: {bounds.Bottom}");
```

---

## Enum `WindowState`

Состояние окна.

```csharp
public enum WindowState
{
    Normal = 0,      // обычное состояние
    Minimized = 1,   // свернуто
    Maximized = 2    // развернуто
}
```

**Пример:**
```csharp
if (window.State == WindowState.Minimized)
{
    window.Restore();
}
```

---

## Исключения

Все исключения наследуются от `WindowManagerException`.

### `WindowManagerException` (базовый)

```csharp
public abstract class WindowManagerException : Exception
{
    public IntPtr Handle { get; }           // HWND связанный с ошибкой
    public int? Win32ErrorCode { get; }     // Win32 код ошибки
    public string? Win32ErrorMessage { get; } // Win32 сообщение об ошибке
}
```

---

### `WindowNotFoundException`

Бросается когда окно не найдено по критериям поиска.

```csharp
public sealed class WindowNotFoundException : WindowManagerException
{
    public string SearchCriteria { get; } // описание критериев поиска
}
```

**Когда бросается:**
- `Window.FindByTitle()` не нашел окно
- Другие методы `Find*()` не нашли окно

**Пример обработки:**
```csharp
try
{
    var window = Window.FindByTitle("NonExistent");
}
catch (WindowNotFoundException ex)
{
    Console.WriteLine($"Не найдено: {ex.SearchCriteria}");
}
```

---

### `WindowOperationException`

Бросается когда операция над окном не удалась.

```csharp
public sealed class WindowOperationException : WindowManagerException
{
    public string Operation { get; } // название операции
}
```

**Когда бросается:**
- `Activate()`, `Minimize()`, `Maximize()`, `Restore()` не удались
- `MoveTo()`, `Resize()`, `SetBounds()` не удались
- Доступ к свойствам не удался

**Пример обработки:**
```csharp
try
{
    window.Maximize();
}
catch (WindowOperationException ex)
{
    Console.WriteLine($"Операция '{ex.Operation}' не удалась");
    Console.WriteLine($"Win32 код: {ex.Win32ErrorCode}");
    Console.WriteLine($"Win32 сообщение: {ex.Win32ErrorMessage}");
}
```

---

### `InvalidWindowHandleException`

Бросается когда handle невалиден или окно закрыто.

```csharp
public sealed class InvalidWindowHandleException : WindowManagerException
{
    public string Reason { get; } // причина невалидности
}
```

**Когда бросается:**
- `Window.FromHandle()` с невалидным handle
- Операции на закрытом окне
- Handle == IntPtr.Zero

**Пример обработки:**
```csharp
try
{
    var window = Window.FromHandle(IntPtr.Zero);
}
catch (InvalidWindowHandleException ex)
{
    Console.WriteLine($"Невалидный handle: {ex.Reason}");
    Console.WriteLine($"HWND: 0x{ex.Handle:X}");
}
```

---

## Типичные сценарии использования

### Найти и активировать окно

```csharp
try
{
    var notepad = Window.FindByTitle("Notepad", exact: false);
    notepad.Activate();
}
catch (WindowNotFoundException)
{
    Console.WriteLine("Notepad не запущен");
}
```

### Изменить размер всех окон Chrome

```csharp
var chromeWindows = Window.FindByTitleRegex(@".*Chrome$");
foreach (var window in chromeWindows)
{
    window.Resize(1024, 768);
}
```

### Найти дочерний контрол и кликнуть

```csharp
var mainWindow = Window.FindByTitle("My Application");
var button = mainWindow.FindChild("Button", "OK");
if (button != null)
{
    button.Activate();
    // Отправить клик через другой API (не в этой библиотеке)
}
```

### Проверка существования окна

```csharp
var window = Window.FindByTitle("My App");
// ... делаем что-то ...
if (!window.IsValid)
{
    Console.WriteLine("Окно было закрыто");
}
```

### Безопасный поиск с Try-паттерном

```csharp
if (Window.TryFindByTitle("Calculator", exact: false, out var calc))
{
    calc.Maximize();
    calc.MoveTo(0, 0);
}
else
{
    // Запустить Calculator
}
```

---

## Производительность

- Поиск окон: < 100ms для ~100 окон
- Операции над окном: < 50ms
- Свойства всегда fresh (запрашивают Win32 при каждом доступе)
- Regex паттерны компилируются и кэшируются

---

## Ограничения

- Только Windows платформа
- Требует .NET 6.0+
- Некоторые операции могут не работать с окнами с повышенными привилегиями (UAC)
- `Close()` отправляет WM_CLOSE, но приложение может проигнорировать
- `Activate()` может не сработать если другое приложение держит фокус
