using FitArmLog.ViewModels;

namespace FitArmLog.Views;

public partial class ExerciseDetailPage : ContentPage
{
    public ExerciseDetailPage(ExerciseDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}