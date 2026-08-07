using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml;
using System.Xml.Xsl;
using KonturTest.Helpers;
using KonturTest.Models;
using KonturTest.Services;

namespace KonturTest.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly IPayrollService _payrollService;
    private readonly IResourcePathProvider _paths;
    private string _statusMessage = "Нажмите «Запустить», чтобы выполнить преобразование.";
    private string _newName = string.Empty;
    private string _newSurname = string.Empty;
    private string _newAmount = string.Empty;
    private string _selectedMonth = MonthNames.All[0];
    private string? _selectedSource;
    private string _employeesXml = string.Empty;

    public MainViewModel(IPayrollService payrollService, IResourcePathProvider paths)
    {
        _payrollService = payrollService;
        _paths = paths;
        RunCommand = new RelayCommand(_ => Run(), _ => !string.IsNullOrWhiteSpace(SelectedSource));
        AddItemCommand = new RelayCommand(_ => AddItem(), _ => CanAddItem());
        RefreshSourcesCommand = new RelayCommand(_ => RefreshSources());
        Months = MonthNames.All;

        Sources = new ObservableCollection<string>();
        RefreshSources();
    }

    public ObservableCollection<EmployeeSummary> Employees { get; } = [];

    public ObservableCollection<MonthlyTotal> MonthlyTotals { get; } = [];

    public ObservableCollection<string> Sources { get; }

    public IReadOnlyList<string> Months { get; }

    public ICommand RunCommand { get; }

    public ICommand AddItemCommand { get; }

    public ICommand RefreshSourcesCommand { get; }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public string NewName
    {
        get => _newName;
        set
        {
            if (SetProperty(ref _newName, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public string NewSurname
    {
        get => _newSurname;
        set
        {
            if (SetProperty(ref _newSurname, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public string NewAmount
    {
        get => _newAmount;
        set
        {
            if (SetProperty(ref _newAmount, value))
            {
                OnPropertyChanged(nameof(AmountValidationHint));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public string? AmountValidationHint =>
        string.IsNullOrWhiteSpace(NewAmount) || AmountParser.TryParse(NewAmount, out _)
            ? null
            : "Сумма должна быть числом (например 1000 или 3001,10).";

    public string SelectedMonth
    {
        get => _selectedMonth;
        set => SetProperty(ref _selectedMonth, value);
    }

    public string? SelectedSource
    {
        get => _selectedSource;
        set
        {
            if (!SetProperty(ref _selectedSource, value))
            {
                return;
            }

            OnPropertyChanged(nameof(IsData1Selected));
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public bool IsData1Selected =>
        string.Equals(SelectedSource, ResourceFileNames.Data1, StringComparison.OrdinalIgnoreCase);

    public string EmployeesXml
    {
        get => _employeesXml;
        private set => SetProperty(ref _employeesXml, value);
    }

    public void RefreshSources()
    {
        var previous = SelectedSource;
        var files = _paths.ListDataFiles();

        Sources.Clear();
        foreach (var fileName in files)
        {
            Sources.Add(fileName);
        }

        SelectedSource = previous is not null
            ? Sources.FirstOrDefault(name => string.Equals(name, previous, StringComparison.OrdinalIgnoreCase))
            : null;

        SelectedSource ??= Sources.FirstOrDefault();

        if (Sources.Count == 0)
        {
            StatusMessage = "Не найдены XML-файлы данных в Resources.";
        }

        CommandManager.InvalidateRequerySuggested();
    }

    private void Run()
    {
        RefreshSources();

        if (string.IsNullOrWhiteSpace(SelectedSource))
        {
            StatusMessage = "Ошибка: не выбран исходный XML-файл.";
            return;
        }

        try
        {
            ApplyResult(_payrollService.Process(SelectedSource));
            StatusMessage = $"Преобразование {SelectedSource} выполнено.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {GetErrorMessage(ex)}";
        }
    }

    private void AddItem()
    {
        try
        {
            SelectedSource = ResourceFileNames.Data1;
            ApplyResult(_payrollService.AddItemAndProcess(NewName.Trim(), NewSurname.Trim(), NewAmount.Trim(), SelectedMonth));
            NewName = string.Empty;
            NewSurname = string.Empty;
            NewAmount = string.Empty;
            SelectedMonth = MonthNames.All[0];
            StatusMessage = "Запись добавлена в Data1.xml, данные пересчитаны.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {GetErrorMessage(ex)}";
        }
    }

    private static string GetErrorMessage(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            var message = current switch
            {
                XmlException => "Файл XML повреждён или имеет неверный формат.",
                XsltException => "Ошибка XSLT-преобразования. Проверьте структуру входного файла.",
                FileNotFoundException fileNotFound => fileNotFound.Message,
                DirectoryNotFoundException => "Папка Resources не найдена.",
                FormatException => current.Message,
                InvalidOperationException => current.Message,
                IOException => "Ошибка чтения или записи файла.",
                UnauthorizedAccessException => "Нет доступа к файлу.",
                _ => null
            };

            if (message is not null)
            {
                return message;
            }
        }

        return "Произошла непредвиденная ошибка.";
    }

    private bool CanAddItem() =>
        IsData1Selected
        && !string.IsNullOrWhiteSpace(NewName)
        && !string.IsNullOrWhiteSpace(NewSurname)
        && AmountParser.TryParse(NewAmount, out _);

    private void ApplyResult(PayrollResult result)
    {
        Employees.Clear();
        foreach (var employee in result.Employees)
        {
            Employees.Add(employee);
        }

        MonthlyTotals.Clear();
        foreach (var monthlyTotal in result.MonthlyTotals)
        {
            MonthlyTotals.Add(monthlyTotal);
        }

        EmployeesXml = result.EmployeesXml;
    }
}
