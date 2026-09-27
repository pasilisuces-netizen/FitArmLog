using CommunityToolkit.Mvvm.ComponentModel;
using FitArmLog.Models;

namespace FitArmLog.ViewModels;

// Envuelve un Exercise agregándole estado de selección (IsSelected),
// sin modificar el Model original. Solo se usa en la pantalla de
// creación de rutinas, donde el usuario elige varios ejercicios con checkbox.
public partial class SelectableExercise : ObservableObject
{
    public Exercise Exercise { get; }

    [ObservableProperty]
    private bool isSelected;

    public SelectableExercise(Exercise exercise)
    {
        Exercise = exercise;
    }
}