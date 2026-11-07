using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;

namespace FitArmLog.Services.Device;

public record GeoPoint(double Latitude, double Longitude);

// GPS: obtiene la ubicacion actual para registrar donde se entreno.
public interface ILocationService
{
    Task<Result<GeoPoint>> GetCurrentAsync();
}

public class LocationService : ILocationService
{
    public async Task<Result<GeoPoint>> GetCurrentAsync()
    {
        try
        {
            // Permiso en tiempo de ejecucion (ubicacion "mientras se usa la app")
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                status = await MainThread.InvokeOnMainThreadAsync(
                    () => Permissions.RequestAsync<Permissions.LocationWhenInUse>());

            if (status != PermissionStatus.Granted)
                return Result<GeoPoint>.Fail(
                    "Sin permiso de ubicación. Podés habilitarlo en Ajustes > Aplicaciones > FitArmLog > Permisos.");

            // Precision media y maximo 10 segundos de espera
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var location = await Geolocation.Default.GetLocationAsync(request, cts.Token);

            if (location is null)
                return Result<GeoPoint>.Fail("No se pudo determinar la ubicación.");

            return Result<GeoPoint>.Success(new GeoPoint(location.Latitude, location.Longitude));
        }
        catch (FeatureNotEnabledException)
        {
            return Result<GeoPoint>.Fail("El GPS está desactivado. Activá la ubicación del teléfono.");
        }
        catch (FeatureNotSupportedException)
        {
            return Result<GeoPoint>.Fail("Este dispositivo no soporta geolocalización.");
        }
        catch (PermissionException)
        {
            return Result<GeoPoint>.Fail("No se otorgó el permiso de ubicación.");
        }
        catch (TaskCanceledException)
        {
            return Result<GeoPoint>.Fail("La ubicación tardó demasiado en responder.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Location] {ex}");
            return Result<GeoPoint>.Fail("No se pudo obtener la ubicación.");
        }
    }
}
