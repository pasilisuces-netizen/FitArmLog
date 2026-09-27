using FitArmLog.Models;

namespace FitArmLog.Services;

public interface IRoutineRepository
{
    Task<List<Routine>> GetAllAsync();
    Task<Routine?> GetByIdAsync(int id);
    Task AddAsync(Routine routine);
    Task DeleteAsync(int id);
}