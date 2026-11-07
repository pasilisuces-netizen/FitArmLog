using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitArmLog.Services;
using FitArmLog.Services.Device;

namespace FitArmLog.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly IHapticService _haptic;

    [ObservableProperty]
    private bool vibrationEnabled;

    [ObservableProperty]
    private bool shakeToUndoEnabled;

    [ObservableProperty]
    private bool saveLocationByDefault;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasInfo))]
    private string infoMessage = string.Empty;

    public bool HasInfo => !string.IsNullOrEmpty(InfoMessage);

    public string VersionText => $"FitArmLog v{AppInfo.Current.VersionString}";

    public SettingsViewModel(ISettingsService settings, IHapticService haptic)
    {
        _settings = settings;
        _haptic = haptic;

        // Se cargan los valores guardados
        vibrationEnabled = settings.VibrationEnabled;
        shakeToUndoEnabled = settings.ShakeToUndoEnabled;
        saveLocationByDefault = settings.SaveLocationByDefault;
    }

    // El toolkit genera estos metodos "parciales": se ejecutan cada vez que cambia la propiedad
    partial void OnVibrationEnabledChanged(bool value) => _settings.VibrationEnabled = value;
    partial void OnShakeToUndoEnabledChanged(bool value) => _settings.ShakeToUndoEnabled = value;
    partial void OnSaveLocationByDefaultChanged(bool value) => _settings.SaveLocationByDefault = value;

    [RelayCommand]
    private void TestVibration()
    {
        var result = _haptic.Vibrate(300);

        InfoMessage = result.IsSuccess
            ? "Listo. Si no sentiste la vibración, revisá que el teléfono no esté en modo silencio."
            : result.ErrorMessage ?? "No se pudo vibrar.";
    }
}
