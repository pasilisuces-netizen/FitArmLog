using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitArmLog.Models;

public class Exercise
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty; 
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;    
}
