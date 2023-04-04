using Android.App;
using Android.Runtime;

namespace ApsMobileApp;


#if DEBUG                                   // connect to local service on the
[Application(UsesCleartextTraffic = true)]  // emulator's host for debugging,
#else                                       // access via http://10.0.2.2
    [Application]                               
#endif

//[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]
//[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
//[assembly: UsesPermission(Android.Manifest.Permission.Vibrate)]
//[assembly: UsesPermission(Android.Manifest.Permission.Camera)]
//[assembly: UsesFeature("android.hardware.camera", Required = true)]
//[assembly: UsesFeature("android.hardware.camera.autofocus", Required = true)]

public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}