# Docker MCP Server

A comprehensive Model Context Protocol (MCP) server for Docker operations, providing complete Docker functionality through Claude AI and other MCP clients. Built with .NET 8 and designed for production use.

## Features

This Docker MCP Server provides **complete Docker functionality** that significantly surpasses all existing implementations:

### 🚀 **Container Operations**
- ✅ **Create containers** with full configuration support (35+ options)
- ✅ **Start, stop, restart containers** with timeout controls
- ✅ **Remove containers** with force and volume options
- ✅ **List containers** with advanced filtering and sizing
- ✅ **Get container logs** with timestamps, follow, and filtering
- ✅ **Inspect containers** for detailed information
- ✅ **Execute commands** in running containers
- ✅ **Get real-time statistics** for monitoring

### 🖼️ **Image Operations**
- ✅ **List images** with filtering options
- ✅ **Pull images** from registries with platform support
- ✅ **Remove images** with force options
- ✅ **Build images** from Dockerfiles with build args
- ✅ **Tag images** for repository management
- ✅ **Inspect images** for detailed information
- ✅ **Get image history** and layer information
- ✅ **Prune unused images** with dangling/all options
- ✅ **Search images** in Docker Hub
- ✅ **Push images** to registries

### 🌐 **Network Operations**
- ✅ **List networks** with complete information
- ✅ **Create networks** with custom drivers and IPAM
- ✅ **Remove networks** safely
- ✅ **Connect/disconnect containers** to/from networks
- ✅ **Inspect networks** for configuration details
- ✅ **Prune unused networks**
- ✅ **Subnet configuration** with gateway support

### 💾 **Volume Operations**
- ✅ **List volumes** with filtering
- ✅ **Create volumes** with custom drivers and options
- ✅ **Remove volumes** with force options
- ✅ **Inspect volumes** for detailed information
- ✅ **Prune unused volumes**
- ✅ **Volume filtering** by labels and metadata

### 🐳 **Docker Compose Operations**
- ✅ **Deploy Compose stacks** from YAML with environment injection
- ✅ **Remove Compose stacks** with cleanup options
- ✅ **List Compose stack containers**
- ✅ **Get Compose stack logs** (all services or specific)
- ✅ **Start/stop/restart stacks**
- ✅ **Scale services** in stacks
- ✅ **Execute commands** in Compose services

### 🛠️ **System Operations**
- ✅ **Check Docker availability** and connectivity
- ✅ **Get system information** and capabilities
- ✅ **Get version information** for daemon and API
- ✅ **System-wide pruning** with selective cleanup
- ✅ **Disk usage information** and monitoring
- ✅ **Event monitoring** with filtering
- ✅ **Process information** for containers

### 🎯 **Advanced Container Configuration**
- ✅ **Full volume mount support** (bind mounts, named volumes, tmpfs)
- ✅ **Environment variables** with validation
- ✅ **Port mappings** (multiple ports, TCP/UDP protocols)
- ✅ **Resource limits** (CPU, memory, PID limits)
- ✅ **Security options** (capabilities, AppArmor, SELinux, seccomp)
- ✅ **Device mappings** with permissions
- ✅ **DNS configuration** and custom resolvers
- ✅ **Extra hosts** for /etc/hosts entries
- ✅ **Tmpfs mounts** for temporary filesystems
- ✅ **Ulimits** (file descriptors, processes, memory locks)
- ✅ **Labels and metadata** management
- ✅ **Restart policies** (no, always, unless-stopped, on-failure)
- ✅ **Network modes** (bridge, host, none, container)
- ✅ **User and working directory** specification
- ✅ **Auto-removal support** (--rm equivalent)
- ✅ **Interactive and TTY modes**
- ✅ **Read-only filesystems** for security
- ✅ **Privileged containers** with safety controls

## Installation and Setup

### Prerequisites
- .NET 8.0 SDK or Runtime
- Docker Desktop or Docker Engine
- MCP-compatible client (Claude Desktop, Cursor, VS Code, etc.)

### Option 1: Use Pre-built Binaries (Recommended)

Download the latest release for your platform from [GitHub Releases](https://github.com/jamesburton/DockerMcpServer/releases):

**Windows:**
```bash
# Download and extract Windows x64 version
curl -L https://github.com/jamesburton/DockerMcpServer/releases/latest/download/DockerMcpServer-win-x64.zip -o DockerMcpServer.zip
# Extract and run
```

**Linux:**
```bash
# Download and extract Linux x64 version
curl -L https://github.com/jamesburton/DockerMcpServer/releases/latest/download/DockerMcpServer-linux-x64.tar.gz -o DockerMcpServer.tar.gz
tar -xzf DockerMcpServer.tar.gz
chmod +x DockerMcpServer
./DockerMcpServer
```

**macOS:**
```bash
# Download and extract macOS version (Intel or Apple Silicon)
curl -L https://github.com/jamesburton/DockerMcpServer/releases/latest/download/DockerMcpServer-osx-x64.tar.gz -o DockerMcpServer.tar.gz
tar -xzf DockerMcpServer.tar.gz
chmod +x DockerMcpServer
./DockerMcpServer
```

### Option 2: Use Docker Container

Run the Docker MCP Server in a container using GitHub Container Registry:

```bash
# Basic usage
docker run -d --name docker-mcp-server \
  -v /var/run/docker.sock:/var/run/docker.sock \
  ghcr.io/jamesburton/dockermcpserver:latest

# With custom configuration
docker run -d --name docker-mcp-server \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v $(pwd)/config:/app/config \
  ghcr.io/jamesburton/dockermcpserver:latest
```

### Option 3: Run from Source (Development)

1. **Clone the repository:**
```bash
git clone https://github.com/jamesburton/DockerMcpServer.git
cd DockerMcpServer
```

2. **Restore dependencies:**
```bash
dotnet restore
```

3. **Run the server:**
```bash
dotnet run --project DockerMcpServer/DockerMcpServer.csproj
```

## MCP Client Configuration

### Claude Desktop Configuration

Add to your Claude Desktop config file:
- **macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Windows:** `%APPDATA%/Claude/claude_desktop_config.json`

**Using pre-built executable:**
```json
{
  "mcpServers": {
    "docker": {
      "command": "/path/to/DockerMcpServer",
      "args": [],
      "env": {}
    }
  }
}
```

**Using Docker container:**
```json
{
  "mcpServers": {
    "docker": {
      "command": "docker",
      "args": ["run", "-i", "--rm", "-v", "/var/run/docker.sock:/var/run/docker.sock", "ghcr.io/jamesburton/dockermcpserver:latest"],
      "env": {}
    }
  }
}
```

**Using dotnet run (development):**
```json
{
  "mcpServers": {
    "docker": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/DockerMcpServer/DockerMcpServer"],
      "env": {}
    }
  }
}
```

### Cursor Configuration

Similar configuration in your Cursor MCP settings file.

## Usage Examples

### Creating a Container (Qdrant Example)
```
Create a Docker container with:
- Image: qdrant/qdrant:latest
- Name: qdrant
- Auto-remove: true
- Ports: ["6333:6333", "6334:6334"]
- Volumes: ["%cd%/qdrant/storage:/qdrant/storage", "%cd%/qdrant/config:/qdrant/config", "%cd%/qdrant/snapshots:/qdrant/snapshots"]
```

### Advanced Container with Security
```
Create a Docker container with:
- Image: nginx:latest
- Name: secure-nginx
- Memory limit: 512m
- CPU limit: 1.0
- Ports: ["8080:80"]
- User: "1000:1000"
- Read-only: true
- CapDrop: ["ALL"]
- CapAdd: ["NET_BIND_SERVICE"]
- SecurityOpt: ["no-new-privileges", "apparmor:docker-default"]
```

### Deploy a Compose Stack
```
Deploy a Docker Compose stack with:
- Project name: my-app
- Compose YAML: |
  version: '3.8'
  services:
    web:
      image: nginx:latest
      ports:
        - "8080:80"
    db:
      image: postgres:13
      environment:
        POSTGRES_PASSWORD: secret
      volumes:
        - db-data:/var/lib/postgresql/data
  volumes:
    db-data:
```

## Feature Comparison with Existing Solutions

| Feature | **Docker MCP Server** | [QuantGeekDev/docker-mcp](https://github.com/QuantGeekDev/docker-mcp) | [suvarchal/docker-mcp](https://github.com/suvarchal/docker-mcp) | [ckreiling/mcp-server-docker](https://github.com/ckreiling/mcp-server-docker) |
|---------|----------------------|-------------------------|---------------------|---------------------------|
| **Container Creation** | ✅ **35+ options** | ❌ **4 basic options** | ❌ **Basic only** | ❌ **Limited options** |
| **Volume Mounts** | ✅ **Full Support** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Basic only** |
| **Auto-removal (--rm)** | ✅ **Supported** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Not Supported** |
| **Multiple Port Mappings** | ✅ **TCP/UDP Support** | ✅ **Basic Support** | ✅ **Basic Support** | ✅ **Basic Support** |
| **Environment Variables** | ✅ **With Validation** | ✅ **Basic Support** | ✅ **Basic Support** | ✅ **Basic Support** |
| **Docker Compose** | ✅ **Complete Suite** | ✅ **Deploy/Remove Only** | ❌ **Not Supported** | ❌ **Not Supported** |
| **Network Management** | ✅ **Full CRUD + IPAM** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Not Supported** |
| **Volume Management** | ✅ **Full CRUD** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Not Supported** |
| **Image Management** | ✅ **Complete Suite** | ❌ **List Only** | ✅ **Basic CRUD** | ❌ **Pull Only** |
| **Resource Limits** | ✅ **CPU/Memory/PID** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Not Supported** |
| **Security Options** | ✅ **Capabilities/SELinux/AppArmor** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Explicitly Excluded** |
| **Device Mappings** | ✅ **Full Support** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Not Supported** |
| **Container Inspection** | ✅ **Complete Details** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Not Supported** |
| **Real-time Stats** | ✅ **CPU/Memory/Network** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Not Supported** |
| **System Information** | ✅ **Full System API** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Not Supported** |
| **Container Logs** | ✅ **Advanced Filtering** | ✅ **Basic Support** | ❌ **Not Supported** | ❌ **Not Supported** |
| **Container Exec** | ✅ **Interactive Support** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Not Supported** |
| **System Cleanup** | ✅ **Comprehensive Pruning** | ❌ **Not Supported** | ❌ **Not Supported** | ❌ **Not Supported** |
| **Platform** | 🟢 **.NET 8** | 🐍 **Python** | 🟨 **Node.js** | 🐍 **Python** |
| **Architecture** | 🏗️ **Modular/Enterprise** | 📝 **Simple Script** | 📝 **Basic CLI Wrapper** | 📝 **Simple Script** |

### Competitor Analysis

#### [QuantGeekDev/docker-mcp](https://github.com/QuantGeekDev/docker-mcp)
- **Scope**: Basic container and Compose operations
- **Limitations**: No volume management, network operations, security features, or system monitoring
- **Architecture**: Python-based simple implementation

#### [suvarchal/docker-mcp](https://github.com/suvarchal/docker-mcp)
- **Scope**: Basic container and image operations
- **Limitations**: No Compose support, no advanced features, CLI wrapper approach
- **Architecture**: Node.js with Docker CLI dependency

#### [ckreiling/mcp-server-docker](https://github.com/ckreiling/mcp-server-docker)
- **Scope**: Limited container operations with safety restrictions
- **Limitations**: Explicitly excludes security features, no Compose/network/volume support
- **Architecture**: Python with safety-first approach

## Architecture

### Core Components

- **`IDockerService`**: Main service interface for all Docker operations
- **`DockerService`**: Comprehensive implementation using Docker.DotNet API
- **`DockerClientFactory`**: Factory for creating and configuring Docker clients
- **Model Classes**: Strongly-typed request/response models with validation

### Modular Design (Partial Classes)

- **`DockerService.Container.cs`**: Container lifecycle management
- **`DockerService.Image.cs`**: Image operations and registry interactions
- **`DockerService.Volume.cs`**: Volume management and storage operations
- **`DockerService.Network.cs`**: Network creation and management
- **`DockerService.Compose.cs`**: Docker Compose stack operations
- **`DockerService.System.cs`**: System-level operations and monitoring
- **`DockerService.Security.cs`**: Security validation and enforcement
- **`DockerService.Helpers.cs`**: Utility methods and parsers

### MCP Command Classes

- **`DockerContainerCommands`**: 11 container management tools
- **`DockerImageCommands`**: 10 image operation tools
- **`DockerNetworkCommands`**: 8 network management tools
- **`DockerVolumeCommands`**: 5 volume operation tools
- **`DockerComposeCommands`**: 7 Compose stack tools
- **`DockerSystemCommands`**: 8 system maintenance tools

**Total: 49+ MCP tools** providing comprehensive Docker functionality

## Dependencies

- **Microsoft.Extensions.Hosting** (8.0.1): Application hosting framework
- **Microsoft.Extensions.Logging** (8.0.1): Structured logging infrastructure
- **ModelContextProtocol** (0.1.0-preview.11): MCP server implementation
- **Docker.DotNet** (3.125.15): Official Docker API client for .NET
- **YamlDotNet** (16.2.1): YAML parsing for Compose files

## Development and Debugging

### MCP Inspector

For development and debugging, use the MCP Inspector:

```bash
# For source-based development
npx @modelcontextprotocol/inspector dotnet run --project /path/to/DockerMcpServer/DockerMcpServer

# For built executable
npx @modelcontextprotocol/inspector /path/to/DockerMcpServer
```

### Logging

The server includes comprehensive logging with different levels:
- **Information**: Successful operations and state changes
- **Warning**: Non-critical issues and fallbacks
- **Error**: Operation failures with detailed context
- **Debug**: Detailed operation traces (development only)

Logs are output to stderr to avoid interfering with MCP communication.

## CI/CD Pipeline

### Automated Builds and Releases

This repository includes a comprehensive CI/CD pipeline that automatically:

- ✅ **Tests** code on every push and pull request
- ✅ **Builds** multi-platform binaries (Windows, Linux, macOS - x64, ARM64)
- ✅ **Creates** Docker images for linux/amd64 and linux/arm64
- ✅ **Publishes** to GitHub Container Registry (ghcr.io)
- ✅ **Generates** GitHub releases with downloadable assets

### Required Setup for Contributors

To enable Docker image publishing, repository maintainers need to configure:

1. **GHCR_TOKEN Secret**: Personal Access Token with `write:packages` scope
   - See [docs/CI-CD-SETUP.md](docs/CI-CD-SETUP.md) for detailed instructions

2. **Optional**: NUGET_API_KEY for NuGet package publishing

### Triggering Releases

```bash
# Create and push a new version tag
git tag v1.0.x
git push origin v1.0.x
```

This automatically triggers the full CI/CD pipeline and creates a GitHub release.

## Docker Images

Docker images are available on GitHub Container Registry:

- **Latest version**: `docker pull ghcr.io/jamesburton/dockermcpserver:latest`
- **Specific version**: `docker pull ghcr.io/jamesburton/dockermcpserver:v1.0.1`

### Image Features
- Multi-platform support (linux/amd64, linux/arm64)
- Non-root user for security
- Health checks included
- Minimal attack surface
- Docker CLI included for Docker-in-Docker scenarios

## Error Handling

The server includes production-grade error handling:
- **Docker daemon connectivity**: Automatic connection retry and clear error messages
- **Invalid configurations**: Parameter validation with helpful error descriptions
- **Resource conflicts**: Graceful handling of naming conflicts and resource locks
- **Permission errors**: Clear guidance on Docker daemon access requirements
- **Network timeouts**: Configurable timeouts with retry logic

All errors are returned as structured `CommandResult` objects with:
- Success/failure status
- Descriptive error messages
- Error context and troubleshooting hints
- Operation-specific data when available

## Security Considerations

### Container Security
- **Default non-privileged**: Containers run without elevated privileges by default
- **Capability management**: Precise control over Linux capabilities
- **Security contexts**: Support for AppArmor, SELinux, and seccomp profiles
- **Read-only filesystems**: Option to mount root filesystem as read-only
- **User specification**: Run containers as non-root users

### Input Validation
- **Parameter sanitization**: All inputs validated before Docker API calls
- **Resource limit parsing**: Memory and CPU limits validated and converted safely
- **Path validation**: Volume and device paths checked for security
- **Command injection prevention**: Command arguments properly escaped

### Network Security
- **Network isolation**: Support for custom networks and isolation
- **Port binding validation**: Port mappings validated for conflicts
- **DNS security**: Custom DNS configurations with validation

## Performance Considerations

- **Async/await patterns**: All operations are fully asynchronous
- **Resource disposal**: Proper cleanup of Docker client connections
- **Stream handling**: Efficient handling of logs and stats streams
- **Memory management**: Minimal memory footprint with proper garbage collection
- **Connection pooling**: Reuse of Docker client connections

## Troubleshooting

### Common Issues

1. **Docker daemon not running**
   - Ensure Docker Desktop or Docker Engine is started
   - Check Docker socket permissions on Linux/macOS

2. **Permission denied errors**
   - Add user to `docker` group on Linux
   - Ensure Docker Desktop has proper permissions on Windows/macOS

3. **MCP server not starting**
   - Verify .NET 8 runtime is installed
   - Check file paths in MCP configuration
   - Review Claude Desktop/client logs

4. **Container creation failures**
   - Validate image names and tags
   - Check available system resources
   - Verify volume mount paths exist

### Debug Commands

Test Docker connectivity:
```bash
# Test Docker daemon
docker info

# Test .NET installation
dotnet --version

# Test MCP server manually (source)
dotnet run --project /path/to/DockerMcpServer/DockerMcpServer

# Test MCP server manually (executable)
./DockerMcpServer
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed contribution guidelines.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Implement changes with tests
4. Ensure all existing tests pass
5. Update documentation as needed
6. Submit a pull request

### Development Guidelines

- Follow C# coding conventions and nullable reference types
- Add XML documentation for all public APIs
- Include unit tests for new functionality
- Update integration tests for API changes
- Maintain backwards compatibility where possible

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support and Community

- **Issues**: Report bugs and request features via GitHub Issues
- **Discussions**: Join community discussions for questions and ideas
- **Documentation**: Comprehensive API documentation available in XML comments
- **Releases**: Pre-built binaries available for all platforms

## Acknowledgments

This implementation leverages the excellent [Docker.DotNet](https://github.com/dotnet/Docker.DotNet) library and follows the [Model Context Protocol](https://modelcontextprotocol.io/) specification.

## Why Choose This Docker MCP Server?

### ✅ **Complete Coverage**
The only Docker MCP server that supports the full Docker ecosystem - containers, images, volumes, networks, Compose, and system operations.

### ✅ **Production Ready**
Enterprise-grade error handling, logging, security, and performance optimizations make it suitable for production environments.

### ✅ **Easy Installation**
Pre-built binaries for all platforms, Docker images, and multiple installation methods.

### ✅ **Developer Friendly**
Comprehensive documentation, intuitive APIs, and excellent debugging support accelerate development workflows.

### ✅ **Security First**
Built-in security features including capability management, security contexts, and input validation protect your infrastructure.

### ✅ **Modern Architecture**
Clean, modular .NET 8 implementation with async/await patterns, dependency injection, and comprehensive testing.

### ✅ **Extensible**
Well-architected codebase makes it easy to add new features and customize for specific requirements.

**Perfect for developers who need complete Docker control through Claude AI and other MCP clients!**

---

*Built with ❤️ using .NET 8, Docker.DotNet, and the Model Context Protocol*