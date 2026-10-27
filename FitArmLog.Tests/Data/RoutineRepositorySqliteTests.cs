using FitArmLog.Models;
using FitArmLog.Services;
using Xunit;

namespace FitArmLog.Tests.Data;

// Tests de integracion con SQLite REAL pero en memoria (":memory:"):
// no crea archivos y desaparece al terminar. Probamos el repositorio de verdad,
// incluyendo la conversion Routine <-> RoutineEntity y el JSON de ejercicios.
//
// La coleccion hace que estos tests corran de a uno (no en paralelo) y cada test
// arranca con la tabla vacia, para que no se pisen entre si.
[Collection("SqliteMemoria")]
public class RoutineRepositorySqliteTests : IAsyncLifetime
{
    private readonly RoutineRepositorySqlite _repo = new(":memory:");

    public async Task InitializeAsync()
    {
        foreach (var routine in await _repo.GetAllAsync())
            await _repo.DeleteAsync(routine.Id);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private static Routine NewRoutine(string name, params string[] exerciseNames) => new()
    {
        Name = name,
        Exercises = exerciseNames.Select(n => new Exercise { Name = n, MuscleGroup = "chest" }).ToList()
    };

    [Fact]
    public async Task GetAllAsync_BaseVacia_DevuelveListaVacia()
    {
        var all = await _repo.GetAllAsync();

        Assert.Empty(all);
    }

    [Fact]
    public async Task AddAsync_AsignaIdGeneradoPorSqlite()
    {
        var routine = NewRoutine("Pecho", "Press");

        await _repo.AddAsync(routine);

        Assert.True(routine.Id > 0);
    }

    [Fact]
    public async Task AddAsync_LuegoGetAll_DevuelveLaRutinaConSusEjercicios()
    {
        await _repo.AddAsync(NewRoutine("Pecho", "Press banca", "Aperturas"));

        var all = await _repo.GetAllAsync();

        Assert.Single(all);
        Assert.Equal("Pecho", all[0].Name);
        Assert.Equal(2, all[0].Exercises.Count);
        Assert.Equal("Press banca", all[0].Exercises[0].Name);
        Assert.Equal("Aperturas", all[0].Exercises[1].Name);
    }

    [Fact]
    public async Task GetByIdAsync_Existente_DevuelveLaRutina()
    {
        var routine = NewRoutine("Espalda", "Remo");
        await _repo.AddAsync(routine);

        var found = await _repo.GetByIdAsync(routine.Id);

        Assert.NotNull(found);
        Assert.Equal("Espalda", found!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Inexistente_DevuelveNull()
    {
        var found = await _repo.GetByIdAsync(999999);

        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteAsync_EliminaSoloLaRutinaIndicada()
    {
        var a = NewRoutine("A", "Press");
        var b = NewRoutine("B", "Remo");
        await _repo.AddAsync(a);
        await _repo.AddAsync(b);

        await _repo.DeleteAsync(a.Id);

        var all = await _repo.GetAllAsync();
        Assert.Single(all);
        Assert.Equal("B", all[0].Name);
    }
}
