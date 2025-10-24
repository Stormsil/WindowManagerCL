using WindowManagerCL.API;

namespace WindowManagerCL.InteractiveDemo;

/// <summary>
/// Interactive console application for testing WindowManagerCL library
/// </summary>
class Program
{
    private static WindowControl? _currentWindow;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        ShowWelcome();

        while (true)
        {
            ShowMainMenu();
            var choice = Console.ReadLine()?.Trim();

            if (choice == "0")
            {
                Console.WriteLine("\nДо свидания!");
                break;
            }

            HandleMainMenuChoice(choice);
        }
    }

    static void ShowWelcome()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                                                          ║");
        Console.WriteLine("║         WindowManagerCL - Интерактивная Демо             ║");
        Console.WriteLine("║                                                          ║");
        Console.WriteLine("║    Библиотека управления окнами для Windows (.NET)       ║");
        Console.WriteLine("║                                                          ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Нажмите Enter для продолжения...");
        Console.ReadLine();
    }

    static void ShowMainMenu()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("═══════════════════ ГЛАВНОЕ МЕНЮ ═══════════════════");
        Console.ResetColor();

        if (_currentWindow != null)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✓ Текущее окно: {_currentWindow.Title}");
            Console.WriteLine($"  Handle: 0x{_currentWindow.Handle:X}");
            Console.WriteLine($"  Состояние: {_currentWindow.State}");
            Console.WriteLine($"  Позиция: {_currentWindow.Bounds}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n○ Окно не выбрано");
            Console.ResetColor();
        }

        Console.WriteLine("\n┌─────────────────────────────────────────────────────┐");
        Console.WriteLine("│ 1. Поиск окон                                       │");
        Console.WriteLine("│ 2. Управление состоянием окна                       │");
        Console.WriteLine("│ 3. Позиция и размер окна                            │");
        Console.WriteLine("│ 4. Информация об окне                               │");
        Console.WriteLine("│ 5. Иерархия окон                                    │");
        Console.WriteLine("│ 6. Список всех окон                                 │");
        Console.WriteLine("│ 7. Примеры использования                            │");
        Console.WriteLine("│                                                     │");
        Console.WriteLine("│ 0. Выход                                            │");
        Console.WriteLine("└─────────────────────────────────────────────────────┘");
        Console.Write("\nВыберите пункт меню: ");
    }

    static void HandleMainMenuChoice(string? choice)
    {
        switch (choice)
        {
            case "1":
                SearchWindowsMenu();
                break;
            case "2":
                WindowStateMenu();
                break;
            case "3":
                WindowPositionMenu();
                break;
            case "4":
                WindowInfoMenu();
                break;
            case "5":
                WindowHierarchyMenu();
                break;
            case "6":
                ListAllWindows();
                break;
            case "7":
                ExamplesMenu();
                break;
            default:
                ShowError("Неверный выбор!");
                break;
        }
    }

    #region Search Windows Menu

    static void SearchWindowsMenu()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════════ ПОИСК ОКОН ═══════════════════");
        Console.ResetColor();
        Console.WriteLine("\n1. Поиск по точному заголовку");
        Console.WriteLine("2. Поиск по частичному заголовку");
        Console.WriteLine("3. Поиск по регулярному выражению");
        Console.WriteLine("4. Поиск по имени класса");
        Console.WriteLine("5. Поиск по Process ID");
        Console.WriteLine("6. Поиск по HWND");
        Console.WriteLine("\n0. Назад");
        Console.Write("\nВыберите способ поиска: ");

        var choice = Console.ReadLine()?.Trim();

        switch (choice)
        {
            case "1":
                SearchByExactTitle();
                break;
            case "2":
                SearchByPartialTitle();
                break;
            case "3":
                SearchByRegex();
                break;
            case "4":
                SearchByClassName();
                break;
            case "5":
                SearchByProcessId();
                break;
            case "6":
                SearchByHandle();
                break;
            case "0":
                return;
            default:
                ShowError("Неверный выбор!");
                Pause();
                SearchWindowsMenu();
                break;
        }
    }

    static void SearchByExactTitle()
    {
        Console.Write("\nВведите точный заголовок окна: ");
        var title = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title))
        {
            ShowError("Заголовок не может быть пустым!");
            Pause();
            return;
        }

        try
        {
            var window = Window.FindByTitle(title, exact: true);
            _currentWindow = window;
            ShowSuccess($"Окно найдено: {window.Title}");
            ShowWindowDetails(window);
            Pause();
        }
        catch (WindowNotFoundException ex)
        {
            ShowError($"Окно не найдено: {ex.SearchCriteria}");
            Pause();
        }
    }

    static void SearchByPartialTitle()
    {
        Console.Write("\nВведите часть заголовка окна: ");
        var title = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title))
        {
            ShowError("Заголовок не может быть пустым!");
            Pause();
            return;
        }

        try
        {
            if (Window.TryFindByTitle(title, exact: false, out var window) && window != null)
            {
                _currentWindow = window;
                ShowSuccess($"Окно найдено: {window.Title}");
                ShowWindowDetails(window);
            }
            else
            {
                ShowError($"Окно с заголовком содержащим '{title}' не найдено");
            }
            Pause();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
            Pause();
        }
    }

    static void SearchByRegex()
    {
        Console.Write("\nВведите регулярное выражение (например, '.*Chrome$'): ");
        var pattern = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(pattern))
        {
            ShowError("Шаблон не может быть пустым!");
            Pause();
            return;
        }

        try
        {
            var windows = Window.FindByTitleRegex(pattern);
            var windowsList = windows.ToList();

            if (windowsList.Count == 0)
            {
                ShowError("Окна не найдены");
                Pause();
                return;
            }

            Console.WriteLine($"\nНайдено окон: {windowsList.Count}");
            for (int i = 0; i < windowsList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {windowsList[i].Title}");
            }

            Console.Write("\nВыберите номер окна (0 - отмена): ");
            if (int.TryParse(Console.ReadLine(), out var index) && index > 0 && index <= windowsList.Count)
            {
                _currentWindow = windowsList[index - 1];
                ShowSuccess($"Выбрано окно: {_currentWindow.Title}");
                ShowWindowDetails(_currentWindow);
            }
            Pause();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
            Pause();
        }
    }

    static void SearchByClassName()
    {
        Console.Write("\nВведите имя класса окна (например, 'Notepad'): ");
        var className = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(className))
        {
            ShowError("Имя класса не может быть пустым!");
            Pause();
            return;
        }

        try
        {
            var windows = Window.FindByClassName(className);
            var windowsList = windows.ToList();

            if (windowsList.Count == 0)
            {
                ShowError($"Окна с классом '{className}' не найдены");
                Pause();
                return;
            }

            Console.WriteLine($"\nНайдено окон: {windowsList.Count}");
            for (int i = 0; i < windowsList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {windowsList[i].Title} - {windowsList[i].ClassName}");
            }

            Console.Write("\nВыберите номер окна (0 - отмена): ");
            if (int.TryParse(Console.ReadLine(), out var index) && index > 0 && index <= windowsList.Count)
            {
                _currentWindow = windowsList[index - 1];
                ShowSuccess($"Выбрано окно: {_currentWindow.Title}");
                ShowWindowDetails(_currentWindow);
            }
            Pause();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
            Pause();
        }
    }

    static void SearchByProcessId()
    {
        Console.Write("\nВведите Process ID: ");
        if (!uint.TryParse(Console.ReadLine(), out var processId))
        {
            ShowError("Неверный формат Process ID!");
            Pause();
            return;
        }

        try
        {
            var windows = Window.FindByProcessId(processId);
            var windowsList = windows.ToList();

            if (windowsList.Count == 0)
            {
                ShowError($"Окна для процесса {processId} не найдены");
                Pause();
                return;
            }

            Console.WriteLine($"\nНайдено окон: {windowsList.Count}");
            for (int i = 0; i < windowsList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {windowsList[i].Title}");
            }

            Console.Write("\nВыберите номер окна (0 - отмена): ");
            if (int.TryParse(Console.ReadLine(), out var index) && index > 0 && index <= windowsList.Count)
            {
                _currentWindow = windowsList[index - 1];
                ShowSuccess($"Выбрано окно: {_currentWindow.Title}");
                ShowWindowDetails(_currentWindow);
            }
            Pause();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
            Pause();
        }
    }

    static void SearchByHandle()
    {
        Console.Write("\nВведите HWND (в hex формате, например 0x123456): ");
        var input = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(input))
        {
            ShowError("HWND не может быть пустым!");
            Pause();
            return;
        }

        try
        {
            IntPtr handle;
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                handle = new IntPtr(Convert.ToInt64(input.Substring(2), 16));
            }
            else
            {
                handle = new IntPtr(Convert.ToInt64(input));
            }

            var window = Window.FromHandle(handle);
            _currentWindow = window;
            ShowSuccess($"Окно найдено: {window.Title}");
            ShowWindowDetails(window);
            Pause();
        }
        catch (InvalidWindowHandleException ex)
        {
            ShowError($"Неверный HWND: {ex.Reason}");
            Pause();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
            Pause();
        }
    }

    #endregion

    #region Window State Menu

    static void WindowStateMenu()
    {
        if (!EnsureWindowSelected()) return;

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════ УПРАВЛЕНИЕ СОСТОЯНИЕМ ═══════════════");
        Console.ResetColor();
        Console.WriteLine($"\nТекущее окно: {_currentWindow!.Title}");
        Console.WriteLine($"Текущее состояние: {_currentWindow.State}");
        Console.WriteLine("\n1. Активировать (вывести на передний план)");
        Console.WriteLine("2. Свернуть");
        Console.WriteLine("3. Развернуть");
        Console.WriteLine("4. Восстановить");
        Console.WriteLine("5. Закрыть");
        Console.WriteLine("\n0. Назад");
        Console.Write("\nВыберите действие: ");

        var choice = Console.ReadLine()?.Trim();

        try
        {
            switch (choice)
            {
                case "1":
                    _currentWindow.Activate();
                    ShowSuccess("Окно активировано");
                    break;
                case "2":
                    _currentWindow.Minimize();
                    ShowSuccess("Окно свернуто");
                    break;
                case "3":
                    _currentWindow.Maximize();
                    ShowSuccess("Окно развернуто");
                    break;
                case "4":
                    _currentWindow.Restore();
                    ShowSuccess("Окно восстановлено");
                    break;
                case "5":
                    Console.Write("\nВы уверены? (y/n): ");
                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        _currentWindow.Close();
                        ShowSuccess("Окно закрыто");
                        _currentWindow = null;
                    }
                    break;
                case "0":
                    return;
                default:
                    ShowError("Неверный выбор!");
                    break;
            }

            if (choice != "5" && choice != "0")
            {
                Thread.Sleep(500);
                Console.WriteLine($"\nНовое состояние: {_currentWindow?.State}");
            }
            Pause();
        }
        catch (WindowOperationException ex)
        {
            ShowError($"Операция '{ex.Operation}' не удалась");
            Console.WriteLine($"Win32 Error: {ex.Win32ErrorCode} - {ex.Win32ErrorMessage}");
            Pause();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
            Pause();
        }
    }

    #endregion

    #region Window Position Menu

    static void WindowPositionMenu()
    {
        if (!EnsureWindowSelected()) return;

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════ ПОЗИЦИЯ И РАЗМЕР ═══════════════");
        Console.ResetColor();
        Console.WriteLine($"\nТекущее окно: {_currentWindow!.Title}");
        Console.WriteLine($"Текущие границы: {_currentWindow.Bounds}");
        Console.WriteLine("\n1. Переместить окно");
        Console.WriteLine("2. Изменить размер");
        Console.WriteLine("3. Установить границы (позиция + размер)");
        Console.WriteLine("\n0. Назад");
        Console.Write("\nВыберите действие: ");

        var choice = Console.ReadLine()?.Trim();

        try
        {
            switch (choice)
            {
                case "1":
                    MoveWindow();
                    break;
                case "2":
                    ResizeWindow();
                    break;
                case "3":
                    SetWindowBounds();
                    break;
                case "0":
                    return;
                default:
                    ShowError("Неверный выбор!");
                    Pause();
                    break;
            }
        }
        catch (WindowOperationException ex)
        {
            ShowError($"Операция '{ex.Operation}' не удалась");
            Console.WriteLine($"Win32 Error: {ex.Win32ErrorCode} - {ex.Win32ErrorMessage}");
            Pause();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
            Pause();
        }
    }

    static void MoveWindow()
    {
        Console.Write("\nВведите X координату: ");
        if (!int.TryParse(Console.ReadLine(), out var x))
        {
            ShowError("Неверный формат X!");
            Pause();
            return;
        }

        Console.Write("Введите Y координату: ");
        if (!int.TryParse(Console.ReadLine(), out var y))
        {
            ShowError("Неверный формат Y!");
            Pause();
            return;
        }

        _currentWindow!.MoveTo(x, y);
        ShowSuccess($"Окно перемещено в ({x}, {y})");
        Console.WriteLine($"Новые границы: {_currentWindow.Bounds}");
        Pause();
    }

    static void ResizeWindow()
    {
        Console.Write("\nВведите ширину: ");
        if (!int.TryParse(Console.ReadLine(), out var width))
        {
            ShowError("Неверный формат ширины!");
            Pause();
            return;
        }

        Console.Write("Введите высоту: ");
        if (!int.TryParse(Console.ReadLine(), out var height))
        {
            ShowError("Неверный формат высоты!");
            Pause();
            return;
        }

        _currentWindow!.Resize(width, height);
        ShowSuccess($"Размер окна изменен на {width}x{height}");
        Console.WriteLine($"Новые границы: {_currentWindow.Bounds}");
        Pause();
    }

    static void SetWindowBounds()
    {
        Console.Write("\nВведите X координату: ");
        if (!int.TryParse(Console.ReadLine(), out var x))
        {
            ShowError("Неверный формат X!");
            Pause();
            return;
        }

        Console.Write("Введите Y координату: ");
        if (!int.TryParse(Console.ReadLine(), out var y))
        {
            ShowError("Неверный формат Y!");
            Pause();
            return;
        }

        Console.Write("Введите ширину: ");
        if (!int.TryParse(Console.ReadLine(), out var width))
        {
            ShowError("Неверный формат ширины!");
            Pause();
            return;
        }

        Console.Write("Введите высоту: ");
        if (!int.TryParse(Console.ReadLine(), out var height))
        {
            ShowError("Неверный формат высоты!");
            Pause();
            return;
        }

        var bounds = new WindowBounds(x, y, width, height);
        _currentWindow!.SetBounds(bounds);
        ShowSuccess("Границы окна установлены");
        Console.WriteLine($"Новые границы: {_currentWindow.Bounds}");
        Pause();
    }

    #endregion

    #region Window Info Menu

    static void WindowInfoMenu()
    {
        if (!EnsureWindowSelected()) return;

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════ ИНФОРМАЦИЯ ОБ ОКНЕ ═══════════════");
        Console.ResetColor();

        ShowWindowDetails(_currentWindow!);
        Pause();
    }

    static void ShowWindowDetails(WindowControl window)
    {
        Console.WriteLine("\n┌─────────────────────────────────────────────────────┐");
        Console.WriteLine($"│ Заголовок: {window.Title.PadRight(40).Substring(0, 40)} │");
        Console.WriteLine($"│ Класс: {window.ClassName.PadRight(44).Substring(0, 44)} │");
        Console.WriteLine($"│ Handle (HWND): 0x{window.Handle:X16}                │");
        Console.WriteLine($"│ Process ID: {window.ProcessId,-38} │");
        Console.WriteLine($"│ Состояние: {window.State,-41} │");
        Console.WriteLine($"│ Границы: {window.Bounds.ToString().PadRight(43).Substring(0, 43)} │");
        Console.WriteLine($"│ Видимо: {(window.IsVisible ? "Да" : "Нет"),-46} │");
        Console.WriteLine($"│ Валидно: {(window.IsValid ? "Да" : "Нет"),-45} │");

        if (window.Parent != null)
        {
            Console.WriteLine($"│ Родитель: {window.Parent.Title.PadRight(41).Substring(0, 41)} │");
        }
        else
        {
            Console.WriteLine($"│ Родитель: нет                                       │");
        }

        Console.WriteLine("└─────────────────────────────────────────────────────┘");
    }

    #endregion

    #region Window Hierarchy Menu

    static void WindowHierarchyMenu()
    {
        if (!EnsureWindowSelected()) return;

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════ ИЕРАРХИЯ ОКОН ═══════════════");
        Console.ResetColor();
        Console.WriteLine($"\nТекущее окно: {_currentWindow!.Title}");
        Console.WriteLine("\n1. Показать родительское окно");
        Console.WriteLine("2. Показать дочерние окна");
        Console.WriteLine("3. Найти дочернее окно по классу");
        Console.WriteLine("\n0. Назад");
        Console.Write("\nВыберите действие: ");

        var choice = Console.ReadLine()?.Trim();

        try
        {
            switch (choice)
            {
                case "1":
                    ShowParentWindow();
                    break;
                case "2":
                    ShowChildWindows();
                    break;
                case "3":
                    FindChildWindow();
                    break;
                case "0":
                    return;
                default:
                    ShowError("Неверный выбор!");
                    Pause();
                    break;
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
            Pause();
        }
    }

    static void ShowParentWindow()
    {
        if (_currentWindow!.Parent != null)
        {
            Console.WriteLine("\nРодительское окно:");
            ShowWindowDetails(_currentWindow.Parent);

            Console.Write("\nСделать родительское окно текущим? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                _currentWindow = _currentWindow.Parent;
                ShowSuccess("Родительское окно стало текущим");
            }
        }
        else
        {
            ShowError("У этого окна нет родителя");
        }
        Pause();
    }

    static void ShowChildWindows()
    {
        var children = _currentWindow!.GetChildren().ToList();

        if (children.Count == 0)
        {
            ShowError("У этого окна нет дочерних окон");
            Pause();
            return;
        }

        Console.WriteLine($"\nНайдено дочерних окон: {children.Count}");

        for (int i = 0; i < children.Count && i < 20; i++)
        {
            var child = children[i];
            Console.WriteLine($"\n{i + 1}. {child.ClassName} - {child.Title}");
            Console.WriteLine($"   Handle: 0x{child.Handle:X}, Видимо: {child.IsVisible}");
            if (child.IsVisible)
            {
                Console.WriteLine($"   Bounds: {child.Bounds}");
            }
        }

        if (children.Count > 20)
        {
            Console.WriteLine($"\n... и еще {children.Count - 20} окон");
        }

        Console.Write("\nВыбрать дочернее окно как текущее? Введите номер (0 - отмена): ");
        if (int.TryParse(Console.ReadLine(), out var index) && index > 0 && index <= children.Count)
        {
            _currentWindow = children[index - 1];
            ShowSuccess($"Выбрано окно: {_currentWindow.ClassName}");
        }

        Pause();
    }

    static void FindChildWindow()
    {
        Console.Write("\nВведите класс дочернего окна: ");
        var className = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(className))
        {
            ShowError("Класс не может быть пустым!");
            Pause();
            return;
        }

        Console.Write("Введите текст дочернего окна (или Enter для пропуска): ");
        var text = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(text))
            text = null;

        var child = _currentWindow!.FindChild(className, text);

        if (child != null)
        {
            ShowSuccess("Дочернее окно найдено:");
            ShowWindowDetails(child);

            Console.Write("\nСделать это окно текущим? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                _currentWindow = child;
                ShowSuccess("Окно стало текущим");
            }
        }
        else
        {
            ShowError("Дочернее окно не найдено");
        }

        Pause();
    }

    #endregion

    #region List All Windows

    static void ListAllWindows()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════ СПИСОК ВСЕХ ОКОН ═══════════════");
        Console.ResetColor();

        Console.WriteLine("\n1. Все окна");
        Console.WriteLine("2. Только видимые окна");
        Console.WriteLine("\n0. Назад");
        Console.Write("\nВыберите: ");

        var choice = Console.ReadLine()?.Trim();

        try
        {
            IEnumerable<WindowControl> windows;

            if (choice == "1")
            {
                windows = Window.FindAll();
            }
            else if (choice == "2")
            {
                windows = Window.FindAll().Where(w => w.IsVisible);
            }
            else if (choice == "0")
            {
                return;
            }
            else
            {
                ShowError("Неверный выбор!");
                Pause();
                return;
            }

            var windowsList = windows.ToList();
            Console.WriteLine($"\nНайдено окон: {windowsList.Count}");
            Console.WriteLine("\nПоказать первые 30:");

            for (int i = 0; i < windowsList.Count && i < 30; i++)
            {
                var w = windowsList[i];
                Console.WriteLine($"{i + 1,3}. {w.Title,-50} [{w.ClassName}]");
            }

            if (windowsList.Count > 30)
            {
                Console.WriteLine($"\n... и еще {windowsList.Count - 30} окон");
            }

            Console.Write("\nВыбрать окно как текущее? Введите номер (0 - отмена): ");
            if (int.TryParse(Console.ReadLine(), out var index) && index > 0 && index <= Math.Min(30, windowsList.Count))
            {
                _currentWindow = windowsList[index - 1];
                ShowSuccess($"Выбрано окно: {_currentWindow.Title}");
                ShowWindowDetails(_currentWindow);
            }

            Pause();
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
            Pause();
        }
    }

    #endregion

    #region Examples Menu

    static void ExamplesMenu()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════ ПРИМЕРЫ ИСПОЛЬЗОВАНИЯ ═══════════════");
        Console.ResetColor();
        Console.WriteLine("\n1. Найти Notepad и развернуть");
        Console.WriteLine("2. Найти все окна Chrome");
        Console.WriteLine("3. Найти Calculator и изменить размер");
        Console.WriteLine("4. Показать информацию о текущем активном окне");
        Console.WriteLine("\n0. Назад");
        Console.Write("\nВыберите пример: ");

        var choice = Console.ReadLine()?.Trim();

        switch (choice)
        {
            case "1":
                ExampleFindAndMaximizeNotepad();
                break;
            case "2":
                ExampleFindAllChrome();
                break;
            case "3":
                ExampleResizeCalculator();
                break;
            case "4":
                ExampleShowActiveWindow();
                break;
            case "0":
                return;
            default:
                ShowError("Неверный выбор!");
                Pause();
                break;
        }
    }

    static void ExampleFindAndMaximizeNotepad()
    {
        Console.WriteLine("\nПример: Поиск Notepad и развертывание окна");
        Console.WriteLine("═══════════════════════════════════════════════");

        try
        {
            Console.WriteLine("\nИщем окно Notepad...");
            if (Window.TryFindByTitle("Notepad", exact: false, out var notepad) && notepad != null)
            {
                ShowSuccess($"Найдено: {notepad.Title}");
                Console.WriteLine($"Текущее состояние: {notepad.State}");

                Console.WriteLine("\nРазворачиваем окно...");
                notepad.Maximize();
                ShowSuccess("Окно развернуто!");

                _currentWindow = notepad;
            }
            else
            {
                ShowError("Notepad не найден. Откройте Notepad и попробуйте снова.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
        }

        Pause();
    }

    static void ExampleFindAllChrome()
    {
        Console.WriteLine("\nПример: Поиск всех окон Chrome");
        Console.WriteLine("═══════════════════════════════════════");

        try
        {
            Console.WriteLine("\nИщем окна Chrome с помощью regex...");
            var chromeWindows = Window.FindByTitleRegex(@".*Chrome$").ToList();

            if (chromeWindows.Count > 0)
            {
                ShowSuccess($"Найдено окон Chrome: {chromeWindows.Count}");

                foreach (var window in chromeWindows)
                {
                    Console.WriteLine($"\n• {window.Title}");
                    Console.WriteLine($"  Handle: 0x{window.Handle:X}");
                    Console.WriteLine($"  Position: {window.Bounds}");
                    Console.WriteLine($"  Process ID: {window.ProcessId}");
                    Console.WriteLine($"  Visible: {window.IsVisible}");
                }
            }
            else
            {
                ShowError("Окна Chrome не найдены. Откройте Chrome и попробуйте снова.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
        }

        Pause();
    }

    static void ExampleResizeCalculator()
    {
        Console.WriteLine("\nПример: Поиск Calculator и изменение размера");
        Console.WriteLine("═══════════════════════════════════════════════");

        try
        {
            Console.WriteLine("\nИщем Calculator...");
            if ((Window.TryFindByTitle("Калькулятор", exact: false, out var calc) ||
                Window.TryFindByTitle("Calculator", exact: false, out calc)) && calc != null)
            {
                ShowSuccess($"Найдено: {calc.Title}");
                Console.WriteLine($"Текущий размер: {calc.Bounds}");

                Console.WriteLine("\nИзменяем размер на 400x600...");
                calc.Resize(400, 600);
                ShowSuccess("Размер изменен!");
                Console.WriteLine($"Новый размер: {calc.Bounds}");

                _currentWindow = calc;
            }
            else
            {
                ShowError("Calculator не найден. Откройте Calculator и попробуйте снова.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
        }

        Pause();
    }

    static void ExampleShowActiveWindow()
    {
        Console.WriteLine("\nПример: Информация о текущем активном окне");
        Console.WriteLine("═══════════════════════════════════════════════");

        try
        {
            Console.WriteLine("\nПолучаем активное окно (переключитесь на нужное окно в течение 3 секунд)...");
            Thread.Sleep(3000);

            var activeWindow = Window.GetForegroundWindow();
            if (activeWindow != null)
            {
                ShowSuccess("Активное окно найдено:");
                ShowWindowDetails(activeWindow);

                Console.Write("\nСделать это окно текущим? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    _currentWindow = activeWindow;
                    ShowSuccess("Окно стало текущим");
                }
            }
            else
            {
                ShowError("Активное окно не найдено");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
        }

        Pause();
    }

    #endregion

    #region Helper Methods

    static bool EnsureWindowSelected()
    {
        if (_currentWindow == null)
        {
            ShowError("Сначала выберите окно через меню 'Поиск окон'");
            Pause();
            return false;
        }

        if (!_currentWindow.IsValid)
        {
            ShowError("Выбранное окно больше не существует. Выберите другое окно.");
            _currentWindow = null;
            Pause();
            return false;
        }

        return true;
    }

    static void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n✓ {message}");
        Console.ResetColor();
    }

    static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n✗ {message}");
        Console.ResetColor();
    }

    static void Pause()
    {
        Console.WriteLine("\nНажмите Enter для продолжения...");
        Console.ReadLine();
    }

    #endregion
}
