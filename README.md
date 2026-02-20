# WindowManagerCL

.NET библиотека для программного управления окнами Windows.

## Что это

WindowManagerCL - это профессиональная библиотека для управления окнами Windows, входящая в состав **Windows Automation SDK**. Позволяет программно находить и управлять окнами Windows Desktop через Win32 API:

- Поиск окон по заголовку, классу, Process ID, regex
- Управление состоянием (активировать, свернуть, развернуть, закрыть)
- Изменение позиции и размера
- Навигация по иерархии окон (родители/дети)

### Экосистема SDK

WindowManagerCL является частью комплексного решения для автоматизации Windows:

- **WindowManagerCL** (эта библиотека) - управление окнами и их поиск
- **SendSequenceCL** - симуляция ввода с клавиатуры и мыши (клики, перемещения, нажатия клавиш)
- **WindowCaptureCL** - захват скриншотов окон и областей экрана
- **ImageSearchCL** - поиск визуальных элементов на экране

Вместе эти библиотеки обеспечивают 90%+ покрытие задач человекоподобной автоматизации Windows.

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

## Интеграция с SDK

WindowManagerCL работает в связке с другими библиотеками SDK для полной автоматизации.

### Пример: Автоматизация заполнения формы

```csharp
using WindowManagerCL.API;
using SendSequenceCL; // библиотека для ввода

// 1. Найти и активировать окно приложения
var app = Window.FindByTitle("Регистрационная форма", exact: false);
app.Activate();
Thread.Sleep(100); // дать время на активацию

// 2. Найти поле ввода имени и кликнуть в него
var nameField = app.FindChild("Edit", "Имя");
if (nameField != null)
{
    // Позиционировать мышь в центр поля
    var bounds = nameField.Bounds;
    MouseInput.MoveTo(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
    MouseInput.Click();

    // Ввести текст
    KeyboardInput.SendKeys("Иван Иванов");
}

// 3. Найти кнопку и кликнуть
var submitButton = app.FindChild("Button", "Отправить");
if (submitButton != null)
{
    var btnBounds = submitButton.Bounds;
    MouseInput.MoveTo(btnBounds.X + btnBounds.Width / 2, btnBounds.Y + btnBounds.Height / 2);
    MouseInput.Click();
}
```

### Пример: Визуальная автоматизация с поиском элементов

```csharp
using WindowManagerCL.API;
using WindowCaptureCL;
using ImageSearchCL;
using SendSequenceCL;

// 1. Найти окно игры
var gameWindow = Window.FindByTitle("My Game", exact: false);
gameWindow.Activate();
gameWindow.SetBounds(new WindowBounds(0, 0, 1920, 1080));

// 2. Захватить скриншот окна
var screenshot = WindowCapture.CaptureWindow(gameWindow.Handle);

// 3. Найти кнопку "Start" на скриншоте
var buttonLocation = ImageSearch.FindImage(screenshot, "start_button_template.png");
if (buttonLocation != null)
{
    // 4. Кликнуть по найденной кнопке
    var gameBounds = gameWindow.Bounds;
    MouseInput.MoveTo(gameBounds.X + buttonLocation.X, gameBounds.Y + buttonLocation.Y);
    MouseInput.Click();
}
```

### Пример: Мониторинг и автоматизация множества окон

```csharp
using WindowManagerCL.API;
using SendSequenceCL;

// Найти все окна браузера
var browserWindows = Window.FindByTitleRegex(@".*(Chrome|Firefox|Edge)$");

foreach (var browser in browserWindows)
{
    // Активировать окно
    browser.Activate();
    Thread.Sleep(200);

    // Обновить страницу (F5)
    KeyboardInput.SendKey(VirtualKeyCode.F5);
    Thread.Sleep(500);
}
```

**Примечание:** Примеры выше демонстрируют интеграцию с другими библиотеками SDK. Для работы с вводом используйте **SendSequenceCL**, для скриншотов - **WindowCaptureCL**, для поиска изображений - **ImageSearchCL**.

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
- **[SDK_INTEGRATION.md](SDK_INTEGRATION.md)** - интеграция с другими библиотеками SDK

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
