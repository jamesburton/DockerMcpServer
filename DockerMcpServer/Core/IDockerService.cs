using DockerMcpServer.Models;
using Docker.DotNet.Models;

namespace DockerMcpServer.Core;

/// <summary>
/// Comprehensive service interface for Docker operations
/// Supports all major Docker features including containers, images, volumes, 
/// networks, compose, system operations, and security features
/// </summary>
public interface IDockerService
{
    #region Core Operations

    /// <summary>
    /// Initializes the Docker service
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Tests the Docker connection
    /// </summary>
    Task<CommandResult> TestConnectionAsync();

    #endregion

    #region Container Operations

    /// <summary>
    /// Creates and optionally starts a new container with comprehensive configuration
    /// </summary>
    Task<CommandResult> CreateContainerAsync(ContainerCreateRequest request, bool start);

    /// <summary>
    /// Lists containers with filtering options
    /// </summary>
    Task<CommandResult> ListContainersAsync(ContainerListOptions? options);

    /// <summary>
    /// Starts a container
    /// </summary>
    Task<CommandResult> StartContainerAsync(string containerIdOrName);

    /// <summary>
    /// Stops a container
    /// </summary>
    Task<CommandResult> StopContainerAsync(string containerIdOrName, int timeoutSeconds);

    /// <summary>
    /// Restarts a container
    /// </summary>
    Task<CommandResult> RestartContainerAsync(string containerIdOrName, int timeoutSeconds);

    /// <summary>
    /// Removes a container
    /// </summary>
    Task<CommandResult> RemoveContainerAsync(string containerIdOrName, bool force, bool removeVolumes);

    /// <summary>
    /// Gets container logs
    /// </summary>
    Task<CommandResult> GetContainerLogsAsync(string containerIdOrName, ContainerLogsOptions? options);

    /// <summary>
    /// Inspects a container
    /// </summary>
    Task<CommandResult> InspectContainerAsync(string containerIdOrName);

    /// <summary>
    /// Gets container statistics
    /// </summary>
    Task<CommandResult> GetContainerStatsAsync(string containerIdOrName, bool stream);

    /// <summary>
    /// Executes a command in a running container
    /// </summary>
    Task<CommandResult> ExecContainerAsync(string containerIdOrName, string command, List<string>? args, bool interactive);

    #endregion

    #region Image Operations

    /// <summary>
    /// Lists images with filtering options
    /// </summary>
    Task<CommandResult> ListImagesAsync(ImageListOptions? options);

    /// <summary>
    /// Pulls an image from registry
    /// </summary>
    Task<CommandResult> PullImageAsync(string imageName, string? tag, string? platform);

    /// <summary>
    /// Builds an image from Dockerfile
    /// </summary>
    Task<CommandResult> BuildImageAsync(string dockerfilePath, string? tag, string? buildContext, Dictionary<string, string>? buildArgs);

    /// <summary>
    /// Removes an image
    /// </summary>
    Task<CommandResult> RemoveImageAsync(string imageIdOrName, bool force, bool noPrune);

    /// <summary>
    /// Inspects an image
    /// </summary>
    Task<CommandResult> InspectImageAsync(string imageIdOrName);

    /// <summary>
    /// Tags an image
    /// </summary>
    Task<CommandResult> TagImageAsync(string sourceImage, string targetImage);

    /// <summary>
    /// Pushes an image to registry
    /// </summary>
    Task<CommandResult> PushImageAsync(string imageName, string? tag, string? authHeader);

    /// <summary>
    /// Searches for images in registry
    /// </summary>
    Task<CommandResult> SearchImagesAsync(string term, int limit);

    /// <summary>
    /// Gets image history
    /// </summary>
    Task<CommandResult> GetImageHistoryAsync(string imageIdOrName);

    #endregion

    #region Volume Operations

    /// <summary>
    /// Lists volumes
    /// </summary>
    Task<CommandResult> ListVolumesAsync();

    /// <summary>
    /// Creates a volume
    /// </summary>
    Task<CommandResult> CreateVolumeAsync(string name, string? driver, Dictionary<string, string>? driverOpts, Dictionary<string, string>? labels);

    /// <summary>
    /// Removes a volume
    /// </summary>
    Task<CommandResult> RemoveVolumeAsync(string volumeName, bool force);

    /// <summary>
    /// Inspects a volume
    /// </summary>
    Task<CommandResult> InspectVolumeAsync(string volumeName);

    /// <summary>
    /// Prunes unused volumes
    /// </summary>
    Task<CommandResult> PruneVolumesAsync();

    /// <summary>
    /// Creates a volume with advanced options
    /// </summary>
    Task<CommandResult> CreateVolumeWithOptionsAsync(string name, string? driver, Dictionary<string, string>? driverOpts, Dictionary<string, string>? labels, string? mountpoint);

    /// <summary>
    /// Lists volumes with filters
    /// </summary>
    Task<CommandResult> ListVolumesByFilterAsync(Dictionary<string, string>? filters);

    #endregion

    #region Network Operations

    /// <summary>
    /// Lists networks
    /// </summary>
    Task<CommandResult> ListNetworksAsync();

    /// <summary>
    /// Creates a network
    /// </summary>
    Task<CommandResult> CreateNetworkAsync(string name, string? driver, bool? isInternal, bool? attachable, Dictionary<string, string>? options, Dictionary<string, string>? labels, List<IPAMConfig>? ipamConfigs);

    /// <summary>
    /// Removes a network
    /// </summary>
    Task<CommandResult> RemoveNetworkAsync(string networkIdOrName);

    /// <summary>
    /// Connects a container to a network
    /// </summary>
    Task<CommandResult> ConnectNetworkAsync(string networkIdOrName, string containerIdOrName, string? alias, List<string>? ipv4Addresses, List<string>? ipv6Addresses);

    /// <summary>
    /// Disconnects a container from a network
    /// </summary>
    Task<CommandResult> DisconnectNetworkAsync(string networkIdOrName, string containerIdOrName, bool force);

    /// <summary>
    /// Inspects a network
    /// </summary>
    Task<CommandResult> InspectNetworkAsync(string networkIdOrName);

    /// <summary>
    /// Prunes unused networks
    /// </summary>
    Task<CommandResult> PruneNetworksAsync();

    /// <summary>
    /// Creates a network with subnet configuration
    /// </summary>
    Task<CommandResult> CreateNetworkWithSubnetAsync(string name, string subnet, string? gateway, string? driver, Dictionary<string, string>? options, Dictionary<string, string>? labels);

    #endregion

    #region Compose Operations

    /// <summary>
    /// Deploys a Docker Compose stack
    /// </summary>
    Task<CommandResult> DeployComposeStackAsync(ComposeStackRequest request);

    /// <summary>
    /// Removes a Docker Compose stack
    /// </summary>
    Task<CommandResult> RemoveComposeStackAsync(string projectName, bool removeVolumes, bool removeImages);

    /// <summary>
    /// Lists containers in a Compose stack
    /// </summary>
    Task<CommandResult> ListComposeStackContainersAsync(string projectName);

    /// <summary>
    /// Gets logs from a Compose stack
    /// </summary>
    Task<CommandResult> GetComposeStackLogsAsync(string projectName, string? serviceName, bool follow, int? tail);

    /// <summary>
    /// Starts a Compose stack
    /// </summary>
    Task<CommandResult> StartComposeStackAsync(string projectName);

    /// <summary>
    /// Stops a Compose stack
    /// </summary>
    Task<CommandResult> StopComposeStackAsync(string projectName);

    /// <summary>
    /// Restarts a Compose stack
    /// </summary>
    Task<CommandResult> RestartComposeStackAsync(string projectName);

    /// <summary>
    /// Scales a service in a Compose stack
    /// </summary>
    Task<CommandResult> ScaleComposeServiceAsync(string projectName, string serviceName, int replicas);

    /// <summary>
    /// Executes a command in a Compose service
    /// </summary>
    Task<CommandResult> ExecComposeServiceAsync(string projectName, string serviceName, string command, List<string>? args, bool interactive);

    #endregion

    #region System Operations

    /// <summary>
    /// Prunes unused containers
    /// </summary>
    Task<CommandResult> PruneContainersAsync();

    /// <summary>
    /// Prunes unused images
    /// </summary>
    Task<CommandResult> PruneImagesAsync(bool dangling, bool all);

    /// <summary>
    /// Prunes all unused Docker objects
    /// </summary>
    Task<CommandResult> PruneSystemAsync(bool volumes);

    /// <summary>
    /// Gets Docker disk usage information
    /// </summary>
    Task<CommandResult> GetDiskUsageAsync();

    /// <summary>
    /// Gets Docker system information
    /// </summary>
    Task<CommandResult> GetDockerInfoAsync();

    /// <summary>
    /// Gets Docker version information
    /// </summary>
    Task<CommandResult> GetDockerVersionAsync();

    /// <summary>
    /// Monitors Docker events
    /// </summary>
    Task<CommandResult> MonitorEventsAsync(DateTime? since, DateTime? until, List<string>? filters);

    /// <summary>
    /// Pings Docker daemon
    /// </summary>
    Task<CommandResult> PingDockerAsync();

    /// <summary>
    /// Gets Docker processes information
    /// </summary>
    Task<CommandResult> GetDockerProcessesAsync();

    #endregion
}
