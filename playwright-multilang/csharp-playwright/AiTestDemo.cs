using Microsoft.Playwright;
using Xunit;
using csharp_playwright.Framework.AI;
using csharp_playwright.Framework.AI.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace csharp_playwright
{
    /// <summary>
    /// Demonstrates the AI test generation capabilities by creating tests similar to existing ones
    /// </summary>
    public class AiTestDemo
    {
        /// <summary>
        /// Demonstrates generating a test for JSONPlaceholder API POST endpoint
        /// based on the existing ApiPostTest.cs
        /// </summary>
        [Fact]
        public async Task DemonstratePostTestGeneration()
        {
            // Initialize Playwright (similar to the existing tests)
            using var playwright = await Playwright.CreateAsync();
            var requestContext = await playwright.APIRequest.NewContextAsync();
            
            // Create context model for API test, similar to existing ApiPostTest.cs
            var apiContext = new csharp_playwright.Framework.AI.Models.AppContext
            {
                Url = "https://jsonplaceholder.typicode.com",
                PageName = "JSONPlaceholder API",
                ApiEndpoints = new List<ApiEndpoint>
                {
                    // Define POST endpoint similar to existing test
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
                        ResponseExample = new { id = 101, title = "QA Automation", body = "Demo from Playwright", userId = 42 }
                    }
                }
            };
            
            // Test description based on existing test
            string testDescription = "Test that posting to /posts endpoint with title='Test Title', body='Test Body', and userId=1 creates a new post, " +
                                    "returns status code 201, and includes the correct title, body, userId in the response";
            
            try
            {
                // Generate test with AI
                Console.WriteLine("Generating test case with AI based on your existing ApiPostTest...");
                var testGenerator = new TestGenerator();
                string testCode = await testGenerator.GenerateTest(apiContext, testDescription);
                
                // Save generated test
                string testFilePath = testGenerator.SaveGeneratedTest("ai-generated-post-test", testCode);
                Console.WriteLine($"Test saved to: {testFilePath}");
                
                // Output part of the generated code
                Console.WriteLine("\nGenerated Test Preview:");
                Console.WriteLine(testCode.Length > 500 ? testCode.Substring(0, 500) + "..." : testCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Demonstrates generating a test for JSONPlaceholder API GET endpoint
        /// based on the existing ApiTest.cs
        /// </summary>
        [Fact]
        public async Task DemonstrateGetTestGeneration()
        {
            // Initialize Playwright (similar to the existing tests)
            using var playwright = await Playwright.CreateAsync();
            var requestContext = await playwright.APIRequest.NewContextAsync();
            
            // Create context model for API test, similar to existing ApiTest.cs
            var apiContext = new csharp_playwright.Framework.AI.Models.AppContext
            {
                Url = "https://jsonplaceholder.typicode.com",
                PageName = "JSONPlaceholder API",
                ApiEndpoints = new List<ApiEndpoint>
                {
                    // Define GET endpoint similar to existing test
                    new ApiEndpoint
                    {
                        Path = "/posts/{id}",
                        Method = "GET",
                        ResponseExample = new { id = 1, userId = 1, title = "Example title", body = "Example body" }
                    }
                }
            };
            
            // Test description based on existing test
            string testDescription = "Test that getting a post with ID 1 returns status code 200, " +
                                    "and the response includes id=1, userId=1, and has a title field";
            
            try
            {
                // Generate test with AI
                Console.WriteLine("Generating test case with AI based on your existing ApiTest...");
                var testGenerator = new TestGenerator();
                string testCode = await testGenerator.GenerateTest(apiContext, testDescription);
                
                // Save generated test
                string testFilePath = testGenerator.SaveGeneratedTest("ai-generated-get-test", testCode);
                Console.WriteLine($"Test saved to: {testFilePath}");
                
                // Output part of the generated code
                Console.WriteLine("\nGenerated Test Preview:");
                Console.WriteLine(testCode.Length > 500 ? testCode.Substring(0, 500) + "..." : testCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
