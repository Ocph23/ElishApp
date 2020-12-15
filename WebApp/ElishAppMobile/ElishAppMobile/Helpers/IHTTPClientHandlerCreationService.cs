using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ElishAppMobile.Helpers
{
    public interface IHTTPClientHandlerCreationService
    {
        HttpClientHandler GetInsecureHandler();
    }
}
