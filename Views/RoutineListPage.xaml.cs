using FitArmLog.ViewModels;

namespace FitArmLog.Views;

public partial class RoutineListPage : ContentPage
{
    private readonly RoutineListViewModel _viewModel;

    public RoutineListPage(RoutineListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        
        _viewModel.LoadRoutinesCommand.Execute(null);
    }
}