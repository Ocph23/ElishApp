using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ApsMobileApp.Helpers;

public interface IHTTPClientHandlerCreationService
{
    HttpClientHandler GetInsecureHandler();
}
