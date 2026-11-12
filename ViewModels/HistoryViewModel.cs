using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;

namespace FitArmLog.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    // La VM solo conoce el servicio (capa de aplicacion), nunca el repositorio.
    private readonly IWorkoutService _workoutService;

    [ObservableProperty]
    private ObservableCollection<WorkoutSession> sessions = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public HistoryViewModel(IWorkoutService workoutService)
    {
        _workoutService = workoutService;
    }

    [RelayCommand]
    private async Task LoadSessionsAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var result = await _workoutService.GetAllAsync();
            if (result.IsSuccess)
                Sessions = new ObservableCollection<WorkoutSession>(result.Value!);
            else
                ErrorMessage = result.ErrorMessage ?? "No se pudo cargar el historial.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteSessionAsync(WorkoutSession? session)
    {
        if (session is null) return;

        var result = await _workoutService.DeleteAsync(session.Id);
        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "No se pudo eliminar el entrenamiento.";
            return;
        }

        await LoadSessionsAsync();
        await NotificationHelper.ShowToastAsync("Entrenamiento eliminado.");
    }
}
