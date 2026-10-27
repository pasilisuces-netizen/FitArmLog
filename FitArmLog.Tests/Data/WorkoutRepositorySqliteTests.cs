using FitArmLog.Models;
using FitArmLog.Services;
using Xunit;

namespace FitArmLog.Tests.Data;

[Collection("SqliteMemoria")]
public class WorkoutRepositorySqliteTests : IAsyncLifetime
{
    private readonly WorkoutRepositorySqlite _repo = new(":memory:");

    public async Task InitializeAsync()
    {
        foreach (var session in await _repo.GetAllAsync())
            await _repo.DeleteAsync(session.Id);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private static WorkoutSession NewSession(string routineName, DateTime date) => new()
    {
        RoutineId = 1,
        RoutineName = routineName,
        Date = date,
        Notes = "notas",
        Sets = new List<SetEntry>
        {
            new() { ExerciseId = "a", ExerciseName = "Press", SetNumber = 1, Reps = 10, WeightKg = 40 },
            new() { ExerciseId = "a", ExerciseName = "Press", SetNumber = 2, Reps = 8, WeightKg = 42.5 }
        }
    };

    [Fact]
    public async Task AddAsync_LuegoGetAll_DevuelveLaSesionConSusSeries()
    {
        await _repo.AddAsync(NewSession("Pecho", new DateTime(2026, 10, 1, 18, 30, 0)));

        var all = await _repo.GetAllAsync();

        Assert.Single(all);
        Assert.Equal("Pecho", all[0].RoutineName);
        Assert.Equal("notas", all[0].Notes);
        Assert.Equal(2, all[0].Sets.Count);
        Assert.Equal(42.5, all[0].Sets[1].WeightKg);
        Assert.Equal(new DateTime(2026, 10, 1, 18, 30, 0), all[0].Date);
    }

    [Fact]
    public async Task AddAsync_LasPropiedadesCalculadasSeRecalculanAlLeer()
    {
        await _repo.AddAsync(NewSession("Pecho", DateTime.Now));

        var session = (await _repo.GetAllAsync()).Single();

        Assert.Equal(2, session.TotalSets);
        Assert.Equal(740, session.TotalVolume);   // 10*40 + 8*42.5
    }

    [Fact]
    public async Task GetAllAsync_DevuelveLasMasRecientesPrimero()
    {
        await _repo.AddAsync(NewSession("Vieja", new DateTime(2026, 9, 1)));
        await _repo.AddAsync(NewSession("Nueva", new DateTime(2026, 10, 1)));

        var all = await _repo.GetAllAsync();

        Assert.Equal("Nueva", all[0].RoutineName);
        Assert.Equal("Vieja", all[1].RoutineName);
    }

    [Fact]
    public async Task DeleteAsync_EliminaLaSesion()
    {
        var session = NewSession("Pecho", DateTime.Now);
        await _repo.AddAsync(session);

        await _repo.DeleteAsync(session.Id);

        Assert.Empty(await _repo.GetAllAsync());
    }
}
