using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Models;
using FitArmLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FitArmLog.ViewModels;

public partial class ExerciseListViewModel : ObservableObject
{
    private readonly IExerciseApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Exercise> exercises = new();

    [ObservableProperty]
    private bool isBusy;

    
    [ObservableProperty]
    private string errorMessage = string.Empty;

    
    public ExerciseListViewModel(IExerciseApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadExercisesAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        var result = await _apiService.GetExercisesAsync();

        if (result.IsSuccess && result.Value is not null)
        {
            Exercises = new ObservableCollection<Exercise>(result.Value);
        }
        else
        {
            ErrorMessage = result.ErrorMessage ?? "No se pudieron cargar los ejercicios.";
        }

        IsBusy = false;
    }
}