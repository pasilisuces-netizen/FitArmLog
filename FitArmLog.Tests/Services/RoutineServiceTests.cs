using FitArmLog.Models;
using FitArmLog.Services;
using FitArmLog.Tests.Fakes;
using Moq;
using Xunit;

namespace FitArmLog.Tests.Services;

public class RoutineServiceTests
{
    private static List<Exercise> TwoExercises() => new()
    {
        new Exercise { Name = "Press banca", MuscleGroup = "chest" },
        new Exercise { Name = "Aperturas", MuscleGroup = "chest" }
    };

    // =====================  Con FAKE (estado real en memoria)  =====================

    [Fact]
    public async Task CreateAsync_DatosValidos_GuardaLaRutinaEnElRepositorio()
    {
        var fake = new RoutineRepositoryFake();
        var service = new RoutineService(fake);

        var result = await service.CreateAsync("  Pecho  ", TwoExercises());

        Assert.True(result.IsSuccess);
        var saved = await fake.GetAllAsync();
        Assert.Single(saved);
        Assert.Equal("Pecho", saved[0].Name);          // se recorta el nombre
        Assert.Equal(2, saved[0].Exercises.Count);
        Assert.True(saved[0].Id > 0);                  // el repositorio asigno un Id
    }

    [Fact]
    public async Task CreateAsync_NombreVacio_FallaYNoGuardaNada()
    {
        var fake = new RoutineRepositoryFake();
        var service = new RoutineService(fake);

        var result = await service.CreateAsync("", TwoExercises());

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(await fake.GetAllAsync());
    }

    [Fact]
    public async Task CreateAsync_SinEjercicios_Falla()
    {
        var service = new RoutineService(new RoutineRepositoryFake());

        var result = await service.CreateAsync("Pecho", new List<Exercise>());

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CreateAsync_ElRepositorioFalla_DevuelveErrorEnVezDeExplotar()
    {
        var fake = new RoutineRepositoryFake { ThrowOnAdd = true };
        var service = new RoutineService(fake);

        var result = await service.CreateAsync("Pecho", TwoExercises());

        Assert.False(result.IsSuccess);
        Assert.Contains("No se pudo guardar", result.ErrorMessage);
    }

    // =====================  Con MOQ (verificamos interacciones)  =====================

    [Fact]
    public async Task CreateAsync_DatosInvalidos_NuncaLlamaAlRepositorio()
    {
        var mock = new Mock<IRoutineRepository>();
        var service = new RoutineService(mock.Object);

        await service.CreateAsync("   ", TwoExercises());

        mock.Verify(r => r.AddAsync(It.IsAny<Routine>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_DatosValidos_LlamaUnaSolaVezAlRepositorio()
    {
        var mock = new Mock<IRoutineRepository>();
        var service = new RoutineService(mock.Object);

        await service.CreateAsync("Pecho", TwoExercises());

        mock.Verify(r => r.AddAsync(It.Is<Routine>(x => x.Name == "Pecho" && x.Exercises.Count == 2)),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_MockLanzaExcepcion_DevuelveFail()
    {
        var mock = new Mock<IRoutineRepository>();
        mock.Setup(r => r.AddAsync(It.IsAny<Routine>()))
            .ThrowsAsync(new InvalidOperationException("boom"));
        var service = new RoutineService(mock.Object);

        var result = await service.CreateAsync("Pecho", TwoExercises());

        Assert.False(result.IsSuccess);
        Assert.Contains("boom", result.ErrorMessage);
    }
}
