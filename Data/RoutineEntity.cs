using SQLite;

namespace FitArmLog.Data;

[Table("Routines")]
public class RoutineEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    
    public string ExercisesJson { get; set; } = "[]";
}