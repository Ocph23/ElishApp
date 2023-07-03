using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ShareModels;
using System;
using ApsWebApp;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ApsWebApp.Services
{
    public class UserStateService : IUserStateService
    {
        private readonly IUserService _userService;
        private readonly HttpClient http;
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticateResponse User { get; set; }

        public UserStateService(
            IUserService userService,
            HttpClient Http, ILocalStorageService localStorageService,
            NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider)
        {
            _userService = userService;
            http = Http;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItemAsync<AuthenticateResponse>("user");
        }

        public async Task<bool> Login(UserLogin model)
        {
            try
            {
                User = await _userService.Authenticate(model);
                if (User != null)
                {
                    await _localStorageService.SetItemAsync("user", User);
                    ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(User.UserName);
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", User.Token);
                    return true;
                }
                return false;
             
            }
            catch (Exception ex)
            {
               throw ex.ThrowException();
            }
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItemAsync("user");
            _navigationManager.NavigateTo("login");
        }
    }
}
