using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace FitArmLog.Services.Device;

// Camara: saca una foto del entrenamiento y la guarda en la carpeta privada de la app.
public interface IPhotoService
{
    Task<Result<string>> TakeWorkoutPhotoAsync();
}

public class PhotoService : IPhotoService
{
    public async Task<Result<string>> TakeWorkoutPhotoAsync()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
                return Result<string>.Fail("Este dispositivo no tiene cámara disponible.");

            // 1) Permiso en tiempo de ejecucion: primero se consulta, y si hace falta se pide
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
                status = await MainThread.InvokeOnMainThreadAsync(
                    () => Permissions.RequestAsync<Permissions.Camera>());

            if (status != PermissionStatus.Granted)
                return Result<string>.Fail(
                    "Sin permiso de cámara. Podés habilitarlo en Ajustes > Aplicaciones > FitArmLog > Permisos.");

            // 2) Abrir la camara
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null)
                return Result<string>.Fail("Foto cancelada.");

            // 3) La foto queda en cache temporal: la copiamos a la carpeta de la app
            var target = Path.Combine(FileSystem.AppDataDirectory, $"workout_{Guid.NewGuid():N}.jpg");
            await using var source = await photo.OpenReadAsync();
            await using var destination = File.OpenWrite(target);
            await source.CopyToAsync(destination);

            return Result<string>.Success(target);
        }
        catch (FeatureNotSupportedException)
        {
            return Result<string>.Fail("La cámara no está soportada en este dispositivo.");
        }
        catch (PermissionException)
        {
            return Result<string>.Fail("No se otorgó el permiso de cámara.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Photo] {ex}");
            return Result<string>.Fail("No se pudo tomar la foto. Intentá de nuevo.");
        }
    }
}
