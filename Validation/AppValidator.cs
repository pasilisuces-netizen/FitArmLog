using System.Globalization;
using FitArmLog.Models;

namespace FitArmLog.Validation;

// Todas las reglas de validacion de la app
public static class AppValidator
{
    public const int MaxRoutineNameLength = 100;
    public const int MaxReps = 100;
    public const double MaxWeightKg = 500;
    public const int MaxNotesLength = 500;

    public static ValidationResult ValidateRoutine(string? name, int selectedExercises)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(name))
            result.Add("Ingresá un nombre para la rutina.");
        else if (name.Trim().Length > MaxRoutineNameLength)
            result.Add($"El nombre no puede superar los {MaxRoutineNameLength} caracteres.");

        if (selectedExercises <= 0)
            result.Add("Seleccioná al menos un ejercicio.");

        return result;
    }

    public static ValidationResult ValidateSet(
        Exercise? exercise, string? repsText, string? weightText,
        out int reps, out double weight)
    {
        var result = new ValidationResult();
        reps = 0;
        weight = 0;

        if (exercise is null)
            result.Add("Elegí un ejercicio.");

        if (!int.TryParse(repsText?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out reps)
            || reps < 1 || reps > MaxReps)
            result.Add($"Las repeticiones deben ser un número entre 1 y {MaxReps}.");

        if (!TryParseWeight(weightText, out weight) || weight < 0 || weight > MaxWeightKg)
            result.Add($"El peso debe ser un número entre 0 y {MaxWeightKg:0} kg.");

        return result;
    }

    public static ValidationResult ValidateSession(IReadOnlyCollection<SetEntry> sets, string? notes)
    {
        var result = new ValidationResult();

        if (sets.Count == 0)
            result.Add("Agregá al menos una serie antes de guardar.");

        if (notes is not null && notes.Trim().Length > MaxNotesLength)
            result.Add($"Las notas no pueden superar los {MaxNotesLength} caracteres.");

        return result;
    }

    public static bool TryParseWeight(string? text, out double weight)
    {
        weight = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;
        var normalized = text.Trim().Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out weight);
    }
}
