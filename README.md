# TMG Hero

[![CI](https://github.com/asbjornb/tmg-hero/actions/workflows/ci.yml/badge.svg)](https://github.com/asbjornb/tmg-hero/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/asbjornb/tmg-hero)

An automated bot for playing Theresmore (theresmoregame.com), built with C# and .NET 9.

## Overview

TMG Hero is a cross-platform application that automates gameplay for Theresmore - an incremental strategy game where you explore and conquer a fantasy world. Using Microsoft Playwright for browser automation, it provides intelligent building management, resource tracking, and strategic decision-making to optimize your settlement's growth from village to city.

## Features

- **Automated Building Management**: Automatically purchases and manages buildings based on configurable strategies
- **Resource Tracking**: Monitors and manages in-game resources (gold, wood, stone, food, etc.)
- **Smart Housing**: Intelligently builds housing to maintain population requirements
- **District Management**: Handles development across Theresmore's 8 districts
- **Save Game Management**: Load and save game states for continuous gameplay
- **Strategy System**: Extensible strategy pattern for implementing different gameplay approaches
- **Real-time Game State Analysis**: Reads and interprets game state directly from the browser

## Technical Architecture

### Core Components

- **GameController**: Main game loop and orchestration
- **GameState**: Real-time game state parsing and management
- **BuildingManager**: Handles building purchases and upgrades
- **ResourceManager**: Tracks and manages resource production and consumption
- **SaveGameManager**: Import/export save game functionality
- **Strategy Pattern**: Pluggable strategies for different playstyles

### Technologies

- **.NET 9** cross-platform library
- **Microsoft Playwright** for browser automation
- **C# 12** with nullable reference types
- **NUnit** for unit testing

## Requirements

- .NET 9 Runtime (Windows, Linux, or macOS)
- Theresmore account (theresmoregame.com)

## Installation

1. Clone the repository:
```bash
git clone https://github.com/asbjornb/tmg-hero.git
cd tmg-hero
```

2. Install development hooks (recommended):
```bash
./scripts/install-hooks.sh
```

3. Build the solution:
```bash
cd tmg-hero/tmg-hero
dotnet build
```

4. Run tests to verify setup:
```bash
dotnet test
```

> **Note**: TMG Hero is currently a library. A user interface will be added in future releases.

## Usage

1. Launch the application
2. Load your save game when prompted
3. Click "Play" to start automation
4. The bot will automatically manage your buildings and resources according to the configured strategy

## Strategy Configuration

The default strategy (`BuildAtCapStrategy`) builds structures when resources are near capacity. You can implement custom strategies by implementing the `IStrategy` interface.

## Testing

Run the test suite:
```bash
dotnet test
```

## Development Setup

### Pre-commit Hooks

To ensure code quality, install git hooks that run build and tests before each commit:

```bash
./scripts/install-hooks.sh
```

This will:
- Build the project before each commit
- Run all tests before each commit
- Prevent commits if build or tests fail

To bypass the hook (not recommended):
```bash
git commit --no-verify
```

### Continuous Integration

The project uses GitHub Actions for CI/CD:
- **Pull Requests**: Automatically builds and tests on Ubuntu, Windows, and macOS
- **Main Branch**: Runs full test suite on merge
- **Cross-platform**: Ensures compatibility across all supported platforms

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## About Theresmore

Theresmore is an incremental strategy game where players:
- Build settlements from villages to cities across 8 districts
- Manage resources including food, wood, stone, and gold
- Command armies with units like archers, warriors, and knights
- Explore a fantasy world filled with enemies from kobolds to demons
- Research technologies from Bronze Age to Industrial Age
- Utilize magic through ancient gods and mana
- Engage in diplomacy with other kingdoms

## Disclaimer

This tool is for educational purposes. Please ensure you comply with Theresmore's terms of service when using automation tools.