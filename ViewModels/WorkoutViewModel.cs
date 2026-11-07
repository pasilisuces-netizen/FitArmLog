using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;
using FitArmLog.Services.Device;
using FitArmLog.Validation;

namespace FitArmLog.ViewModels;

public partial class WorkoutViewModel : ObservableObject, IQueryAttributable
{
    private readonly IWorkoutService _workoutService;
    private readonly INavigationService _navigationService;
    private readonly IPhotoService _photoService;
    private readonly ILocationService _locationService;
    private readonly IHapticService _haptic;
    private readonly IShakeDetector _shakeDetector;
    private readonly ISettingsService _settings;

    private DateTime _lastShake = DateTime.MinValue;

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
    [NotifyPropertyChangedFor(nameof(HasPhoto))]
    private string photoPath = string.Empty;

    [ObservableProperty]
    private bool saveLocation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasInfo))]
    private string infoMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool HasInfo => !string.IsNullOrEmpty(InfoMessage);
    public bool HasPhoto => !string.IsNullOrEmpty(PhotoPath);
    public bool IsNotBusy => !IsBusy;

    public ObservableCollection<SetEntry> Sets { get; } = new();

    public WorkoutViewModel(
        IWorkoutService workoutService,
        INavigationService navigationService,
        IPhotoService photoService,
        ILocationService locationService,
        IHapticService haptic,
        IShakeDetector shakeDetector,
        ISettingsService settings)
    {
        _workoutService = workoutService;
        _navigationService = navigationService;
        _photoService = photoService;
        _locationService = locationService;
        _haptic = haptic;
        _shakeDetector = shakeDetector;
        _settings = settings;

        // El interruptor de ubicacion arranca como lo dejo el usuario en Ajustes
        saveLocation = settings.SaveLocationByDefault;
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

    // ---------------- Acelerometro (sacudir = quitar la ultima serie) ----------------

    // La pagina llama a estos dos metodos en OnAppearing / OnDisappearing
    public void StartSensors()
    {
        // Si el usuario lo desactivo en Ajustes, el acelerometro ni se enciende
        if (!_settings.ShakeToUndoEnabled) return;

        _shakeDetector.ShakeDetected -= OnShakeDetected;   // evita suscripciones duplicadas
        _shakeDetector.ShakeDetected += OnShakeDetected;
        _shakeDetector.Start();
    }

    public void StopSensors()
    {
        _shakeDetector.ShakeDetected -= OnShakeDetected;
        _shakeDetector.Stop();
    }

    private void OnShakeDetected(object? sender, EventArgs e)
    {
        // El sensor avisa desde otro hilo: hay que volver al hilo de la interfaz
        MainThread.BeginInvokeOnMainThread(UndoLastSet);
    }

    private void UndoLastSet()
    {
        // Un solo "deshacer" por segundo, aunque el sensor dispare varias veces
        if ((DateTime.Now - _lastShake).TotalSeconds < 1) return;
        _lastShake = DateTime.Now;

        if (Sets.Count == 0) return;

        Sets.RemoveAt(Sets.Count - 1);
        _haptic.Tick();
        InfoMessage = "Se quitó la última serie (sacudida detectada).";
    }

    // ---------------- Series ----------------

    [RelayCommand]
    private void AddSet()
    {
        ErrorMessage = string.Empty;
        InfoMessage = string.Empty;

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

        _haptic.Tick();               // confirmacion tactil
        RepsText = string.Empty;      // el peso queda para cargar la siguiente serie mas rapido
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

    // ---------------- Camara ----------------

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        ErrorMessage = string.Empty;
        InfoMessage = string.Empty;

        var result = await _photoService.TakeWorkoutPhotoAsync();
        if (result.IsSuccess && result.Value is not null)
            PhotoPath = result.Value;
        else
            InfoMessage = result.ErrorMessage ?? string.Empty;
    }

    [RelayCommand]
    private void RemovePhoto() => PhotoPath = string.Empty;

    // ---------------- Guardar ----------------

    [RelayCommand]
    private async Task FinishAsync()
    {
        ErrorMessage = string.Empty;
        InfoMessage = string.Empty;
        IsBusy = true;
        try
        {
            // GPS (opcional): si falla, se guarda igual pero sin ubicacion
            double? latitude = null;
            double? longitude = null;
            if (SaveLocation)
            {
                var location = await _locationService.GetCurrentAsync();
                if (location.IsSuccess && location.Value is not null)
                {
                    latitude = location.Value.Latitude;
                    longitude = location.Value.Longitude;
                }
                else
                {
                    await NotificationHelper.ShowToastAsync(
                        $"{location.ErrorMessage} Se guarda sin ubicación.");
                }
            }

            // La validacion y el guardado viven en WorkoutService (capa de aplicacion)
            var result = await _workoutService.SaveAsync(
                Routine, Sets.ToList(), Notes,
                string.IsNullOrEmpty(PhotoPath) ? null : PhotoPath,
                latitude, longitude);

            if (!result.IsSuccess)
            {
                ErrorMessage = result.ErrorMessage ?? "No se pudo guardar el entrenamiento.";
                return;
            }

            _haptic.Tick();
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
