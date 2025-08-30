using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Playwright;
using csharp_playwright.Framework.AI;
using csharp_playwright.Framework.AI.Models;

namespace csharp_playwright
{
    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("AI-Powered Playwright Test Generator");
            Console.WriteLine("====================================");
            
            try 
            {
                // Initialize Playwright
                Console.WriteLine("Initializing Playwright...");
                using var playwright = await Playwright.CreateAsync();
                var requestContext = await playwright.APIRequest.NewContextAsync();
                
                // Set up API test context
                string baseUrl = "https://jsonplaceholder.typicode.com";
                string apiName = "JSONPlaceholder API";
                string testDescription = "Test that posting to /posts endpoint with title, body, and userId creates a new post and returns status 201";
                
                Console.WriteLine($"Base URL: {baseUrl}");
                Console.WriteLine($"API Name: {apiName}");
                Console.WriteLine($"Test to generate: {testDescription}");
                
                // Create a manual API context since we already know the endpoints
                var appContext = new csharp_playwright.Framework.AI.Models.AppContext
                {
                    Url = baseUrl,
                    PageName = apiName,
                    ApiEndpoints = new List<ApiEndpoint>
                    {
                        new ApiEndpoint
                        {
                            Path = "/posts",
                            Method = "POST",
                            Parameters = new Dictionary<string, object>
                            {
                                { "title", "string" },
                                { "body", "string" },
                                { "userId", "integer" }
                            },
                            ResponseExample = new { id = 101, title = "Sample", body = "Sample body", userId = 1 }
                        }
                    }
                };
                
                // Generate test with AI
                Console.WriteLine("\nGenerating test case with AI...");
                var testGenerator = new TestGenerator();
                string testCode = await testGenerator.GenerateTest(appContext, testDescription);
                
                // Save generated test
                Console.WriteLine("\nSaving generated test...");
                string testFilePath = testGenerator.SaveGeneratedTest("generated-post-test", testCode);
                Console.WriteLine($"Test saved to: {testFilePath}");
                
                // Show preview
                Console.WriteLine("\nGenerated Test Preview:");
                Console.WriteLine("------------------------------------");
                Console.WriteLine(testCode.Length > 500 ? testCode.Substring(0, 500) + "..." : testCode);
                
                Console.WriteLine("\nTest generation complete!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"\nInner exception: {ex.InnerException.Message}");
                }
            }
        }
    }
}
