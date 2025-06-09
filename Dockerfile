# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["DockerMcpServer/DockerMcpServer.csproj", "DockerMcpServer/"]
COPY ["DockerMcpServer.sln", "./"]

# Restore dependencies
RUN dotnet restore "DockerMcpServer/DockerMcpServer.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/DockerMcpServer"
RUN dotnet build "DockerMcpServer.csproj" -c Release -o /app/build
RUN dotnet publish "DockerMcpServer.csproj" -c Release -o /app/publish \
    --no-restore --self-contained false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install Docker CLI for Docker-in-Docker scenarios
RUN apt-get update && apt-get install -y \
    curl \
    ca-certificates \
    gnupg \
    lsb-release \
    && curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/debian $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null \
    && apt-get update \
    && apt-get install -y docker-ce-cli docker-compose-plugin \
    && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=build /app/publish .

# Create non-root user
RUN groupadd -r mcpuser && useradd -r -g mcpuser mcpuser
RUN chown -R mcpuser:mcpuser /app
USER mcpuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Expose port (if needed for HTTP transport)
EXPOSE 8080

# Set entrypoint
ENTRYPOINT ["dotnet", "DockerMcpServer.dll"]
