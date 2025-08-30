using System.Collections.Generic;

namespace csharp_playwright.Framework.AI.Models
{
    /// <summary>
    /// Main context model that captures all necessary information
    /// about an application to generate relevant tests.
    /// 
    /// This class serves as a comprehensive representation of application structure
    /// that can be fed to an AI model to generate meaningful tests. It supports both:
    /// - API testing: Through documented endpoints, methods and responses
    /// - UI testing: Through components, selectors and user flows
    /// 
    /// This model follows the "Model Context Protocol" pattern where we build a rich 
    /// context model that an AI can understand to generate high-quality, relevant tests.
    /// 
    /// Usage:
    /// 1. Create an instance of this class
    /// 2. Populate it with discovered components, flows or API endpoints
    /// 3. Pass it to TestGenerator to create tests
    /// </summary>
    public class AppContext
    {
        /// <summary>
        /// Target URL of the application or API
        /// 
        /// Examples:
        /// - "https://jsonplaceholder.typicode.com" (for API testing)
        /// - "https://www.example.com/login" (for UI testing)
        /// 
        /// This should be the base URL that tests will interact with
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// Descriptive name of the page or API
        /// 
        /// This name helps in generating meaningful test names and descriptions.
        /// Examples:
        /// - "JSONPlaceholder API"
        /// - "User Authentication Page"
        /// </summary>
        public string PageName { get; set; }
        
        /// <summary>
        /// UI components identified on the page (for UI testing)
        /// 
        /// This collection contains all interactive elements on a page
        /// with their selectors and properties. Used primarily for UI testing.
        /// 
        /// For API testing scenarios, this can be left empty.
        /// </summary>
        public List<Component> Components { get; set; } = new List<Component>();
        
        /// <summary>
        /// Common user flows identified (for UI testing)
        /// 
        /// This collection describes common interaction patterns that users
        /// perform on the application, such as "Login", "Purchase Item", etc.
        /// 
        /// Each flow contains steps and expected outcomes to guide test generation.
        /// </summary>
        public List<UserFlow> UserFlows { get; set; } = new List<UserFlow>();
        
        /// <summary>
        /// API endpoints identified (for API testing)
        /// 
        /// This collection contains all discovered API endpoints with their
        /// methods, parameters, and example responses. Used primarily for API testing.
        /// 
        /// For UI testing scenarios, this can be left empty.
        /// </summary>
        public List<ApiEndpoint> ApiEndpoints { get; set; } = new List<ApiEndpoint>();
    }
    
    /// <summary>
    /// Represents a UI component on a page for UI test generation
    /// 
    /// Each component maps to an interactive element on a web page
    /// that tests might need to interact with. This model provides
    /// all the information needed for the AI to generate accurate
    /// element selection and interaction code.
    /// 
    /// Example components:
    /// - Login button
    /// - Username field
    /// - Navigation menu
    /// - Product listing
    /// </summary>
    public class Component
    {
        /// <summary>
        /// Human-readable name of the component
        /// 
        /// This name should be descriptive of the component's purpose.
        /// Examples: "LoginButton", "UsernameField", "SubmitForm"
        /// 
        /// The name will be used in generated test code comments and
        /// can influence variable naming in the generated tests.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// CSS or XPath selector to locate the element
        /// 
        /// This selector must uniquely identify the element on the page.
        /// Playwright supports various selector types:
        /// - CSS: "#login-button", ".submit-btn"
        /// - XPath: "//button[contains(text(), 'Login')]"
        /// - Text: "text=Login"
        /// - Other Playwright selectors: "id=loginBtn", etc.
        /// </summary>
        public string Selector { get; set; }
        
        /// <summary>
        /// Element type (button, input, etc)
        /// 
        /// This helps the AI understand what kind of element it's dealing with
        /// and what operations make sense (click, type, select, etc).
        /// 
        /// Common types: "button", "input", "select", "checkbox", etc.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Additional properties of the element
        /// 
        /// Any other relevant attributes that might be useful for test generation:
        /// - placeholders for input fields
        /// - validation rules
        /// - options for select elements
        /// - default values
        /// - required/optional status
        /// </summary>
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Represents a common user interaction flow for UI test generation
    /// 
    /// A user flow is a sequence of steps that users typically perform
    /// to accomplish a specific task in the application. These flows
    /// are perfect candidates for end-to-end testing.
    /// 
    /// Examples:
    /// - Login flow
    /// - Checkout process
    /// - User registration
    /// - Content creation
    /// </summary>
    public class UserFlow
    {
        /// <summary>
        /// Descriptive name of the flow
        /// 
        /// This name should clearly identify what task this flow accomplishes.
        /// Examples: "UserLogin", "ProductPurchase", "AccountRegistration"
        /// 
        /// This name will influence the naming of generated test methods.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Ordered list of steps in the flow
        /// 
        /// Each step should be a clear, concise description of a user action.
        /// Examples:
        /// - "Navigate to login page"
        /// - "Enter username 'testuser'"
        /// - "Click login button"
        /// - "Verify dashboard is displayed"
        /// 
        /// The AI will translate these steps into Playwright test actions.
        /// </summary>
        public List<string> Steps { get; set; } = new List<string>();
        
        /// <summary>
        /// Expected result after completing the flow
        /// 
        /// This describes what should happen when the flow executes successfully.
        /// It guides the AI in creating appropriate assertions.
        /// 
        /// Examples:
        /// - "User should be logged in and redirected to dashboard"
        /// - "Order confirmation page should display order number"
        /// - "New account should appear in the user list"
        /// </summary>
        public string ExpectedOutcome { get; set; }
    }
    
    /// <summary>
    /// Represents an API endpoint for API test generation
    /// 
    /// This class models everything needed to understand and test an API endpoint:
    /// - How to access it (path and method)
    /// - What parameters it accepts
    /// - What responses it returns
    /// 
    /// This information allows the AI to generate comprehensive API tests.
    /// </summary>
    public class ApiEndpoint
    {
        /// <summary>
        /// Endpoint path (e.g., "/posts/1")
        /// 
        /// The relative path to the API endpoint from the base URL.
        /// May include path parameters indicated with curly braces.
        /// 
        /// Examples:
        /// - "/users" - List all users
        /// - "/users/{id}" - Get user by ID
        /// - "/posts/{postId}/comments" - Get comments for a post
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// HTTP method (GET, POST, PUT, DELETE, PATCH, etc.)
        /// 
        /// The HTTP verb that defines what operation this endpoint performs.
        /// 
        /// Common mapping:
        /// - GET: Retrieve data
        /// - POST: Create new resources
        /// - PUT: Update entire resources
        /// - PATCH: Partial updates
        /// - DELETE: Remove resources
        /// </summary>
        public string Method { get; set; }
        
        /// <summary>
        /// Parameters accepted by the endpoint
        /// 
        /// A dictionary of parameter names and their expected types/formats.
        /// These can represent:
        /// - Query parameters for GET requests
        /// - Body parameters for POST/PUT/PATCH requests
        /// - Path parameters that need to be substituted in the URL
        /// 
        /// Types are represented as strings like "string", "number", "boolean", etc.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// Example of expected response
        /// 
        /// A sample response object that shows what data structure to expect.
        /// This helps the AI understand what to assert on in generated tests.
        /// 
        /// This can be:
        /// - An anonymous object with sample values
        /// - A JsonElement from an actual API response
        /// - A string representation of JSON
        /// </summary>
        public object ResponseExample { get; set; }
    }
}
