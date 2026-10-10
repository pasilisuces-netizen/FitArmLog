using System.Text.Json.Serialization;

namespace FitArmLog.Models;

// Una serie individual dentro de una sesion de entrenamiento
public class SetEntry
{
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    public int SetNumber { get; set; }
    public int Reps { get; set; }
    public double WeightKg { get; set; }

    // Propiedades calculadas: no se guardan en el JSON
    [JsonIgnore]
    public double Volume => Reps * WeightKg;

    [JsonIgnore]
    public string Summary => $"{ExerciseName} - Serie {SetNumber}: {Reps} x {WeightKg:0.##} kg";
}
