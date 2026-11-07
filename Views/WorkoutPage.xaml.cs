using FitArmLog.ViewModels;

namespace FitArmLog.Views;

public partial class WorkoutPage : ContentPage
{
    private readonly WorkoutViewModel _viewModel;

    public WorkoutPage(WorkoutViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    // El acelerometro solo debe estar prendido mientras esta pantalla es visible
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.StartSensors();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopSensors();
    }
}
