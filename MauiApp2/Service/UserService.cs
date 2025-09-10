using MauiApp2.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using MauiApp2.Services;
namespace MauiApp2.Service
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly NavigationManager _navigationManager;

        public UserService(HttpClient httpClient, NavigationManager navigationManager, AuthService authService)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _authService = authService;
        }
        private async Task SetAuthorizationHeaderIfNeeded()
        {
            var token = await SecureStorage.GetAsync("authToken");

            if (!string.IsNullOrEmpty(token) &&
                (_httpClient.DefaultRequestHeaders.Authorization == null ||
                 _httpClient.DefaultRequestHeaders.Authorization.Parameter != token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        // Fetch User Details by ID
        public async Task<UserModel?> GetUserByIdAsync(string userId)
        {
            //if (!await _authService.EnsureValidTokenAsync())
            //    throw new Exception("User is logged out.");
            await SetAuthorizationHeaderIfNeeded();
            var response = await _httpClient.GetAsync($"api/Users/{userId}");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                SecureStorage.Remove("authToken");
                _navigationManager.NavigateTo("/", true);
                return null;
            }
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<UserModel>();
        }
        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Users/{user.UserId}", user);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Update Failed: {error}");
            return false;
        }



    }
}
