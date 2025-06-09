using System.ComponentModel;
using DockerMcpServer.Core;
using DockerMcpServer.Models;
using ModelContextProtocol.Server;
using System.Text.Json;

namespace DockerMcpServer.Services;

/// <summary>
/// Provides Docker container management commands as MCP tools
/// </summary>
[McpServerToolType]
public class DockerContainerCommands
{
    private readonly IDockerService _dockerService;

    /// <summary>Initializes a new instance of the DockerContainerCommands class.</summary>
    public DockerContainerCommands(IDockerService dockerService)
    {
        _dockerService = dockerService;
    }

    /// <summary>Creates and optionally starts a new Docker container with comprehensive configuration options.</summary>
    [McpServerTool, Description("Creates and optionally starts a new Docker container with comprehensive configuration options")]
    public async Task<string> CreateContainerAsync(
        [Description("Docker image name (e.g., 'nginx:latest', 'ubuntu:22.04')")] string image,
        [Description("Container name")] string? name = null,
        [Description("Command to run in the container")] string? command = null,
        [Description("Arguments for the command (JSON array of strings)")] string? args = null,
        [Description("Environment variables (JSON array of 'KEY=value' strings)")] string? environment = null,
        [Description("Port mappings (JSON array of 'host:container' strings, e.g., ['8080:80', '443:443'])")] string? ports = null,
        [Description("Volume mounts (JSON array of 'host:container' strings, e.g., ['/data:/app/data', '/config:/etc/app:ro'])")] string? volumes = null,
        [Description("Working directory inside the container")] string? workingDir = null,
        [Description("User to run as (e.g., 'root', '1000', 'user:group')")] string? user = null,
        [Description("Hostname for the container")] string? hostname = null,
        [Description("Domain name for the container")] string? domainname = null,
        [Description("Run container in detached mode")] bool detach = true,
        [Description("Automatically remove container when it exits")] bool autoRemove = false,
        [Description("Run container in interactive mode")] bool interactive = false,
        [Description("Allocate a pseudo-TTY")] bool tty = false,
        [Description("Mount container's root filesystem as read only")] bool readOnly = false,
        [Description("Give extended privileges to this container")] bool privileged = false,
        [Description("Network mode (e.g., 'bridge', 'host', 'none', 'container:name')")] string? network = null,
        [Description("Restart policy (e.g., 'no', 'always', 'unless-stopped', 'on-failure')")] string? restartPolicy = null,
        [Description("CPU limit (number of CPUs)")] double? cpuLimit = null,
        [Description("Memory limit (e.g., '512m', '1g')")] string? memoryLimit = null,
        [Description("Additional capabilities to add (JSON array)")] string? capAdd = null,
        [Description("Capabilities to drop (JSON array)")] string? capDrop = null,
        [Description("Device mappings (JSON array of '/host/device:/container/device' strings)")] string? devices = null,
        [Description("DNS servers (JSON array)")] string? dns = null,
        [Description("Extra hosts to add to /etc/hosts (JSON array of 'hostname:ip' strings)")] string? extraHosts = null,
        [Description("Labels to add to the container (JSON object)")] string? labels = null,
        [Description("Security options (JSON array)")] string? securityOpt = null,
        [Description("Tmpfs mounts (JSON array)")] string? tmpfs = null,
        [Description("Ulimits (JSON array of 'name=soft:hard' strings)")] string? ulimits = null,
        [Description("Start the container after creation")] bool start = true)
    {
        var request = new ContainerCreateRequest
        {
            Image = image,
            Name = name,
            Command = command,
            Args = ParseJsonArray(args),
            Environment = ParseJsonArray(environment),
            Ports = ParseJsonArray(ports),
            Volumes = ParseJsonArray(volumes),
            WorkingDir = workingDir,
            User = user,
            Hostname = hostname,
            Domainname = domainname,
            Detach = detach,
            AutoRemove = autoRemove,
            Interactive = interactive,
            Tty = tty,
            ReadOnly = readOnly,
            Privileged = privileged,
            Network = network,
            RestartPolicy = restartPolicy,
            CpuLimit = cpuLimit,
            MemoryLimit = memoryLimit,
            CapAdd = ParseJsonArray(capAdd),
            CapDrop = ParseJsonArray(capDrop),
            Devices = ParseJsonArray(devices),
            Dns = ParseJsonArray(dns),
            ExtraHosts = ParseJsonArray(extraHosts),
            Labels = ParseJsonObject(labels),
            SecurityOpt = ParseJsonArray(securityOpt),
            Tmpfs = ParseJsonArray(tmpfs),
            Ulimits = ParseJsonArray(ulimits)
        };

        var result = await _dockerService.CreateContainerAsync(request, start);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Lists Docker containers with filtering options.</summary>
    [McpServerTool, Description("Lists Docker containers with filtering options")]
    public async Task<string> ListContainersAsync(
        [Description("Show all containers (default shows just running)")] bool all = false,
        [Description("Show container sizes")] bool size = false,
        [Description("Limit the number of results")] int? limit = null,
        [Description("Only show containers created since this container ID")] string? since = null,
        [Description("Only show containers created before this container ID")] string? before = null,
        [Description("Filter containers by labels (JSON object)")] string? labels = null)
    {
        var options = new ContainerListOptions
        {
            All = all,
            Size = size,
            Limit = limit,
            Since = since,
            Before = before,
            Labels = ParseJsonObject(labels)
        };

        var result = await _dockerService.ListContainersAsync(options);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Starts a Docker container.</summary>
    [McpServerTool, Description("Starts a Docker container")]
    public async Task<string> StartContainerAsync(
        [Description("Container ID or name")] string containerIdOrName)
    {
        var result = await _dockerService.StartContainerAsync(containerIdOrName);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Stops a Docker container.</summary>
    [McpServerTool, Description("Stops a Docker container")]
    public async Task<string> StopContainerAsync(
        [Description("Container ID or name")] string containerIdOrName,
        [Description("Timeout in seconds before forcefully killing the container")] int timeoutSeconds = 10)
    {
        var result = await _dockerService.StopContainerAsync(containerIdOrName, timeoutSeconds);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Restarts a Docker container.</summary>
    [McpServerTool, Description("Restarts a Docker container")]
    public async Task<string> RestartContainerAsync(
        [Description("Container ID or name")] string containerIdOrName,
        [Description("Timeout in seconds before forcefully killing the container")] int timeoutSeconds = 10)
    {
        var result = await _dockerService.RestartContainerAsync(containerIdOrName, timeoutSeconds);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Removes a Docker container.</summary>
    [McpServerTool, Description("Removes a Docker container")]
    public async Task<string> RemoveContainerAsync(
        [Description("Container ID or name")] string containerIdOrName,
        [Description("Force removal of running container")] bool force = false,
        [Description("Remove associated volumes")] bool removeVolumes = false)
    {
        var result = await _dockerService.RemoveContainerAsync(containerIdOrName, force, removeVolumes);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Gets logs from a Docker container.</summary>
    [McpServerTool, Description("Gets logs from a Docker container")]
    public async Task<string> GetContainerLogsAsync(
        [Description("Container ID or name")] string containerIdOrName,
        [Description("Show stdout")] bool stdout = true,
        [Description("Show stderr")] bool stderr = true,
        [Description("Show timestamps")] bool timestamps = false,
        [Description("Follow log output")] bool follow = false,
        [Description("Number of lines to show from the end of the logs")] int? tail = null,
        [Description("Show logs since timestamp (RFC3339 or Unix timestamp)")] string? since = null,
        [Description("Show logs until timestamp (RFC3339 or Unix timestamp)")] string? until = null)
    {
        var options = new ContainerLogsOptions
        {
            Stdout = stdout,
            Stderr = stderr,
            Timestamps = timestamps,
            Follow = follow,
            Tail = tail,
            Since = since,
            Until = until
        };

        var result = await _dockerService.GetContainerLogsAsync(containerIdOrName, options);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Inspects a Docker container and returns detailed information.</summary>
    [McpServerTool, Description("Inspects a Docker container and returns detailed information")]
    public async Task<string> InspectContainerAsync(
        [Description("Container ID or name")] string containerIdOrName)
    {
        var result = await _dockerService.InspectContainerAsync(containerIdOrName);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Gets real-time statistics for a Docker container.</summary>
    [McpServerTool, Description("Gets real-time statistics for a Docker container")]
    public async Task<string> GetContainerStatsAsync(
        [Description("Container ID or name")] string containerIdOrName,
        [Description("Stream statistics continuously")] bool stream = false)
    {
        var result = await _dockerService.GetContainerStatsAsync(containerIdOrName, stream);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>Executes a command in a running Docker container.</summary>
    [McpServerTool, Description("Executes a command in a running Docker container")]
    public async Task<string> ExecContainerAsync(
        [Description("Container ID or name")] string containerIdOrName,
        [Description("Command to execute")] string command,
        [Description("Arguments for the command (JSON array of strings)")] string? args = null,
        [Description("Run in interactive mode")] bool interactive = false)
    {
        var result = await _dockerService.ExecContainerAsync(containerIdOrName, command, ParseJsonArray(args), interactive);
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
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
