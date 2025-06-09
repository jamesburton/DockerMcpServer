using Docker.DotNet.Models;
using DockerMcpServer.Models;
using Microsoft.Extensions.Logging;

namespace DockerMcpServer.Services;

/// <summary>
/// Network operations for DockerService
/// </summary>
public partial class DockerService
{
    #region Network Operations

    /// <summary>Lists all Docker networks in the system.</summary>
    public async Task<CommandResult> ListNetworksAsync()
    {
        EnsureDockerClient();

        try
        {
            var networks = await _dockerClient!.Networks.ListNetworksAsync();

            var networkInfos = networks.Select(n => new NetworkInfo
            {
                Id = n.ID,
                Name = n.Name,
                Driver = n.Driver,
                Scope = n.Scope,
                Internal = n.Internal,
                Attachable = n.Attachable,
                Ingress = n.Ingress,
                Created = n.Created,
                Labels = n.Labels?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? [],
            }).ToList();

            return new CommandResult
            {
                Success = true,
                Message = $"Found {networkInfos.Count} networks",
                Data = networkInfos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list networks");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to list networks",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Creates a new Docker network with specified configuration options.</summary>
    public async Task<CommandResult> CreateNetworkAsync(
        string name,
        string? driver = null,
        bool? isInternal = null,
        bool? attachable = null,
        Dictionary<string, string>? options = null,
        Dictionary<string, string>? labels = null,
        List<IPAMConfig>? ipamConfigs = null)
    {
        EnsureDockerClient();

        try
        {
            var createParams = new NetworksCreateParameters
            {
                Name = name,
                Driver = driver ?? "bridge",
                Internal = isInternal ?? false,
                Attachable = attachable ?? true,
                Options = options,
                Labels = labels
            };

            if (ipamConfigs?.Count > 0)
            {
                createParams.IPAM = new IPAM
                {
                    Config = ipamConfigs
                };
            }

            var response = await _dockerClient!.Networks.CreateNetworkAsync(createParams);

            return new CommandResult
            {
                Success = true,
                Message = $"Network {name} created successfully",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create network {Network}", name);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to create network {name}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Removes a Docker network by ID or name.</summary>
    public async Task<CommandResult> RemoveNetworkAsync(string networkIdOrName)
    {
        EnsureDockerClient();

        try
        {
            await _dockerClient!.Networks.DeleteNetworkAsync(networkIdOrName);

            return new CommandResult
            {
                Success = true,
                Message = $"Network {networkIdOrName} removed successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove network {Network}", networkIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to remove network {networkIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Connects a container to a Docker network with optional network configuration.</summary>
    public async Task<CommandResult> ConnectNetworkAsync(
        string networkIdOrName, 
        string containerIdOrName, 
        string? alias = null, 
        List<string>? ipv4Addresses = null, 
        List<string>? ipv6Addresses = null)
    {
        EnsureDockerClient();

        try
        {
            var connectParams = new NetworkConnectParameters
            {
                Container = containerIdOrName
            };

            if (!string.IsNullOrEmpty(alias) || ipv4Addresses?.Count > 0 || ipv6Addresses?.Count > 0)
            {
                connectParams.EndpointConfig = new EndpointSettings();

                if (!string.IsNullOrEmpty(alias))
                {
                    connectParams.EndpointConfig.Aliases = [alias];
                }

                if (ipv4Addresses?.Count > 0 || ipv6Addresses?.Count > 0)
                {
                    connectParams.EndpointConfig.IPAMConfig = new EndpointIPAMConfig
                    {
                        IPv4Address = ipv4Addresses?.FirstOrDefault(),
                        IPv6Address = ipv6Addresses?.FirstOrDefault()
                    };
                }
            }

            await _dockerClient!.Networks.ConnectNetworkAsync(networkIdOrName, connectParams);

            return new CommandResult
            {
                Success = true,
                Message = $"Container {containerIdOrName} connected to network {networkIdOrName}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect container {Container} to network {Network}", containerIdOrName, networkIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to connect container {containerIdOrName} to network {networkIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Disconnects a container from a Docker network with optional force parameter.</summary>
    public async Task<CommandResult> DisconnectNetworkAsync(string networkIdOrName, string containerIdOrName, bool force = false)
    {
        EnsureDockerClient();

        try
        {
            var disconnectParams = new NetworkDisconnectParameters
            {
                Container = containerIdOrName,
                Force = force
            };

            await _dockerClient!.Networks.DisconnectNetworkAsync(networkIdOrName, disconnectParams);

            return new CommandResult
            {
                Success = true,
                Message = $"Container {containerIdOrName} disconnected from network {networkIdOrName}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to disconnect container {Container} from network {Network}", containerIdOrName, networkIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to disconnect container {containerIdOrName} from network {networkIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Inspects a Docker network and returns detailed configuration information.</summary>
    public async Task<CommandResult> InspectNetworkAsync(string networkIdOrName)
    {
        EnsureDockerClient();

        try
        {
            var network = await _dockerClient!.Networks.InspectNetworkAsync(networkIdOrName);

            return new CommandResult
            {
                Success = true,
                Message = $"Network {networkIdOrName} inspected successfully",
                Data = network
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inspect network {Network}", networkIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to inspect network {networkIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Removes unused Docker networks to free up system resources.</summary>
    public async Task<CommandResult> PruneNetworksAsync()
    {
        EnsureDockerClient();

        try
        {
            var response = await _dockerClient!.Networks.PruneNetworksAsync();

            return new CommandResult
            {
                Success = true,
                Message = $"Pruned {response.NetworksDeleted?.Count ?? 0} networks",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to prune networks");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to prune networks",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Creates a Docker network with a specific subnet configuration.</summary>
    public async Task<CommandResult> CreateNetworkWithSubnetAsync(
        string name,
        string subnet,
        string? gateway = null,
        string? driver = null,
        Dictionary<string, string>? options = null,
        Dictionary<string, string>? labels = null)
    {
        EnsureDockerClient();

        try
        {
            var ipamConfig = new IPAMConfig
            {
                Subnet = subnet
            };

            if (!string.IsNullOrEmpty(gateway))
            {
                ipamConfig.Gateway = gateway;
            }

            var createParams = new NetworksCreateParameters
            {
                Name = name,
                Driver = driver ?? "bridge",
                Options = options,
                Labels = labels,
                IPAM = new IPAM
                {
                    Config = [ipamConfig]
                }
            };

            var response = await _dockerClient!.Networks.CreateNetworkAsync(createParams);

            return new CommandResult
            {
                Success = true,
                Message = $"Network {name} created successfully with subnet {subnet}",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create network {Network} with subnet {Subnet}", name, subnet);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to create network {name} with subnet {subnet}",
                ErrorMessage = ex.Message
            };
        }
    }

    #endregion
}
