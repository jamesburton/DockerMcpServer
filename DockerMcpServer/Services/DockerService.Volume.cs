using Docker.DotNet.Models;
using DockerMcpServer.Models;
using Microsoft.Extensions.Logging;

namespace DockerMcpServer.Services;

/// <summary>
/// Volume operations for DockerService
/// </summary>
public partial class DockerService
{
    #region Volume Operations

    /// <summary>Lists all Docker volumes in the system.</summary>
    public async Task<CommandResult> ListVolumesAsync()
    {
        EnsureDockerClient();

        try
        {
            var volumes = await _dockerClient!.Volumes.ListAsync();

            var volumeInfos = volumes.Volumes.Select(v => new VolumeInfo
            {
                Name = v.Name,
                Driver = v.Driver,
                Mountpoint = v.Mountpoint,
                Scope = v.Scope,
                CreatedAt = DateTime.TryParse(v.CreatedAt, out var createdAt) ? createdAt : DateTime.MinValue,
                Labels = v.Labels?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? [],
                Options = v.Options?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? [],
            }).ToList();

            return new CommandResult
            {
                Success = true,
                Message = $"Found {volumeInfos.Count} volumes",
                Data = volumeInfos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list volumes");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to list volumes",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Creates a new Docker volume with optional driver and configuration options.</summary>
    public async Task<CommandResult> CreateVolumeAsync(string name, string? driver = null, Dictionary<string, string>? driverOpts = null, Dictionary<string, string>? labels = null)
    {
        EnsureDockerClient();

        try
        {
            var createParams = new VolumesCreateParameters
            {
                Name = name,
                Driver = driver ?? "local",
                DriverOpts = driverOpts,
                Labels = labels
            };

            var response = await _dockerClient!.Volumes.CreateAsync(createParams);

            return new CommandResult
            {
                Success = true,
                Message = $"Volume {name} created successfully",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create volume {Volume}", name);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to create volume {name}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Removes a Docker volume by name with optional force parameter.</summary>
    public async Task<CommandResult> RemoveVolumeAsync(string volumeName, bool force = false)
    {
        EnsureDockerClient();

        try
        {
            await _dockerClient!.Volumes.RemoveAsync(volumeName, force);

            return new CommandResult
            {
                Success = true,
                Message = $"Volume {volumeName} removed successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove volume {Volume}", volumeName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to remove volume {volumeName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Inspects a Docker volume and returns detailed configuration information.</summary>
    public async Task<CommandResult> InspectVolumeAsync(string volumeName)
    {
        EnsureDockerClient();

        try
        {
            var volume = await _dockerClient!.Volumes.InspectAsync(volumeName);

            return new CommandResult
            {
                Success = true,
                Message = $"Volume {volumeName} inspected successfully",
                Data = volume
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inspect volume {Volume}", volumeName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to inspect volume {volumeName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Removes unused Docker volumes to free up disk space.</summary>
    public async Task<CommandResult> PruneVolumesAsync()
    {
        EnsureDockerClient();

        try
        {
            var response = await _dockerClient!.Volumes.PruneAsync();

            return new CommandResult
            {
                Success = true,
                Message = $"Pruned {response.VolumesDeleted?.Count ?? 0} volumes, reclaimed {response.SpaceReclaimed} bytes",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to prune volumes");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to prune volumes",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Creates a Docker volume with advanced configuration options including custom mountpoints.</summary>
    public async Task<CommandResult> CreateVolumeWithOptionsAsync(
        string name,
        string? driver = null,
        Dictionary<string, string>? driverOpts = null,
        Dictionary<string, string>? labels = null,
        string? mountpoint = null)
    {
        EnsureDockerClient();

        try
        {
            var createParams = new VolumesCreateParameters
            {
                Name = name,
                Driver = driver ?? "local",
                DriverOpts = driverOpts ?? [],
                Labels = labels ?? []
            };

            // Add mountpoint to driver options if specified
            if (!string.IsNullOrEmpty(mountpoint))
            {
                createParams.DriverOpts["device"] = mountpoint;
                createParams.DriverOpts["o"] = "bind";
                createParams.DriverOpts["type"] = "none";
            }

            var response = await _dockerClient!.Volumes.CreateAsync(createParams);

            return new CommandResult
            {
                Success = true,
                Message = $"Volume {name} created successfully with custom options",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create volume {Volume} with options", name);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to create volume {name} with options",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Lists Docker volumes with custom filtering options.</summary>
    public async Task<CommandResult> ListVolumesByFilterAsync(Dictionary<string, string>? filters = null)
    {
        EnsureDockerClient();

        try
        {
            var listParams = new VolumesListParameters();

            if (filters?.Count > 0)
            {
                listParams.Filters = new Dictionary<string, IDictionary<string, bool>>();
                foreach (var filter in filters)
                {
                    listParams.Filters[filter.Key] = new Dictionary<string, bool> { [filter.Value] = true };
                }
            }

            var volumes = await _dockerClient!.Volumes.ListAsync(listParams);

            var volumeInfos = volumes.Volumes.Select(v => new VolumeInfo
            {
                Name = v.Name,
                Driver = v.Driver,
                Mountpoint = v.Mountpoint,
                Scope = v.Scope,
                CreatedAt = DateTime.TryParse(v.CreatedAt, out var createdAt) ? createdAt : DateTime.MinValue,
                Labels = v.Labels?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? [],
                Options = v.Options?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? []
            }).ToList();

            return new CommandResult
            {
                Success = true,
                Message = $"Found {volumeInfos.Count} volumes matching filters",
                Data = volumeInfos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list volumes with filters");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to list volumes with filters",
                ErrorMessage = ex.Message
            };
        }
    }

    #endregion
}
