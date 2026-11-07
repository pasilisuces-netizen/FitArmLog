using Microsoft.Maui.Devices;

namespace FitArmLog.Services.Device;

// Vibracion como confirmacion tactil (ej.: al agregar una serie).
public interface IHapticService
{
    // Vibra solo si el usuario lo dejo habilitado en Ajustes
    void Tick();

    // Vibra siempre y dice si pudo (lo usa el boton "Probar vibracion")
    Result<bool> Vibrate(int milliseconds);
}

public class HapticService : IHapticService
{
    private readonly ISettingsService _settings;

    public HapticService(ISettingsService settings)
    {
        _settings = settings;
    }

    public void Tick()
    {
        if (!_settings.VibrationEnabled) return;

        var result = Vibrate(150);
        if (!result.IsSuccess)
            System.Diagnostics.Debug.WriteLine($"[Haptic] {result.ErrorMessage}");
    }

    public Result<bool> Vibrate(int milliseconds)
    {
        try
        {
            if (!Vibration.Default.IsSupported)
                return Result<bool>.Fail("Este dispositivo no tiene vibración disponible.");

            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(milliseconds));
            return Result<bool>.Success(true);
        }
        catch (FeatureNotSupportedException)
        {
            return Result<bool>.Fail("Este dispositivo no soporta la vibración.");
        }
        catch (Exception ex)
        {
            // Si falta el permiso VIBRATE en el AndroidManifest, el error cae aca
            return Result<bool>.Fail($"No se pudo vibrar: {ex.Message}");
        }
    }
}
