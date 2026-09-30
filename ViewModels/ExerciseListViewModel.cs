using System.Collections.ObjectModel;
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

    // Mensaje de estado (distinto del error): informa qué está pasando
    // durante la carga, sin ser necesariamente un problema.
    [ObservableProperty]
    private string statusMessage = string.Empty;

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
        StatusMessage = "Cargando ejercicios...";

        var result = await _apiService.GetExercisesAsync();

        if (result.IsSuccess && result.Value is not null)
        {
            Exercises = new ObservableCollection<Exercise>(result.Value);
            StatusMessage = $"{Exercises.Count} ejercicios cargados.";
            await NotificationHelper.ShowToastAsync(StatusMessage);
        }
        else
        {
            StatusMessage = string.Empty;
            ErrorMessage = result.ErrorMessage ?? "No se pudieron cargar los ejercicios.";
        }

        IsBusy = false;
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Exercise? exercise)
    {
        if (exercise is null)
        {
            await NotificationHelper.ShowToastAsync("No se pudo abrir el detalle de este ejercicio.");
            return;
        }

        var parameters = new Dictionary<string, object>
        {
            { "Exercise", exercise }
        };

        await _navigationService.GoToAsync(nameof(ExerciseDetailPage), parameters);
    }
}