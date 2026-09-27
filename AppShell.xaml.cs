using FitArmLog.Views;

namespace FitArmLog;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ExerciseDetailPage), typeof(ExerciseDetailPage));
        Routing.RegisterRoute(nameof(RoutineFormPage), typeof(RoutineFormPage));
        Routing.RegisterRoute(nameof(RoutineDetailPage), typeof(RoutineDetailPage));
    }
}