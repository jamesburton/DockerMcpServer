using System.ComponentModel;
using DockerMcpServer.Core;
using DockerMcpServer.Models;
using ModelContextProtocol.Server;
using System.Text.Json;

namespace DockerMcpServer.Services;

/// <summary>
/// Provides Docker image management commands as MCP tools
/// </summary>
[McpServerToolType]
public class DockerImageCommands
{
    private readonly IDockerService _dockerService;

    /// <summary>Initializes a new instance of the DockerImageCommands class.</summary>
    public DockerImageCommands(IDockerService dockerService)
    {
        _dockerService = dockerService;
    }

    /// <summary>Lists Docker images with filtering options.</summary>
    [McpServerTool, Description("Lists Docker images with filtering options")]
    public async Task<string> ListImagesAsync(
        [Description("Show all images (default hides intermediate images)")] bool all = false,
        [Description("Show dangling images only")] bool dangling = false,
        [Description("Filter images by labels (JSON object)")] string? labels = null,
        [Description("Filter by reference pattern")] string? reference = null)
    {
        var options = new ImageListOptions
        {
            All = all,
            Dangling = dangling,
            Labels = ParseJsonObject(labels),
            Reference = reference
        };

        var result = await _dockerService.ListImagesAsync(options);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Pulls a Docker image from a registry.</summary>
    [McpServerTool, Description("Pulls a Docker image from a registry")]
    public async Task<string> PullImageAsync(
        [Description("Image name (e.g., 'nginx', 'ubuntu')")] string imageName,
        [Description("Image tag (defaults to 'latest')")] string? tag = null,
        [Description("Platform (e.g., 'linux/amd64')")] string? platform = null)
    {
        var result = await _dockerService.PullImageAsync(imageName, tag, platform);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes a Docker image.</summary>
    [McpServerTool, Description("Removes a Docker image")]
    public async Task<string> RemoveImageAsync(
        [Description("Image ID or name")] string imageIdOrName,
        [Description("Force removal of the image")] bool force = false,
        [Description("Do not delete untagged parent images")] bool noPrune = false)
    {
        var result = await _dockerService.RemoveImageAsync(imageIdOrName, force, noPrune);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Builds a Docker image from a Dockerfile.</summary>
    [McpServerTool, Description("Builds a Docker image from a Dockerfile")]
    public async Task<string> BuildImageAsync(
        [Description("Path to Dockerfile")] string dockerfilePath,
        [Description("Image tag")] string? tag = null,
        [Description("Build context directory (defaults to Dockerfile directory)")] string? buildContext = null,
        [Description("Build arguments (JSON object)")] string? buildArgs = null)
    {
        var result = await _dockerService.BuildImageAsync(dockerfilePath, tag, buildContext, ParseJsonObject(buildArgs));
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Tags a Docker image.</summary>
    [McpServerTool, Description("Tags a Docker image")]
    public async Task<string> TagImageAsync(
        [Description("Source image name or ID")] string sourceImage,
        [Description("Target image name with tag")] string targetImage)
    {
        var result = await _dockerService.TagImageAsync(sourceImage, targetImage);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Inspects a Docker image and returns detailed information.</summary>
    [McpServerTool, Description("Inspects a Docker image and returns detailed information")]
    public async Task<string> InspectImageAsync(
        [Description("Image ID or name")] string imageIdOrName)
    {
        var result = await _dockerService.InspectImageAsync(imageIdOrName);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Gets the history of a Docker image.</summary>
    [McpServerTool, Description("Gets the history of a Docker image")]
    public async Task<string> GetImageHistoryAsync(
        [Description("Image ID or name")] string imageIdOrName)
    {
        var result = await _dockerService.GetImageHistoryAsync(imageIdOrName);
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

    /// <summary>Searches for images in Docker registry.</summary>
    [McpServerTool, Description("Searches for images in Docker registry")]
    public async Task<string> SearchImagesAsync(
        [Description("Search term")] string term,
        [Description("Limit number of results")] int limit = 25)
    {
        var result = await _dockerService.SearchImagesAsync(term, limit);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Pushes an image to registry.</summary>
    [McpServerTool, Description("Pushes an image to registry")]
    public async Task<string> PushImageAsync(
        [Description("Image name")] string imageName,
        [Description("Image tag")] string? tag = null,
        [Description("Authentication header")] string? authHeader = null)
    {
        var result = await _dockerService.PushImageAsync(imageName, tag, authHeader);
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
