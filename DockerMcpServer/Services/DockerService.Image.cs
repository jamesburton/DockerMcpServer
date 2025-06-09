using Docker.DotNet.Models;
using DockerMcpServer.Models;
using Microsoft.Extensions.Logging;

namespace DockerMcpServer.Services;

/// <summary>
/// Image operations for DockerService
/// </summary>
public partial class DockerService
{
    #region Image Operations

    /// <summary>Lists Docker images with optional filtering by labels, dangling status, and reference patterns.</summary>
    public async Task<CommandResult> ListImagesAsync(ImageListOptions? options = null)
    {
        EnsureDockerClient();

        try
        {
            options ??= new ImageListOptions();

            var listParameters = new ImagesListParameters
            {
                All = options.All
            };

            if (options.Labels?.Count > 0)
            {
                listParameters.Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["label"] = options.Labels.ToDictionary(kvp => $"{kvp.Key}={kvp.Value}", _ => true)
                };
            }

            if (options.Dangling)
            {
                listParameters.Filters ??= new Dictionary<string, IDictionary<string, bool>>();
                listParameters.Filters["dangling"] = new Dictionary<string, bool> { ["true"] = true };
            }

            if (!string.IsNullOrEmpty(options.Reference))
            {
                listParameters.Filters ??= new Dictionary<string, IDictionary<string, bool>>();
                listParameters.Filters["reference"] = new Dictionary<string, bool> { [options.Reference] = true };
            }

            var images = await _dockerClient!.Images.ListImagesAsync(listParameters);

            var imageInfos = images.Select(i => new ImageInfo
            {
                Id = i.ID,
                RepoTags = i.RepoTags?.ToList() ?? [],
                RepoDigests = i.RepoDigests?.ToList() ?? [],
                Size = i.Size,
                VirtualSize = i.VirtualSize,
                Created = i.Created,
                Labels = i.Labels?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? []
            }).ToList();

            return new CommandResult
            {
                Success = true,
                Message = $"Found {imageInfos.Count} images",
                Data = imageInfos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list images");
            return new CommandResult
            {
                Success = false,
                Message = "Failed to list images",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Pulls a Docker image from a registry with optional tag and platform specification.</summary>
    public async Task<CommandResult> PullImageAsync(string imageName, string? tag = null, string? platform = null)
    {
        EnsureDockerClient();

        try
        {
            var imageNameWithTag = string.IsNullOrEmpty(tag) ? imageName : $"{imageName}:{tag}";

            var createParameters = new ImagesCreateParameters
            {
                FromImage = imageName,
                Tag = tag,
                Platform = platform
            };

            await _dockerClient!.Images.CreateImageAsync(createParameters, null, new Progress<JSONMessage>());

            return new CommandResult
            {
                Success = true,
                Message = $"Image {imageNameWithTag} pulled successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pull image {Image}:{Tag}", imageName, tag);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to pull image {imageName}:{tag}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Builds a Docker image from a Dockerfile with optional tag, build context, and build arguments.</summary>
    public async Task<CommandResult> BuildImageAsync(string dockerfilePath, string? tag = null, string? buildContext = null, Dictionary<string, string>? buildArgs = null)
    {
        EnsureDockerClient();

        try
        {
            var tarStream = CreateTarStreamForBuild(dockerfilePath, buildContext);

            var buildParameters = new ImageBuildParameters
            {
                Tags = string.IsNullOrEmpty(tag) ? [] : [tag],
                Dockerfile = Path.GetFileName(dockerfilePath),
                BuildArgs = buildArgs
            };

            await _dockerClient!.Images.BuildImageFromDockerfileAsync(buildParameters, tarStream, null, null, new Progress<JSONMessage>(), CancellationToken.None);

            return new CommandResult
            {
                Success = true,
                Message = $"Image built successfully{(string.IsNullOrEmpty(tag) ? "" : $" with tag {tag}")}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build image from {Dockerfile}", dockerfilePath);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to build image from {dockerfilePath}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Removes a Docker image with options for force removal and pruning control.</summary>
    public async Task<CommandResult> RemoveImageAsync(string imageIdOrName, bool force = false, bool noPrune = false)
    {
        EnsureDockerClient();

        try
        {
            var removeParameters = new ImageDeleteParameters
            {
                Force = force,
                NoPrune = noPrune
            };

            var response = await _dockerClient!.Images.DeleteImageAsync(imageIdOrName, removeParameters);

            return new CommandResult
            {
                Success = true,
                Message = $"Image {imageIdOrName} removed successfully",
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove image {Image}", imageIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to remove image {imageIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Inspects a Docker image and returns detailed metadata and configuration information.</summary>
    public async Task<CommandResult> InspectImageAsync(string imageIdOrName)
    {
        EnsureDockerClient();

        try
        {
            var image = await _dockerClient!.Images.InspectImageAsync(imageIdOrName);

            return new CommandResult
            {
                Success = true,
                Message = $"Image {imageIdOrName} inspected successfully",
                Data = image
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inspect image {Image}", imageIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to inspect image {imageIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Creates a new tag for an existing Docker image.</summary>
    public async Task<CommandResult> TagImageAsync(string sourceImage, string targetImage)
    {
        EnsureDockerClient();

        try
        {
            var tagParts = targetImage.Split(':');
            var repo = tagParts[0];
            var tag = tagParts.Length > 1 ? tagParts[1] : "latest";

            var tagParameters = new ImageTagParameters
            {
                RepositoryName = repo,
                Tag = tag
            };

            await _dockerClient!.Images.TagImageAsync(sourceImage, tagParameters);

            return new CommandResult
            {
                Success = true,
                Message = $"Image {sourceImage} tagged as {targetImage}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to tag image {Source} as {Target}", sourceImage, targetImage);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to tag image {sourceImage} as {targetImage}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Pushes a Docker image to a registry with optional authentication.</summary>
    public async Task<CommandResult> PushImageAsync(string imageName, string? tag = null, string? authHeader = null)
    {
        EnsureDockerClient();

        try
        {
            var imageNameWithTag = string.IsNullOrEmpty(tag) ? imageName : $"{imageName}:{tag}";

            var pushParameters = new ImagePushParameters
            {
                ImageID = imageName,
                Tag = tag
            };

            AuthConfig? authConfig = null;
            if (!string.IsNullOrEmpty(authHeader))
            {
                // Parse auth header if provided
                authConfig = new AuthConfig(); // You would parse the auth header here
            }

            await _dockerClient!.Images.PushImageAsync(imageName, pushParameters, authConfig, new Progress<JSONMessage>());

            return new CommandResult
            {
                Success = true,
                Message = $"Image {imageNameWithTag} pushed successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to push image {Image}:{Tag}", imageName, tag);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to push image {imageName}:{tag}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Searches for Docker images in registries matching the specified term.</summary>
    public async Task<CommandResult> SearchImagesAsync(string term, int limit = 25)
    {
        EnsureDockerClient();

        try
        {
            var searchParameters = new ImagesSearchParameters
            {
                Term = term,
                Limit = limit
            };

            var results = await _dockerClient!.Images.SearchImagesAsync(searchParameters);

            return new CommandResult
            {
                Success = true,
                Message = $"Found {results.Count} images matching '{term}'",
                Data = results
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search images for term {Term}", term);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to search images for term {term}",
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>Gets the build history of a Docker image showing layer information.</summary>
    public async Task<CommandResult> GetImageHistoryAsync(string imageIdOrName)
    {
        EnsureDockerClient();

        try
        {
            var history = await _dockerClient!.Images.GetImageHistoryAsync(imageIdOrName);

            return new CommandResult
            {
                Success = true,
                Message = $"Retrieved history for image {imageIdOrName}",
                Data = history
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get history for image {Image}", imageIdOrName);
            return new CommandResult
            {
                Success = false,
                Message = $"Failed to get history for image {imageIdOrName}",
                ErrorMessage = ex.Message
            };
        }
    }

    #endregion
}
