// Copyright (c) Microsoft. All rights reserved.

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.OpenApi;

namespace Plugins;

/// <summary>
/// TBD
/// </summary>
public sealed class ParsingFilter(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task ShowReadingJsonAsStreamAsync()
    {
        var kernel = new Kernel();

        void RequestPayloadHandler(string requestPayload)
        {
            output.WriteLine("Request payload");
            output.WriteLine(requestPayload);
        }

        void DocumentParsingFilter(RestApiSpecification specificationModel)
        {
            foreach (var operation in specificationModel.Operations)
            {
                var parametersWithSameName = operation.Parameters.GroupBy(p => p, p => p.Name);

                if (parametersWithSameName.Count() >= 2)
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        parameter.AlternativeName = $"{parameter.Location}_{parameter.Name}";
                    }
                }
            }
        }

        using HttpClient httpClient = new(new StubHttpHandler(RequestPayloadHandler));

        // Register the custom HTTP content reader
        var executionParameters = new OpenApiFunctionExecutionParameters()
        {
            HttpClient = httpClient,
            OpenApiDocumentParsingFilter = DocumentParsingFilter
        };

        // Create OpenAPI plugin
        var plugin = await OpenApiKernelPluginFactory.CreateFromOpenApiAsync("RepairService", "Resources/Plugins/RepairServicePlugin/repair-service-poc.json", executionParameters);

        // Create a repair so it can be read as a stream in the following step
        var arguments = new KernelArguments
        {
            ["title"] = "The Case of the Broken Gizmo",
            ["description"] = "It's broken. Send help!",
            ["Header_assignedTo"] = "Tech Magician",
            ["Query_assignedTo"] = "Tech Magician"
        };
        var createResult = await plugin["listRepairs"].InvokeAsync(kernel, arguments);
        Console.WriteLine(createResult.ToString());

        // List relevant repairs
        arguments = new KernelArguments
        {
            ["assignedTo"] = "Tech Magician"
        };
        var listResult = await plugin["listRepairs"].InvokeAsync(kernel, arguments);
        using var reader = new StreamReader((Stream)listResult.GetValue<RestApiOperationResponse>()!.Content!);
        var content = await reader.ReadToEndAsync();
        var repairs = JsonSerializer.Deserialize<Repair[]>(content);
        Console.WriteLine(content);

        // Delete the repair
        arguments = new KernelArguments
        {
            ["id"] = repairs!.Where(r => r.AssignedTo == "Tech Magician").First().Id.ToString()
        };
        var deleteResult = await plugin["deleteRepair"].InvokeAsync(kernel, arguments);
        Console.WriteLine(deleteResult.ToString());
    }

    /// <summary>
    /// A custom HTTP content reader to change the default behavior of reading HTTP content.
    /// </summary>
    /// <param name="context">The HTTP response content reader context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The HTTP response content.</returns>
    private static async Task<object?> ReadHttpResponseContentAsync(HttpResponseContentReaderContext context, CancellationToken cancellationToken)
    {
        // Read JSON content as a stream rather than as a string, which is the default behavior
        if (context.Response.Content.Headers.ContentType?.MediaType == "application/json")
        {
            return await context.Response.Content.ReadAsStreamAsync(cancellationToken);
        }

        // HTTP request and response properties can be used to decide how to read the content.
        // The 'if' operator below is not relevant to the current example and is just for demonstration purposes.
        if (context.Request.Headers.Contains("x-stream"))
        {
            return await context.Response.Content.ReadAsStreamAsync(cancellationToken);
        }

        // Return null to indicate that any other HTTP content not handled above should be read by the default reader.
        return null;
    }

    private sealed class Repair
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("assignedTo")]
        public string? AssignedTo { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }
    }

    private sealed class StubHttpHandler : DelegatingHandler
    {
        private readonly Action<string> _requestPayloadHandler;
        private readonly JsonSerializerOptions _options;

        public StubHttpHandler(Action<string> requestPayloadHandler) : base()
        {
            this._requestPayloadHandler = requestPayloadHandler;
            this._options = new JsonSerializerOptions { WriteIndented = true };
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage? request, CancellationToken cancellationToken)
        {
            var requestJson = await request!.Content!.ReadFromJsonAsync<JsonElement>(cancellationToken);

            this._requestPayloadHandler(JsonSerializer.Serialize(requestJson, this._options));

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("Success", System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
