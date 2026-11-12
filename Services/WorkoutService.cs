using FitArmLog.Models;
using FitArmLog.Validation;

namespace FitArmLog.Services;

// Capa de aplicacion: reglas de negocio para guardar un entrenamiento.
public interface IWorkoutService
{
    Task<Result<WorkoutSession>> SaveAsync(
        Routine? routine, IReadOnlyCollection<SetEntry> sets, string? notes,
        string? photoPath = null, double? latitude = null, double? longitude = null);
    Task<Result<List<WorkoutSession>>> GetAllAsync();
    Task<Result<bool>> DeleteAsync(int id);
}

public class WorkoutService : IWorkoutService
{
    private readonly IWorkoutRepository _repository;

    public WorkoutService(IWorkoutRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<WorkoutSession>> SaveAsync(
        Routine? routine, IReadOnlyCollection<SetEntry> sets, string? notes,
        string? photoPath = null, double? latitude = null, double? longitude = null)
    {
        if (routine is null)
            return Result<WorkoutSession>.Fail("No hay una rutina seleccionada para entrenar.");

        var validation = AppValidator.ValidateSession(sets, notes);
        if (!validation.IsValid)
            return Result<WorkoutSession>.Fail(validation.FirstError);

        var locationValidation = AppValidator.ValidateLocation(latitude, longitude);
        if (!locationValidation.IsValid)
            return Result<WorkoutSession>.Fail(locationValidation.FirstError);

        try
        {
            var session = new WorkoutSession
            {
                RoutineId = routine.Id,
                RoutineName = routine.Name,
                Date = DateTime.Now,
                Sets = sets.ToList(),
                Notes = notes?.Trim() ?? string.Empty,
                PhotoPath = string.IsNullOrWhiteSpace(photoPath) ? null : photoPath,
                Latitude = latitude,
                Longitude = longitude
            };

            await _repository.AddAsync(session);
            return Result<WorkoutSession>.Success(session);
        }
        catch (Exception ex)
        {
            return Result<WorkoutSession>.Fail($"No se pudo guardar el entrenamiento: {ex.Message}");
        }
    }

    public async Task<Result<List<WorkoutSession>>> GetAllAsync()
    {
        try
        {
            return Result<List<WorkoutSession>>.Success(await _repository.GetAllAsync());
        }
        catch (Exception ex)
        {
            return Result<List<WorkoutSession>>.Fail($"No se pudo cargar el historial: {ex.Message}");
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
            return Result<bool>.Fail($"No se pudo eliminar el entrenamiento: {ex.Message}");
        }
    }
}
