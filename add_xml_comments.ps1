# PowerShell script to add XML comments to remaining methods

$files = @(
    "C:\Development\MCP\DockerMcpServer\DockerMcpServer\Services\DockerContainerCommands.cs",
    "C:\Development\MCP\DockerMcpServer\DockerMcpServer\Services\DockerImageCommands.cs",
    "C:\Development\MCP\DockerMcpServer\DockerMcpServer\Services\DockerNetworkCommands.cs",
    "C:\Development\MCP\DockerMcpServer\DockerMcpServer\Services\DockerSystemCommands.cs",
    "C:\Development\MCP\DockerMcpServer\DockerMcpServer\Services\DockerVolumeCommands.cs"
)

# Dictionary of method patterns and their XML comments
$methodComments = @{
    "CreateContainerAsync" = "/// <summary>Creates and optionally starts a new Docker container with comprehensive configuration options.</summary>"
    "ListContainersAsync" = "/// <summary>Lists Docker containers with filtering options.</summary>"
    "StartContainerAsync" = "/// <summary>Starts a Docker container.</summary>"
    "StopContainerAsync" = "/// <summary>Stops a Docker container.</summary>"
    "RestartContainerAsync" = "/// <summary>Restarts a Docker container.</summary>"
    "RemoveContainerAsync" = "/// <summary>Removes a Docker container.</summary>"
    "GetContainerLogsAsync" = "/// <summary>Gets logs from a Docker container.</summary>"
    "InspectContainerAsync" = "/// <summary>Inspects a Docker container and returns detailed information.</summary>"
    "GetContainerStatsAsync" = "/// <summary>Gets real-time statistics for a Docker container.</summary>"
    "ExecContainerAsync" = "/// <summary>Executes a command in a running Docker container.</summary>"
    
    "ListImagesAsync" = "/// <summary>Lists Docker images with filtering options.</summary>"
    "PullImageAsync" = "/// <summary>Pulls a Docker image from a registry.</summary>"
    "RemoveImageAsync" = "/// <summary>Removes a Docker image.</summary>"
    "BuildImageAsync" = "/// <summary>Builds a Docker image from a Dockerfile.</summary>"
    "TagImageAsync" = "/// <summary>Tags a Docker image.</summary>"
    "InspectImageAsync" = "/// <summary>Inspects a Docker image and returns detailed information.</summary>"
    "GetImageHistoryAsync" = "/// <summary>Gets the history of a Docker image.</summary>"
    "PruneImagesAsync" = "/// <summary>Removes unused Docker images.</summary>"
    "SearchImagesAsync" = "/// <summary>Searches for images in Docker registry.</summary>"
    "PushImageAsync" = "/// <summary>Pushes an image to registry.</summary>"
    
    "ListNetworksAsync" = "/// <summary>Lists Docker networks.</summary>"
    "CreateNetworkAsync" = "/// <summary>Creates a Docker network.</summary>"
    "RemoveNetworkAsync" = "/// <summary>Removes a Docker network.</summary>"
    "ConnectNetworkAsync" = "/// <summary>Connects a container to a network.</summary>"
    "DisconnectNetworkAsync" = "/// <summary>Disconnects a container from a network.</summary>"
    "InspectNetworkAsync" = "/// <summary>Inspects a Docker network and returns detailed information.</summary>"
    "PruneNetworksAsync" = "/// <summary>Removes unused Docker networks.</summary>"
    
    "TestConnectionAsync" = "/// <summary>Tests Docker connection.</summary>"
    "GetDockerInfoAsync" = "/// <summary>Gets Docker system information.</summary>"
    "GetDockerVersionAsync" = "/// <summary>Gets Docker version information.</summary>"
    "PruneContainersAsync" = "/// <summary>Removes unused Docker containers.</summary>"
    "PruneNetworksAsync" = "/// <summary>Removes unused Docker networks.</summary>"
    "PruneVolumesAsync" = "/// <summary>Removes unused Docker volumes.</summary>"
    "PruneSystemAsync" = "/// <summary>Removes all unused Docker objects (containers, images, networks, volumes).</summary>"
    "GetDiskUsageAsync" = "/// <summary>Gets Docker disk usage information.</summary>"
    "PingDockerAsync" = "/// <summary>Pings Docker daemon.</summary>"
    "GetDockerProcessesAsync" = "/// <summary>Gets Docker processes information.</summary>"
    
    "ListVolumesAsync" = "/// <summary>Lists Docker volumes.</summary>"
    "CreateVolumeAsync" = "/// <summary>Creates a Docker volume.</summary>"
    "RemoveVolumeAsync" = "/// <summary>Removes a Docker volume.</summary>"
    "InspectVolumeAsync" = "/// <summary>Inspects a Docker volume and returns detailed information.</summary>"
}

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        
        foreach ($method in $methodComments.Keys) {
            $pattern = "\[McpServerTool, Description.*?\]\s*public async Task<string> $method"
            $replacement = "$($methodComments[$method])`n    [McpServerTool, Description"
            
            $content = $content -replace $pattern, $replacement
        }
        
        Set-Content $file $content -NoNewline
        Write-Host "Updated $file"
    }
}

Write-Host "XML comments added to all remaining methods!"
