using FitArmLog.Models;
using FitArmLog.Validation;

namespace FitArmLog.Services;

// Capa de aplicacion: reglas de negocio de las rutinas.
// No conoce la UI (MAUI) ni SQLite: solo el validador y la interfaz del repositorio.
public interface IRoutineService
{
    Task<Result<List<Routine>>> GetAllAsync();
    Task<Result<Routine>> CreateAsync(string? name, IReadOnlyList<Exercise> exercises);
    Task<Result<bool>> DeleteAsync(int id);
}

public class RoutineService : IRoutineService
{
    private readonly IRoutineRepository _repository;

    public RoutineService(IRoutineRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<Routine>>> GetAllAsync()
    {
        try
        {
            return Result<List<Routine>>.Success(await _repository.GetAllAsync());
        }
        catch (Exception ex)
        {
            return Result<List<Routine>>.Fail($"No se pudieron cargar las rutinas: {ex.Message}");
        }
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

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail($"No se pudo eliminar la rutina: {ex.Message}");
        }
    }
}
