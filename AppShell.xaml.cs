using FitArmLog.Views;

namespace FitArmLog;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(ExerciseDetailPage), typeof(ExerciseDetailPage));
    }
}