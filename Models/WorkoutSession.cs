using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitArmLog.Models;
// Una sesion de entrenamiento registrada por el usuario .
public class WorkoutSession
{
    public int Id { get; set; }
    public int RoutineId { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public List<SetEntry> Sets { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}
