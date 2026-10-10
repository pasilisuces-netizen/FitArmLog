using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;
using FitArmLog.Views;

namespace FitArmLog.ViewModels;

public partial class RoutineDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Routine? routine;

    public RoutineDetailViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Routine", out var value) && value is Routine received)
        {
            Routine = received;
        }
        else
        {
            HandleInvalidParameters();
        }
    }

    private async void HandleInvalidParameters()
    {
        await NotificationHelper.ShowToastAsync("No se pudo cargar la rutina seleccionada.");
        await _navigationService.GoBackAsync();
    }

    [RelayCommand]
    private async Task StartWorkoutAsync()
    {
        if (Routine is null) return;

        var parameters = new Dictionary<string, object> { { "Routine", Routine } };
        await _navigationService.GoToAsync(nameof(WorkoutPage), parameters);
    }

    [RelayCommand]
    private async Task GoBackAsync() => await _navigationService.GoBackAsync();
}