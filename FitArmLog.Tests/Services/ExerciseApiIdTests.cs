using System.Net;
using System.Text;
using FitArmLog.Services;
using Xunit;

namespace FitArmLog.Tests.Services;

public class ExerciseApiIdTests
{
    private sealed class StubHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """[ { "id": "Barbell_Curl", "name": "Curl", "primaryMuscles": ["biceps"], "instructions": ["x"], "images": ["Barbell_Curl/0.jpg"] } ]""",
                    Encoding.UTF8, "application/json")
            });
    }

    [Fact]
    public async Task GetExercisesAsync_UsaElIdDeLaApi_NoUnoNuevoEnCadaCarga()
    {
        var service = new ExerciseApiService(new HttpClient(new StubHandler()));

        var first = await service.GetExercisesAsync();
        var second = await service.GetExercisesAsync();

        Assert.Equal("Barbell_Curl", first.Value![0].Id);
        Assert.Equal(first.Value[0].Id, second.Value![0].Id);   // estable entre cargas
    }
}
