using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DockerMcpServer.Core;
using DockerMcpServer.Services;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging to stderr to avoid interfering with MCP communication
builder.Logging.AddConsole(cfg =>
{
    cfg.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Add Docker services
builder.Services.AddSingleton<IDockerClientFactory, DockerClientFactory>();
builder.Services.AddSingleton<IDockerService, DockerService>();

// Add MCP server with stdio transport and auto-discover tools from assembly
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Initialize Docker client
var dockerService = app.Services.GetRequiredService<IDockerService>();
await dockerService.InitializeAsync();

await app.RunAsync();
