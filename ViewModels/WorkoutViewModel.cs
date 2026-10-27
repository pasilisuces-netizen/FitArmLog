using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;
using FitArmLog.Validation;

namespace FitArmLog.ViewModels;

public partial class WorkoutViewModel : ObservableObject, IQueryAttributable
{
    private readonly IWorkoutService _workoutService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Routine? routine;

    [ObservableProperty]
    private Exercise? selectedExercise;

    [ObservableProperty]
    private string repsText = string.Empty;

    [ObservableProperty]
    private string weightText = string.Empty;

    [ObservableProperty]
    private string notes = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool IsNotBusy => !IsBusy;

    public ObservableCollection<SetEntry> Sets { get; } = new();

    public WorkoutViewModel(IWorkoutService workoutService, INavigationService navigationService)
    {
        _workoutService = workoutService;
        _navigationService = navigationService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Routine", out var value) && value is Routine received)
        {
            Routine = received;
            SelectedExercise = received.Exercises.FirstOrDefault();
        }
        else
        {
            HandleInvalidParameters();
        }
    }

    private async void HandleInvalidParameters()
    {
        await NotificationHelper.ShowToastAsync("No se pudo cargar la rutina para entrenar.");
        await _navigationService.GoBackAsync();
    }

    [RelayCommand]
    private void AddSet()
    {
        ErrorMessage = string.Empty;

        var validation = AppValidator.ValidateSet(SelectedExercise, RepsText, WeightText,
            out var reps, out var weight);
        if (!validation.IsValid)
        {
            ErrorMessage = validation.FirstError;
            return;
        }

        var exercise = SelectedExercise!;
        Sets.Add(new SetEntry
        {
            ExerciseId = exercise.Id,
            ExerciseName = exercise.Name,
            SetNumber = Sets.Count(s => s.ExerciseId == exercise.Id) + 1,
            Reps = reps,
            WeightKg = weight
        });

        RepsText = string.Empty; // el peso queda para cargar la siguiente serie mas rapido
    }

    [RelayCommand]
    private void RemoveSet(SetEntry? set)
    {
        if (set is null) return;
        Sets.Remove(set);
        RenumberSets();
    }

    // Despues de borrar una serie, vuelve a numerar las de cada ejercicio (1, 2, 3...)
    private void RenumberSets()
    {
        var counters = new Dictionary<string, int>();
        var renumbered = new List<SetEntry>();

        foreach (var s in Sets)
        {
            counters[s.ExerciseId] = counters.GetValueOrDefault(s.ExerciseId) + 1;
            renumbered.Add(new SetEntry
            {
                ExerciseId = s.ExerciseId,
                ExerciseName = s.ExerciseName,
                SetNumber = counters[s.ExerciseId],
                Reps = s.Reps,
                WeightKg = s.WeightKg
            });
        }

        Sets.Clear();
        foreach (var s in renumbered) Sets.Add(s);
    }

    [RelayCommand]
    private async Task FinishAsync()
    {
        ErrorMessage = string.Empty;
        IsBusy = true;
        try
        {
            // La validacion y el guardado viven en WorkoutService (capa de aplicacion)
            var result = await _workoutService.SaveAsync(Routine, Sets.ToList(), Notes);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.ErrorMessage ?? "No se pudo guardar el entrenamiento.";
                return;
            }

            await NotificationHelper.ShowToastAsync("¡Entrenamiento guardado! Lo ves en Historial.");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _navigationService.GoBackAsync();
            });
        }
        finally
        {
            IsBusy = false;
        }
    }
}
