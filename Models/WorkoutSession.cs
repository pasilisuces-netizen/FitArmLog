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

    public int TotalSets => Sets.Count;
    public double TotalVolume => Sets.Sum(s => s.Volume);
}
