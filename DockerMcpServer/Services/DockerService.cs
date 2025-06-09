using Docker.DotNet;
using Microsoft.Extensions.Logging;
using DockerMcpServer.Models;
using DockerMcpServer.Core;

namespace DockerMcpServer.Services;

/// <summary>
/// Main Docker service providing comprehensive Docker operations
/// Features: Container Management, Image Operations, Volume Management, 
/// Network Management, Docker Compose, System Operations, Security Options
/// 
/// This class is organized using partial classes for better maintainability:
/// - DockerService.Container.cs: Container operations
/// - DockerService.Image.cs: Image operations  
/// - DockerService.Volume.cs: Volume operations
/// - DockerService.Network.cs: Network operations
/// - DockerService.Compose.cs: Docker Compose operations
/// - DockerService.System.cs: System operations and cleanup
/// - DockerService.Security.cs: Security-related functionality
/// - DockerService.Helpers.cs: Helper methods and utilities
/// </summary>
public partial class DockerService : IDockerService, IDisposable
{
    private readonly ILogger<DockerService> _logger;
    private DockerClient? _dockerClient;
    private bool _disposed = false;

    /// <summary>Initializes a new instance of the DockerService class.</summary>
    public DockerService(ILogger<DockerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Initializes the Docker client connection
    /// </summary>
    public Task InitializeAsync()
    {
        try
        {
            _dockerClient = new DockerClientConfiguration()
                .CreateClient();

            _logger.LogInformation("Docker client initialized successfully");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Docker client");
            throw;
        }
    }

    /// <summary>
    /// Tests the Docker connection
    /// </summary>
    public async Task<CommandResult> TestConnectionAsync()
    {
        try
        {
            EnsureDockerClient();
            var version = await _dockerClient!.System.GetVersionAsync();
            
            return new CommandResult
            {
                Success = true,
                Message = "Docker connection successful",
                Data = version
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Docker connection test failed");
            return new CommandResult
            {
                Success = false,
                Message = "Docker connection failed",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Ensures Docker client is initialized
    /// </summary>
    private void EnsureDockerClient()
    {
        if (_dockerClient == null)
            throw new InvalidOperationException("Docker client not initialized. Call InitializeAsync first.");
    }

    /// <summary>Disposes the DockerService and releases resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Disposes the DockerService and releases resources.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dockerClient?.Dispose();
            }
            _disposed = true;
        }
    }
}
