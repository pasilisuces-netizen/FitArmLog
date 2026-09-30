using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;
using System.Collections.ObjectModel;

namespace FitArmLog.ViewModels;

public partial class RoutineFormViewModel : ObservableObject
{
    private readonly IExerciseApiService _apiService;
    private readonly IRoutineRepository _routineRepository;
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
        IRoutineRepository routineRepository,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _routineRepository = routineRepository;
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

        if (string.IsNullOrWhiteSpace(RoutineName))
        {
            ErrorMessage = "Ingresá un nombre para la rutina.";
            return;
        }

        var selected = AvailableExercises
            .Where(e => e.IsSelected)
            .Select(e => e.Exercise)
            .ToList();

        if (selected.Count == 0)
        {
            ErrorMessage = "Seleccioná al menos un ejercicio.";
            return;
        }

        try
        {
            var routine = new Routine
            {
                Name = RoutineName.Trim(),
                Exercises = selected
            };

            await _routineRepository.AddAsync(routine);

            SuccessMessage = $"¡Rutina '{routine.Name}' guardada!";
            await NotificationHelper.ShowToastAsync(SuccessMessage);

            
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _navigationService.GoBackAsync();
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo guardar la rutina: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[RoutineForm] ERROR: {ex}");
        }
    }
}