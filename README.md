# NesDev - NES Development IDE

<div align="center">

![NES Development](https://img.shields.io/badge/Platform-NES-red)
![.NET Framework](https://img.shields.io/badge/.NET-4.5-blue)
![License](https://img.shields.io/badge/License-See%20Components-green)

An Integrated Development Environment (IDE) for Nintendo Entertainment System (NES) game development, featuring code editor, emulator, debugger, and JavaScript automation.

[Features](#features) • [Installation](#installation) • [Usage](#usage) • [Architecture](#architecture) • [Contributing](#contributing)

</div>

---

## 📋 Table of Contents

- [About](#about)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Technologies](#technologies)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)

---

## 🎮 About

**NesDev** is a comprehensive IDE designed specifically for NES homebrew game development. It combines modern development tools with NES emulation capabilities, providing developers with everything they need to write, compile, debug, and test NES games in a single integrated environment.

### Initial Vision

The project was created to provide:
- **Unified Environment**: All tools in one place - no more switching between editor, compiler, and emulator
- **Code Editor**: Syntax-highlighted editor powered by Scintilla for C and 6502 assembly
- **Integrated Emulator**: Launch and debug games directly from the IDE
- **JavaScript Automation**: Script and automate development tasks using embedded JavaScript engine
- **Runtime Patching**: Experimental feature to patch/modify running games by translating 6502 assembly to JavaScript

---

## ✨ Features

### 🔧 Development Tools
- **Multi-Document Editor**: Tabbed interface with syntax highlighting for C, Assembly, and Makefiles
- **Code Navigation**: Go to line, search and replace, file browsing
- **Build System Integration**: Integrated cc65 compiler, ca65 assembler, and ld65 linker
- **Customizable Commands**: Create custom build commands with macro expansion (e.g., `$(FileName)`, `$(ProjectDir)`)
- **Output Window**: Real-time compiler output and error messages

### 🎯 Emulation & Debugging
- **Embedded NES Emulator**: Full NES hardware emulation (CPU, PPU, APU)
- **Multiple Renderers**: Choose between SharpDX and SDL rendering backends
- **Save States**: Save and load game states for testing
- **Cheat System**: Test game behavior with cheat codes
- **Controller Support**: Joypad and Zapper light gun emulation

### ⚙️ Advanced Features
- **JavaScript Engine**: Automate tasks using embedded Jint JavaScript interpreter
- **Macro Expansion**: Template-based command generation with variable substitution
- **JSONRPC Support**: Inter-process communication for external tool integration
- **Docking Panels**: Customizable workspace with floating/docking windows
- **Project Management**: Manage include paths and project settings

### 🎵 Planned Features
- **Music/Sound Editor**: FamiTracker integration for NSF music composition
- **Sprite Editor**: Visual sprite and tileset editing
- **Debugger**: Breakpoints, step execution, memory inspection
- **Language Server**: Code completion and IntelliSense support

---

## 📦 Prerequisites

### System Requirements
- **Operating System**: Windows 7 or later (Windows Forms application)
- **Framework**: .NET Framework 4.5 or higher
- **IDE**: Visual Studio 2013 or later (for building from source)
- **RAM**: 2GB minimum, 4GB recommended
- **Disk Space**: 500MB for IDE and tools

### Development Tools (for NES compilation)
- **cc65**: C compiler for 6502 architecture
- **ca65**: Macro assembler
- **ld65**: Linker and object file manager
- **Optional**: FamiTracker for music composition

> **Note**: The cc65 toolchain is recommended to be installed and added to your system PATH, or configure absolute paths in custom commands.

---

## 🚀 Installation

### Binary Release (Recommended)
1. Download the latest release from [Releases](../../releases)
2. Extract the archive to your preferred location
3. Run `NesDev.exe`
4. Configure cc65 paths in Settings → Custom Commands

### Build from Source

1. **Clone the repository**:
   ```bash
   git clone https://github.com/sunneo/NesDev.git
   cd NesDev
   ```

2. **Open in Visual Studio**:
   ```
   Open NesDev.sln
   ```

3. **Restore NuGet packages**:
   ```
   Right-click Solution → Restore NuGet Packages
   ```

4. **Build the solution**:
   ```
   Build → Build Solution (Ctrl+Shift+B)
   ```

5. **Run the IDE**:
   ```
   Debug → Start Debugging (F5)
   ```

### SDK Setup
The IDE works best with the [nes-sdk](https://github.com/sunneo/nes-sdk) which provides:
- NES development libraries
- Example projects
- Build scripts and makefiles
- FamiTone2 music engine integration

---

## 🎯 Quick Start

### Creating Your First NES Project

1. **Launch NesDev**: Open `NesDev.exe`

2. **Open a file**: 
   - File → Open (`Ctrl+O`)
   - Navigate to your NES project source files (`.c`, `.s`, `.asm`)

3. **Configure build commands**:
   - Tools → Custom Commands
   - Add compiler command: `cc65 -t nes $(FileName)`
   - Add assembler command: `ca65 $(FileName)`
   - Add linker command: `ld65 -o $(FileNameNoExt).nes $(ObjectFiles)`

4. **Build your project**:
   - Select your build command from the toolbar or menu
   - Check the Output window for compilation results

5. **Run in emulator**:
   - File → Load ROM
   - Select the generated `.nes` file
   - Game runs in the embedded emulator

---

## 📖 Usage

### Editor Features

#### Opening Files
```
File → Open (Ctrl+O)
```
Supports: `.c`, `.h`, `.s`, `.asm`, `.bat`, `makefile`, `.txt`, `.mk`, `.js`, `.nes`

#### Saving Files
```
File → Save (Ctrl+S)
File → Save As (Ctrl+Shift+S)
```

#### Search and Replace
```
Edit → Find (Ctrl+F)
Edit → Replace (Ctrl+H)
```

#### Go to Line
```
Edit → Go to Line (Ctrl+G)
```

### Building Projects

#### Custom Commands
Create reusable build commands with macro variables:

**Macro Variables**:
- `$(FileName)` - Current file name with extension
- `$(FileNameNoExt)` - Current file name without extension
- `$(FileDir)` - Directory of current file
- `$(ProjectDir)` - Project root directory
- Custom variables via configuration

**Example Build Command**:
```
Name: Compile C File
Command: cc65 -t nes -O $(FileName) -o $(FileNameNoExt).s
Working Directory: $(FileDir)
```

### JavaScript Automation

#### Executing JavaScript
Create `.js` files and run them via custom commands:

```javascript
// Example: Automate file operations
var editor = getEditor();
editor.save();
editor.openFile("main.c");
```

#### Available JavaScript API
- File I/O operations
- Editor control (open, save, modify content)
- Command execution
- Emulator control (planned)

### Emulator Usage

#### Loading ROMs
```
Emulator → Load ROM
```
Select a `.nes` file to run in the embedded emulator.

#### Save States
```
Emulator → Save State (F5)
Emulator → Load State (F7)
```

#### Controller Configuration
```
Emulator → Input Configuration
```
Map keyboard keys or game controller buttons to NES controls.

---

## 📂 Project Structure

```
NesDev/
├── NesDev/                 # Main IDE application
│   ├── Config/            # Application settings and custom commands
│   ├── Dialogs/           # UI dialogs (command editor, macro editor)
│   ├── Forms/             # Document windows and output panels
│   ├── MainForm.cs        # Primary IDE window
│   └── Program.cs         # Application entry point
├── EmuNes-vs2013/         # NES emulator components
│   ├── NesCore/           # Emulation core (CPU, PPU, APU)
│   └── SharpNES/          # Rendering and UI layer
├── HalfNes/               # Alternative emulator implementation
├── Interfaces/            # Core abstractions and interfaces
│   ├── IEditor.cs         # Editor control interface
│   ├── IRPC.cs            # RPC protocol interface
│   └── GlobalEvents.cs    # Application-wide events
├── JSONRPC/               # JSON-RPC communication layer
│   ├── JsonRPCServer.cs   # RPC server implementation
│   └── JsonRPCClient.cs   # RPC client implementation
├── Serializers/           # Configuration serialization
├── clangd/                # Language server (in development)
└── NesDev.sln            # Visual Studio solution
```

---

## 🏗️ Architecture

For detailed architecture documentation, see [ARCHITECT.md](ARCHITECT.md).

### High-Level Overview

```
┌─────────────────────────────────────────────┐
│           NesDev Main IDE                    │
│  ┌──────────────┐  ┌────────────────────┐   │
│  │ Code Editor  │  │  Build System      │   │
│  │ (Scintilla)  │  │  (cc65/ca65/ld65) │   │
│  └──────────────┘  └────────────────────┘   │
│  ┌──────────────┐  ┌────────────────────┐   │
│  │ JS Engine    │  │  Custom Commands   │   │
│  │ (Jint)       │  │  (Macro Expansion) │   │
│  └──────────────┘  └────────────────────┘   │
└─────────────────────────────────────────────┘
         │                          │
         │ JSONRPC                  │ Embed
         ▼                          ▼
┌─────────────────┐        ┌────────────────┐
│ External Tools  │        │  NES Emulator  │
│ (Language Srv)  │        │  (SharpNES)    │
└─────────────────┘        └────────────────┘
                                    │
                           ┌────────┴─────────┐
                           │                  │
                     ┌─────▼──────┐   ┌──────▼─────┐
                     │  NesCore   │   │ Renderers  │
                     │ (Hardware) │   │ (DX/SDL)   │
                     └────────────┘   └────────────┘
```

### Component Responsibilities

| Component | Responsibility |
|-----------|---------------|
| **NesDev** | Main UI, editor, build orchestration, JavaScript execution |
| **SharpNES** | Rendering layer, audio output, input handling |
| **NesCore** | NES hardware emulation (CPU, PPU, APU, cartridge) |
| **HalfNes** | Alternative emulator with audio focus |
| **Interfaces** | Contracts for inter-component communication |
| **JSONRPC** | RPC protocol for process communication |
| **Serializers** | Configuration persistence |
| **clangd** | Language server protocol (future) |

---

## 🔧 Technologies

This project adopts the following components and technologies:

### Core Frameworks
- **[.NET Framework 4.5](https://dotnet.microsoft.com/)** - Application runtime
- **[Windows Forms](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)** - UI framework

### UI & Editor
- **[ScintillaNET](https://github.com/jacobslusser/ScintillaNET)** - Code editor with syntax highlighting (from NuGet)
- **[DockPanelSuite](https://github.com/dockpanelsuite/dockpanelsuite)** - Docking panel system with VS2015 theme (from NuGet)

### Scripting & Serialization
- **[Jint](https://github.com/sebastienros/jint)** - JavaScript interpreter for .NET (from NuGet)
- **[Json.NET](https://github.com/JamesNK/Newtonsoft.Json)** - JSON serialization (from NuGet)

### Emulation & Media
- **[EmuNes](https://github.com/colinvella/EmuNes)** - NES emulator (modified for DockPanel embedding)
- **[NAudio](https://github.com/naudio/NAudio)** - Audio playback (from NuGet)
- **[SharpDX](http://sharpdx.org/)** - DirectX wrapper for rendering
- **[AForge.NET](https://github.com/andrewkirillov/AForge.NET)** - Image processing (from NuGet)
- **[BumpKit](https://github.com/DataDink/Bumpkit)** - Sprite sheet utilities (from NuGet)

### Development Tools
- **[cc65](https://cc65.github.io/)** - C compiler for 6502 processors
- **[ca65](https://cc65.github.io/)** - Macro assembler for 6502
- **[ld65](https://cc65.github.io/)** - Linker and object file manager
- **[nes-sdk](https://github.com/sunneo/nes-sdk)** - NES development SDK with libraries and examples

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

### Reporting Issues
- Use the [Issue Tracker](../../issues)
- Include detailed reproduction steps
- Attach screenshots if applicable
- Specify your Windows version and .NET Framework version

### Development Setup
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Make your changes
4. Test thoroughly on Windows
5. Commit with descriptive messages: `git commit -m "Add: Feature description"`
6. Push to your fork: `git push origin feature/your-feature`
7. Open a Pull Request

### Code Style Guidelines
- Follow C# naming conventions (PascalCase for public members)
- Add XML documentation for public APIs
- Keep methods focused and concise
- Write unit tests for new features
- Ensure existing tests pass

### Areas for Contribution
- 🐛 Bug fixes and stability improvements
- 📝 Documentation enhancements
- 🎨 UI/UX improvements
- ⚡ Performance optimizations
- 🧪 Test coverage expansion
- 🌐 Internationalization (i18n)
- 🔌 Plugin system development
- 🎵 Music/sprite editor implementation

---

## 📄 License

This project incorporates multiple open-source components, each with their own licenses:

| Component | License |
|-----------|---------|
| NesDev (main) | See LICENSE file |
| ScintillaNET | MIT License |
| DockPanelSuite | MIT License |
| Jint | BSD 2-Clause License |
| Json.NET (Newtonsoft.Json) | MIT License |
| NAudio | MIT License |
| EmuNes | Original license |
| cc65 | zlib License |

Please refer to individual component licenses and the LICENSE file for full details.

---

## 🙏 Acknowledgments

This project would not be possible without the following:

- **[nes-sdk](https://github.com/sunneo/nes-sdk)** - NES development SDK and libraries
- **[EmuNes](https://github.com/colinvella/EmuNes)** - Modified for DockPanel integration
- The **cc65** team for the excellent 6502 toolchain
- The **NES homebrew community** for documentation and support
- All contributors to the open-source libraries used in this project

### Special Thanks
- NESDev community for comprehensive NES technical documentation
- ScintillaNET for the powerful code editor control
- DockPanelSuite for the professional docking interface

---

## 📞 Contact & Support

- **Repository**: [https://github.com/sunneo/NesDev](https://github.com/sunneo/NesDev)
- **Issues**: [https://github.com/sunneo/NesDev/issues](https://github.com/sunneo/NesDev/issues)
- **NES SDK**: [https://github.com/sunneo/nes-sdk](https://github.com/sunneo/nes-sdk)

---

## 🗺️ Roadmap

### Current Status
- ✅ Code editor with syntax highlighting
- ✅ Build system integration (cc65 toolchain)
- ✅ Embedded NES emulator
- ✅ JavaScript automation engine
- ✅ Custom command system
- ✅ JSONRPC communication layer

### Planned Features
- 🔄 Complete debugger with breakpoints
- 🔄 Language Server Protocol (LSP) integration
- ⏳ Sprite and tileset visual editor
- ⏳ FamiTracker music editor integration
- ⏳ Project templates and wizards
- ⏳ Git version control integration
- ⏳ Performance profiler and optimization tools
- ⏳ Cross-platform support (Avalonia UI or .NET MAUI)

### Experimental Features
- 🧪 Runtime patching via JavaScript bytecode translation
- 🧪 6502 assembly to JavaScript transpiler
- 🧪 Live code reloading during emulation

---

<div align="center">

**Happy NES Development!** 🎮

*Made with ❤️ for the NES homebrew community*

[⬆ Back to Top](#nesdev---nes-development-ide)

</div>
