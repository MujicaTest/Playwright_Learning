using System;
using System.Text.Json;

namespace csharp_playwright.Extensions
{
    /// <summary>
    /// Extension methods for working with System.Text.Json.JsonElement
    /// 
    /// These extensions simplify the process of extracting and validating data
    /// from JSON responses in API tests. They provide a more concise syntax
    /// for common operations when working with JsonElement objects.
    /// 
    /// The extensions handle nullable JsonElement to prevent NullReferenceExceptions
    /// and provide clean type conversion for various JSON property types.
    /// </summary>
    public static class JsonElementExtensions
    {
        /// <summary>
        /// Gets a property value from JsonElement with specified type conversion
        /// 
        /// This extension simplifies accessing JSON properties with automatic type conversion.
        /// It's particularly useful in test assertions to extract and verify values.
        /// 
        /// Example usage:
        /// ```csharp
        /// var json = await response.JsonAsync();
        /// string title = json.Value<string>("title");
        /// int userId = json.Value<int>("userId");
        /// bool isActive = json.Value<bool>("isActive");
        /// ```
        /// 
        /// The method handles nullable JsonElement and missing properties gracefully
        /// by returning default(T) in those cases.
        /// </summary>
        /// <typeparam name="T">Target type to convert the JSON value to (string, int, bool, etc.)</typeparam>
        /// <param name="element">The JsonElement to extract value from (can be null)</param>
        /// <param name="propertyName">Name of the JSON property to access</param>
        /// <returns>The property value converted to the specified type, or default(T) if not found</returns>
        public static T? Value<T>(this JsonElement? element, string propertyName)
        {
            // Handle null element or property not found cases
            if (element == null || !element.Value.TryGetProperty(propertyName, out var property))
                return default;

            // Convert the raw JSON text to the requested type
            // Trim quotes for string values to get the clean content
            return (T)Convert.ChangeType(property.GetRawText().Trim('"'), typeof(T));
        }

        /// <summary>
        /// Checks if JsonElement contains a property with the specified name
        /// 
        /// This extension provides a simple way to verify the existence of properties
        /// in a JSON response without having to handle exceptions or complex checks.
        /// 
        /// Example usage:
        /// ```csharp
        /// var json = await response.JsonAsync();
        /// Assert.True(json.ContainsKey("id"));
        /// Assert.False(json.ContainsKey("nonExistentProperty"));
        /// ```
        /// 
        /// Particularly useful for testing API responses where certain fields
        /// must be present, even if the exact values are unpredictable.
        /// </summary>
        /// <param name="element">The JsonElement to check (can be null)</param>
        /// <param name="propertyName">Name of the JSON property to check for</param>
        /// <returns>True if the property exists, false otherwise or if element is null</returns>
        public static bool ContainsKey(this JsonElement? element, string propertyName)
        {
            // Handle null element case
            if (element == null)
                return false;

            // Try to get the property but discard the output (using _ discard)
            // TryGetProperty returns true if the property exists, false otherwise
            return element.Value.TryGetProperty(propertyName, out _);
        }
    }
}
