using System.Globalization;

namespace FitArmLog.Models;

// Una sesion de entrenamiento registrada por el usuario
public class WorkoutSession
{
    public int Id { get; set; }
    public int RoutineId { get; set; }
    public string RoutineName { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public List<SetEntry> Sets { get; set; } = new();
    public string Notes { get; set; } = string.Empty;

    // Datos de sensores (opcionales)
    public string? PhotoPath { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public int TotalSets => Sets.Count;
    public double TotalVolume => Sets.Sum(s => s.Volume);

    public bool HasPhoto => !string.IsNullOrEmpty(PhotoPath);
    public bool HasLocation => Latitude.HasValue && Longitude.HasValue;

    public string LocationText => HasLocation
        ? string.Format(CultureInfo.InvariantCulture, "Ubicación: {0:0.0000}, {1:0.0000}", Latitude, Longitude)
        : string.Empty;
}
