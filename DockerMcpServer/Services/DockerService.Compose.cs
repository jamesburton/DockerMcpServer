using DockerMcpServer.Models;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using System.Text;

namespace DockerMcpServer.Services;

/// <summary>
/// Docker Compose operations for DockerService
/// </summary>
public partial class DockerService
{
    #region Compose Operations

    /// <summary>Deploys a Docker Compose stack using the provided YAML configuration.</summary>
    public async Task<CommandResult> DeployComposeStackAsync(ComposeStackRequest request)
    {
        try
        {
            // Parse and validate the compose file
            var deserializer = new DeserializerBuilder().Build();
            var composeData = deserializer.Deserialize(request.ComposeYaml);

            // Save compose file to temporary location
            var tempDir = Path.Combine(Path.GetTempPath(), $"docker-compose-{request.ProjectName}-{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDir);

            var composeFilePath = Path.Combine(tempDir, "docker-compose.yml");
            await File.WriteAllTextAsync(composeFilePath, request.ComposeYaml);

            try
            {
                // Use docker-compose CLI for now (could be improved with direct API calls)
                var args = new List<string>
                {
                    "compose",
                    "-f", composeFilePath,
                    "-p", request.ProjectName
                };

                if (request.Build)
                    args.Add("--build");

                if (request.ForceRecreate)
                    args.Add("--force-recreate");

                if (request.NoDeps)
                    args.Add("--no-deps");

                if (request.Pull)
                    args.Add("--pull");

                args.Add("up");
                args.Add("-d"); // Always run in detached mode

                var result = await ExecuteDockerCommand(args, request.WorkingDirectory, request.Environment);

                return new CommandResult
                {
                    Success = result.Success,
                    Message = result.Success ? $"Compose stack {request.ProjectName} deployed successfully" : "Failed to deploy compose stack",
                    ErrorMessage = result.Success ? null : result.Output,
                    Data = new { ProjectName = request.ProjectName, Output = result.Output }
                };
            }
            finally
            {
                // Clean up temporary files
                try
                {
                    Directory.Delete(tempDir, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deploy compose stack {ProjectName}", request.ProjectName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to deploy compose stack {request.ProjectName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Removes a Docker Compose stack and optionally its volumes and images.</summary>
    public async Task<CommandResult> RemoveComposeStackAsync(string projectName, bool removeVolumes = false, bool removeImages = false)
    {
        try
        {
            var args = new List<string>
            {
                "compose",
                "-p", projectName,
                "down"
            };

            if (removeVolumes)
                args.Add("--volumes");

            if (removeImages)
                args.Add("--rmi=all");

            var result = await ExecuteDockerCommand(args);

            return new CommandResult
            {
                Success = result.Success,
                Message = result.Success ? $"Compose stack {projectName} removed successfully" : "Failed to remove compose stack",
                ErrorMessage = result.Success ? null : result.Output,
                Data = new { ProjectName = projectName, Output = result.Output }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove compose stack {ProjectName}", projectName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to remove compose stack {projectName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Lists all containers in the specified Docker Compose stack.</summary>
    public async Task<CommandResult> ListComposeStackContainersAsync(string projectName)
    {
        try
        {
            var args = new List<string>
            {
                "compose",
                "-p", projectName,
                "ps",
                "--format", "json"
            };

            var result = await ExecuteDockerCommand(args);

            return new CommandResult
            {
                Success = result.Success,
                Message = result.Success ? $"Retrieved containers for compose stack {projectName}" : "Failed to list compose stack containers",
                ErrorMessage = result.Success ? null : result.Output,
                Data = new { ProjectName = projectName, Output = result.Output }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list containers for compose stack {ProjectName}", projectName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to list containers for compose stack {projectName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Gets logs from a Docker Compose stack, optionally following the output.</summary>
    public async Task<CommandResult> GetComposeStackLogsAsync(string projectName, string? serviceName = null, bool follow = false, int? tail = null)
    {
        try
        {
            var args = new List<string>
            {
                "compose",
                "-p", projectName,
                "logs"
            };

            if (follow)
                args.Add("--follow");

            if (tail.HasValue)
            {
                args.Add("--tail");
                args.Add(tail.Value.ToString());
            }

            if (!string.IsNullOrEmpty(serviceName))
                args.Add(serviceName);

            var result = await ExecuteDockerCommand(args);

            return new CommandResult
            {
                Success = result.Success,
                Message = result.Success ? $"Retrieved logs for compose stack {projectName}" : "Failed to get compose stack logs",
                ErrorMessage = result.Success ? null : result.Output,
                Data = new { ProjectName = projectName, ServiceName = serviceName, Output = result.Output }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get logs for compose stack {ProjectName}", projectName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to get logs for compose stack {projectName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Starts all services in a Docker Compose stack.</summary>
    public async Task<CommandResult> StartComposeStackAsync(string projectName)
    {
        try
        {
            var args = new List<string>
            {
                "compose",
                "-p", projectName,
                "start"
            };

            var result = await ExecuteDockerCommand(args);

            return new CommandResult
            {
                Success = result.Success,
                Message = result.Success ? $"Compose stack {projectName} started successfully" : "Failed to start compose stack",
                ErrorMessage = result.Success ? null : result.Output,
                Data = new { ProjectName = projectName, Output = result.Output }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start compose stack {ProjectName}", projectName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to start compose stack {projectName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Stops all services in a Docker Compose stack.</summary>
    public async Task<CommandResult> StopComposeStackAsync(string projectName)
    {
        try
        {
            var args = new List<string>
            {
                "compose",
                "-p", projectName,
                "stop"
            };

            var result = await ExecuteDockerCommand(args);

            return new CommandResult
            {
                Success = result.Success,
                Message = result.Success ? $"Compose stack {projectName} stopped successfully" : "Failed to stop compose stack",
                ErrorMessage = result.Success ? null : result.Output,
                Data = new { ProjectName = projectName, Output = result.Output }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop compose stack {ProjectName}", projectName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to stop compose stack {projectName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Restarts all services in a Docker Compose stack.</summary>
    public async Task<CommandResult> RestartComposeStackAsync(string projectName)
    {
        try
        {
            var args = new List<string>
            {
                "compose",
                "-p", projectName,
                "restart"
            };

            var result = await ExecuteDockerCommand(args);

            return new CommandResult
            {
                Success = true,
                Message = result.Success ? $"Compose stack {projectName} restarted successfully" : "Failed to restart compose stack",
                ErrorMessage = result.Success ? null : result.Output,
                Data = new { ProjectName = projectName, Output = result.Output }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restart compose stack {ProjectName}", projectName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to restart compose stack {projectName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Scales a specific service in a Docker Compose stack to the specified number of replicas.</summary>
    public async Task<CommandResult> ScaleComposeServiceAsync(string projectName, string serviceName, int replicas)
    {
        try
        {
            var args = new List<string>
            {
                "compose",
                "-p", projectName,
                "scale",
                $"{serviceName}={replicas}"
            };

            var result = await ExecuteDockerCommand(args);

            return new CommandResult
            {
                Success = result.Success,
                Message = result.Success ? $"Service {serviceName} scaled to {replicas} replicas" : "Failed to scale service",
                ErrorMessage = result.Success ? null : result.Output,
                Data = new { ProjectName = projectName, ServiceName = serviceName, Replicas = replicas, Output = result.Output }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scale service {Service} in compose stack {ProjectName}", serviceName, projectName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to scale service {serviceName} in compose stack {projectName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Executes a command in a running service within a Docker Compose stack.</summary>
    public async Task<CommandResult> ExecComposeServiceAsync(string projectName, string serviceName, string command, List<string>? args = null, bool interactive = false)
    {
        try
        {
            var execArgs = new List<string>
            {
                "compose",
                "-p", projectName,
                "exec"
            };

            if (!interactive)
                execArgs.Add("-T");

            execArgs.Add(serviceName);
            execArgs.Add(command);

            if (args?.Count > 0)
                execArgs.AddRange(args);

            var result = await ExecuteDockerCommand(execArgs);

            return new CommandResult
            {
                Success = result.Success,
                Message = result.Success ? $"Command executed in service {serviceName}" : "Failed to execute command",
                ErrorMessage = result.Success ? null : result.Output,
                Data = new { ProjectName = projectName, ServiceName = serviceName, Command = command, Output = result.Output }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute command in service {Service} of compose stack {ProjectName}", serviceName, projectName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to execute command in service {serviceName} of compose stack {projectName}",
                ErrorMessage = ex.Message
            };
        }
    }

    #endregion
}
