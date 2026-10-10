using FitArmLog.ViewModels;

namespace FitArmLog.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _viewModel;

    public HistoryPage(HistoryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Igual que en RoutineListPage: no usamos await aca para no congelar la UI
        _viewModel.LoadSessionsCommand.Execute(null);
    }
}
