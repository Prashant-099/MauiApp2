
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static MauiApp2.Components.Pages.Login;


namespace MauiApp2.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;

        public AuthService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;

        }
        public async Task<bool> TryRefreshTokenAsync()
        {
            var refreshToken = await SecureStorage.GetAsync("refreshToken");
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var response = await _httpClient.PostAsJsonAsync("api/Auth/refresh-token", new { RefreshToken = refreshToken });

            if (!response.IsSuccessStatusCode)
                return false;

            var authData = await response.Content.ReadFromJsonAsync<AuthResult>();
            if (authData == null)
                return false;

            await SecureStorage.SetAsync("authToken", authData.Token);
            //await SecureStorage.SetAsync("refreshToken", authData.RefreshToken);
            //await SecureStorage.SetAsync("tokenExpiry", authData.tokenExp.ToString());

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authData.Token);

            return true;
        }

        public async Task<bool> EnsureValidTokenAsync()
        {
            var tokenExpiryStr = await SecureStorage.GetAsync("tokenExpiry");
            var refreshToken = await SecureStorage.GetAsync("refreshToken");

            if (string.IsNullOrEmpty(tokenExpiryStr) || string.IsNullOrEmpty(refreshToken))
            {
                await LogoutAsync();
                //_navigationManager.NavigateTo("/", true);
                return false;
            }

            var tokenExpiryUnix = long.Parse(tokenExpiryStr);
            var tokenExpiryTime = DateTimeOffset.FromUnixTimeSeconds(tokenExpiryUnix);

            if (DateTimeOffset.UtcNow >= tokenExpiryTime)
            {
                // Token expired, try to refresh
                var refreshSuccess = await TryRefreshTokenAsync();
                if (!refreshSuccess)
                {
                    await LogoutAsync(); // Refresh token expired or invalid
                    _navigationManager.NavigateTo("/", true);
                    return false;
                }
            }

            return true; // Token is still valid or successfully refreshed
        }

        public async Task<AuthResult> LoginAsync(LoginModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);
          
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Login failed: {error}");
            }
            var result = await response.Content.ReadFromJsonAsync<AuthResult>();
            if (result == null)
                throw new Exception("Invalid login response.");

            return result;
        }



        public async Task<string> ForgetAsync(string Email)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/forgot-password", new { email = Email});
            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Login failed: {error}");
                }
                else
                {
                    // var result = await response.Content.ReadFromJsonAsync<AuthResult>();
                    // return result.Token;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error:{e.Message}");
            }
            var result = await response.Content.ReadFromJsonAsync<AuthResult>();
            return result.Token;
        }

        public async Task<string> resetpasswordAsync(string Email, string Token, string NewPassword)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/reset-password", new
            {
                email = Email,
                token = Token,
                newPassword = NewPassword // 👈 exact match!
            });

            var rawContent = await response.Content.ReadAsStringAsync();

            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Password reset failed: {rawContent}");
                }

                return rawContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public event Action? OnLogout;
        public async Task LogoutAsync()
        {
            // Remove the token from secure storage
            SecureStorage.Remove("authToken");
            SecureStorage.Remove("refreshToken");
            SecureStorage.Remove("tokenExpiry");

            OnLogout?.Invoke();

        }

        public class AuthResult
        {
            public string Token { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            //public string Rolename { get; set; }
            //public string RefreshToken { get; set; }
            //public long tokenExp { get; set; }
        }
    }
}
