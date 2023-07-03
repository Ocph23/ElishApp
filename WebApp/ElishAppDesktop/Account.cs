using ElishAppDesktop.Models;
using Newtonsoft.Json;
using ShareModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElishAppDesktop
{
    public class Account
    {
         private static  Windows.Storage.ApplicationDataContainer SecureStorage = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static bool UserIsLogin {
            get {
                var user = GetUser();
                return user != null;
                                
            }
        }

        public static AuthenticateResponse GetUser()
        {
            var userString = (string)SecureStorage.Values["User"];
            if (string.IsNullOrEmpty(userString))
                return null;
            else
                return JsonConvert.DeserializeObject<AuthenticateResponse>(userString);
        }

        public static void SetUser(AuthenticateResponse response)
        {
            try
            {
                var userString = JsonConvert.SerializeObject(response);
                SecureStorage.Values["User"]= userString;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public static void LogOut()
        {
            try
            {
                SecureStorage.Values["User"] = null;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        internal static void SetProfile(Profile response)
        {
            var userString = JsonConvert.SerializeObject(response);
             SecureStorage.Values["Profile"]= userString;
        }

        public static Profile GetProfile()
        {
            var userString = SecureStorage.Values["Profile"].ToString();
            if (string.IsNullOrEmpty(userString))
                return null;
            else
                return JsonConvert.DeserializeObject<Profile>(userString);
        }

        public static string Token
        {
            get
            {
                var user = GetUser();
                return user == null ? string.Empty : user.Token;
            }
        }

        public static bool UserInRole(string roleName)
        {
            var user =  GetUser();
            if (user != null)
            {
                var role = user.Roles.Where(x => x.ToLower() == roleName.ToLower()).FirstOrDefault();
                return !string.IsNullOrEmpty(role);
            }
            return false;
        }
    }
}
