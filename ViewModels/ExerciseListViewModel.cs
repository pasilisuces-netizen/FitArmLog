using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;
using FitArmLog.Views;

namespace FitArmLog.ViewModels;

public partial class ExerciseListViewModel : ObservableObject
{
    private readonly IExerciseApiService _apiService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Exercise> exercises = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ExerciseListViewModel(IExerciseApiService apiService, INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task LoadExercisesAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        var result = await _apiService.GetExercisesAsync();

        if (result.IsSuccess && result.Value is not null)
        {
            Exercises = new ObservableCollection<Exercise>(result.Value);
            await NotificationHelper.ShowToastAsync($"Se cargaron {Exercises.Count} ejercicios.");
        }
        else
        {
            ErrorMessage = result.ErrorMessage ?? "No se pudieron cargar los ejercicios.";
        }

        IsBusy = false;
    }

 
    [RelayCommand]
    private async Task GoToDetailAsync(Exercise? exercise)
    {
        if (exercise is null)
        {
            await Toast.Make("No se pudo abrir el detalle de este ejercicio.").Show();
            return;
        }

        var parameters = new Dictionary<string, object>
        {
            { "Exercise", exercise }
        };

        await _navigationService.GoToAsync(nameof(ExerciseDetailPage), parameters);
    }
}