# CI/CD Setup Guide

This document explains how to configure the required secrets for the CI/CD pipeline.

## Required Secrets

### GHCR_TOKEN (Required for Docker Image Publishing)

The CI/CD pipeline uses GitHub Container Registry (ghcr.io) to publish Docker images. You need to create a Personal Access Token (PAT) with appropriate permissions.

#### Creating the GHCR_TOKEN

1. **Navigate to GitHub Settings**
   - Go to GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
   - Or visit: https://github.com/settings/tokens

2. **Create New Token**
   - Click "Generate new token (classic)"
   - Give it a descriptive name: `DockerMcpServer GHCR Token`
   - Set expiration as needed (recommended: 90 days or 1 year)

3. **Select Required Scopes**
   For GitHub Container Registry, you need:
   - ✅ `write:packages` - Upload packages to GitHub Package Registry
   - ✅ `read:packages` - Download packages from GitHub Package Registry
   - ✅ `delete:packages` - Delete packages from GitHub Package Registry (optional)

4. **Copy the Token**
   - Click "Generate token"
   - **Important**: Copy the token immediately - you won't be able to see it again

#### Adding the Secret to Repository

1. **Navigate to Repository Settings**
   - Go to your repository → Settings → Secrets and variables → Actions

2. **Add Repository Secret**
   - Click "New repository secret"
   - Name: `GHCR_TOKEN`
   - Value: Paste the Personal Access Token you created
   - Click "Add secret"

## Optional Secrets

### NUGET_API_KEY (Optional - for NuGet Package Publishing)

If you want to publish NuGet packages automatically:

1. **Get NuGet API Key**
   - Go to https://www.nuget.org/account/apikeys
   - Create a new API key with push permissions
   - Scope it to your package if desired

2. **Add to Repository**
   - Repository Settings → Secrets and variables → Actions
   - Name: `NUGET_API_KEY`
   - Value: Your NuGet API key

3. **Enable NuGet Publishing**
   - Uncomment the NuGet publishing section in `.github/workflows/ci-cd.yml`

## Verification

After adding the `GHCR_TOKEN` secret:

1. **Trigger the Pipeline**
   - Push a new tag: `git tag v1.0.3 && git push origin v1.0.3`
   - Or push to main branch

2. **Check GitHub Actions**
   - Go to Actions tab in your repository
   - Monitor the workflow execution
   - Verify Docker images are published to `ghcr.io/yourusername/dockermcpserver`

3. **Test Docker Image**
   ```bash
   docker pull ghcr.io/yourusername/dockermcpserver:latest
   docker run --rm ghcr.io/yourusername/dockermcpserver:latest --help
   ```

## Troubleshooting

### Docker Login Failed
- **Error**: `Error: failed to login to ghcr.io`
- **Solution**: Verify `GHCR_TOKEN` secret is correctly set with `write:packages` scope

### Token Permissions
- **Error**: `insufficient_scope: The request requires higher privileges than provided`
- **Solution**: Ensure token has `write:packages` and `read:packages` scopes

### Package Visibility
- **Issue**: Docker images not visible
- **Solution**: Check package visibility settings in GitHub → Packages
- **Note**: Packages inherit repository visibility by default

## Security Best Practices

1. **Token Scope**: Use minimal required scopes (`write:packages`, `read:packages`)
2. **Token Expiration**: Set reasonable expiration dates and rotate tokens
3. **Repository Secrets**: Use repository secrets, not organization secrets unless needed
4. **Environment Protection**: Consider using environment protection rules for production

## Alternative: Using GITHUB_TOKEN

If you prefer not to use a custom PAT, you can modify the workflow to use the default `GITHUB_TOKEN`, but note these limitations:

- May have reduced permissions in some scenarios
- Cannot be used for cross-repository operations
- Some package operations might be restricted

To use `GITHUB_TOKEN`, change in `.github/workflows/ci-cd.yml`:
```yaml
password: ${{ secrets.GITHUB_TOKEN }}
```

However, using a dedicated `GHCR_TOKEN` is recommended for better control and reliability.
