using Microsoft.Playwright;
using Xunit;
using System.Text.Json;
using System.Threading.Tasks;
using csharp_playwright.Extensions;

namespace csharp_playwright.GeneratedTests
{
    /// <summary>
    /// Tests for GET requests to the JSONPlaceholder API
    /// This test was generated using AI-powered test generation
    /// </summary>
    public class posts{id}ApiTests
    {
        [Fact]
        public async Task PostEndpoint_WithValidData_ReturnsExpectedStructure()
        {
            // Arrange: Initialize Playwright and create a new API request context
            using var playwright = await Playwright.CreateAsync();
            var context = await playwright.APIRequest.NewContextAsync();

            // Act: Send GET request to the endpoint
            var response = await context.GetAsync("https://jsonplaceholder.typicode.com/posts/{id}");

            // Assert: Verify correct status code
            Assert.Equal(200, response.Status);
            
            // Parse the response JSON
            var jsonElement = await response.JsonAsync();
            
            // Handle both array and single object responses
            if (jsonElement.ValueKind == JsonValueKind.Array)
            {
                // For collections (e.g., /posts), verify it's not empty
                var jsonArray = jsonElement.EnumerateArray();
                Assert.True(jsonArray.Count() > 0, "API should return at least one item");
                
                // Check first item has expected structure
                var firstItem = jsonArray.First();
                Assert.True(firstItem.TryGetProperty("id", out _));
                
                // If it's posts endpoint, check for title and body
                if ("posts" == "posts/{id}")
                {
                    Assert.True(firstItem.TryGetProperty("title", out _));
                    Assert.True(firstItem.TryGetProperty("body", out _));
                    Assert.True(firstItem.TryGetProperty("userId", out _));
                }
            }
            else
            {
                // For single resource (e.g., /posts/1)
                JsonElement? json = jsonElement;
                Assert.True(json.ContainsKey("id"));
                
                // If it's a post, check for expected properties
                if ("posts" == "posts")
                {
                    Assert.True(json.ContainsKey("title"));
                    Assert.True(json.ContainsKey("body"));
                    Assert.True(json.ContainsKey("userId"));
                }
            }
        }
    }
}