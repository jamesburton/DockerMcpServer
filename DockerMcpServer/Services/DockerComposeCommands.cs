using System.ComponentModel;
using DockerMcpServer.Core;
using DockerMcpServer.Models;
using ModelContextProtocol.Server;
using System.Text.Json;

namespace DockerMcpServer.Services;

/// <summary>
/// Provides Docker Compose management commands as MCP tools
/// </summary>
[McpServerToolType]
public class DockerComposeCommands
{
    private readonly IDockerService _dockerService;

    /// <summary>Initializes a new instance of the DockerComposeCommands class.</summary>
    public DockerComposeCommands(IDockerService dockerService)
    {
        _dockerService = dockerService;
    }

    /// <summary>Deploys a Docker Compose stack from YAML configuration.</summary>
    [McpServerTool, Description("Deploys a Docker Compose stack from YAML configuration")]
    public async Task<string> DeployComposeStackAsync(
        [Description("Project name for the compose stack")] string projectName,
        [Description("Docker Compose YAML content")] string composeYaml,
        [Description("Environment variables for the compose file (JSON object)")] string? environment = null,
        [Description("Working directory for the compose operation")] string? workingDirectory = null,
        [Description("Build images before starting containers")] bool build = false,
        [Description("Recreate containers even if their configuration hasn't changed")] bool forceRecreate = false,
        [Description("Don't start linked services")] bool noDeps = false,
        [Description("Pull images before starting containers")] bool pull = false)
    {
        var request = new ComposeStackRequest
        {
            ProjectName = projectName,
            ComposeYaml = composeYaml,
            Environment = ParseJsonObject(environment),
            WorkingDirectory = workingDirectory,
            Build = build,
            ForceRecreate = forceRecreate,
            NoDeps = noDeps,
            Pull = pull
        };

        var result = await _dockerService.DeployComposeStackAsync(request);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Stops and removes a Docker Compose stack.</summary>
    [McpServerTool, Description("Stops and removes a Docker Compose stack")]
    public async Task<string> RemoveComposeStackAsync(
        [Description("Project name of the compose stack")] string projectName,
        [Description("Remove named volumes declared in the compose file")] bool removeVolumes = false,
        [Description("Remove images used by services")] bool removeImages = false)
    {
        var result = await _dockerService.RemoveComposeStackAsync(projectName, removeVolumes, removeImages);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Lists containers in a Docker Compose stack.</summary>
    [McpServerTool, Description("Lists containers in a Docker Compose stack")]
    public async Task<string> ListComposeStackContainersAsync(
        [Description("Project name of the compose stack")] string projectName)
    {
        var result = await _dockerService.ListComposeStackContainersAsync(projectName);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Gets logs from a Docker Compose stack.</summary>
    [McpServerTool, Description("Gets logs from a Docker Compose stack")]
    public async Task<string> GetComposeStackLogsAsync(
        [Description("Project name of the compose stack")] string projectName,
        [Description("Specific service name (optional, gets logs from all services if not specified)")] string? serviceName = null,
        [Description("Follow log output")] bool follow = false,
        [Description("Number of lines to show from the end of the logs")] int? tail = null)
    {
        var result = await _dockerService.GetComposeStackLogsAsync(projectName, serviceName, follow, tail);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Starts a Docker Compose stack.</summary>
    [McpServerTool, Description("Starts a Docker Compose stack")]
    public async Task<string> StartComposeStackAsync(
        [Description("Project name of the compose stack")] string projectName)
    {
        var result = await _dockerService.StartComposeStackAsync(projectName);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Stops a Docker Compose stack.</summary>
    [McpServerTool, Description("Stops a Docker Compose stack")]
    public async Task<string> StopComposeStackAsync(
        [Description("Project name of the compose stack")] string projectName)
    {
        var result = await _dockerService.StopComposeStackAsync(projectName);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Restarts a Docker Compose stack.</summary>
    [McpServerTool, Description("Restarts a Docker Compose stack")]
    public async Task<string> RestartComposeStackAsync(
        [Description("Project name of the compose stack")] string projectName)
    {
        var result = await _dockerService.RestartComposeStackAsync(projectName);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Scales a service in a Docker Compose stack.</summary>
    [McpServerTool, Description("Scales a service in a Docker Compose stack")]
    public async Task<string> ScaleComposeServiceAsync(
        [Description("Project name of the compose stack")] string projectName,
        [Description("Service name to scale")] string serviceName,
        [Description("Number of replicas")] int replicas)
    {
        var result = await _dockerService.ScaleComposeServiceAsync(projectName, serviceName, replicas);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    private static Dictionary<string, string>? ParseJsonObject(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }
        catch
        {
            return null;
        }
    }
}
