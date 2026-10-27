using FitArmLog.Models;
using FitArmLog.Validation;

namespace FitArmLog.Services;

// Capa de aplicacion: reglas de negocio para guardar un entrenamiento.
public interface IWorkoutService
{
    Task<Result<WorkoutSession>> SaveAsync(Routine? routine, IReadOnlyCollection<SetEntry> sets, string? notes);
}

public class WorkoutService : IWorkoutService
{
    private readonly IWorkoutRepository _repository;

    public WorkoutService(IWorkoutRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<WorkoutSession>> SaveAsync(
        Routine? routine, IReadOnlyCollection<SetEntry> sets, string? notes)
    {
        if (routine is null)
            return Result<WorkoutSession>.Fail("No hay una rutina seleccionada para entrenar.");

        var validation = AppValidator.ValidateSession(sets, notes);
        if (!validation.IsValid)
            return Result<WorkoutSession>.Fail(validation.FirstError);

        try
        {
            var session = new WorkoutSession
            {
                RoutineId = routine.Id,
                RoutineName = routine.Name,
                Date = DateTime.Now,
                Sets = sets.ToList(),
                Notes = notes?.Trim() ?? string.Empty
            };

            await _repository.AddAsync(session);
            return Result<WorkoutSession>.Success(session);
        }
        catch (Exception ex)
        {
            return Result<WorkoutSession>.Fail($"No se pudo guardar el entrenamiento: {ex.Message}");
        }
    }
}
