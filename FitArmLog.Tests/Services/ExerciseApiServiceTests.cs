using System.Net;
using System.Text;
using FitArmLog.Services;
using Xunit;

namespace FitArmLog.Tests.Services;

// Tests del consumo de API sin internet: reemplazamos el HttpMessageHandler
// por uno falso que devuelve la respuesta que nosotros queramos.
public class ExerciseApiServiceTests
{
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(_responder(request));
    }

    private static ExerciseApiService CreateService(Func<HttpRequestMessage, HttpResponseMessage> responder) =>
        new(new HttpClient(new StubHandler(responder)));

    private static HttpResponseMessage Json(string json) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };

    private const string ThreeExercisesJson = """
    [
      { "id": "a", "name": "Press banca", "primaryMuscles": ["chest"], "instructions": ["Paso 1", "Paso 2"], "images": ["a/0.jpg"] },
      { "id": "b", "name": "Sin imagen",  "primaryMuscles": ["legs"],  "instructions": ["Paso 1"],           "images": [] },
      { "id": "c", "name": "Remo",        "primaryMuscles": [],        "instructions": [],                   "images": ["c/0.jpg"] }
    ]
    """;

    [Fact]
    public async Task GetExercisesAsync_RespuestaOk_MapeaYFiltraEjerciciosSinImagen()
    {
        var service = CreateService(_ => Json(ThreeExercisesJson));

        var result = await service.GetExercisesAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);                       // el "Sin imagen" se descarta
        Assert.Equal("Press banca", result.Value[0].Name);
        Assert.Equal("chest", result.Value[0].MuscleGroup);
        Assert.Equal("Paso 1", result.Value[0].Description);
        Assert.EndsWith("a/0.jpg", result.Value[0].ImageUrl);
        Assert.StartsWith("https://", result.Value[0].ImageUrl);
    }

    [Fact]
    public async Task GetExercisesAsync_SinMusculoNiInstrucciones_UsaValoresPorDefecto()
    {
        var service = CreateService(_ => Json(ThreeExercisesJson));

        var result = await service.GetExercisesAsync();

        var remo = result.Value![1];
        Assert.Equal("General", remo.MuscleGroup);
        Assert.Equal("Sin descripción disponible.", remo.Description);
    }

    [Fact]
    public async Task GetExercisesAsync_Error404_DevuelveMensajeEspecifico()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.NotFound));

        var result = await service.GetExercisesAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.ErrorMessage);
    }

    [Fact]
    public async Task GetExercisesAsync_Error500_DevuelveMensajeDeServidor()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var result = await service.GetExercisesAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains("500", result.ErrorMessage);
    }

    [Fact]
    public async Task GetExercisesAsync_JsonInvalido_DevuelveErrorDeFormato()
    {
        var service = CreateService(_ => Json("esto no es json"));

        var result = await service.GetExercisesAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains("formato", result.ErrorMessage);
    }

    [Fact]
    public async Task GetExercisesAsync_SinConexion_DevuelveErrorDeConexion()
    {
        var service = CreateService(_ => throw new HttpRequestException("sin red"));

        var result = await service.GetExercisesAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains("conectar", result.ErrorMessage);
    }

    [Fact]
    public async Task GetExercisesAsync_Timeout_DevuelveErrorDeTiempo()
    {
        var service = CreateService(_ => throw new TaskCanceledException());

        var result = await service.GetExercisesAsync();

        Assert.False(result.IsSuccess);
        Assert.Contains("tiempo", result.ErrorMessage);
    }
}
