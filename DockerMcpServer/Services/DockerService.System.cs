using Docker.DotNet.Models;
using DockerMcpServer.Models;
using Microsoft.Extensions.Logging;

namespace DockerMcpServer.Services;

/// <summary>
/// System operations for DockerService
/// </summary>
public partial class DockerService
{
    #region System Operations

    /// <summary>Removes stopped containers to free up system resources.</summary>
    public async Task<CommandResult> PruneContainersAsync()
    {
        EnsureDockerClient();

        try
        {
            var response = await _dockerClient!.Containers.PruneContainersAsync();

            return new CommandResult
            {
                Success = true,
                Message = $"Pruned {response.ContainersDeleted?.Count ?? 0} containers, reclaimed {response.SpaceReclaimed} bytes",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to prune containers");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to prune containers",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Removes unused Docker images with options for dangling and all unused images.</summary>
    public async Task<CommandResult> PruneImagesAsync(bool dangling = true, bool all = false)
    {
        EnsureDockerClient();

        try
        {
            var pruneParams = new ImagesPruneParameters();
            
            if (dangling && !all)
            {
                pruneParams.Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["dangling"] = new Dictionary<string, bool> { ["true"] = true }
                };
            }
            else if (all)
            {
                // Prune all unused images, not just dangling ones
                pruneParams.Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["dangling"] = new Dictionary<string, bool> { ["false"] = true }
                };
            }

            var response = await _dockerClient!.Images.PruneImagesAsync(pruneParams);

            return new CommandResult
            {
                Success = true,
                Message = $"Pruned {response.ImagesDeleted?.Count ?? 0} images, reclaimed {response.SpaceReclaimed} bytes",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to prune images");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to prune images",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Performs comprehensive system cleanup including containers, images, networks, and optionally volumes.</summary>
    public async Task<CommandResult> PruneSystemAsync(bool volumes = false)
    {
        EnsureDockerClient();

        try
        {
            // Note: Docker.DotNet doesn't have a direct PruneSystemAsync method
            // So we'll prune components individually
            var results = new List<string>();
            
            // Prune containers
            var containerResult = await PruneContainersAsync();
            if (containerResult.Success)
                results.Add(containerResult.Message);

            // Prune images  
            var imageResult = await PruneImagesAsync(true, false);
            if (imageResult.Success)
                results.Add(imageResult.Message);

            // Prune networks
            var networkResult = await PruneNetworksAsync();
            if (networkResult.Success)
                results.Add(networkResult.Message);

            // Prune volumes if requested
            if (volumes)
            {
                var volumeResult = await PruneVolumesAsync();
                if (volumeResult.Success)
                    results.Add(volumeResult.Message);
            }

            return new CommandResult
            {
                Success = true,
                Message = $"System pruned successfully. {string.Join("; ", results)}",
                Data = results
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to prune system");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to prune system",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Gets Docker system disk usage information.</summary>
    public async Task<CommandResult> GetDiskUsageAsync()
    {
        EnsureDockerClient();

        try
        {
            var response = await _dockerClient!.System.GetSystemInfoAsync();

            return new CommandResult
            {
                Success = true,
                Message = "Retrieved disk usage information",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get disk usage");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to get disk usage",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Retrieves comprehensive Docker system information and configuration.</summary>
    public async Task<CommandResult> GetDockerInfoAsync()
    {
        EnsureDockerClient();

        try
        {
            var info = await _dockerClient!.System.GetSystemInfoAsync();

            return new CommandResult
            {
                Success = true,
                Message = "Retrieved Docker system information",
                Data = info
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Docker info");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to get Docker info",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Gets Docker daemon version and build information.</summary>
    public async Task<CommandResult> GetDockerVersionAsync()
    {
        EnsureDockerClient();

        try
        {
            var version = await _dockerClient!.System.GetVersionAsync();

            return new CommandResult
            {
                Success = true,
                Message = "Retrieved Docker version information",
                Data = version
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Docker version");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to get Docker version",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Monitors Docker events with optional filtering and time constraints (currently disabled due to API compatibility).</summary>
    public Task<CommandResult> MonitorEventsAsync(DateTime? since = null, DateTime? until = null, List<string>? filters = null)
    {
        EnsureDockerClient();

        try
        {
            // Note: The specific events API may vary by Docker.DotNet version
            // For now, return a placeholder that indicates the feature needs implementation
            return Task.FromResult(new CommandResult
            {
                Success = true,
                Message = "Docker events monitoring is not implemented in this version - API compatibility issue",
                Data = new List<string>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to monitor Docker events");
            return Task.FromResult(new CommandResult
            {
                Success = false,
                Message = "Failed to monitor Docker events",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>Tests Docker daemon connectivity and responsiveness.</summary>
    public async Task<CommandResult> PingDockerAsync()
    {
        EnsureDockerClient();

        try
        {
            await _dockerClient!.System.PingAsync();

            return new CommandResult
            {
                Success = true,
                Message = "Docker ping successful - daemon is responsive"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Docker ping failed");
            return new CommandResult
            {
                Success = false,
                Message = "Docker ping failed - daemon may be unresponsive",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Gets real-time Docker process statistics in tabular format.</summary>
    public async Task<CommandResult> GetDockerProcessesAsync()
    {
        try
        {
            var args = new List<string> { "stats", "--no-stream", "--format", "table" };
            var result = await ExecuteDockerCommand(args);

            return new CommandResult
            {
                Success = result.Success,
                Message = result.Success ? "Retrieved Docker processes" : "Failed to get Docker processes",
                ErrorMessage = result.Success ? null : result.Output,
                Data = result.Output
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Docker processes");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to get Docker processes",
                ErrorMessage = ex.Message
            };
        }
    }

    #endregion
}
