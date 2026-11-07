using Microsoft.Maui.Storage;

namespace FitArmLog.Services;

// Preferencias del usuario. Se guardan con Preferences (clave-valor, Unidad 3):
// ideal para datos simples como "activado / desactivado".
public interface ISettingsService
{
    bool VibrationEnabled { get; set; }
    bool ShakeToUndoEnabled { get; set; }
    bool SaveLocationByDefault { get; set; }
}

public class SettingsService : ISettingsService
{
    private const string VibrationKey = "vibration_enabled";
    private const string ShakeKey = "shake_enabled";
    private const string LocationKey = "location_default";

    public bool VibrationEnabled
    {
        get => Preferences.Default.Get(VibrationKey, true);
        set => Preferences.Default.Set(VibrationKey, value);
    }

    public bool ShakeToUndoEnabled
    {
        get => Preferences.Default.Get(ShakeKey, true);
        set => Preferences.Default.Set(ShakeKey, value);
    }

    public bool SaveLocationByDefault
    {
        get => Preferences.Default.Get(LocationKey, false);
        set => Preferences.Default.Set(LocationKey, value);
    }
}
