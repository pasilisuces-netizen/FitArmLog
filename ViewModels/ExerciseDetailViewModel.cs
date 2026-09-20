using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;

namespace FitArmLog.ViewModels;


public partial class ExerciseDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Exercise? exercise;

    public ExerciseDetailViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

   
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        
        if (query.TryGetValue("Exercise", out var value) && value is Exercise receivedExercise)
        {
            Exercise = receivedExercise;
        }
        else
        {
            HandleInvalidParameters();
        }
    }

   
    private async void HandleInvalidParameters()
    {
        await Toast.Make("No se pudo cargar el ejercicio seleccionado.").Show();
        await _navigationService.GoBackAsync();
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await _navigationService.GoBackAsync();
    }
}