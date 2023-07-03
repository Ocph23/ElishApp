using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;

namespace ApsWebApp
{
    [Serializable]
    internal class ExceptionErrorModel 
    {
        private HttpResponseMessage result;

        public ExceptionErrorModel(HttpResponseMessage result)
        {
            this.result = result;
            var resultString = result.Content.ReadAsStringAsync().Result;
            var errorModel = JsonSerializer.Deserialize<ErrorModel>(resultString,Helper.JsonOption);

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