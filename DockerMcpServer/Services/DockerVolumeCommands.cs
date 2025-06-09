using System.ComponentModel;
using DockerMcpServer.Core;
using ModelContextProtocol.Server;
using System.Text.Json;

namespace DockerMcpServer.Services;

/// <summary>
/// Provides Docker volume management commands as MCP tools
/// </summary>
[McpServerToolType]
public class DockerVolumeCommands
{
    private readonly IDockerService _dockerService;

    /// <summary>Initializes a new instance of the DockerVolumeCommands class.</summary>
    public DockerVolumeCommands(IDockerService dockerService)
    {
        _dockerService = dockerService;
    }

    /// <summary>Lists Docker volumes.</summary>
    [McpServerTool, Description("Lists Docker volumes")]
    public async Task<string> ListVolumesAsync()
    {
        var result = await _dockerService.ListVolumesAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Creates a Docker volume.</summary>
    [McpServerTool, Description("Creates a Docker volume")]
    public async Task<string> CreateVolumeAsync(
        [Description("Volume name")] string name,
        [Description("Volume driver (defaults to 'local')")] string? driver = null,
        [Description("Driver options (JSON object)")] string? driverOpts = null,
        [Description("Labels to add to the volume (JSON object)")] string? labels = null)
    {
        var result = await _dockerService.CreateVolumeAsync(name, driver, ParseJsonObject(driverOpts), ParseJsonObject(labels));
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes a Docker volume.</summary>
    [McpServerTool, Description("Removes a Docker volume")]
    public async Task<string> RemoveVolumeAsync(
        [Description("Volume name")] string volumeName,
        [Description("Force removal of the volume")] bool force = false)
    {
        var result = await _dockerService.RemoveVolumeAsync(volumeName, force);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Inspects a Docker volume and returns detailed information.</summary>
    [McpServerTool, Description("Inspects a Docker volume and returns detailed information")]
    public async Task<string> InspectVolumeAsync(
        [Description("Volume name")] string volumeName)
    {
        var result = await _dockerService.InspectVolumeAsync(volumeName);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes unused Docker volumes.</summary>
    [McpServerTool, Description("Removes unused Docker volumes")]
    public async Task<string> PruneVolumesAsync()
    {
        var result = await _dockerService.PruneVolumesAsync();
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
