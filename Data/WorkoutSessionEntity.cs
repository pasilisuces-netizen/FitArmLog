using SQLite;

namespace FitArmLog.Data;

[Table("WorkoutSessions")]
public class WorkoutSessionEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int RoutineId { get; set; }

    [MaxLength(100)]
    public string RoutineName { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string Notes { get; set; } = string.Empty;

    // Las series se guardan como JSON, igual que los ejercicios de las rutinas
    public string SetsJson { get; set; } = "[]";
}
