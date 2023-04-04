using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;


namespace ApsMobileApp.Helpers;




public interface IToas
{
    Task ShowLong(string message);
    Task ShowShort(string message);
}

public class Toas
{
    public static async Task ShowLong(string message) {

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;
        var toast = Toast.Make(message, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);
    }

    public static async Task ShowShort(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ToastDuration duration = ToastDuration.Long;
        double fontSize = 14;
        var toast = Toast.Make(message, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);
    }

}
