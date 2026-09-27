using FitArmLog.ViewModels;

namespace FitArmLog.Views;

public partial class RoutineFormPage : ContentPage
{
    private readonly RoutineFormViewModel _viewModel;

    public RoutineFormPage(RoutineFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadExercisesCommand.ExecuteAsync(null);
    }
}