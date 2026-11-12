using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;
using FitArmLog.Views;
using System.Collections.ObjectModel;

namespace FitArmLog.ViewModels;

public partial class RoutineListViewModel : ObservableObject
{
    // La VM solo conoce el servicio (capa de aplicacion), nunca el repositorio.
    private readonly IRoutineService _routineService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Routine> routines = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public RoutineListViewModel(IRoutineService routineService, INavigationService navigationService)
    {
        _routineService = routineService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task LoadRoutinesAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var result = await _routineService.GetAllAsync();
            if (result.IsSuccess)
                Routines = new ObservableCollection<Routine>(result.Value!);
            else
                ErrorMessage = result.ErrorMessage ?? "No se pudieron cargar las rutinas.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await _navigationService.GoToAsync(nameof(RoutineFormPage));
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Routine? routine)
    {
        if (routine is null)
        {
            await NotificationHelper.ShowToastAsync("No se pudo abrir la rutina.");
            return;
        }

        var parameters = new Dictionary<string, object> { { "Routine", routine } };
        await _navigationService.GoToAsync(nameof(RoutineDetailPage), parameters);
    }

    [RelayCommand]
    private async Task DeleteRoutineAsync(Routine? routine)
    {
        if (routine is null) return;

        var result = await _routineService.DeleteAsync(routine.Id);
        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "No se pudo eliminar la rutina.";
            await NotificationHelper.ShowToastAsync(ErrorMessage);
            return;
        }

        await LoadRoutinesAsync();
        await NotificationHelper.ShowToastAsync($"Rutina '{routine.Name}' eliminada.");
    }
}
