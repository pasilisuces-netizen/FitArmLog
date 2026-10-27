using FitArmLog.Models;
using FitArmLog.Services;
using FitArmLog.Tests.Fakes;
using Moq;
using Xunit;

namespace FitArmLog.Tests.Services;

public class WorkoutServiceTests
{
    private static Routine SampleRoutine() => new() { Id = 7, Name = "Pecho" };

    private static List<SetEntry> SampleSets() => new()
    {
        new SetEntry { ExerciseId = "a", ExerciseName = "Press", SetNumber = 1, Reps = 10, WeightKg = 40 },
        new SetEntry { ExerciseId = "a", ExerciseName = "Press", SetNumber = 2, Reps = 8, WeightKg = 42.5 }
    };

    [Fact]
    public async Task SaveAsync_DatosValidos_GuardaLaSesionConElNombreDeLaRutina()
    {
        var fake = new WorkoutRepositoryFake();
        var service = new WorkoutService(fake);

        var result = await service.SaveAsync(SampleRoutine(), SampleSets(), "  buen dia  ");

        Assert.True(result.IsSuccess);
        var saved = await fake.GetAllAsync();
        Assert.Single(saved);
        Assert.Equal(7, saved[0].RoutineId);
        Assert.Equal("Pecho", saved[0].RoutineName);
        Assert.Equal("buen dia", saved[0].Notes);
        Assert.Equal(2, saved[0].TotalSets);
    }

    [Fact]
    public async Task SaveAsync_VolumenTotal_EsLaSumaDeRepsPorPeso()
    {
        var service = new WorkoutService(new WorkoutRepositoryFake());

        var result = await service.SaveAsync(SampleRoutine(), SampleSets(), null);

        // 10*40 + 8*42.5 = 400 + 340
        Assert.Equal(740, result.Value!.TotalVolume);
    }

    [Fact]
    public async Task SaveAsync_SinSeries_Falla()
    {
        var fake = new WorkoutRepositoryFake();
        var service = new WorkoutService(fake);

        var result = await service.SaveAsync(SampleRoutine(), new List<SetEntry>(), null);

        Assert.False(result.IsSuccess);
        Assert.Empty(await fake.GetAllAsync());
    }

    [Fact]
    public async Task SaveAsync_SinRutina_Falla()
    {
        var service = new WorkoutService(new WorkoutRepositoryFake());

        var result = await service.SaveAsync(null, SampleSets(), null);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task SaveAsync_ElRepositorioFalla_DevuelveFail()
    {
        var service = new WorkoutService(new WorkoutRepositoryFake { ThrowOnAdd = true });

        var result = await service.SaveAsync(SampleRoutine(), SampleSets(), null);

        Assert.False(result.IsSuccess);
        Assert.Contains("No se pudo guardar", result.ErrorMessage);
    }

    [Fact]
    public async Task SaveAsync_DatosInvalidos_NuncaLlamaAlRepositorio_Moq()
    {
        var mock = new Mock<IWorkoutRepository>();
        var service = new WorkoutService(mock.Object);

        await service.SaveAsync(SampleRoutine(), new List<SetEntry>(), null);

        mock.Verify(r => r.AddAsync(It.IsAny<WorkoutSession>()), Times.Never);
    }

    [Fact]
    public async Task SaveAsync_DatosValidos_LlamaUnaVezAlRepositorio_Moq()
    {
        var mock = new Mock<IWorkoutRepository>();
        var service = new WorkoutService(mock.Object);

        await service.SaveAsync(SampleRoutine(), SampleSets(), null);

        mock.Verify(r => r.AddAsync(It.Is<WorkoutSession>(s => s.RoutineName == "Pecho" && s.Sets.Count == 2)),
            Times.Once);
    }
}
