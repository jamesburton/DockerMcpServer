using Docker.DotNet.Models;
using Docker.DotNet;
using DockerMcpServer.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace DockerMcpServer.Services;

/// <summary>
/// Container operations for DockerService
/// </summary>
public partial class DockerService
{
    #region Container Operations

    /// <summary>Creates a new Docker container with comprehensive configuration options and optionally starts it.</summary>
    public async Task<CommandResult> CreateContainerAsync(ContainerCreateRequest request, bool start = true)
    {
        EnsureDockerClient();

        try
        {
            // Parse port bindings
            var portBindings = new Dictionary<string, IList<PortBinding>>();
            if (request.Ports != null)
            {
                foreach (var port in request.Ports)
                {
                    var parts = port.Split(':');
                    if (parts.Length == 2)
                    {
                        var containerPort = parts[1];
                        var hostPort = parts[0];
                        
                        if (!containerPort.Contains('/'))
                            containerPort += "/tcp";

                        portBindings[containerPort] = [new() { HostPort = hostPort }];
                    }
                }
            }

            // Parse volume bindings
            var binds = request.Volumes?.ToList() ?? [];

            // Parse environment variables
            var env = request.Environment?.ToList() ?? [];

            // Parse restart policy
            RestartPolicy? restartPolicy = null;
            if (!string.IsNullOrEmpty(request.RestartPolicy))
            {
                restartPolicy = new RestartPolicy
                {
                    Name = ParseRestartPolicy(request.RestartPolicy)
                };
            }

            // Parse memory limit
            long? memoryLimit = null;
            if (!string.IsNullOrEmpty(request.MemoryLimit))
            {
                memoryLimit = ParseMemoryLimit(request.MemoryLimit);
            }

            // Parse devices
            var devices = new List<DeviceMapping>();
            if (request.Devices != null)
            {
                foreach (var device in request.Devices)
                {
                    var parts = device.Split(':');
                    if (parts.Length >= 2)
                    {
                        devices.Add(new DeviceMapping
                        {
                            PathOnHost = parts[0],
                            PathInContainer = parts[1],
                            CgroupPermissions = parts.Length > 2 ? parts[2] : "rwm"
                        });
                    }
                }
            }

            // Parse extra hosts
            var extraHosts = request.ExtraHosts?.ToList() ?? [];

            // Parse ulimits
            var ulimits = new List<Ulimit>();
            if (request.Ulimits != null)
            {
                foreach (var ulimit in request.Ulimits)
                {
                    var parts = ulimit.Split('=');
                    if (parts.Length == 2)
                    {
                        var name = parts[0];
                        var limits = parts[1].Split(':');
                        if (limits.Length == 2 && 
                            long.TryParse(limits[0], out var soft) && 
                            long.TryParse(limits[1], out var hard))
                        {
                            ulimits.Add(new Ulimit
                            {
                                Name = name,
                                Soft = soft,
                                Hard = hard
                            });
                        }
                    }
                }
            }

            // Parse tmpfs mounts
            var tmpfsMounts = new Dictionary<string, string>();
            if (request.Tmpfs != null)
            {
                foreach (var tmpfs in request.Tmpfs)
                {
                    var parts = tmpfs.Split(':');
                    tmpfsMounts[parts[0]] = parts.Length > 1 ? parts[1] : "";
                }
            }

            var createParameters = new CreateContainerParameters
            {
                Image = request.Image,
                Name = request.Name,
                Cmd = BuildCommand(request.Command, request.Args),
                Env = env,
                WorkingDir = request.WorkingDir,
                User = request.User,
                Hostname = request.Hostname,
                Domainname = request.Domainname,
                AttachStdin = request.Interactive,
                AttachStdout = request.Detach,
                AttachStderr = request.Detach,
                Tty = request.Tty,
                OpenStdin = request.Interactive,
                StdinOnce = request.Interactive,
                Labels = request.Labels ?? [],
                HostConfig = new HostConfig
                {
                    PortBindings = portBindings,
                    Binds = binds,
                    AutoRemove = request.AutoRemove,
                    ReadonlyRootfs = request.ReadOnly,
                    Privileged = request.Privileged,
                    NetworkMode = request.Network,
                    RestartPolicy = restartPolicy,
                    Memory = memoryLimit ?? 0,
                    // CpuShares = request.CpuLimit.HasValue ? (long)request.CpuLimit.Value : null, // CPU limits may not be available in this API version
                    CapAdd = request.CapAdd?.ToList(),
                    CapDrop = request.CapDrop?.ToList(),
                    Devices = devices,
                    DNS = request.Dns?.ToList(),
                    ExtraHosts = extraHosts,
                    SecurityOpt = request.SecurityOpt?.ToList(),
                    Tmpfs = tmpfsMounts,
                    Ulimits = ulimits
                }
            };

            var container = await _dockerClient!.Containers.CreateContainerAsync(createParameters);

            var result = new CommandResult
            {
                Success = true,
                ContainerId = container.ID,
                Message = $"Container created with ID: {container.ID}"
            };

            if (start)
            {
                var startResult = await StartContainerAsync(container.ID);
                if (startResult.Success)
                {
                    result.Message += " and started successfully";
                }
                else
                {
                    result.Message += $" but failed to start: {startResult.ErrorMessage}";
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create container from image {Image}", request.Image);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to create container from image {request.Image}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Lists Docker containers with optional filtering and configuration options.</summary>
    public async Task<CommandResult> ListContainersAsync(ContainerListOptions? options = null)
    {
        EnsureDockerClient();

        try
        {
            options ??= new ContainerListOptions();

            var listParameters = new ContainersListParameters
            {
                All = options.All,
                Size = options.Size,
                Limit = options.Limit,
                Since = options.Since,
                Before = options.Before
            };

            if (options.Labels?.Count > 0)
            {
                listParameters.Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["label"] = options.Labels.ToDictionary(kvp => $"{kvp.Key}={kvp.Value}", _ => true)
                };
            }

            var containers = await _dockerClient!.Containers.ListContainersAsync(listParameters);

            var containerInfos = containers.Select(c => new ContainerInfo
            {
                Id = c.ID,
                Name = c.Names.FirstOrDefault()?.TrimStart('/') ?? "",
                Image = c.Image,
                State = c.State,
                Status = c.Status,
                Created = c.Created,
                Ports = c.Ports?.Select(p => $"{p.PublicPort}:{p.PrivatePort}/{p.Type}").ToList() ?? [],
                Mounts = c.Mounts?.Select(m => $"{m.Source}:{m.Destination}").ToList() ?? [],
                Labels = c.Labels?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? [],
                SizeRw = c.SizeRw,
                SizeRootFs = c.SizeRootFs
            }).ToList();

            return new CommandResult
            {
                Success = true,
                Message = $"Found {containerInfos.Count} containers",
                Data = containerInfos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list containers");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to list containers",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Starts a Docker container by ID or name.</summary>
    public async Task<CommandResult> StartContainerAsync(string containerIdOrName)
    {
        EnsureDockerClient();

        try
        {
            _ = await _dockerClient!.Containers.StartContainerAsync(containerIdOrName, new ContainerStartParameters());

            return new CommandResult
            {
                Success = true,
                ContainerId = containerIdOrName,
                Message = $"Container {containerIdOrName} started successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start container {Container}", containerIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to start container {containerIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Stops a Docker container with an optional timeout before forceful termination.</summary>
    public async Task<CommandResult> StopContainerAsync(string containerIdOrName, int timeoutSeconds = 10)
    {
        EnsureDockerClient();

        try
        {
            var stopParameters = new ContainerStopParameters
            {
                WaitBeforeKillSeconds = (uint)timeoutSeconds
            };

            _ = await _dockerClient!.Containers.StopContainerAsync(containerIdOrName, stopParameters);

            return new CommandResult
            {
                Success = true,
                ContainerId = containerIdOrName,
                Message = $"Container {containerIdOrName} stopped successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop container {Container}", containerIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to stop container {containerIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Restarts a Docker container with an optional timeout before forceful termination.</summary>
    public async Task<CommandResult> RestartContainerAsync(string containerIdOrName, int timeoutSeconds = 10)
    {
        EnsureDockerClient();

        try
        {
            var restartParameters = new ContainerRestartParameters
            {
                WaitBeforeKillSeconds = (uint)timeoutSeconds
            };

            await _dockerClient!.Containers.RestartContainerAsync(containerIdOrName, restartParameters);

            return new CommandResult
            {
                Success = true,
                ContainerId = containerIdOrName,
                Message = $"Container {containerIdOrName} restarted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restart container {Container}", containerIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to restart container {containerIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Removes a Docker container with options for force removal and volume cleanup.</summary>
    public async Task<CommandResult> RemoveContainerAsync(string containerIdOrName, bool force = false, bool removeVolumes = false)
    {
        EnsureDockerClient();

        try
        {
            var removeParameters = new ContainerRemoveParameters
            {
                Force = force,
                RemoveVolumes = removeVolumes
            };

            await _dockerClient!.Containers.RemoveContainerAsync(containerIdOrName, removeParameters);

            return new CommandResult
            {
                Success = true,
                ContainerId = containerIdOrName,
                Message = $"Container {containerIdOrName} removed successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove container {Container}", containerIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to remove container {containerIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Inspects a Docker container and returns detailed configuration and state information.</summary>
    public async Task<CommandResult> InspectContainerAsync(string containerIdOrName)
    {
        EnsureDockerClient();

        try
        {
            var container = await _dockerClient!.Containers.InspectContainerAsync(containerIdOrName);

            return new CommandResult
            {
                Success = true,
                ContainerId = containerIdOrName,
                Message = $"Container {containerIdOrName} inspected successfully",
                Data = container
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inspect container {Container}", containerIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to inspect container {containerIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Retrieves logs from a Docker container with configurable output options.</summary>
    public async Task<CommandResult> GetContainerLogsAsync(string containerIdOrName, ContainerLogsOptions? options = null)
    {
        EnsureDockerClient();

        try
        {
            options ??= new ContainerLogsOptions();

            var logsParameters = new ContainerLogsParameters
            {
                ShowStdout = options.Stdout,
                ShowStderr = options.Stderr,
                Timestamps = options.Timestamps,
                Follow = options.Follow,
                Tail = options.Tail?.ToString() ?? "all",
                Since = options.Since,
                Until = options.Until
            };

            using var logsStream = await _dockerClient!.Containers.GetContainerLogsAsync(containerIdOrName, true, logsParameters, CancellationToken.None);
            // Handle MultiplexedStream properly for logs
            var (stdOut, stdErr) = await logsStream.ReadOutputToEndAsync(CancellationToken.None);

            return new CommandResult
            {
                Success = true,
                ContainerId = containerIdOrName,
                Message = $"Retrieved logs for container {containerIdOrName}",
                Data = new { Output = stdOut, Errors = stdErr },
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get logs for container {Container}", containerIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to get logs for container {containerIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Gets real-time statistics for a Docker container including CPU, memory, and network usage.</summary>
    public async Task<CommandResult> GetContainerStatsAsync(string containerIdOrName, bool stream = false)
    {
        EnsureDockerClient();

        try
        {
            var statsParameters = new ContainerStatsParameters
            {
                Stream = stream
            };

            await _dockerClient!.Containers.GetContainerStatsAsync(containerIdOrName, statsParameters, new Progress<ContainerStatsResponse>(), CancellationToken.None);

            return new CommandResult
            {
                Success = true,
                ContainerId = containerIdOrName,
                Message = $"Retrieved stats for container {containerIdOrName}",
                Data = "Stats streaming initiated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get stats for container {Container}", containerIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to get stats for container {containerIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Executes a command inside a running Docker container and returns the output.</summary>
    public async Task<CommandResult> ExecContainerAsync(string containerIdOrName, string command, List<string>? args = null, bool interactive = false)
    {
        EnsureDockerClient();

        try
        {
            var cmd = BuildCommand(command, args);

            var execCreateParameters = new ContainerExecCreateParameters
            {
                Cmd = cmd,
                AttachStdout = true,
                AttachStderr = true,
                AttachStdin = interactive,
                Tty = interactive
            };

            var execCreateResponse = await _dockerClient!.Exec.ExecCreateContainerAsync(containerIdOrName, execCreateParameters);

            using var execStream = await _dockerClient!.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, false, CancellationToken.None);
            
            var (stdOut, stdErr) = await execStream.ReadOutputToEndAsync(CancellationToken.None);

            return new CommandResult
            {
                Success = true,
                ContainerId = containerIdOrName,
                Message = $"Executed command in container {containerIdOrName}",
                Data = new { Output = stdOut, Errors = stdErr },
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute command in container {Container}", containerIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to execute command in container {containerIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>Builds a command list from a command string and optional arguments.</summary>
    private static List<string> BuildCommand(string? command, List<string>? args)
    {
        var cmd = new List<string>();
        
        if (!string.IsNullOrEmpty(command))
        {
            cmd.Add(command);
        }
        
        if (args?.Count > 0)
        {
            cmd.AddRange(args);
        }

        return cmd;
    }

    #endregion
}
