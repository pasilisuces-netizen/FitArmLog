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
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<IExerciseApiService, ExerciseApiService>();

        builder.Services.AddTransient<ExerciseListViewModel>();
        builder.Services.AddTransient<ExerciseListPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}