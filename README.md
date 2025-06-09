# Docker MCP Server

A comprehensive Model Context Protocol (MCP) server for Docker operations, providing full Docker functionality through Claude AI and other MCP clients.

## Features

This Docker MCP Server provides **complete Docker functionality** that surpasses all existing implementations:

### 🚀 **Container Operations**
- ✅ **Create containers** with full configuration support
- ✅ **Start, stop, restart containers**
- ✅ **Remove containers** with force and volume options
- ✅ **List containers** with advanced filtering
- ✅ **Get container logs** with timestamps, follow, and filtering
- ✅ **Inspect containers** for detailed information
- ✅ **Execute commands** in running containers
- ✅ **Get real-time statistics**

### 🖼️ **Image Operations**
- ✅ **List images** with filtering options
- ✅ **Pull images** from registries
- ✅ **Remove images** with force options
- ✅ **Build images** from Dockerfiles
- ✅ **Tag images**
- ✅ **Inspect images** for detailed information
- ✅ **Get image history**
- ✅ **Prune unused images**

### 🌐 **Network Operations**
- ✅ **List networks**
- ✅ **Create networks** with custom drivers
- ✅ **Remove networks**
- ✅ **Connect/disconnect containers** to/from networks
- ✅ **Inspect networks**
- ✅ **Prune unused networks**

### 💾 **Volume Operations**
- ✅ **List volumes**
- ✅ **Create volumes** with custom drivers and options
- ✅ **Remove volumes**
- ✅ **Inspect volumes**
- ✅ **Prune unused volumes**

### 🐳 **Docker Compose Operations**
- ✅ **Deploy Compose stacks** from YAML
- ✅ **Remove Compose stacks**
- ✅ **List Compose stack containers**
- ✅ **Get Compose stack logs**

### 🛠️ **System Operations**
- ✅ **Check Docker availability**
- ✅ **Get system information**
- ✅ **Get version information**
- ✅ **System-wide pruning**
- ✅ **Disk usage information**

### 🎯 **Advanced Container Configuration**
- ✅ **Full volume mount support** (bind mounts, volumes)
- ✅ **Environment variables**
- ✅ **Port mappings** (multiple ports, protocols)
- ✅ **Resource limits** (CPU, memory)
- ✅ **Security options** (capabilities, security-opt)
- ✅ **Device mappings**
- ✅ **DNS configuration**
- ✅ **Extra hosts**
- ✅ **Tmpfs mounts**
- ✅ **Ulimits**
- ✅ **Labels and metadata**
- ✅ **Restart policies**
- ✅ **Network modes**
- ✅ **User and working directory**
- ✅ **Auto-removal support**
- ✅ **Interactive and TTY modes**
- ✅ **Read-only filesystems**
- ✅ **Privileged containers**

## Your Qdrant Command Support

**✅ FULLY SUPPORTED**: Your complex Qdrant command is now fully supported:

```bash
docker run --name qdrant --rm -p 6333:6333 -p 6334:6334 \
  -v %cd%/qdrant/storage:/qdrant/storage \
  -v %cd%/qdrant/config:/qdrant/config \
  -v %cd%/qdrant/snapshots:/qdrant/snapshots \
  qdrant/qdrant:latest
```

**MCP Command Equivalent:**
```json
{
  "image": "qdrant/qdrant:latest",
  "name": "qdrant",
  "autoRemove": true,
  "ports": ["6333:6333", "6334:6334"],
  "volumes": [
    "%cd%/qdrant/storage:/qdrant/storage",
    "%cd%/qdrant/config:/qdrant/config", 
    "%cd%/qdrant/snapshots:/qdrant/snapshots"
  ]
}
```

## Comparison with Existing Solutions

| Feature | **Docker MCP Server** | QuantGeekDev/docker-mcp | suvarchal/docker-mcp |
|---------|----------------------|-------------------------|---------------------|
| **Volume Mounts** | ✅ Full Support | ❌ Not Supported | ✅ Basic Support |
| **Auto-removal (--rm)** | ✅ Supported | ❌ Not Supported | ❌ Not Supported |
| **Multiple Port Mappings** | ✅ Supported | ✅ Supported | ✅ Supported |
| **Environment Variables** | ✅ Supported | ✅ Supported | ✅ Supported |
| **Docker Compose** | ✅ Full Support | ✅ Basic Support | ❌ Not Supported |
| **Network Management** | ✅ Full Support | ❌ Not Supported | ❌ Not Supported |
| **Volume Management** | ✅ Full Support | ❌ Not Supported | ❌ Not Supported |
| **Image Management** | ✅ Full Support | ❌ Limited | ✅ Basic Support |
| **Resource Limits** | ✅ Supported | ❌ Not Supported | ❌ Not Supported |
| **Security Options** | ✅ Supported | ❌ Not Supported | ❌ Not Supported |
| **Device Mappings** | ✅ Supported | ❌ Not Supported | ❌ Not Supported |
| **Capabilities** | ✅ Supported | ❌ Not Supported | ❌ Not Supported |
| **Container Inspection** | ✅ Supported | ❌ Not Supported | ❌ Not Supported |
| **Real-time Stats** | ✅ Supported | ❌ Not Supported | ❌ Not Supported |
| **System Information** | ✅ Supported | ❌ Not Supported | ❌ Not Supported |

## Installation

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop or Docker Engine
- ModelContextProtocol library

### Build and Run

1. **Clone or create the project:**
```bash
git clone <repository-url>
cd DockerMcpServer
```

2. **Restore dependencies:**
```bash
dotnet restore
```

3. **Build the project:**
```bash
dotnet build
```

4. **Run the MCP server:**
```bash
dotnet run
```

### Claude Desktop Configuration

Add this to your Claude Desktop config file:

**macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`
**Windows:** `%APPDATA%/Claude/claude_desktop_config.json`

```json
{
  "mcpServers": {
    "docker": {
      "command": "dotnet",
      "args": ["run", "--project", "C:\\Development\\MCP\\DockerMcpServer\\DockerMcpServer"],
      "env": {}
    }
  }
}
```

## Usage Examples

### Creating a Container (Your Qdrant Example)
```
Create a Docker container with:
- Image: qdrant/qdrant:latest
- Name: qdrant
- Auto-remove: true
- Ports: ["6333:6333", "6334:6334"]
- Volumes: ["%cd%/qdrant/storage:/qdrant/storage", "%cd%/qdrant/config:/qdrant/config", "%cd%/qdrant/snapshots:/qdrant/snapshots"]
```

### Advanced Container with Resource Limits
```
Create a Docker container with:
- Image: nginx:latest
- Name: my-nginx
- Memory limit: 512m
- CPU limit: 1.5
- Ports: ["8080:80"]
- Environment: ["ENV=production", "DEBUG=false"]
- Read-only: true
- Restart policy: always
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
```

### List All Resources
```
- List all containers (running and stopped)
- List all images
- List all networks
- List all volumes
```

## Architecture

### Core Components

- **`IDockerService`**: Main service interface for Docker operations
- **`DockerService`**: Implementation using Docker.DotNet API
- **`DockerClientFactory`**: Factory for creating Docker clients
- **Model Classes**: Comprehensive request/response models

### MCP Command Classes

- **`DockerContainerCommands`**: Container lifecycle management
- **`DockerImageCommands`**: Image operations
- **`DockerNetworkCommands`**: Network management
- **`DockerVolumeCommands`**: Volume management
- **`DockerComposeCommands`**: Compose stack operations
- **`DockerSystemCommands`**: System-level operations

## Dependencies

- **Microsoft.Extensions.Hosting**: Application hosting
- **Microsoft.Extensions.Logging**: Logging infrastructure
- **ModelContextProtocol**: MCP server implementation
- **Docker.DotNet**: Docker API client
- **System.Text.Json**: JSON serialization
- **YamlDotNet**: YAML parsing for Compose files

## Error Handling

The server includes comprehensive error handling:
- Docker daemon connectivity issues
- Invalid container configurations
- Resource conflicts
- Permission errors
- Network timeouts

All errors are returned as structured JSON responses with detailed error messages.

## Security Considerations

- Containers run with the permissions of the Docker daemon
- Privileged containers require explicit permission
- Volume mounts are validated for security
- Network access is controlled through Docker's security model

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

This MCP server provides the most comprehensive Docker functionality available, supporting:
- ✅ All Docker run command options
- ✅ Complete container lifecycle management
- ✅ Full Docker Compose support
- ✅ Advanced networking and storage
- ✅ System monitoring and maintenance
- ✅ Production-ready security features

Perfect for developers who need complete Docker control through Claude AI!
