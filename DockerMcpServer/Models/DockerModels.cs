using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DockerMcpServer.Models;

/// <summary>
/// Represents the configuration for creating a Docker container
/// </summary>
public class ContainerCreateRequest
{
    /// <summary>Docker image name (e.g., 'nginx:latest', 'ubuntu:22.04').</summary>
    [Description("Docker image name (e.g., 'nginx:latest', 'ubuntu:22.04')")]
    public required string Image { get; set; }

    /// <summary>Container name.</summary>
    [Description("Container name")]
    public string? Name { get; set; }

    /// <summary>Command to run in the container.</summary>
    [Description("Command to run in the container")]
    public string? Command { get; set; }

    /// <summary>Arguments for the command.</summary>
    [Description("Arguments for the command")]
    public List<string>? Args { get; set; }

    /// <summary>Environment variables (e.g., ['KEY=value', 'DEBUG=true']).</summary>
    [Description("Environment variables (e.g., ['KEY=value', 'DEBUG=true'])")]
    public List<string>? Environment { get; set; }

    /// <summary>Port mappings (e.g., ['8080:80', '443:443']).</summary>
    [Description("Port mappings (e.g., ['8080:80', '443:443'])")]
    public List<string>? Ports { get; set; }

    /// <summary>Volume mounts (e.g., ['/host/path:/container/path', '/data:/app/data:ro']).</summary>
    [Description("Volume mounts (e.g., ['/host/path:/container/path', '/data:/app/data:ro'])")]
    public List<string>? Volumes { get; set; }

    /// <summary>Working directory inside the container.</summary>
    [Description("Working directory inside the container")]
    public string? WorkingDir { get; set; }

    /// <summary>User to run as (e.g., 'root', '1000', 'user:group').</summary>
    [Description("User to run as (e.g., 'root', '1000', 'user:group')")]
    public string? User { get; set; }

    /// <summary>Hostname for the container.</summary>
    [Description("Hostname for the container")]
    public string? Hostname { get; set; }

    /// <summary>Domain name for the container.</summary>
    [Description("Domain name for the container")]
    public string? Domainname { get; set; }

    /// <summary>Run container in detached mode.</summary>
    [Description("Run container in detached mode")]
    public bool Detach { get; set; } = true;

    /// <summary>Automatically remove container when it exits.</summary>
    [Description("Automatically remove container when it exits")]
    public bool AutoRemove { get; set; } = false;

    /// <summary>Run container in interactive mode.</summary>
    [Description("Run container in interactive mode")]
    public bool Interactive { get; set; } = false;

    /// <summary>Allocate a pseudo-TTY.</summary>
    [Description("Allocate a pseudo-TTY")]
    public bool Tty { get; set; } = false;

    /// <summary>Mount container's root filesystem as read only.</summary>
    [Description("Mount container's root filesystem as read only")]
    public bool ReadOnly { get; set; } = false;

    /// <summary>Give extended privileges to this container.</summary>
    [Description("Give extended privileges to this container")]
    public bool Privileged { get; set; } = false;

    /// <summary>Network mode (e.g., 'bridge', 'host', 'none', 'container:name').</summary>
    [Description("Network mode (e.g., 'bridge', 'host', 'none', 'container:name')")]
    public string? Network { get; set; }

    /// <summary>Restart policy (e.g., 'no', 'always', 'unless-stopped', 'on-failure').</summary>
    [Description("Restart policy (e.g., 'no', 'always', 'unless-stopped', 'on-failure')")]
    public string? RestartPolicy { get; set; }

    /// <summary>CPU limit (number of CPUs).</summary>
    [Description("CPU limit (number of CPUs)")]
    public double? CpuLimit { get; set; }

    /// <summary>Memory limit (e.g., '512m', '1g').</summary>
    [Description("Memory limit (e.g., '512m', '1g')")]
    public string? MemoryLimit { get; set; }

    /// <summary>Additional capabilities to add.</summary>
    [Description("Additional capabilities to add")]
    public List<string>? CapAdd { get; set; }

    /// <summary>Capabilities to drop.</summary>
    [Description("Capabilities to drop")]
    public List<string>? CapDrop { get; set; }

    /// <summary>Device mappings (e.g., ['/dev/sda:/dev/sda']).</summary>
    [Description("Device mappings (e.g., ['/dev/sda:/dev/sda'])")]
    public List<string>? Devices { get; set; }

    /// <summary>DNS servers.</summary>
    [Description("DNS servers")]
    public List<string>? Dns { get; set; }

    /// <summary>Extra hosts to add to /etc/hosts.</summary>
    [Description("Extra hosts to add to /etc/hosts")]
    public List<string>? ExtraHosts { get; set; }

    /// <summary>Labels to add to the container.</summary>
    [Description("Labels to add to the container")]
    public Dictionary<string, string>? Labels { get; set; }

    /// <summary>Security options.</summary>
    [Description("Security options")]
    public List<string>? SecurityOpt { get; set; }

    /// <summary>Tmpfs mounts.</summary>
    [Description("Tmpfs mounts")]
    public List<string>? Tmpfs { get; set; }

    /// <summary>Ulimits.</summary>
    [Description("Ulimits")]
    public List<string>? Ulimits { get; set; }
}

/// <summary>
/// Represents Docker Compose configuration
/// </summary>
public class ComposeStackRequest
{
    /// <summary>Project name for the compose stack.</summary>
    [Description("Project name for the compose stack")]
    public required string ProjectName { get; set; }

    /// <summary>Docker Compose YAML content.</summary>
    [Description("Docker Compose YAML content")]
    public required string ComposeYaml { get; set; }

    /// <summary>Environment variables for the compose file.</summary>
    [Description("Environment variables for the compose file")]
    public Dictionary<string, string>? Environment { get; set; }

    /// <summary>Working directory for the compose operation.</summary>
    [Description("Working directory for the compose operation")]
    public string? WorkingDirectory { get; set; }

    /// <summary>Build images before starting containers.</summary>
    [Description("Build images before starting containers")]
    public bool Build { get; set; } = false;

    /// <summary>Recreate containers even if their configuration hasn't changed.</summary>
    [Description("Recreate containers even if their configuration hasn't changed")]
    public bool ForceRecreate { get; set; } = false;

    /// <summary>Don't start linked services.</summary>
    [Description("Don't start linked services")]
    public bool NoDeps { get; set; } = false;

    /// <summary>Pull images before starting containers.</summary>
    [Description("Pull images before starting containers")]
    public bool Pull { get; set; } = false;
}

/// <summary>
/// Represents options for listing containers
/// </summary>
public class ContainerListOptions
{
    /// <summary>Show all containers (default shows just running).</summary>
    [Description("Show all containers (default shows just running)")]
    public bool All { get; set; } = false;

    /// <summary>Show container sizes.</summary>
    [Description("Show container sizes")]
    public bool Size { get; set; } = false;

    /// <summary>Limit the number of results.</summary>
    [Description("Limit the number of results")]
    public int? Limit { get; set; }

    /// <summary>Only show containers created since this container ID.</summary>
    [Description("Only show containers created since this container ID")]
    public string? Since { get; set; }

    /// <summary>Only show containers created before this container ID.</summary>
    [Description("Only show containers created before this container ID")]
    public string? Before { get; set; }

    /// <summary>Filter containers by labels.</summary>
    [Description("Filter containers by labels")]
    public Dictionary<string, string>? Labels { get; set; }
}

/// <summary>
/// Represents options for getting container logs
/// </summary>
public class ContainerLogsOptions
{
    /// <summary>Show stdout.</summary>
    [Description("Show stdout")]
    public bool Stdout { get; set; } = true;

    /// <summary>Show stderr.</summary>
    [Description("Show stderr")]
    public bool Stderr { get; set; } = true;

    /// <summary>Show timestamps.</summary>
    [Description("Show timestamps")]
    public bool Timestamps { get; set; } = false;

    /// <summary>Follow log output.</summary>
    [Description("Follow log output")]
    public bool Follow { get; set; } = false;

    /// <summary>Number of lines to show from the end of the logs.</summary>
    [Description("Number of lines to show from the end of the logs")]
    public int? Tail { get; set; }

    /// <summary>Show logs since timestamp (RFC3339 or Unix timestamp).</summary>
    [Description("Show logs since timestamp (RFC3339 or Unix timestamp)")]
    public string? Since { get; set; }

    /// <summary>Show logs until timestamp (RFC3339 or Unix timestamp).</summary>
    [Description("Show logs until timestamp (RFC3339 or Unix timestamp)")]
    public string? Until { get; set; }
}

/// <summary>
/// Represents options for listing images
/// </summary>
public class ImageListOptions
{
    /// <summary>Show all images (default hides intermediate images).</summary>
    [Description("Show all images (default hides intermediate images)")]
    public bool All { get; set; } = false;

    /// <summary>Show dangling images only.</summary>
    [Description("Show dangling images only")]
    public bool Dangling { get; set; } = false;

    /// <summary>Filter images by labels.</summary>
    [Description("Filter images by labels")]
    public Dictionary<string, string>? Labels { get; set; }

    /// <summary>Filter by reference pattern.</summary>
    [Description("Filter by reference pattern")]
    public string? Reference { get; set; }
}

/// <summary>
/// Represents a simplified container information
/// </summary>
public class ContainerInfo
{
    /// <summary>Container ID.</summary>
    /// <summary>Image ID.</summary>
    /// <summary>Network ID.</summary>
    /// <summary>Image ID.</summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>Container name.</summary>
    /// <summary>Network name.</summary>
    /// <summary>Volume name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Container image.</summary>
    public string Image { get; set; } = string.Empty;
    /// <summary>Container state.</summary>
    public string State { get; set; } = string.Empty;
    /// <summary>Container status.</summary>
    public string Status { get; set; } = string.Empty;
    /// <summary>Container creation time.</summary>
    /// <summary>Image creation time.</summary>
    /// <summary>Network creation time.</summary>
    /// <summary>Image creation time.</summary>
    public DateTime Created { get; set; }
    /// <summary>Container port mappings.</summary>
    public List<string> Ports { get; set; } = [];
    /// <summary>Container volume mounts.</summary>
    public List<string> Mounts { get; set; } = [];
    /// <summary>Container labels.</summary>
    /// <summary>Image labels.</summary>
    /// <summary>Network labels.</summary>
    /// <summary>Volume labels.</summary>
    /// <summary>Image labels.</summary>
    public Dictionary<string, string> Labels { get; set; } = [];
    /// <summary>Container size (read-write layer).</summary>
    public long? SizeRw { get; set; }
    /// <summary>Container size (total filesystem).</summary>
    public long? SizeRootFs { get; set; }
}

/// <summary>
/// Represents simplified image information
/// </summary>
public class ImageInfo
{
    /// <summary>Image ID.</summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>Image repository tags.</summary>
    public List<string> RepoTags { get; set; } = [];
    /// <summary>Image repository digests.</summary>
    public List<string> RepoDigests { get; set; } = [];
    /// <summary>Image size in bytes.</summary>
    public long Size { get; set; }
    /// <summary>Image virtual size in bytes.</summary>
    public long VirtualSize { get; set; }
    /// <summary>Image creation time.</summary>
    public DateTime Created { get; set; }
    /// <summary>Image labels.</summary>
    public Dictionary<string, string> Labels { get; set; } = [];
    /// <summary>Image architecture.</summary>
    public string Architecture { get; set; } = string.Empty;
    /// <summary>Image operating system.</summary>
    public string Os { get; set; } = string.Empty;
}

/// <summary>
/// Represents network information
/// </summary>
public class NetworkInfo
{
    /// <summary>Network ID.</summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>Network name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Network driver.</summary>
    /// <summary>Volume driver.</summary>
    public string Driver { get; set; } = string.Empty;
    /// <summary>Network scope.</summary>
    /// <summary>Volume scope.</summary>
    public string Scope { get; set; } = string.Empty;
    /// <summary>Is network internal.</summary>
    public bool Internal { get; set; }
    /// <summary>Is network attachable.</summary>
    public bool Attachable { get; set; }
    /// <summary>Is network ingress.</summary>
    public bool Ingress { get; set; }
    /// <summary>Network creation time.</summary>
    public DateTime Created { get; set; }
    /// <summary>Network labels.</summary>
    public Dictionary<string, string> Labels { get; set; } = [];
}

/// <summary>
/// Represents volume information
/// </summary>
public class VolumeInfo
{
    /// <summary>Volume name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Volume driver.</summary>
    public string Driver { get; set; } = string.Empty;
    /// <summary>Volume mountpoint.</summary>
    public string Mountpoint { get; set; } = string.Empty;
    /// <summary>Volume scope.</summary>
    public string Scope { get; set; } = string.Empty;
    /// <summary>Volume creation time.</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Volume labels.</summary>
    public Dictionary<string, string> Labels { get; set; } = [];
    /// <summary>Volume options.</summary>
    public Dictionary<string, string> Options { get; set; } = [];
}

/// <summary>
/// Represents command execution result
/// </summary>
public class CommandResult
{
    /// <summary>Indicates if the command was successful.</summary>
    public bool Success { get; set; }
    /// <summary>Command result message.</summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>Error message if command failed.</summary>
    public string? ErrorMessage { get; set; }
    /// <summary>Container ID if applicable.</summary>
    public string? ContainerId { get; set; }
    /// <summary>Additional data returned by the command.</summary>
    public object? Data { get; set; }
}