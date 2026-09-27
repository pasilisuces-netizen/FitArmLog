using FitArmLog.Models;

namespace FitArmLog.Services;

// Implementación EN MEMORIA: los datos viven mientras la app está abierta
// y se pierden al cerrarla. 
public class InMemoryRoutineRepository : IRoutineRepository
{
    private readonly List<Routine> _routines = new();
    private int _nextId = 1;

    public Task<List<Routine>> GetAllAsync() =>
        Task.FromResult(_routines.ToList());

    public Task<Routine?> GetByIdAsync(int id) =>
        Task.FromResult(_routines.FirstOrDefault(r => r.Id == id));

    public Task AddAsync(Routine routine)
    {
        routine.Id = _nextId++;
        _routines.Add(routine);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var routine = _routines.FirstOrDefault(r => r.Id == id);
        if (routine is not null)
            _routines.Remove(routine);

        return Task.CompletedTask;
    }
}