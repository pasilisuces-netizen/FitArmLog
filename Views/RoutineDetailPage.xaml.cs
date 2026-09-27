using FitArmLog.ViewModels;

namespace FitArmLog.Views;

public partial class RoutineDetailPage : ContentPage
{
    public RoutineDetailPage(RoutineDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}