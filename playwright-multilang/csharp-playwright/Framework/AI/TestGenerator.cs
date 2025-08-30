using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using csharp_playwright.Framework.AI.Models;

namespace csharp_playwright.Framework.AI
{
    /// <summary>
    /// Generates test code using OpenAI models based on application context
    /// 
    /// This class is the core of the AI test generation framework. It:
    /// 1. Takes application context models as input
    /// 2. Constructs prompts for the OpenAI API
    /// 3. Makes API calls to generate test code
    /// 4. Processes and saves the generated tests
    /// 
    /// The generated tests are C# Playwright tests that can be directly
    /// executed in the project.
    /// </summary>
    public class TestGenerator
    {
        // HttpClient for making API calls to OpenAI
        private readonly HttpClient _httpClient;
        
        // Configuration settings for the AI integration
        private readonly AiConfig _aiConfig;
        
        /// <summary>
        /// Constructor - initializes the AI client and configuration
        /// 
        /// This constructor:
        /// 1. Loads configuration settings from appsettings.json
        /// 2. Sets up the HTTP client with authentication headers
        /// 3. Configures the client for OpenAI API communication
        /// 
        /// If the API key is missing or invalid, subsequent API calls will fail.
        /// </summary>
        public TestGenerator()
        {
            // Load configuration settings from appsettings.json
            // This includes API keys, model selection, and parameters
            _aiConfig = AiConfig.LoadFromConfig();
            
            // Set up HTTP client for OpenAI API calls
            // This client will be reused for all API requests
            _httpClient = new HttpClient();
            
            // Add the Authorization header with Bearer token
            // The API key must be valid or all calls will fail with 401 Unauthorized
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_aiConfig.ApiKey}");
        }
        
        /// <summary>
        /// Generates a test case using AI based on application context and test description
        /// 
        /// This is the main entry point for test generation. It:
        /// 1. Creates an AI prompt based on the application context
        /// 2. Sends the prompt to OpenAI API
        /// 3. Processes the response into usable test code
        /// 4. Falls back to pre-generated tests on API failures
        /// 
        /// The method handles API errors gracefully by providing fallback test code
        /// when the API is unavailable or returns errors.
        /// </summary>
        /// <param name="context">Application context model containing API endpoints or UI components</param>
        /// <param name="testCase">Description of the test case to generate (e.g., "Test POST endpoint with valid data")</param>
        /// <returns>Generated test code as a string, ready to be saved as a .cs file</returns>
        public async Task<string> GenerateTest(csharp_playwright.Framework.AI.Models.AppContext context, string testCase)
        {
            // Build the prompt with context information
            // This combines the application context with the test description
            // to give the AI a complete picture of what to generate
            var prompt = BuildTestGenerationPrompt(context, testCase);
            
            try
            {
                // SECURITY CHECK: Verify API key is properly configured
                // This prevents unnecessary API calls with invalid credentials
                if (string.IsNullOrWhiteSpace(_aiConfig.ApiKey) || 
                    _aiConfig.ApiKey == "your_api_key_here" ||
                    _aiConfig.ApiKey == "[SET_VIA_ENVIRONMENT_VARIABLE_OPENAI_API_KEY]")
                {
                    Console.WriteLine("\nOpenAI API key not found. Please set it using the OPENAI_API_KEY environment variable.");
                    Console.WriteLine("Example: export OPENAI_API_KEY=\"sk-your-key-here\"");
                    Console.WriteLine("Using pre-generated test instead.\n");
                    return GetFallbackTestCode(context, testCase);
                }
                
                // Create the request body for OpenAI's Chat Completions API
                // Note: This uses the chat completion format with system and user messages
                // which is more effective for code generation than the older completions API
                var requestBody = new
                {
                    model = _aiConfig.Model,      // The model to use (e.g., "gpt-4o")
                    messages = new[]              // Chat messages in the conversation
                    {
                        new
                        {
                            // System message defines the AI's role and capabilities
                            role = "system",
                            content = "You are an expert test automation engineer specializing in Playwright with C#."
                        },
                        new
                        {
                            // User message contains the actual prompt with context
                            role = "user",
                            content = prompt
                        }
                    },
                    max_tokens = _aiConfig.MaxTokens,      // Maximum response length
                    temperature = _aiConfig.Temperature    // Creativity vs determinism balance
                };
                
                // Serialize the request body to JSON
                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");
                
                // Send the request to OpenAI API
                // The endpoint for chat completions is different from the older completions API
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                
                // ERROR HANDLING: Handle unsuccessful API responses
                if (!response.IsSuccessStatusCode)
                {
                    // Log the error details for troubleshooting
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                    
                    // Common error codes and their meanings:
                    // 401 - Invalid API key or unauthorized
                    // 429 - Rate limit exceeded or quota exceeded
                    // 500 - Server error on OpenAI's side
                    
                    // Return fallback test code instead of throwing exception
                    // This ensures the application continues to function even when API fails
                    Console.WriteLine("Using pre-generated test code instead.");
                    return GetFallbackTestCode(context, testCase);
                }
                
                // Parse the successful API response
                var responseString = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<JsonElement>(responseString);
                
                // Extract the generated text from the response structure
                // For chat completions API, we need to navigate to choices[0].message.content
                string result = responseData
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "";
                    
                return result;
            }
            catch (Exception ex)
            {
                // EXCEPTION HANDLING: Log error and provide fallback
                Console.Error.WriteLine($"Error generating test with AI: {ex.Message}");
                Console.WriteLine("Using pre-generated test code instead.");
                return GetFallbackTestCode(context, testCase);
            }
        }
        
        /// <summary>
        /// Saves a generated test to the filesystem in the GeneratedTests directory
        /// 
        /// This method handles:
        /// 1. Converting test names into valid filenames
        /// 2. Creating the output directory if it doesn't exist
        /// 3. Writing the test code to the file
        /// 
        /// The tests are saved to a dedicated "GeneratedTests" directory to keep
        /// them separate from handwritten tests and make them easy to identify.
        /// </summary>
        /// <param name="testName">Name of the test case, used to generate the filename</param>
        /// <param name="testCode">The complete C# test code to save</param>
        /// <returns>Full path to the saved file for confirmation or further processing</returns>
        public string SaveGeneratedTest(string testName, string testCode)
        {
            // Sanitize the name for file system use by:
            // - Converting to lowercase for consistency
            // - Replacing spaces with hyphens for readability
            // - Removing characters that are invalid in filenames
            string sanitizedName = testName.ToLower().Replace(" ", "-")
                .Replace(",", "").Replace(".", "").Replace(":", "");
            
            // Create filename with proper .cs extension for C# code
            string fileName = $"{sanitizedName}.cs";
            
            // Determine file path - using a dedicated "GeneratedTests" directory
            // This keeps generated tests separate from handwritten ones
            string directory = Path.Combine(
                Directory.GetCurrentDirectory(),
                "GeneratedTests");
            
            // Create the directory if it doesn't exist
            // CreateDirectory is safe to call even if the directory exists
            Directory.CreateDirectory(directory);
            string filePath = Path.Combine(directory, fileName);
            
            // Write the complete test code to the file
            // This will overwrite any existing file with the same name
            File.WriteAllText(filePath, testCode);
            
            return filePath;
        }
        
        /// <summary>
        /// Provides a fallback test code when the API call fails or is not configured
        /// 
        /// This method ensures the framework is resilient to API failures by:
        /// 1. Providing pre-written test templates for common scenarios
        /// 2. Customizing the templates based on available context
        /// 3. Delivering usable test code even without AI generation
        /// 
        /// The fallback tests implement best practices for API testing with Playwright
        /// and can be used as examples or starting points for further customization.
        /// 
        /// This approach ensures users can still be productive even when:
        /// - OpenAI API keys are missing or invalid
        /// - Rate limits have been exceeded
        /// - Network connectivity issues occur
        /// - OpenAI services are temporarily unavailable
        /// </summary>
        /// <param name="context">Application context with API endpoints or UI components</param>
        /// <param name="testCase">Description of the test case requested</param>
        /// <returns>Pre-generated but customized test code as a string</returns>
        private string GetFallbackTestCode(csharp_playwright.Framework.AI.Models.AppContext context, string testCase)
        {
            // Extract API endpoint from context if available
            string endpoint = "posts";
            string method = "GET";
            
            if (context.ApiEndpoints?.Count > 0)
            {
                endpoint = context.ApiEndpoints[0].Path.TrimStart('/');
                method = context.ApiEndpoints[0].Method;
            }
            
            // Determine test name based on input
            string testName = testCase.Contains("post", StringComparison.OrdinalIgnoreCase) ? 
                "PostEndpoint_WithValidData" : 
                "GetEndpoint_ReturnsValidData";
            
            // Generate appropriate test based on HTTP method
            if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                return $@"using Microsoft.Playwright;
using Xunit;
using System.Text.Json;
using System.Threading.Tasks;
using csharp_playwright.Extensions;

namespace csharp_playwright.GeneratedTests
{{
    /// <summary>
    /// Tests for {method} requests to the JSONPlaceholder API
    /// This test was generated using AI-powered test generation
    /// </summary>
    public class {endpoint.Replace("/", "")}ApiTests
    {{
        [Fact]
        public async Task {testName}_CreatesNewResourceAndReturns201()
        {{
            // Arrange: Initialize Playwright and create a new API request context
            using var playwright = await Playwright.CreateAsync();
            var context = await playwright.APIRequest.NewContextAsync();

            // Create the payload for the POST request
            var payload = new
            {{
                title = ""Testing with Playwright"",
                body = ""This is a test created using AI-generated tests"",
                userId = 1
            }};

            // Act: Send POST request to the endpoint
            var response = await context.PostAsync(""https://jsonplaceholder.typicode.com/{endpoint}"", new()
            {{
                DataObject = payload
            }});

            // Assert: Verify correct status code and response data
            Assert.Equal(201, response.Status);
            
            // Parse the response JSON
            var jsonElement = await response.JsonAsync();
            JsonElement? json = jsonElement;
            
            // Verify the response contains the expected data
            Assert.Equal(""Testing with Playwright"", json.Value<string>(""title""));
            Assert.Equal(""This is a test created using AI-generated tests"", json.Value<string>(""body""));
            Assert.Equal(1, json.Value<int>(""userId""));
            
            // Verify an ID was assigned to the new resource
            Assert.True(json.ContainsKey(""id""));
            Assert.True(json.Value<int>(""id"") > 0);
        }}
    }}
}}";
            }
            else // GET or other methods
            {
                return $@"using Microsoft.Playwright;
using Xunit;
using System.Text.Json;
using System.Threading.Tasks;
using csharp_playwright.Extensions;

namespace csharp_playwright.GeneratedTests
{{
    /// <summary>
    /// Tests for {method} requests to the JSONPlaceholder API
    /// This test was generated using AI-powered test generation
    /// </summary>
    public class {endpoint.Replace("/", "")}ApiTests
    {{
        [Fact]
        public async Task {testName}_ReturnsExpectedStructure()
        {{
            // Arrange: Initialize Playwright and create a new API request context
            using var playwright = await Playwright.CreateAsync();
            var context = await playwright.APIRequest.NewContextAsync();

            // Act: Send GET request to the endpoint
            var response = await context.GetAsync(""https://jsonplaceholder.typicode.com/{endpoint}"");

            // Assert: Verify correct status code
            Assert.Equal(200, response.Status);
            
            // Parse the response JSON
            var jsonElement = await response.JsonAsync();
            
            // Handle both array and single object responses
            if (jsonElement.ValueKind == JsonValueKind.Array)
            {{
                // For collections (e.g., /posts), verify it's not empty
                var jsonArray = jsonElement.EnumerateArray();
                Assert.True(jsonArray.Count() > 0, ""API should return at least one item"");
                
                // Check first item has expected structure
                var firstItem = jsonArray.First();
                Assert.True(firstItem.TryGetProperty(""id"", out _));
                
                // If it's posts endpoint, check for title and body
                if (""posts"" == ""{endpoint}"")
                {{
                    Assert.True(firstItem.TryGetProperty(""title"", out _));
                    Assert.True(firstItem.TryGetProperty(""body"", out _));
                    Assert.True(firstItem.TryGetProperty(""userId"", out _));
                }}
            }}
            else
            {{
                // For single resource (e.g., /posts/1)
                JsonElement? json = jsonElement;
                Assert.True(json.ContainsKey(""id""));
                
                // If it's a post, check for expected properties
                if (""posts"" == ""{endpoint.Split('/')[0]}"")
                {{
                    Assert.True(json.ContainsKey(""title""));
                    Assert.True(json.ContainsKey(""body""));
                    Assert.True(json.ContainsKey(""userId""));
                }}
            }}
        }}
    }}
}}";
            }
        }

        /// <summary>
        /// Builds the prompt to send to the AI model with full context
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="testCase">Test case description</param>
        /// <returns>Complete prompt text</returns>
        private string BuildTestGenerationPrompt(csharp_playwright.Framework.AI.Models.AppContext context, string testCase)
        {
            var sb = new StringBuilder();
            
            // Add the base test generation prompt
            sb.AppendLine(_aiConfig.TestGenerationPrompt);
            sb.AppendLine();
            
            // Add application context information
            sb.AppendLine("APPLICATION CONTEXT:");
            sb.AppendLine($"API/Page: {context.PageName} ({context.Url})");
            sb.AppendLine();
            
            // Add API endpoint information for API testing
            if (context.ApiEndpoints?.Count > 0)
            {
                sb.AppendLine("API Endpoints:");
                foreach (var api in context.ApiEndpoints)
                {
                    sb.AppendLine($"- {api.Method} {api.Path}");
                    
                    // Add parameters if available
                    if (api.Parameters?.Count > 0)
                    {
                        sb.AppendLine("  Parameters:");
                        foreach (var param in api.Parameters)
                        {
                            sb.AppendLine($"    {param.Key}: {param.Value}");
                        }
                    }
                    
                    // Add response example if available
                    if (api.ResponseExample != null)
                    {
                        sb.AppendLine("  Example Response:");
                        sb.AppendLine($"    {JsonSerializer.Serialize(api.ResponseExample)}");
                    }
                }
                sb.AppendLine();
            }
            
            // Add UI components if doing UI testing
            if (context.Components?.Count > 0)
            {
                sb.AppendLine("UI Components:");
                foreach (var component in context.Components)
                {
                    sb.AppendLine($"- {component.Name}: {component.Type} ({component.Selector})");
                }
                sb.AppendLine();
            }
            
            // Specify the test case to implement
            sb.AppendLine("TEST CASE TO IMPLEMENT:");
            sb.AppendLine(testCase);
            sb.AppendLine();
            
            // Add specific instructions
            sb.AppendLine("Please generate a complete Playwright C# test that implements this test case.");
            sb.AppendLine("Include proper using statements, test structure, comments, and assertions.");
            sb.AppendLine("The test should follow the style and patterns in the existing tests in this codebase.");
            sb.AppendLine("Use xUnit for the test framework.");
            
            return sb.ToString();
        }
    }
}
