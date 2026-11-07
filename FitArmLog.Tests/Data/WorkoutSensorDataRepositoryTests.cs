using FitArmLog.Models;
using FitArmLog.Services;
using Xunit;

namespace FitArmLog.Tests.Data;

[Collection("SqliteMemoria")]
public class WorkoutSensorDataRepositoryTests : IAsyncLifetime
{
    private readonly WorkoutRepositorySqlite _repo = new(":memory:");

    public async Task InitializeAsync()
    {
        foreach (var session in await _repo.GetAllAsync())
            await _repo.DeleteAsync(session.Id);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddAsync_ConFotoYUbicacion_SePersistenYSeLeenIguales()
    {
        await _repo.AddAsync(new WorkoutSession
        {
            RoutineName = "Pecho",
            Date = DateTime.Now,
            Sets = new List<SetEntry> { new() { Reps = 10, WeightKg = 40 } },
            PhotoPath = "/data/foto.jpg",
            Latitude = -34.6037,
            Longitude = -58.3816
        });

        var saved = (await _repo.GetAllAsync()).Single();

        Assert.Equal("/data/foto.jpg", saved.PhotoPath);
        Assert.Equal(-34.6037, saved.Latitude);
        Assert.Equal(-58.3816, saved.Longitude);
        Assert.True(saved.HasPhoto);
        Assert.True(saved.HasLocation);
    }

    [Fact]
    public async Task AddAsync_SinSensores_LosCamposOpcionalesVuelvenNull()
    {
        await _repo.AddAsync(new WorkoutSession
        {
            RoutineName = "Espalda",
            Date = DateTime.Now,
            Sets = new List<SetEntry> { new() { Reps = 8, WeightKg = 30 } }
        });

        var saved = (await _repo.GetAllAsync()).Single();

        Assert.Null(saved.PhotoPath);
        Assert.Null(saved.Latitude);
        Assert.Null(saved.Longitude);
    }
}
