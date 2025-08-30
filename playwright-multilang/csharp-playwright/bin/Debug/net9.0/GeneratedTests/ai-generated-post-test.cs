using Microsoft.Playwright;
using Xunit;
using System.Text.Json;
using System.Threading.Tasks;
using csharp_playwright.Extensions;

namespace csharp_playwright.GeneratedTests
{
    /// <summary>
    /// Tests for POST requests to the JSONPlaceholder API
    /// This test was generated using AI-powered test generation
    /// </summary>
    public class postsApiTests
    {
        [Fact]
        public async Task PostEndpoint_WithValidData_CreatesNewResourceAndReturns201()
        {
            // Arrange: Initialize Playwright and create a new API request context
            using var playwright = await Playwright.CreateAsync();
            var context = await playwright.APIRequest.NewContextAsync();

            // Create the payload for the POST request
            var payload = new
            {
                title = "Testing with Playwright",
                body = "This is a test created using AI-generated tests",
                userId = 1
            };

            // Act: Send POST request to the endpoint
            var response = await context.PostAsync("https://jsonplaceholder.typicode.com/posts", new()
            {
                DataObject = payload
            });

            // Assert: Verify correct status code and response data
            Assert.Equal(201, response.Status);
            
            // Parse the response JSON
            var jsonElement = await response.JsonAsync();
            JsonElement? json = jsonElement;
            
            // Verify the response contains the expected data
            Assert.Equal("Testing with Playwright", json.Value<string>("title"));
            Assert.Equal("This is a test created using AI-generated tests", json.Value<string>("body"));
            Assert.Equal(1, json.Value<int>("userId"));
            
            // Verify an ID was assigned to the new resource
            Assert.True(json.ContainsKey("id"));
            Assert.True(json.Value<int>("id") > 0);
        }
    }
}