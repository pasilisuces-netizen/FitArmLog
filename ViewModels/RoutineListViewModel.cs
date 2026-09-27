using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;
using FitArmLog.Views;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace FitArmLog.ViewModels;

public partial class RoutineListViewModel : ObservableObject
{
    private readonly IRoutineRepository _routineRepository;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Routine> routines = new();

    [ObservableProperty]
    private bool isBusy;

    public RoutineListViewModel(IRoutineRepository routineRepository, INavigationService navigationService)
    {
        _routineRepository = routineRepository;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task LoadRoutinesAsync()
    {
        IsBusy = true;
        var list = await _routineRepository.GetAllAsync();
        Routines = new ObservableCollection<Routine>(list);
        IsBusy = false;
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

        await _routineRepository.DeleteAsync(routine.Id);
        await LoadRoutinesAsync();
        await NotificationHelper.ShowToastAsync($"Rutina '{routine.Name}' eliminada.");
    }
}