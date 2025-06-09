using System.ComponentModel;
using DockerMcpServer.Core;
using ModelContextProtocol.Server;
using System.Text.Json;

namespace DockerMcpServer.Services;

/// <summary>
/// Provides Docker system management commands as MCP tools
/// </summary>
[McpServerToolType]
public class DockerSystemCommands
{
    private readonly IDockerService _dockerService;

    /// <summary>Initializes a new instance of the DockerSystemCommands class.</summary>
    public DockerSystemCommands(IDockerService dockerService)
    {
        _dockerService = dockerService;
    }

    /// <summary>Tests Docker connection.</summary>
    [McpServerTool, Description("Tests Docker connection")]
    public async Task<string> TestConnectionAsync()
    {
        var result = await _dockerService.TestConnectionAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Gets Docker system information.</summary>
    [McpServerTool, Description("Gets Docker system information")]
    public async Task<string> GetDockerInfoAsync()
    {
        var result = await _dockerService.GetDockerInfoAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Gets Docker version information.</summary>
    [McpServerTool, Description("Gets Docker version information")]
    public async Task<string> GetDockerVersionAsync()
    {
        var result = await _dockerService.GetDockerVersionAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes unused Docker containers.</summary>
    [McpServerTool, Description("Removes unused Docker containers")]
    public async Task<string> PruneContainersAsync()
    {
        var result = await _dockerService.PruneContainersAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes unused Docker images.</summary>
    [McpServerTool, Description("Removes unused Docker images")]
    public async Task<string> PruneImagesAsync(
        [Description("Remove only dangling images (default: true)")] bool dangling = true,
        [Description("Remove all unused images")] bool all = false)
    {
        var result = await _dockerService.PruneImagesAsync(dangling, all);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes unused Docker networks.</summary>
    [McpServerTool, Description("Removes unused Docker networks")]
    public async Task<string> PruneNetworksAsync()
    {
        var result = await _dockerService.PruneNetworksAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes unused Docker volumes.</summary>
    [McpServerTool, Description("Removes unused Docker volumes")]
    public async Task<string> PruneVolumesAsync()
    {
        var result = await _dockerService.PruneVolumesAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes all unused Docker objects (containers, images, networks, volumes).</summary>
    [McpServerTool, Description("Removes all unused Docker objects (containers, images, networks, volumes)")]
    public async Task<string> PruneSystemAsync(
        [Description("Also remove unused volumes")] bool volumes = false)
    {
        var result = await _dockerService.PruneSystemAsync(volumes);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Gets Docker disk usage information.</summary>
    [McpServerTool, Description("Gets Docker disk usage information")]
    public async Task<string> GetDiskUsageAsync()
    {
        var result = await _dockerService.GetDiskUsageAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Pings Docker daemon.</summary>
    [McpServerTool, Description("Pings Docker daemon")]
    public async Task<string> PingDockerAsync()
    {
        var result = await _dockerService.PingDockerAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Gets Docker processes information.</summary>
    [McpServerTool, Description("Gets Docker processes information")]
    public async Task<string> GetDockerProcessesAsync()
    {
        var result = await _dockerService.GetDockerProcessesAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }
}
