using EmployeeHub.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using EmployeeHub.Models.Entities;
using EmployeeHub.Common.ApiResponse;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using RLIMS.Services.ChatService;

namespace EmployeeHub.Pages.ChatHub
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IChatService _chatService;

        public IndexModel(IHttpClientFactory httpClientFactory, IChatService chatService)
        {
            _httpClientFactory = httpClientFactory;
            _chatService = chatService;
        }

        // Chat history for the main chat space
        public List<ChatHistory> ChatHistory { get; set; } = new List<ChatHistory>();
        public Guid CurrentUserId { get; set; }
        public string CurrentUserName { get; set; } = string.Empty;
        public Guid OtherUserId { get; set; }
        public string OtherUserName { get; set; } = string.Empty;
        public async Task<IActionResult> OnGetChatListAsync()
        {
            string apiUrl = "http://localhost:5139/api/chat/chat-list";
            Console.WriteLine("Fetching chat list from API: " + apiUrl);

            try
            {
                var client = _httpClientFactory.CreateClient();

                // Retrieve the token from the Authorization header
                var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Extract claims
                CurrentUserId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
                CurrentUserName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;


                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token is missing.");
                    return Unauthorized();
                }

                // Add the token to the request headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await client.GetAsync(apiUrl);
                Console.WriteLine("API response status code: " + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("API response content: " + jsonResponse);

                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<Chat>>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new JsonResult(apiResponse?.Data ?? new List<Chat>());
                }
                else
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching chat list: {ex.Message}");
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        public async Task<IActionResult> OnGetChatHistoryAsync(Guid chatId)
{
    string apiUrl = $"http://localhost:5139/api/chat/chat-history/{chatId}";
    Console.WriteLine("Fetching chat history for Chat ID: " + chatId);

    try
    {
        var client = _httpClientFactory.CreateClient();
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await client.GetAsync(apiUrl);
        Console.WriteLine("API response status code: " + response.StatusCode);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<ChatHistory>>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            ChatHistory = apiResponse?.Data ?? new List<ChatHistory>();

            // Get current user ID from token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            CurrentUserId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);

            // Fetch chat details
            var chat = await _chatService.GetChatByIdAsync(chatId);
            Console.WriteLine($"Chat object: {chat}");
            Console.WriteLine($"Chat.User1Id: {chat?.User1Id}, Chat.User2Id: {chat?.User2Id}");

            if (chat == null)
            {
                Console.WriteLine($"Chat with ID {chatId} not found.");
                return Partial("_ChatHistory", new List<ChatHistory>());
            }

            OtherUserId = chat.User1Id == CurrentUserId ? chat.User2Id : chat.User1Id;
            OtherUserName = chat.User1Id == CurrentUserId
                ? chat.User2?.FirstName + " " + chat.User2?.LastName
                : chat.User1?.FirstName + " " + chat.User1?.LastName;

            Console.WriteLine($"OtherUserId: {OtherUserId}, OtherUserName: {OtherUserName}");

            ViewData["CurrentUserId"] = CurrentUserId;

            return Partial("_ChatHistory", ChatHistory);
        }
        else
        {
            Console.WriteLine($"API Error: {response.StatusCode}");
            return Partial("_ChatHistory", new List<ChatHistory>());
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching chat history: {ex.Message}");
        return Partial("_ChatHistory", new List<ChatHistory>());
    }
}
    }
}