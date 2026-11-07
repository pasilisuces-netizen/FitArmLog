using FitArmLog.Models;
using FitArmLog.Services;
using FitArmLog.Tests.Fakes;
using FitArmLog.Validation;
using Xunit;

namespace FitArmLog.Tests.Services;

// Etapa 9: la logica que rodea a los sensores (validacion de ubicacion y guardado de foto/GPS).
// Los sensores en si (camara, GPS, acelerometro) se prueban a mano en el celular.
public class SensorDataTests
{
    private static Routine SampleRoutine() => new() { Id = 1, Name = "Pecho" };

    private static List<SetEntry> SampleSets() => new()
    {
        new SetEntry { ExerciseId = "a", ExerciseName = "Press", SetNumber = 1, Reps = 10, WeightKg = 40 }
    };

    // ---------- Validador de ubicacion ----------

    [Theory]
    [InlineData(null, null)]          // sin ubicacion: valido (es opcional)
    [InlineData(-34.6037, -58.3816)]  // Buenos Aires
    [InlineData(90.0, 180.0)]
    [InlineData(-90.0, -180.0)]
    public void ValidateLocation_Valida(double? lat, double? lon)
    {
        Assert.True(AppValidator.ValidateLocation(lat, lon).IsValid);
    }

    [Theory]
    [InlineData(91.0, 0.0)]
    [InlineData(-91.0, 0.0)]
    [InlineData(0.0, 181.0)]
    [InlineData(0.0, -181.0)]
    [InlineData(10.0, null)]            // incompleta
    [InlineData(null, 10.0)]            // incompleta
    [InlineData(double.NaN, 0.0)]
    public void ValidateLocation_Invalida(double? lat, double? lon)
    {
        Assert.False(AppValidator.ValidateLocation(lat, lon).IsValid);
    }

    // ---------- Servicio ----------

    [Fact]
    public async Task SaveAsync_ConFotoYUbicacion_LosGuarda()
    {
        var fake = new WorkoutRepositoryFake();
        var service = new WorkoutService(fake);

        var result = await service.SaveAsync(SampleRoutine(), SampleSets(), null,
            "/data/foto.jpg", -34.6037, -58.3816);

        Assert.True(result.IsSuccess);
        var saved = (await fake.GetAllAsync()).Single();
        Assert.Equal("/data/foto.jpg", saved.PhotoPath);
        Assert.True(saved.HasPhoto);
        Assert.True(saved.HasLocation);
        Assert.Equal(-34.6037, saved.Latitude);
    }

    [Fact]
    public async Task SaveAsync_SinFotoNiUbicacion_QuedanEnNull()
    {
        var fake = new WorkoutRepositoryFake();
        var service = new WorkoutService(fake);

        await service.SaveAsync(SampleRoutine(), SampleSets(), null);

        var saved = (await fake.GetAllAsync()).Single();
        Assert.Null(saved.PhotoPath);
        Assert.Null(saved.Latitude);
        Assert.False(saved.HasPhoto);
        Assert.False(saved.HasLocation);
        Assert.Equal(string.Empty, saved.LocationText);
    }

    [Fact]
    public async Task SaveAsync_UbicacionFueraDeRango_FallaYNoGuarda()
    {
        var fake = new WorkoutRepositoryFake();
        var service = new WorkoutService(fake);

        var result = await service.SaveAsync(SampleRoutine(), SampleSets(), null, null, 200.0, 10.0);

        Assert.False(result.IsSuccess);
        Assert.Empty(await fake.GetAllAsync());
    }

    [Fact]
    public void LocationText_FormateaConPuntoDecimal()
    {
        var session = new WorkoutSession { Latitude = -34.6037, Longitude = -58.3816 };

        Assert.Equal("Ubicación: -34.6037, -58.3816", session.LocationText);
    }
}
