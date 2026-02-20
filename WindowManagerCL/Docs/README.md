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

## Интеграция с SDK

WindowManagerCL входит в состав **Windows Automation SDK** и предназначен для работы совместно с другими библиотеками:

### Библиотеки-компаньоны

1. **SendSequenceCL** - симуляция ввода с клавиатуры и мыши
   - Клики мыши (левая/правая кнопка, двойной клик)
   - Перемещение курсора
   - Ввод текста и нажатие клавиш
   - Комбинации клавиш (Ctrl+C, Alt+Tab, и т.д.)

2. **WindowCaptureCL** - захват содержимого экрана
   - Скриншоты окон по HWND
   - Захват областей экрана
   - Сохранение в различных форматах

3. **ImageSearchCL** - поиск визуальных элементов
   - Поиск изображений на скриншотах
   - Template matching
   - Координаты найденных элементов

### Типичные сценарии интеграции

#### Сценарий 1: Автоматизация заполнения веб-формы

```csharp
using WindowManagerCL.API;
using SendSequenceCL;

// Найти окно браузера
var browser = Window.FindByTitle("Chrome", exact: false);
browser.Activate();
browser.Maximize();

// Дать время на активацию
Thread.Sleep(300);

// Перейти к первому полю (Tab)
KeyboardInput.SendKey(VirtualKeyCode.Tab);
Thread.Sleep(100);

// Заполнить имя
KeyboardInput.SendKeys("Иван Иванов");
Thread.Sleep(100);

// Перейти к следующему полю
KeyboardInput.SendKey(VirtualKeyCode.Tab);

// Заполнить email
KeyboardInput.SendKeys("ivan@example.com");
Thread.Sleep(100);

// Отправить форму (Enter)
KeyboardInput.SendKey(VirtualKeyCode.Enter);
```

#### Сценарий 2: Клик по кнопке с использованием координат

```csharp
using WindowManagerCL.API;
using SendSequenceCL;

// Найти окно приложения
var app = Window.FindByTitle("My Application");
app.Activate();

// Найти дочерний элемент (кнопку)
var button = app.FindChild("Button", "OK");
if (button != null)
{
    // Получить координаты центра кнопки
    var bounds = button.Bounds;
    int centerX = bounds.X + bounds.Width / 2;
    int centerY = bounds.Y + bounds.Height / 2;

    // Переместить мышь и кликнуть
    MouseInput.MoveTo(centerX, centerY);
    Thread.Sleep(50);
    MouseInput.Click();
}
```

#### Сценарий 3: Визуальная автоматизация (поиск элемента по изображению)

```csharp
using WindowManagerCL.API;
using WindowCaptureCL;
using ImageSearchCL;
using SendSequenceCL;

// 1. Найти и активировать целевое окно
var gameWindow = Window.FindByTitle("My Game", exact: false);
gameWindow.Activate();

// 2. Убедиться что окно в нужной позиции
gameWindow.SetBounds(new WindowBounds(0, 0, 1920, 1080));
Thread.Sleep(200);

// 3. Захватить скриншот окна
var screenshot = WindowCapture.CaptureWindow(gameWindow.Handle);

// 4. Найти кнопку "Play" на скриншоте
var playButtonPos = ImageSearch.FindImage(screenshot, "templates/play_button.png");
if (playButtonPos != null)
{
    // 5. Кликнуть по найденной кнопке
    var windowBounds = gameWindow.Bounds;
    int clickX = windowBounds.X + playButtonPos.X;
    int clickY = windowBounds.Y + playButtonPos.Y;

    MouseInput.MoveTo(clickX, clickY);
    Thread.Sleep(100);
    MouseInput.Click();
}
```

#### Сценарий 4: Автоматизация работы с несколькими окнами

```csharp
using WindowManagerCL.API;
using SendSequenceCL;

// Найти все окна Notepad
var notepadWindows = Window.FindByClassName("Notepad");

foreach (var notepad in notepadWindows)
{
    // Активировать окно
    notepad.Activate();
    Thread.Sleep(200);

    // Выделить всё (Ctrl+A)
    KeyboardInput.SendKeyCombo(VirtualKeyCode.Control, VirtualKeyCode.A);
    Thread.Sleep(50);

    // Скопировать (Ctrl+C)
    KeyboardInput.SendKeyCombo(VirtualKeyCode.Control, VirtualKeyCode.C);
    Thread.Sleep(50);

    // Вставить дважды (Ctrl+V)
    KeyboardInput.SendKeyCombo(VirtualKeyCode.Control, VirtualKeyCode.V);
    Thread.Sleep(50);
    KeyboardInput.SendKeyCombo(VirtualKeyCode.Control, VirtualKeyCode.V);
}
```

### Важные замечания при интеграции

1. **Тайминги:** Всегда добавляйте небольшие задержки между операциями (50-300ms) для стабильности
2. **Активация окна:** Перед отправкой ввода всегда вызывайте `window.Activate()`
3. **Проверка существования:** Проверяйте `window.IsValid` если окно может закрыться
4. **Координаты:** При использовании мыши учитывайте, что `window.Bounds` дает экранные координаты
5. **Иерархия:** Используйте `FindChild()` для точного поиска элементов интерфейса

### Разделение ответственности

**WindowManagerCL отвечает за:**
- Поиск окон
- Управление состоянием окон (minimize, maximize, restore)
- Позиционирование окон
- Навигация по иерархии (parent, children)

**WindowManagerCL НЕ отвечает за:**
- Клики мышью → используйте **SendSequenceCL**
- Ввод с клавиатуры → используйте **SendSequenceCL**
- Захват скриншотов → используйте **WindowCaptureCL**
- Поиск элементов по изображению → используйте **ImageSearchCL**

Такое разделение обеспечивает чистую архитектуру, упрощает тестирование и позволяет использовать библиотеки независимо друг от друга.

## Требования

- .NET 6.0+
- Windows 10 1809+
- Без сторонних зависимостей

## Полная документация

См. [API_REFERENCE.md](API_REFERENCE.md) для полного описания всех методов и свойств.
