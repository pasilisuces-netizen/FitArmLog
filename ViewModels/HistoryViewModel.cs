using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;

namespace FitArmLog.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly IWorkoutRepository _workoutRepository;

    [ObservableProperty]
    private ObservableCollection<WorkoutSession> sessions = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public HistoryViewModel(IWorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }

    [RelayCommand]
    private async Task LoadSessionsAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var list = await _workoutRepository.GetAllAsync();
            Sessions = new ObservableCollection<WorkoutSession>(list);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo cargar el historial: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[History] ERROR: {ex}");
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

        try
        {
            await _workoutRepository.DeleteAsync(session.Id);
            await LoadSessionsAsync();
            await NotificationHelper.ShowToastAsync("Entrenamiento eliminado.");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo eliminar: {ex.Message}";
        }
    }
}
