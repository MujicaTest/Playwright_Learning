using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System;
using System.IO;

namespace csharp_playwright.Framework.AI
{
    /// <summary>
    /// Manages configuration settings for the AI test generation system.
    /// Contains API keys, model parameters and prompt templates.
    /// 
    /// This class serves as the central configuration hub for all AI-related settings.
    /// It reads from appsettings.json and provides default values when settings are missing.
    /// 
    /// Usage:
    /// 1. Ensure appsettings.json has an "AI" section with required parameters
    /// 2. Call AiConfig.LoadFromConfig() to create a configured instance
    /// </summary>
    public class AiConfig
    {
        /// <summary>
        /// OpenAI API key for authentication
        /// 
        /// SECURITY IMPORTANT:
        /// - Never commit this value directly to source control
        /// - Use environment variables or secure vaults in production
        /// - For development, store in user secrets or local appsettings.json (git-ignored)
        /// 
        /// If invalid, API calls will return 401 Unauthorized errors
        /// </summary>
        public string ApiKey { get; set; }
        
        /// <summary>
        /// AI model to use (e.g., "gpt-4", "gpt-4o", "gpt-3.5-turbo")
        /// 
        /// Different models have different capabilities and costs:
        /// - GPT-4: Most capable, but more expensive and slower
        /// - GPT-3.5-turbo: Faster and cheaper, but less sophisticated
        /// 
        /// Check OpenAI documentation for current models and capabilities
        /// </summary>
        public string Model { get; set; }
        
        /// <summary>
        /// Maximum tokens in AI response
        /// 
        /// A token is roughly 4 characters in English.
        /// For test generation, recommended values:
        /// - 2000: Simple tests
        /// - 4000: Moderate complexity tests (default)
        /// - 8000: Complex tests with many assertions
        /// 
        /// Higher values increase API costs
        /// </summary>
        public int MaxTokens { get; set; }
        
        /// <summary>
        /// Temperature controls randomness (0.0-1.0)
        /// 
        /// Lower values produce more deterministic outputs:
        /// - 0.0-0.3: Very consistent, predictable code
        /// - 0.5-0.7: Good balance for code generation (default: 0.7)
        /// - 0.8-1.0: More creative but potentially less reliable code
        /// </summary>
        public float Temperature { get; set; }
        
        /// <summary>
        /// Base prompt template for test generation
        /// 
        /// This template guides the AI in generating tests.
        /// It can contain special markers that will be replaced with context
        /// from the application being tested.
        /// 
        /// Good prompts specify:
        /// - Test structure requirements
        /// - Assertion styles
        /// - Coding standards to follow
        /// - Documentation expectations
        /// </summary>
        public string TestGenerationPrompt { get; set; }
        
        /// <summary>
        /// Loads configuration from multiple sources in order of priority:
        /// 1. Environment variables (highest priority)
        /// 2. appsettings.json file
        /// 3. Default values (lowest priority)
        /// 
        /// This approach follows security best practices by allowing sensitive
        /// information like API keys to be stored in environment variables
        /// rather than in source-controlled files.
        /// 
        /// Environment variable naming convention:
        /// - OPENAI_API_KEY: API key for OpenAI
        /// - OPENAI_MODEL: Model name to use (e.g. "gpt-4o")
        /// - OPENAI_MAX_TOKENS: Maximum token limit
        /// - OPENAI_TEMPERATURE: Temperature setting (0.0-1.0)
        /// 
        /// Expected JSON structure in appsettings.json:
        /// {
        ///   "AI": {
        ///     "ApiKey": "your-api-key-here", // Better to use env vars instead
        ///     "Model": "gpt-4o",
        ///     "MaxTokens": 4000,
        ///     "Temperature": 0.7,
        ///     "TestGenerationPrompt": "Your prompt template here"
        ///   }
        /// }
        /// </summary>
        /// <returns>Populated AiConfig instance with settings from environment variables, appsettings.json, or defaults</returns>
        public static AiConfig LoadFromConfig()
        {
            // Create configuration builder and load from JSON and environment variables
            // The configuration system will automatically look for environment variables
            // SetBasePath ensures we look in the correct directory for the config file
            // AddJsonFile loads settings with optional=true meaning the app won't crash if file is missing
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables() // Add environment variables as a source
                .Build();
            
            // Create and populate config from settings
            // Each property uses a prioritized approach to configuration:
            // 1. Try environment variable first
            // 2. Fall back to appsettings.json
            // 3. Use default value as last resort
            return new AiConfig
            {
                // API Key is required for OpenAI authentication
                // First check environment variable OPENAI_API_KEY (most secure approach)
                // Then fall back to appsettings.json if not found in environment
                ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                         config["AI:ApiKey"] ?? 
                         string.Empty,
                
                // Model selection - prioritize environment variable, then config, then default
                Model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? 
                        config["AI:Model"] ?? 
                        "gpt-4",
                
                // MaxTokens - parse from environment variable if available
                MaxTokens = ParseIntSafe(
                    Environment.GetEnvironmentVariable("OPENAI_MAX_TOKENS"),
                    config["AI:MaxTokens"] ?? "4000"),
                
                // Temperature - parse from environment variable if available
                Temperature = ParseFloatSafe(
                    Environment.GetEnvironmentVariable("OPENAI_TEMPERATURE"),
                    config["AI:Temperature"] ?? "0.7"),
                
                // Default prompt template that guides AI test generation
                // This is less sensitive, so environment variable is less critical
                TestGenerationPrompt = Environment.GetEnvironmentVariable("OPENAI_PROMPT_TEMPLATE") ?? 
                                       config["AI:TestGenerationPrompt"] ?? 
                                       @"
                    Given the application context, generate a Playwright C# test that:
                    1. Validates the key functionality described
                    2. Uses proper assertions with xUnit
                    3. Follows best practices for API test isolation
                    4. Includes appropriate comments
                "
            };
        }
        
        /// <summary>
        /// Safely parses an integer from a string, with a fallback value if parsing fails
        /// </summary>
        /// <param name="value">String to parse</param>
        /// <param name="fallback">Fallback value if parsing fails</param>
        /// <returns>Parsed integer or fallback value</returns>
        private static int ParseIntSafe(string? value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return int.Parse(fallback);
            }
            
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            
            return int.Parse(fallback);
        }
        
        /// <summary>
        /// Safely parses a float from a string, with a fallback value if parsing fails
        /// </summary>
        /// <param name="value">String to parse</param>
        /// <param name="fallback">Fallback value if parsing fails</param>
        /// <returns>Parsed float or fallback value</returns>
        private static float ParseFloatSafe(string? value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return float.Parse(fallback);
            }
            
            if (float.TryParse(value, out float result))
            {
                return result;
            }
            
            return float.Parse(fallback);
        }
    }
}
