using FitArmLog.Models;
using FitArmLog.Services;
using FitArmLog.Tests.Fakes;
using Xunit;

namespace FitArmLog.Tests.Services;

// Tests de los metodos de lectura y borrado agregados en la limpieza de capas.
public class ServiceListDeleteTests
{
    private static List<Exercise> OneExercise() => new() { new Exercise { Name = "Remo" } };

    private static List<SetEntry> OneSet() => new()
    {
        new SetEntry { ExerciseId = "a", ExerciseName = "Remo", SetNumber = 1, Reps = 10, WeightKg = 20 }
    };

    // ---------- RoutineService ----------

    [Fact]
    public async Task Routine_GetAllAsync_DevuelveLasRutinasGuardadas()
    {
        var service = new RoutineService(new RoutineRepositoryFake());
        await service.CreateAsync("Pecho", OneExercise());

        var result = await service.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    [Fact]
    public async Task Routine_GetAllAsync_ElRepositorioFalla_DevuelveError()
    {
        var service = new RoutineService(new RoutineRepositoryFake { ThrowOnGet = true });

        var result = await service.GetAllAsync();

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task Routine_DeleteAsync_EliminaLaRutina()
    {
        var fake = new RoutineRepositoryFake();
        var service = new RoutineService(fake);
        var created = await service.CreateAsync("Pecho", OneExercise());

        var result = await service.DeleteAsync(created.Value!.Id);

        Assert.True(result.IsSuccess);
        Assert.Empty(await fake.GetAllAsync());
    }

    [Fact]
    public async Task Routine_DeleteAsync_ElRepositorioFalla_DevuelveError()
    {
        var service = new RoutineService(new RoutineRepositoryFake { ThrowOnDelete = true });

        var result = await service.DeleteAsync(1);

        Assert.False(result.IsSuccess);
    }

    // ---------- WorkoutService ----------

    [Fact]
    public async Task Workout_GetAllAsync_DevuelveLosEntrenamientosGuardados()
    {
        var service = new WorkoutService(new WorkoutRepositoryFake());
        await service.SaveAsync(new Routine { Id = 1, Name = "Pecho" }, OneSet(), "ok");

        var result = await service.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    [Fact]
    public async Task Workout_GetAllAsync_ElRepositorioFalla_DevuelveError()
    {
        var service = new WorkoutService(new WorkoutRepositoryFake { ThrowOnGet = true });

        var result = await service.GetAllAsync();

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Workout_DeleteAsync_EliminaElEntrenamiento()
    {
        var fake = new WorkoutRepositoryFake();
        var service = new WorkoutService(fake);
        var saved = await service.SaveAsync(new Routine { Id = 1, Name = "Pecho" }, OneSet(), null);

        var result = await service.DeleteAsync(saved.Value!.Id);

        Assert.True(result.IsSuccess);
        Assert.Empty(await fake.GetAllAsync());
    }

    [Fact]
    public async Task Workout_DeleteAsync_ElRepositorioFalla_DevuelveError()
    {
        var service = new WorkoutService(new WorkoutRepositoryFake { ThrowOnDelete = true });

        var result = await service.DeleteAsync(1);

        Assert.False(result.IsSuccess);
    }
}
