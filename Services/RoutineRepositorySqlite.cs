using System.Text.Json;
using FitArmLog.Data;
using FitArmLog.Models;
using SQLite;

namespace FitArmLog.Services;

public class RoutineRepositorySqlite : IRoutineRepository
{
    private readonly SQLiteAsyncConnection _db;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    
    public RoutineRepositorySqlite(string dbPath)
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
            await _db.CreateTableAsync<RoutineEntity>();
            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<List<Routine>> GetAllAsync()
    {
        await InitAsync();
        var entities = await _db.Table<RoutineEntity>().ToListAsync();
        return entities.Select(ToModel).ToList();
    }

    public async Task<Routine?> GetByIdAsync(int id)
    {
        await InitAsync();
        var entity = await _db.Table<RoutineEntity>()
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync();
        return entity is null ? null : ToModel(entity);
    }

    public async Task AddAsync(Routine routine)
    {
        await InitAsync();
        var entity = ToEntity(routine);
        await _db.InsertAsync(entity);
        routine.Id = entity.Id;
    }

    public async Task DeleteAsync(int id)
    {
        await InitAsync();
        await _db.DeleteAsync<RoutineEntity>(id);
    }

    private static Routine ToModel(RoutineEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        CreatedAt = e.CreatedAt,
        Exercises = JsonSerializer.Deserialize<List<Exercise>>(e.ExercisesJson) ?? new()
    };

    private static RoutineEntity ToEntity(Routine r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        CreatedAt = r.CreatedAt,
        ExercisesJson = JsonSerializer.Serialize(r.Exercises)
    };
}
