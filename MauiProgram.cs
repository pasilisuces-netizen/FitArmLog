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

        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<IExerciseApiService, ExerciseApiService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransient<ExerciseListViewModel>();
        builder.Services.AddTransient<ExerciseListPage>();
        builder.Services.AddTransient<ExerciseDetailViewModel>();
        builder.Services.AddTransient<ExerciseDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}