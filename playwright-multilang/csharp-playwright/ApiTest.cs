using Microsoft.Playwright;
using Xunit;
using System.Threading.Tasks;
using csharp_playwright.Extensions;
using System.Text.Json;

public class ApiTests
{
    [Fact]
    public async Task GetPost_ReturnsPostWithId1()
    {
        using var playwright = await Playwright.CreateAsync();
        var requestContext = await playwright.APIRequest.NewContextAsync();
        var response = await requestContext.GetAsync("https://jsonplaceholder.typicode.com/posts/1");
        Assert.Equal(200, response.Status);
        
        var jsonElement = await response.JsonAsync();
        JsonElement? json = jsonElement;
        Assert.Equal(1, json.Value<int>("id"));
        Assert.Equal(1, json.Value<int>("userId"));
        Assert.True(json.ContainsKey("title"));
    }
}