using FitArmLog.ViewModels;

namespace FitArmLog.Views;

public partial class ExerciseListPage : ContentPage
{
    private readonly ExerciseListViewModel _viewModel;

    public ExerciseListPage(ExerciseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadExercisesCommand.Execute(null);
    }
}