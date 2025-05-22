using EmployeeHub.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EmployeeHub.Models.Entities;
using EmployeeHub.Common.ApiResponse;

namespace EmployeeHub.Pages.ChatHub
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Chat list for the sidebar
        public List<Chat> ChatList { get; set; } = new List<Chat>();

        // Chat history for the main chat space
        public List<ChatHistory> ChatHistory { get; set; } = new List<ChatHistory>();

        public async Task OnGetAsync()
        {
            string apiUrl = "http://localhost:5139/api/chat/chat-list/{userId}";

            try
            {
                var client = _httpClientFactory.CreateClient();
                var userId = "491A1237-78A0-44E1-B31F-250718611219"; // Replace with the actual user ID
                var response = await client.GetAsync(apiUrl.Replace("{userId}", userId));

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserialize into ApiResponse<List<Chat>>
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<Chat>>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Extract the data property
                    ChatList = apiResponse?.Data ?? new List<Chat>();
                }
                else
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    ChatList = new List<Chat>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching chat list: {ex.Message}");
                ChatList = new List<Chat>();
            }
        }

        public async Task<IActionResult> OnGetChatHistoryAsync(Guid chatId)
        {
            // Replace with your actual API endpoint
            string apiUrl = $"http://localhost:5139/api/chat/chat-history/{chatId}";

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var ApiResponse = JsonSerializer.Deserialize<ApiResponse<List<ChatHistory>>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    ChatHistory = ApiResponse?.Data ?? [];
                }
                else
                {
                    // Handle API error response
                    ChatHistory = [];
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network issues)
                Console.WriteLine($"Error fetching chat history: {ex.Message}");
                ChatHistory = new List<ChatHistory>();
            }

            return Partial("_ChatHistory", ChatHistory);
        }
    }
}