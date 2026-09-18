using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using FitArmLog.Models;
using FitArmLog.Services.Dtos;

namespace FitArmLog.Services;

public interface IExerciseApiService
{
    Task<Result<List<Exercise>>> GetExercisesAsync(CancellationToken token = default);
}

public class ExerciseApiService : IExerciseApiService
{
    
    private const string ExercisesJsonUrl =
        "https://raw.githubusercontent.com/yuhonas/free-exercise-db/main/dist/exercises.json";

    
    private const string ImageBaseUrl =
        "https://raw.githubusercontent.com/yuhonas/free-exercise-db/main/exercises/";

    private readonly HttpClient _http;

    
    public ExerciseApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<Result<List<Exercise>>> GetExercisesAsync(CancellationToken token = default)
    {
        try
        {
            using var response = await _http.GetAsync(ExercisesJsonUrl, token);

            
            if (!response.IsSuccessStatusCode)
            {
                return Result<List<Exercise>>.Fail(
                    $"El servidor respondió con un error ({(int)response.StatusCode}).");
            }

            var json = await response.Content.ReadAsStringAsync(token);

            var dtos = JsonSerializer.Deserialize<List<ExerciseApiDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (dtos is null)
                return Result<List<Exercise>>.Fail("No se recibieron datos.");

            
            var exercises = dtos
                .Where(d => d.Images.Count > 0)
                .Take(40)
                .Select(d => new Exercise
                {
                    Name = d.Name,
                    MuscleGroup = d.PrimaryMuscles.FirstOrDefault() ?? "General",
                    Description = d.Instructions.FirstOrDefault() ?? "Sin descripción disponible.",
                    ImageUrl = ImageBaseUrl + d.Images.First()
                })
                .ToList();

            return Result<List<Exercise>>.Success(exercises);
        }
        catch (TaskCanceledException)
        {
            return Result<List<Exercise>>.Fail(
                "La operación fue cancelada o excedió el tiempo de espera.");
        }
        catch (JsonException)
        {
            return Result<List<Exercise>>.Fail(
                "Los datos recibidos no tienen el formato esperado.");
        }
        catch (HttpRequestException)
        {
            return Result<List<Exercise>>.Fail(
                "No se pudo conectar con el servicio. Verificá tu conexión e intentá nuevamente.");
        }
        catch (Exception)
        {
            return Result<List<Exercise>>.Fail("Ocurrió un error inesperado. Intentá más tarde.");
        }
    }
}