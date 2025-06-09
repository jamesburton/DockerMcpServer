# Contributing to Docker MCP Server

We welcome contributions to the Docker MCP Server! This document provides guidelines for contributing to the project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [How to Contribute](#how-to-contribute)
- [Development Guidelines](#development-guidelines)
- [Pull Request Process](#pull-request-process)
- [Issue Reporting](#issue-reporting)

## Code of Conduct

This project and everyone participating in it is governed by our Code of Conduct. By participating, you are expected to uphold this code. Please report unacceptable behavior to the project maintainers.

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Docker Desktop or Docker Engine
- Git
- Your favorite IDE (Visual Studio, VS Code, Rider, etc.)

### Setting up the Development Environment

1. **Fork the repository**
   ```bash
   git clone https://github.com/yourusername/DockerMcpServer.git
   cd DockerMcpServer
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

5. **Run the MCP server**
   ```bash
   dotnet run --project DockerMcpServer
   ```

## How to Contribute

### Types of Contributions

We welcome several types of contributions:

- **Bug fixes**: Fix issues in the existing codebase
- **Feature enhancements**: Add new Docker operations or improve existing ones
- **Documentation improvements**: Update README, add examples, improve code comments
- **Performance optimizations**: Improve speed, memory usage, or resource efficiency
- **Security enhancements**: Strengthen security features and validation
- **Test coverage**: Add unit tests, integration tests, or test improvements

### Before You Start

1. **Check existing issues**: Look for existing issues or feature requests
2. **Create an issue**: If none exists, create one to discuss your proposed changes
3. **Get feedback**: Wait for maintainer feedback before starting significant work

## Development Guidelines

### Code Style

- Follow C# coding conventions and best practices
- Use nullable reference types consistently
- Add XML documentation for all public APIs
- Write clear, descriptive variable and method names
- Keep methods focused and reasonably sized

### Architecture Principles

- **Modular design**: Keep related functionality in dedicated partial classes
- **Separation of concerns**: Maintain clear boundaries between layers
- **Async/await**: Use asynchronous patterns for all I/O operations
- **Error handling**: Implement comprehensive error handling with proper logging
- **Resource management**: Ensure proper disposal of resources

### Testing

- **Unit tests**: Add unit tests for new functionality
- **Integration tests**: Include integration tests for Docker operations
- **Test coverage**: Aim for high test coverage on new code
- **Test naming**: Use descriptive test method names that explain the scenario

### Documentation

- **XML documentation**: Add comprehensive XML documentation for public APIs
- **Code comments**: Add comments for complex logic or business rules
- **README updates**: Update documentation for new features
- **Examples**: Provide usage examples for new functionality

## Pull Request Process

### Before Submitting

1. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes**
   - Follow the development guidelines
   - Add appropriate tests
   - Update documentation

3. **Test your changes**
   ```bash
   dotnet test
   dotnet build --configuration Release
   ```

4. **Commit your changes**
   ```bash
   git commit -m "feat(scope): add new feature description"
   ```
   Use [Conventional Commits](https://www.conventionalcommits.org/) format.

### Submitting the Pull Request

1. **Push your branch**
   ```bash
   git push origin feature/your-feature-name
   ```

2. **Create a pull request**
   - Use a clear and descriptive title
   - Provide a detailed description of changes
   - Reference any related issues
   - Include screenshots or examples if applicable

3. **Respond to feedback**
   - Address reviewer comments promptly
   - Make requested changes
   - Keep the conversation constructive

### Pull Request Requirements

- [ ] All tests pass
- [ ] Code follows project standards
- [ ] Documentation is updated
- [ ] Backwards compatibility is maintained (unless breaking change is justified)
- [ ] Performance impact is considered
- [ ] Security implications are addressed

## Issue Reporting

### Bug Reports

When reporting bugs, please include:

- **Environment details**: OS, .NET version, Docker version
- **Steps to reproduce**: Clear, step-by-step instructions
- **Expected behavior**: What should happen
- **Actual behavior**: What actually happens
- **Error messages**: Full error messages and stack traces
- **Configuration**: Relevant MCP configuration

### Feature Requests

When requesting features, please include:

- **Use case**: Why this feature is needed
- **Proposed solution**: How you envision it working
- **Alternatives considered**: Other approaches you've considered
- **Additional context**: Any other relevant information

### Security Issues

For security issues:
- **Do not** create public issues
- **Email** security concerns to the maintainers privately
- **Include** full details and potential impact

## Development Workflow

### Branch Strategy

- **main**: Stable release branch
- **develop**: Integration branch for new features
- **feature/***: Feature development branches
- **hotfix/***: Critical bug fix branches

### Commit Messages

Use [Conventional Commits](https://www.conventionalcommits.org/) format:

- `feat(scope): add new feature`
- `fix(scope): fix bug description`
- `docs: update documentation`
- `style: formatting changes`
- `refactor: code refactoring`
- `test: add or update tests`
- `chore: maintenance tasks`

### Release Process

1. Features are merged to `develop`
2. Release candidates are created from `develop`
3. After testing, releases are merged to `main`
4. Tags are created for releases
5. GitHub releases are published with binaries

## Recognition

Contributors will be recognized in:
- Project README
- Release notes
- Git commit history

## Getting Help

If you need help:

- **Documentation**: Check the README and code documentation
- **Issues**: Search existing issues for similar problems
- **Discussions**: Use GitHub Discussions for questions
- **Contact**: Reach out to maintainers for guidance

## License

By contributing to Docker MCP Server, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to Docker MCP Server! Your efforts help make this the most comprehensive Docker MCP server available.
