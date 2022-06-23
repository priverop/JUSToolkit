# .NET Project template [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/) ![Build and release](https://github.com/pleonex/template-csharp/workflows/Build%20and%20release/badge.svg)

This is a template repository for .NET projects. It uses the build system and
DevOps pipeline [PleOps.Cake](https://github.com/pleonex/PleOps.Cake).

<!-- prettier-ignore -->
| Release | Package                                                           |
| ------- | ----------------------------------------------------------------- |
| Stable  | [![Azure Artifacts](https://feeds.dev.azure.com/benito356/339c91a8-9d6c-4082-8b1a-93c2ae76b637/_apis/public/Packaging/Feeds/e3acf8ba-ec70-46f0-b1a5-da1ce3dd5d9f/Packages/b8696a32-e71a-4479-9b0e-002997b8d8ef/Badge)](https://dev.azure.com/benito356/NetDevOpsTest/_packaging?_a=package&feed=e3acf8ba-ec70-46f0-b1a5-da1ce3dd5d9f&package=b8696a32-e71a-4479-9b0e-002997b8d8ef&preferRelease=true) |
| Preview | [Azure Artifacts](https://dev.azure.com/SceneGate/SceneGate/_packaging?_a=feed&feed=SceneGate-Preview) |

## Setup

Follow the
[checklist in PleOps.Cake](https://github.com/pleonex/PleOps.Cake/blob/develop/docs/guides/Project%20setup.md)
to adapt this template to your project.

## Documentation

Feel free to ask any question in the
[project Discussion site!](https://github.com/pleonex/template-csharp/discussions)

Check our on-line [documentation](https://www.pleonex.dev/PleOps.Cake/).

## Build

The project requires to build .NET 6.0 SDK (Linux and MacOS require also Mono).
If you open the project with VS Code and you did install the
[VS Code Remote Containers](https://code.visualstudio.com/docs/remote/containers)
extension, you can have an already pre-configured development environment with
Docker or Podman.

To build, test and generate artifacts run:

```sh
# Only required the first time
dotnet tool restore

# Default target is Stage-Artifacts
dotnet cake
```

To just build and test quickly, run:

```sh
dotnet cake --target=BuildTest
```
