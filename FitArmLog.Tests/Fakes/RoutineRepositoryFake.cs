using FitArmLog.Models;
using FitArmLog.Services;

namespace FitArmLog.Tests.Fakes;

// Fake: implementacion real pero simple (lista en memoria) para usar en tests.
// A diferencia de un mock, tiene comportamiento propio: lo que guardas, lo podes leer.
public class RoutineRepositoryFake : IRoutineRepository
{
    private readonly List<Routine> _routines = new();
    private int _nextId = 1;

    // Permiten simular fallos de la base de datos
    public bool ThrowOnAdd { get; set; }
    public bool ThrowOnGet { get; set; }
    public bool ThrowOnDelete { get; set; }

    public Task<List<Routine>> GetAllAsync()
    {
        if (ThrowOnGet)
            throw new InvalidOperationException("fallo simulado de la base de datos");
        return Task.FromResult(_routines.ToList());
    }

    public Task<Routine?> GetByIdAsync(int id) =>
        Task.FromResult(_routines.FirstOrDefault(r => r.Id == id));

    public Task AddAsync(Routine routine)
    {
        if (ThrowOnAdd)
            throw new InvalidOperationException("fallo simulado de la base de datos");

        routine.Id = _nextId++;
        _routines.Add(routine);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        if (ThrowOnDelete)
            throw new InvalidOperationException("fallo simulado de la base de datos");
        _routines.RemoveAll(r => r.Id == id);
        return Task.CompletedTask;
    }
}
