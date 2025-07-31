using Microsoft.Playwright;
using Xunit;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

public class ApiPostTests
{
    [Fact]
    public async Task PostCreatesNewPost()
    {
        using var playwright = await Playwright.CreateAsync();
        var context = await playwright.APIRequest.NewContextAsync();

        var payload = new
        {
            title = "QA Automation",
            body = "Demo from Playwright",
            userId = 42
        };

        var response = await context.PostAsync("https://jsonplaceholder.typicode.com/posts", new()
        {
            DataObject = payload
        });

        Assert.Equal(201, response.Status);
        var json = await response.JsonAsync();
        Assert.Equal("QA Automation", json?.Value<string>("title"));
        Assert.Equal(42, json?.Value<int>("userId"));
        Assert.True(json?.ContainsKey("id"));
    }
}