using Docker.DotNet.Models;
using DockerMcpServer.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace DockerMcpServer.Services;

/// <summary>
/// Helper methods for DockerService
/// </summary>
public partial class DockerService
{
    #region Helper Methods

    /// <summary>Parses a restart policy string to the appropriate enum value.</summary>
    private static RestartPolicyKind ParseRestartPolicy(string policy)
    {
        return policy.ToLowerInvariant() switch
        {
            "no" => RestartPolicyKind.No,
            "always" => RestartPolicyKind.Always,
            "unless-stopped" => RestartPolicyKind.UnlessStopped,
            "on-failure" => RestartPolicyKind.OnFailure,
            _ => RestartPolicyKind.No
        };
    }

    /// <summary>Parses a memory limit string with units (k, m, g, t) to bytes.</summary>
    private static long ParseMemoryLimit(string memoryLimit)
    {
        var value = memoryLimit.ToLowerInvariant();
        var multiplier = 1L;

        if (value.EndsWith("k"))
        {
            multiplier = 1024;
            value = value[..^1];
        }
        else if (value.EndsWith("m"))
        {
            multiplier = 1024 * 1024;
            value = value[..^1];
        }
        else if (value.EndsWith("g"))
        {
            multiplier = 1024 * 1024 * 1024;
            value = value[..^1];
        }
        else if (value.EndsWith("t"))
        {
            multiplier = 1024L * 1024 * 1024 * 1024;
            value = value[..^1];
        }

        if (long.TryParse(value, out var numericValue))
        {
            return numericValue * multiplier;
        }

        throw new ArgumentException($"Invalid memory limit format: {memoryLimit}");
    }

    /// <summary>Converts CPU limit from number of CPUs to nano CPUs for Docker API.</summary>
    private static long ParseCpuLimit(double cpuLimit)
    {
        // Convert CPU limit (in CPUs) to nano CPUs (1 CPU = 1,000,000,000 nano CPUs)
        return (long)(cpuLimit * 1_000_000_000);
    }

    /// <summary>Creates a basic tar stream for Docker build context (simplified implementation).</summary>
    private static Stream CreateTarStreamForBuild(string dockerfilePath, string? buildContext)
    {
        // Simple implementation - in production, you'd want a more robust tar creation
        var contextPath = buildContext ?? Path.GetDirectoryName(dockerfilePath) ?? Directory.GetCurrentDirectory();
        
        // For now, just return a basic stream with the Dockerfile
        // In a full implementation, you'd create a proper tar archive
        var dockerfileContent = File.ReadAllBytes(dockerfilePath);
        return new MemoryStream(dockerfileContent);
    }

    /// <summary>Executes a Docker CLI command and returns the success status and output.</summary>
    private async Task<(bool Success, string Output)> ExecuteDockerCommand(
        List<string> args, 
        string? workingDirectory = null, 
        Dictionary<string, string>? environment = null)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory()
            };

            foreach (var arg in args)
            {
                processStartInfo.ArgumentList.Add(arg);
            }

            if (environment != null)
            {
                foreach (var env in environment)
                {
                    processStartInfo.Environment[env.Key] = env.Value;
                }
            }

            using var process = new Process { StartInfo = processStartInfo };
            
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    outputBuilder.AppendLine(e.Data);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    errorBuilder.AppendLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            var output = outputBuilder.ToString().Trim();
            var error = errorBuilder.ToString().Trim();
            var combinedOutput = string.IsNullOrEmpty(error) ? output : $"{output}\n{error}";

            return (process.ExitCode == 0, combinedOutput);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute docker command: {Args}", string.Join(" ", args));
            return (false, ex.Message);
        }
    }

    /// <summary>
    /// Validates port mapping format
    /// </summary>
    private static bool IsValidPortMapping(string portMapping)
    {
        var parts = portMapping.Split(':');
        if (parts.Length != 2)
            return false;

        return int.TryParse(parts[0], out var hostPort) && 
               int.TryParse(parts[1].Split('/')[0], out var containerPort) &&
               hostPort > 0 && hostPort <= 65535 &&
               containerPort > 0 && containerPort <= 65535;
    }

    /// <summary>
    /// Validates volume mount format
    /// </summary>
    private static bool IsValidVolumeMount(string volumeMount)
    {
        var parts = volumeMount.Split(':');
        if (parts.Length < 2 || parts.Length > 3)
            return false;

        var hostPath = parts[0];
        var containerPath = parts[1];

        // Basic validation
        return !string.IsNullOrWhiteSpace(hostPath) && 
               !string.IsNullOrWhiteSpace(containerPath) &&
               Path.IsPathRooted(containerPath);
    }

    /// <summary>
    /// Parses resource limits for containers
    /// </summary>
    private static HostConfig ParseResourceLimits(
        string? memoryLimit = null,
        double? cpuLimit = null,
        long? pidsLimit = null,
        Dictionary<string, Ulimit>? ulimits = null)
    {
        var hostConfig = new HostConfig();

        if (!string.IsNullOrEmpty(memoryLimit))
        {
            hostConfig.Memory = ParseMemoryLimit(memoryLimit);
        }

        if (cpuLimit.HasValue)
        {
            // hostConfig.CpuShares = (long)cpuLimit.Value; // CPU limits may not be available in this API version
        }

        if (ulimits?.Count > 0)
        {
            hostConfig.Ulimits = ulimits.Values.ToList();
        }

        return hostConfig;
    }

    /// <summary>
    /// Creates network endpoint settings
    /// </summary>
    private static EndpointSettings CreateNetworkEndpoint(
        string? networkAlias = null,
        string? ipv4Address = null,
        string? ipv6Address = null,
        List<string>? links = null)
    {
        var endpoint = new EndpointSettings();

        if (!string.IsNullOrEmpty(networkAlias))
        {
            endpoint.Aliases = [networkAlias];
        }

        if (!string.IsNullOrEmpty(ipv4Address) || !string.IsNullOrEmpty(ipv6Address))
        {
            endpoint.IPAMConfig = new EndpointIPAMConfig
            {
                IPv4Address = ipv4Address,
                IPv6Address = ipv6Address
            };
        }

        if (links?.Count > 0)
        {
            endpoint.Links = links;
        }

        return endpoint;
    }

    /// <summary>
    /// Validates container name format
    /// </summary>
    private static bool IsValidContainerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // Container names must match [a-zA-Z0-9][a-zA-Z0-9_.-]*
        if (!char.IsLetterOrDigit(name[0]))
            return false;

        return name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.' || c == '-');
    }

    /// <summary>
    /// Validates image name format
    /// </summary>
    private static bool IsValidImageName(string imageName)
    {
        if (string.IsNullOrWhiteSpace(imageName))
            return false;

        // Basic validation for image names
        // More comprehensive validation would check registry, repository, and tag formats
        var parts = imageName.Split(':');
        var nameWithoutTag = parts[0];

        return !string.IsNullOrWhiteSpace(nameWithoutTag) && 
               !nameWithoutTag.Contains("//") &&
               !nameWithoutTag.StartsWith('/') &&
               !nameWithoutTag.EndsWith('/');
    }

    /// <summary>
    /// Formats bytes to human readable string
    /// </summary>
    private static string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int counter = 0;
        decimal number = bytes;
        
        while (Math.Round(number / 1024) >= 1)
        {
            number = number / 1024;
            counter++;
        }
        
        return $"{number:n1} {suffixes[counter]}";
    }

    /// <summary>
    /// Sanitizes environment variable key
    /// </summary>
    private static string SanitizeEnvironmentKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Environment variable key cannot be empty");

        // Environment variable names should match [a-zA-Z_][a-zA-Z0-9_]*
        if (!char.IsLetter(key[0]) && key[0] != '_')
            throw new ArgumentException($"Invalid environment variable key: {key}");

        if (!key.All(c => char.IsLetterOrDigit(c) || c == '_'))
            throw new ArgumentException($"Invalid environment variable key: {key}");

        return key.ToUpperInvariant();
    }

    /// <summary>
    /// Validates label key format
    /// </summary>
    private static bool IsValidLabelKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        // Label keys should follow reverse domain notation
        // Allow alphanumeric, dots, hyphens, and slashes
        return key.All(c => char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '/');
    }

    /// <summary>
    /// Creates default logging configuration
    /// </summary>
    private static LogConfig CreateDefaultLogConfig()
    {
        return new LogConfig
        {
            Type = "json-file",
            Config = new Dictionary<string, string>
            {
                ["max-size"] = "10m",
                ["max-file"] = "3"
            }
        };
    }

    #endregion
}
