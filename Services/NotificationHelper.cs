using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;

namespace FitArmLog.Services;


public static class NotificationHelper
{
    public static async Task ShowToastAsync(string message)
    {
        try
        {
            await Toast.Make(message).Show();
        }
        catch (Exception ex)
        {
          
            Debug.WriteLine($"[NotificationHelper] No se pudo mostrar el Toast: {ex.Message}");
        }
    }
}