using ApsMobileApp.Models;
using ShareModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace ApsMobileApp;


public class Account
{
    public static bool UserIsLogin
    {
        get
        {
            return GetUser() != null;
        }
    }

    public static AuthenticateResponse GetUser()
    {
        var userString = Preferences.Get("User", null);
        if (string.IsNullOrEmpty(userString))
            return null;
        else
            return JsonSerializer.Deserialize<AuthenticateResponse>(userString, Helper.JsonOption);
    }

    public static Task SetUser(AuthenticateResponse response)
    {
        try
        {
            var userString = JsonSerializer.Serialize(response);
            Preferences.Set("User", userString);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new SystemException(ex.Message);
        }
    }

    public static Task LogOut()
    {
        try
        {
            Preferences.Set("User", string.Empty);
            Preferences.Set("Profile", string.Empty);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new SystemException(ex.Message);
        }
    }

    internal static Task SetProfile(Profile response)
    {
        var userString = JsonSerializer.Serialize(response);
        Preferences.Set("Profile", userString);
        return Task.CompletedTask;
    }

    public static async Task<Profile> GetProfile()
    {
        await Task.Delay(100);
        var userString = Preferences.Get("Profile", null);
        if (string.IsNullOrEmpty(userString))
            return null;
        else
            return JsonSerializer.Deserialize<Profile>(userString, Helper.JsonOption);
    }

    public static string Token
    {
        get
        {
            var user = GetUser();
            return user == null ? string.Empty : user.Token;
        }
    }

    public static Task<bool> UserInRole(string roleName)
    {
        var user = GetUser();
        if (user != null)
        {
            var role = user.Roles.Where(x => x.ToLower() == roleName.ToLower()).FirstOrDefault();
            return Task.FromResult(!string.IsNullOrEmpty(role));
        }
        return Task.FromResult(false);
    }
}