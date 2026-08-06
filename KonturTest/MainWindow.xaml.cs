using System.Windows;
using KonturTest.Services;
using KonturTest.ViewModels;

namespace KonturTest;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = CreateViewModel();
    }

    private static MainViewModel CreateViewModel()
    {
        var paths = new ResourcePathProvider();
        var payrollService = new PayrollService(
            paths,
            new Data1Repository(),
            new XsltTransformService(),
            new EmployeeDocumentService());

        return new MainViewModel(payrollService, paths);
    }

    private void SourcesComboBox_OnDropDownOpened(object sender, EventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.RefreshSourcesCommand.Execute(null);
        }
    }
}
