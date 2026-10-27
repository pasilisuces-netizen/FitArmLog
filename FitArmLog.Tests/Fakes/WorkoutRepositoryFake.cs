using FitArmLog.Models;
using FitArmLog.Services;

namespace FitArmLog.Tests.Fakes;

public class WorkoutRepositoryFake : IWorkoutRepository
{
    private readonly List<WorkoutSession> _sessions = new();
    private int _nextId = 1;

    public bool ThrowOnAdd { get; set; }

    public Task<List<WorkoutSession>> GetAllAsync() =>
        Task.FromResult(_sessions.OrderByDescending(s => s.Date).ToList());

    public Task AddAsync(WorkoutSession session)
    {
        if (ThrowOnAdd)
            throw new InvalidOperationException("fallo simulado de la base de datos");

        session.Id = _nextId++;
        _sessions.Add(session);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _sessions.RemoveAll(s => s.Id == id);
        return Task.CompletedTask;
    }
}
