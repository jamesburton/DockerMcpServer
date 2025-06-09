using Docker.DotNet;

namespace DockerMcpServer.Core;

/// <summary>
/// Factory for creating Docker clients
/// </summary>
public interface IDockerClientFactory
{
    /// <summary>
    /// Creates a Docker client
    /// </summary>
    /// <returns>Docker client instance</returns>
    DockerClient CreateClient();
}

/// <summary>
/// Default Docker client factory
/// </summary>
public class DockerClientFactory : IDockerClientFactory
{
    /// <summary>Creates a Docker client instance using default configuration.</summary>
    public DockerClient CreateClient()
    {
        // Try to connect to Docker daemon using default configuration
        var config = new DockerClientConfiguration();
        return config.CreateClient();
    }
}
