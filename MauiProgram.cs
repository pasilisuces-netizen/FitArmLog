using CommunityToolkit.Maui;
using FFImageLoading.Maui;
using FitArmLog.Services;
using FitArmLog.ViewModels;
using FitArmLog.Views;
using Microsoft.Extensions.Logging;

namespace FitArmLog;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseFFImageLoading()
            .UseMauiCommunityToolkit()   
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton(sp => new HttpClient { Timeout = TimeSpan.FromSeconds(10) });
        builder.Services.AddSingleton<IExerciseApiService, ExerciseApiService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "fitarmlog.db3");
        builder.Services.AddSingleton<IRoutineRepository>(sp => new RoutineRepositorySqlite(dbPath));
        builder.Services.AddSingleton<IWorkoutRepository>(sp => new WorkoutRepositorySqlite(dbPath));

        builder.Services.AddTransient<ExerciseListViewModel>();
        builder.Services.AddTransient<ExerciseListPage>();
        builder.Services.AddTransient<ExerciseDetailViewModel>();
        builder.Services.AddTransient<ExerciseDetailPage>();

        builder.Services.AddTransient<RoutineListViewModel>();
        builder.Services.AddTransient<RoutineListPage>();
        builder.Services.AddTransient<RoutineFormViewModel>();
        builder.Services.AddTransient<RoutineFormPage>();
        builder.Services.AddTransient<RoutineDetailViewModel>();
        builder.Services.AddTransient<RoutineDetailPage>();

        builder.Services.AddTransient<WorkoutViewModel>();
        builder.Services.AddTransient<WorkoutPage>();
        builder.Services.AddTransient<HistoryViewModel>();
        builder.Services.AddTransient<HistoryPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}