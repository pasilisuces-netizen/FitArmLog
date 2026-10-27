using FitArmLog.Models;
using FitArmLog.Validation;

namespace FitArmLog.Services;

// Capa de aplicacion: reglas de negocio para crear rutinas.
// No conoce la UI (MAUI) ni SQLite: solo el validador y la interfaz del repositorio.
public interface IRoutineService
{
    Task<Result<Routine>> CreateAsync(string? name, IReadOnlyList<Exercise> exercises);
}

public class RoutineService : IRoutineService
{
    private readonly IRoutineRepository _repository;

    public RoutineService(IRoutineRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Routine>> CreateAsync(string? name, IReadOnlyList<Exercise> exercises)
    {
        var validation = AppValidator.ValidateRoutine(name, exercises.Count);
        if (!validation.IsValid)
            return Result<Routine>.Fail(validation.FirstError);

        try
        {
            var routine = new Routine
            {
                Name = name!.Trim(),
                Exercises = exercises.ToList()
            };

            await _repository.AddAsync(routine);
            return Result<Routine>.Success(routine);
        }
        catch (Exception ex)
        {
            return Result<Routine>.Fail($"No se pudo guardar la rutina: {ex.Message}");
        }
    }
}
