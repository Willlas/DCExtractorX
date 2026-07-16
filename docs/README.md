# DCExtractorX Documentation

## Overview
DCExtractorX is a reverse engineering and asset extraction tool for PlayStation 2 games, specifically Dark Cloud 1 and Dark Cloud 2. It serves as a preservation/reverse-engineering toolkit that unpacks and converts assets from PS2-era Level-5 game files.

### Installation
To install DCExtractorX, ensure you have .NET 10 SDK installed. Then, clone the repository and build it:

```sh
git clone https://github.com/Willlas/DCExtractorX.git
cd DCExtractorX
dotnet build
```

### Usage
#### Command-Line Interface (CLI)
To use the CLI, navigate to the project directory and run:

```sh
dotnet DCExtractorX.Cli.dll
```

#### Graphical User Interface (GUI) [WIP]
The GUI is currently in development.

## Features
- **Extract DAT archives using HD2/HD3 indexes**
- **Extract PAK-family container files (.PAK, .CHR, .EFP, etc.)**
- **MDS model extraction and conversion to OBJ format**
- **MDS model extraction and conversion to SMD format**
- **IMG texture package extraction to PNG format**
- **TM2 texture extraction to PNG format**
