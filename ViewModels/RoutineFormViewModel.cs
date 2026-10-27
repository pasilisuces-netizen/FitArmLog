using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;
using System.Collections.ObjectModel;

namespace FitArmLog.ViewModels;

public partial class RoutineFormViewModel : ObservableObject
{
    private readonly IExerciseApiService _apiService;
    private readonly IRoutineService _routineService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string routineName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<SelectableExercise> availableExercises = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string successMessage = string.Empty;

    public RoutineFormViewModel(
        IExerciseApiService apiService,
        IRoutineService routineService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _routineService = routineService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task LoadExercisesAsync()
    {
        IsBusy = true;

        var result = await _apiService.GetExercisesAsync();
        if (result.IsSuccess && result.Value is not null)
        {
            AvailableExercises = new ObservableCollection<SelectableExercise>(
                result.Value.Select(e => new SelectableExercise(e)));
        }
        else
        {
            ErrorMessage = result.ErrorMessage ?? "No se pudieron cargar los ejercicios.";
        }

        IsBusy = false;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        var selected = AvailableExercises
            .Where(e => e.IsSelected)
            .Select(e => e.Exercise)
            .ToList();

        // La validacion y el guardado viven en RoutineService (capa de aplicacion)
        var result = await _routineService.CreateAsync(RoutineName, selected);
        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "No se pudo guardar la rutina.";
            return;
        }

        SuccessMessage = $"¡Rutina '{result.Value!.Name}' guardada!";
        await NotificationHelper.ShowToastAsync(SuccessMessage);

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await _navigationService.GoBackAsync();
        });
    }
}
