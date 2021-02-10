using Android.Net;
using ElishAppMobile.Droid;
using ElishAppMobile.Helpers;
using Javax.Net.Ssl;
using System.Net.Http;
using Xamarin.Android.Net;


[assembly: Xamarin.Forms.Dependency(typeof(HTTPClientHandlerCreationServiceAndroid))]
namespace ElishAppMobile.Droid
{
    public class HTTPClientHandlerCreationServiceAndroid : IHTTPClientHandlerCreationService
    {
        public HttpClientHandler GetInsecureHandler()
        {
            return new IgnoreSSLClientHandler();
        }
    }

    public class IgnoreSSLClientHandler : AndroidClientHandler
    {
        protected override SSLSocketFactory ConfigureCustomSSLSocketFactory(HttpsURLConnection connection)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return SSLCertificateSocketFactory.GetInsecure(1000, null);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        protected override IHostnameVerifier GetSSLHostnameVerifier(HttpsURLConnection connection)
        {
            return new IgnoreSSLHostnameVerifier();
        }
    }

    public class IgnoreSSLHostnameVerifier : Java.Lang.Object, IHostnameVerifier
    {
        public bool Verify(string hostname, ISSLSession session)
        {
            return true;
        }
    }
}