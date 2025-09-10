using MauiApp2.Models;
using MauiApp2.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Storage;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
namespace MauiApp2.Service
{
    public class ImageService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly NavigationManager _navigationManager;
        public ImageService(HttpClient httpClient, AuthService authService, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _authService = authService;
            _navigationManager = navigationManager;
        }

        /// <summary>
        /// Sets the Authorization header if an auth token exists in secure storage.
        /// </summary>
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
        //private async Task<HttpResponseMessage> SendWithRefreshRetry(Func<Task<HttpResponseMessage>> sendRequest)
        //{
        //    await SetAuthorizationHeaderIfNeeded();
        //    var response = await sendRequest();

        //    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //    {
        //        var refreshed = await _authService.TryRefreshTokenAsync();
        //        if (refreshed)
        //        {
        //            // Retry original request
        //            response = await sendRequest();
        //        }
        //    }

        //    return response;
        //}

        public async Task<List<ImageModel>> GetReportAsync()
        {
            try
            {
                await SetAuthorizationHeaderIfNeeded();
                var response = await _httpClient.GetAsync("api/ImgData/report");
                response.EnsureSuccessStatusCode();

                var reportData = await response.Content.ReadFromJsonAsync<List<ImageModel>>();
                return reportData ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Failed to fetch report data: " + ex.Message);
                throw;
            }
        }
        public async Task<string> GetHtmlReportAsync(string? user = null)
        {
            string url = "api/ImgData/report/html";
            if (!string.IsNullOrWhiteSpace(user))
                url += $"?user={Uri.EscapeDataString(user)}";

            return await _httpClient.GetStringAsync(url);
        }


        /// <summary>
        /// Fetches the list of all image records from the API.
        /// </summary>
        public async Task<ImageResponse> GetImaggeAsync()
        {
            try
            {
                //if (!await _authService.EnsureValidTokenAsync())
                //    throw new Exception("User is logged out.");
                await SetAuthorizationHeaderIfNeeded();
                string? UserName = await SecureStorage.GetAsync("UserName");
                string? role = await SecureStorage.GetAsync("role");

                string url = "api/ImgData";

                // If not admin, filter by user
                if (!string.IsNullOrEmpty(role) && role != "Admin")
                {
                    url += $"?user={Uri.EscapeDataString(UserName)}";
                }

                var response = await _httpClient.GetAsync(url);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    //    SecureStorage.Remove("authToken");
                    //    _navigationManager.NavigateTo("/", true);
                    //    return null;
                    _authService.LogoutAsync();

                }
                response.EnsureSuccessStatusCode();

                var imageResponse = await response.Content.ReadFromJsonAsync<ImageResponse>();
                if (imageResponse == null)
                {
                    throw new Exception("Failed to parse image data.");
                }

                return imageResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching images: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Adds a new image record to the API.
        /// </summary>
        public async Task<ImageModel> AddImageAsync(string imageBase64, string extractedText, string location, string ImgDataUserUploaded, string ImgDataCreateUid, string ImgDataEditedUid)
        {
            try
            {
                await SetAuthorizationHeaderIfNeeded();

                var base64 = imageBase64?.Replace("data:image/jpeg;base64,", "") ?? "";
                var imageBytes = Convert.FromBase64String(base64);

                using var content = new MultipartFormDataContent();

                // Add the image as a file
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(imageContent, "ImageFile", "capture.jpeg");

                // Add other form fields
                content.Add(new StringContent(location ?? ""), "Location");
                content.Add(new StringContent(extractedText ?? ""), "ExtractedText");
                content.Add(new StringContent(ImgDataUserUploaded ?? ""), "ImgDataUserUploaded");
                content.Add(new StringContent(ImgDataCreateUid ?? ""), "ImgDataCreateUid");
                content.Add(new StringContent(ImgDataEditedUid ?? ""), "ImgDataEditedUid");
                //content.Add(new StringContent(base64 ?? ""), "base64");


                var response = await _httpClient.PostAsync("api/ImgData/upload-base64", content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine("⬅️ Response JSON:\n" + responseJson);

                var savedImage = await response.Content.ReadFromJsonAsync<ImageModel>();
                if (savedImage == null)
                    throw new Exception("Upload succeeded but parsing response failed.");

                return savedImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Upload failed: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Updates an existing image record.
        /// </summary>
        //public async Task UpdateImageAsync(ImageModel image)
        //{
        //    try
        //    {
        //        await SetAuthorizationHeaderIfNeeded();
        //        var response = await _httpClient.PutAsJsonAsync($"api/ImgData/{image.ImgDataId}", image);
        //        response.EnsureSuccessStatusCode();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error updating image with ID {image.ImgDataId}: {ex.Message}", ex);
        //    }
        //}

        /// <summary>
        /// Deletes an image record by ID.
        /// </summary>
        public async Task DeleteImageAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderIfNeeded();
                var response = await _httpClient.DeleteAsync($"api/ImgData/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting image with ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Optional: Gets a single image by ID (for editing).
        /// </summary>
        public async Task<ImageModel?> GetImageByIdAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderIfNeeded();
                var response = await _httpClient.GetAsync($"api/ImgData/{id}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ImageModel>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching image with ID {id}: {ex.Message}", ex);
            }
        }
    }
}
