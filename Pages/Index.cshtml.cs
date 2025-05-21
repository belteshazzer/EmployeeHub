using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeHub.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    // Example property to display dynamic data on the page
    public string WelcomeMessage { get; private set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    // This method is called when the page is accessed via a GET request
    public void OnGet()
    {
        // Set a welcome message or perform other initialization logic
        WelcomeMessage = "Welcome to EmployeeHub!";
        _logger.LogInformation("Index page loaded successfully.");
    }
}