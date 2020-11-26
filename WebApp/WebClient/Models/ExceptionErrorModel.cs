using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace WebClient
{
    [Serializable]
    internal class ExceptionErrorModel 
    {
        private HttpResponseMessage result;

        public ExceptionErrorModel(HttpResponseMessage result)
        {
            this.result = result;
            var resultString = result.Content.ReadAsStringAsync().Result;
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(resultString);

            if (errorModel == null)
                Message = result.ReasonPhrase;
            else
                Message = errorModel.Message;
        }
      
        public string Message { get; }
    }


    public class ErrorModel
    {
        public string Message { get; set; }
    }

}