# JUSToolkit

Romhacking tools for Jump Ultimate Stars! (NDS)
[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/)

## Features

Done ✔️ To test / with issues ⚠️ Not done 🛑

### Containers

- Pack .aar ALAR3 ✔️
- Unpack .aar ALAR3 ✔️
- Pack .aar ALAR2 ✔️
- Unpack .aar ALAR2 ✔️

### Graphics

- Extract ALMT + DIG ✔️
- Import ALMT + DIG ✔️
- Extract DTX ✔️
- Import DTX 🛑

### Texts

- In progress, mostly done 🛑

### Batch Features

- Extract every .dig from any .aar (ALAR2/ALAR3) to PNG ✔️
- Import multiple PNGs to an .aar ALAR3 container ✔️

## Tinke

You can use [Tinke by PleoNex](https://github.com/pleonex/tinke) to unpack
containers and view .dig files.

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

## How To Use

`./JUS.CLI jus [type] [feature] [args]`

- graphics
  - export-dtx
  - export-dig
  - import-dig
- containers
  - export-alar3
  - import-alar3
  - export-alar2
  - import-alar2
- batch
  - export-alar-png
  - import-png-alar3

To get the arguments of a feature you can use:
`./JUS.CLI jus [type] [feature] -h`

Examples:
`./JUS.CLI jus containers export-alar3 --container test.aar --output myDirectory`
`./JUS.CLI jus containers export-alar3 -h`

## Documentation

### Documents

You can find these documents in our `docs/dev` folder:

- Alar Specification
- DTX Specification by PleoNex
- DTX Research by PleoNex
- FileFormats by Uknown Hacker

### Videos

PleoNex did some research on Streaming:

- [DIRECTO ROM Hacking: Triple reto de imágenes](https://www.youtube.com/watch?v=r1Rsx6RRe1U)
- [DIRECTO Domingos de desensamblador: imágenes de Devil Survivor y JUS y ordenar textos de MetalMax 3](https://www.youtube.com/watch?v=R2h-UEcO_-k)
- [DIRECTO Predomingos de desensamblador: CLYT de 3DS y el complejo caso de los sprites de JUS](https://www.youtube.com/watch?v=1KT4u_Kvaws)

## Stack

- C# Net Core 6
- [YARHL by PleoNex](https://github.com/SceneGate/Yarhl)
- [PleOps by PleoNex](https://github.com/pleonex/PleOps.Cake)

## Credits

Special thanks to [PleoNex](https://github.com/pleonex) for his help, for Yarhl
and PleOps.Cake. Thanks to [TraduSquare](https://tradusquare.es) for the
inspiration and support. Thanks to the Jump Ultimate Stars! devs for this
amazing game.
