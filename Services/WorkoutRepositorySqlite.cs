using System.Text.Json;
using FitArmLog.Data;
using FitArmLog.Models;
using SQLite;

namespace FitArmLog.Services;

public class WorkoutRepositorySqlite : IWorkoutRepository
{
    private readonly SQLiteAsyncConnection _db;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public WorkoutRepositorySqlite(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
    }

    private async Task InitAsync()
    {
        if (_initialized) return;
        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return;
            await _db.CreateTableAsync<WorkoutSessionEntity>();
            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    // Mas recientes primero
    public async Task<List<WorkoutSession>> GetAllAsync()
    {
        await InitAsync();
        var entities = await _db.Table<WorkoutSessionEntity>()
            .OrderByDescending(s => s.Date)
            .ToListAsync();
        return entities.Select(ToModel).ToList();
    }

    public async Task AddAsync(WorkoutSession session)
    {
        await InitAsync();
        var entity = ToEntity(session);
        await _db.InsertAsync(entity);
        session.Id = entity.Id;
    }

    public async Task DeleteAsync(int id)
    {
        await InitAsync();
        await _db.DeleteAsync<WorkoutSessionEntity>(id);
    }

    private static WorkoutSession ToModel(WorkoutSessionEntity e) => new()
    {
        Id = e.Id,
        RoutineId = e.RoutineId,
        RoutineName = e.RoutineName,
        Date = e.Date,
        Notes = e.Notes,
        Sets = JsonSerializer.Deserialize<List<SetEntry>>(e.SetsJson) ?? new()
    };

    private static WorkoutSessionEntity ToEntity(WorkoutSession s) => new()
    {
        Id = s.Id,
        RoutineId = s.RoutineId,
        RoutineName = s.RoutineName,
        Date = s.Date,
        Notes = s.Notes,
        SetsJson = JsonSerializer.Serialize(s.Sets)
    };
}
