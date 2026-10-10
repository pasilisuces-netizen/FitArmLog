using FitArmLog.Models;

namespace FitArmLog.Services;

public interface IWorkoutRepository
{
    Task<List<WorkoutSession>> GetAllAsync();
    Task AddAsync(WorkoutSession session);
    Task DeleteAsync(int id);
}
