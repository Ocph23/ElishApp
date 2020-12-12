using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ElishAppMobile
{
    public class RestService : HttpClient
    {
        public static string DeviceToken { get; set; }

        public RestService()
        {
            string _server = Helper.Url;
            this.BaseAddress = new Uri(_server);
            this.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            if (Account.UserIsLogin)
            {
                SetToken(Account.Token);
            }
        }

        

        public RestService(string apiUrl)
        {
            this.BaseAddress = new Uri(apiUrl);

        }


        public void SetToken(string token)
        {
            if (token != null)
            {
                this.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
                    token);
                this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            }
        }

        internal Task DeleteAsync(string id, StringContent content)
        {

            throw new NotImplementedException();
        }

        public StringContent GenerateHttpContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }


        public async Task<string> Error(HttpResponseMessage response)
        {
            try
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"'{response.RequestMessage.RequestUri.LocalPath}'  Not Found";
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                    throw new SystemException();

                if (content.Contains("message"))
                {
                    var error = JsonConvert.DeserializeObject<ErrorMessage>(content);
                    return error.Message;
                }
                return content;
            }
            catch (Exception)
            {
                return "Maaf Terjadi Kesalahan, Silahkan Ulangi Lagi Nanti";
            }
        }
    }



    public class ErrorMessage
    {
        public string Message { get; set; }
    }




    public static class RestServiceExtention {
        
        public static async Task<T> GetResult<T>(this HttpResponseMessage response) 
        {
            try
            {
                string stringContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(stringContent))
                    return default;
                var result = JsonConvert.DeserializeObject<T>(stringContent);
                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
    
    }
}
