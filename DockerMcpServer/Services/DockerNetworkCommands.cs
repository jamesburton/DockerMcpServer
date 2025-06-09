using System.ComponentModel;
using DockerMcpServer.Core;
using ModelContextProtocol.Server;
using System.Text.Json;

namespace DockerMcpServer.Services;

/// <summary>
/// Provides Docker network management commands as MCP tools
/// </summary>
[McpServerToolType]
public class DockerNetworkCommands
{
    private readonly IDockerService _dockerService;

    /// <summary>Initializes a new instance of the DockerNetworkCommands class.</summary>
    public DockerNetworkCommands(IDockerService dockerService)
    {
        _dockerService = dockerService;
    }

    /// <summary>Lists Docker networks.</summary>
    [McpServerTool, Description("Lists Docker networks")]
    public async Task<string> ListNetworksAsync()
    {
        var result = await _dockerService.ListNetworksAsync();
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Creates a Docker network.</summary>
    [McpServerTool, Description("Creates a Docker network")]
    public async Task<string> CreateNetworkAsync(
        [Description("Network name")] string name,
        [Description("Network driver (defaults to 'bridge')")] string? driver = null,
        [Description("Is internal network")] bool? isInternal = null,
        [Description("Is attachable")] bool? attachable = null,
        [Description("Network options (JSON object)")] string? options = null,
        [Description("Labels to add to the network (JSON object)")] string? labels = null)
    {
        var result = await _dockerService.CreateNetworkAsync(name, driver, isInternal, attachable, ParseJsonObject(options), ParseJsonObject(labels), null);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes a Docker network.</summary>
    [McpServerTool, Description("Removes a Docker network")]
    public async Task<string> RemoveNetworkAsync(
        [Description("Network ID or name")] string networkIdOrName)
    {
        var result = await _dockerService.RemoveNetworkAsync(networkIdOrName);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Connects a container to a network.</summary>
    [McpServerTool, Description("Connects a container to a network")]
    public async Task<string> ConnectNetworkAsync(
        [Description("Network ID or name")] string networkIdOrName,
        [Description("Container ID or name")] string containerIdOrName,
        [Description("Network alias for container")] string? alias = null,
        [Description("IPv4 addresses (JSON array)")] string? ipv4Addresses = null,
        [Description("IPv6 addresses (JSON array)")] string? ipv6Addresses = null)
    {
        var result = await _dockerService.ConnectNetworkAsync(networkIdOrName, containerIdOrName, alias, ParseJsonArray(ipv4Addresses), ParseJsonArray(ipv6Addresses));
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Disconnects a container from a network.</summary>
    [McpServerTool, Description("Disconnects a container from a network")]
    public async Task<string> DisconnectNetworkAsync(
        [Description("Network ID or name")] string networkIdOrName,
        [Description("Container ID or name")] string containerIdOrName,
        [Description("Force disconnection")] bool force = false)
    {
        var result = await _dockerService.DisconnectNetworkAsync(networkIdOrName, containerIdOrName, force);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Inspects a Docker network and returns detailed information.</summary>
    [McpServerTool, Description("Inspects a Docker network and returns detailed information")]
    public async Task<string> InspectNetworkAsync(
        [Description("Network ID or name")] string networkIdOrName)
    {
        var result = await _dockerService.InspectNetworkAsync(networkIdOrName);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes unused Docker networks.</summary>
    [McpServerTool, Description("Removes unused Docker networks")]
    public async Task<string> PruneNetworksAsync()
    {
        var result = await _dockerService.PruneNetworksAsync();
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

    private static List<string>? ParseJsonArray(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json);
        }
        catch
        {
            return null;
        }
    }
}
