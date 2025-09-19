# Contributing to Fellowship SDK

Thank you for your interest in contributing to the Fellowship SDK! This document provides guidelines and information for contributors.

## Development Setup

### Prerequisites
- .NET 8.0 SDK
- Git
- Visual Studio 2022, VS Code, or JetBrains Rider

### Getting Started
1. Fork the repository
2. Clone your fork: `git clone https://github.com/YOUR_USERNAME/Fellowship.SDK.git`
3. Navigate to the project: `cd Fellowship.SDK`
4. Restore dependencies: `dotnet restore`
5. Build the project: `dotnet build`
6. Run tests: `dotnet test`

## Development Workflow

### Branch Strategy
- `main`: Production-ready code
- `develop`: Integration branch for features
- `feature/*`: Feature branches
- `hotfix/*`: Critical fixes

### Making Changes
1. Create a feature branch from `develop`
2. Make your changes
3. Add/update tests as needed
4. Ensure all tests pass: `dotnet test`
5. Update documentation if needed
6. Submit a pull request

### Code Standards
- Follow Microsoft C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Maintain test coverage above 80%
- Ensure no compiler warnings

### Testing Requirements
- All new features must include unit tests
- Integration tests for API interactions
- Tests should be independent and repeatable
- Use descriptive test names that explain the scenario

### Pull Request Process
1. Ensure your PR description clearly describes the changes
2. Link any relevant issues
3. Ensure all CI checks pass
4. Request review from maintainers
5. Address any feedback promptly

## Project Structure

```
Fellowship.SDK/
├── .github/                 # GitHub Actions workflows
├── Fellowship.SDK/          # Main SDK library
│   ├── Api/                 # API client implementations
│   ├── Models/              # Data models
│   ├── Filters/             # Filtering system
├── Fellowdhip.SDK.Tests/    # Unit tests
├── lotrRunner/              # Sample application
└── docs/                    # Documentation
```

## Reporting Issues

When reporting issues, please include:
- .NET version
- Operating system
- Clear description of the problem
- Steps to reproduce
- Expected vs actual behavior
- Any error messages or stack traces

## Feature Requests

For new features:
- Check existing issues first
- Provide clear use case and rationale
- Consider backward compatibility
- Be open to discussion about implementation

## Code of Conduct

This project follows the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).

## Getting Help

- Check the [README](../README.md) for basic usage
- Review the [DESIGN](../DESIGN.md) document for architecture details
- Open an issue for questions or problems
- Join discussions in existing issues

## Recognition

Contributors will be recognized in:
- Release notes
- Contributors section (coming soon)
- Special thanks for significant contributions

Thank you for contributing to Fellowship SDK!
