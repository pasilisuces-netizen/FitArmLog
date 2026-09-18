using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitArmLog.Models;
// Una serie individual dentro de una sesion de entrenamiento
public class SetEntry
{
    public int Id { get; set; }
    public int ExerciseId { get; set; }
    public int SetNumber { get; set; }
    public int Reps { get; set; }
    public double WeightKg { get; set; }
}
