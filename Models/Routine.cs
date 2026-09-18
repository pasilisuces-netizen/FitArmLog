using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitArmLog.Models;
// Una rutina armada por el usuario compuesta por varios ejercicios
public class Routine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<Exercise> Exercises { get; set; } = new();
}