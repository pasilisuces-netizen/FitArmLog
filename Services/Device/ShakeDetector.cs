using Microsoft.Maui.Devices.Sensors;

namespace FitArmLog.Services.Device;

// Acelerometro: avisa cuando el usuario sacude el telefono.
public interface IShakeDetector
{
    event EventHandler? ShakeDetected;
    void Start();
    void Stop();
}

public class ShakeDetector : IShakeDetector
{
    public event EventHandler? ShakeDetected;

    public void Start()
    {
        try
        {
            if (!Accelerometer.Default.IsSupported || Accelerometer.Default.IsMonitoring)
                return;

            Accelerometer.Default.ShakeDetected += OnShakeDetected;
            Accelerometer.Default.Start(SensorSpeed.Game);   // "Game" es la velocidad recomendada para sacudidas
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Shake] No se pudo iniciar: {ex.Message}");
        }
    }

    public void Stop()
    {
        try
        {
            // Siempre se des-suscribe y se apaga el sensor: si queda prendido gasta bateria
            Accelerometer.Default.ShakeDetected -= OnShakeDetected;
            if (Accelerometer.Default.IsSupported && Accelerometer.Default.IsMonitoring)
                Accelerometer.Default.Stop();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Shake] No se pudo detener: {ex.Message}");
        }
    }

    private void OnShakeDetected(object? sender, EventArgs e) => ShakeDetected?.Invoke(this, EventArgs.Empty);
}
