# NesDev Architecture Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Solution Structure](#solution-structure)
3. [Component Architecture](#component-architecture)
4. [Core Technologies](#core-technologies)
5. [Data Flow & Interactions](#data-flow--interactions)
6. [Key Design Patterns](#key-design-patterns)
7. [Build & Development Workflow](#build--development-workflow)
8. [Extension Points](#extension-points)

---

## Project Overview

**NesDev** is a comprehensive Integrated Development Environment (IDE) specifically designed for Nintendo Entertainment System (NES) game development. The project combines modern IDE features with NES emulation capabilities, providing developers with a complete toolchain for writing, compiling, debugging, and testing NES games.

### Vision
The project aims to create an all-in-one development environment that eliminates the need to switch between multiple tools during NES game development. It integrates:
- **Code Editor** with syntax highlighting and customization
- **Build System** with cc65 toolchain integration
- **Emulator** for testing and debugging
- **JavaScript Engine** for automation and scripting
- **Runtime Patching** capabilities via JavaScript bytecode translation

### Target Users
- Homebrew NES game developers
- Retro game enthusiasts
- Students learning 6502 assembly and NES architecture
- Researchers working on retro gaming preservation

---

## Solution Structure

The solution consists of 7 main projects organized in a modular architecture:

```
NesDev.sln
├── NesDev (Main Application)          [WinExe]
├── clangd (Language Server)           [Library]
├── EmuNes (Solution Folder)
│   ├── NesCore (Emulator Core)        [Library]
│   └── SharpNES (UI Renderers)        [Library]
├── HalfNes (Alternative Emulator)     [Library]
├── Interfaces (Core Abstractions)     [Library]
├── JSONRPC (RPC Communication)        [Library]
└── Serializers (Configuration)        [Library]
```

### Project Dependencies

```
NesDev (Main)
├─> References SharpNES
├─> References Interfaces
├─> References Serializers
├─> References JSONRPC
└─> NuGet Dependencies:
    ├─> DockPanelSuite (3.0.6)
    ├─> ScintillaNET (3.6.3)
    ├─> Jint (2.11.58)
    └─> Newtonsoft.Json (12.0.2)

SharpNES
├─> References NesCore
└─> NuGet: NAudio, SharpDX

JSONRPC
└─> References Interfaces

HalfNes
└─> Standalone emulator implementation
```

---

## Component Architecture

### 1. NesDev (Main Application)

**Responsibility**: Primary IDE orchestration, UI management, and user interaction.

**Key Components**:
- `MainForm.cs` - Main IDE window with DockPanel integration
- `MainForm.Exec.cs` - External process execution and management
- `MainForm.ExternalTool.cs` - Custom tool integration
- `MainForm.JSEngine.cs` - JavaScript engine management
- `Program.cs` - Application entry point and initialization

**Sub-components**:
```
NesDev/
├── Config/
│   ├── AppConfig.cs          # Application settings
│   ├── CustomCommand.cs      # Custom command definitions
│   └── CustomCommandList.cs  # Command collection management
├── Dialogs/
│   ├── CommandEditorDialog   # Edit custom commands
│   ├── IncludePathEditor     # Manage include paths
│   └── MacroValueWindow      # Macro expansion UI
└── Forms/
    ├── DocWindow             # Document editor window
    ├── OutputWindow          # Build output display
    └── DocWidgets/
        ├── GotoLine          # Navigation widget
        └── SearchAndReplace  # Search functionality
```

**Features**:
- Multi-document interface (MDI) with docking panels
- Syntax-highlighted code editor (ScintillaNET)
- Build system integration (cc65, ca65, ld65)
- Customizable commands with macro expansion
- Output window for compiler messages
- File management (open, save, reload)

---

### 2. EmuNes (Emulator Components)

#### NesCore (Emulation Engine)
**Responsibility**: Low-level NES hardware emulation.

**Emulated Components**:
- **CPU**: MOS Technology 6502 processor
- **PPU**: Picture Processing Unit (graphics)
- **APU**: Audio Processing Unit (sound)
- **Memory**: RAM, ROM, and mapper implementations
- **Cartridge**: Multiple mapper support (NROM, MMC1, MMC3, etc.)
- **Controllers**: Joypad, Zapper light gun support

#### SharpNES (Rendering Layer)
**Responsibility**: Display and audio output management.

**Key Classes**:
- `IRenderer` - Abstraction for rendering backends
- `SharpDXRenderer` - DirectX-based rendering
- `SDLMMControl` - SDL multimedia control
- `ApuAudioProvider` - Audio output via NAudio
- `MainForm` - Emulator UI window

**Features**:
- Multiple rendering backends (SharpDX, SDL)
- Save state management
- Cheat code system
- Input mapping and configuration
- Database-driven cartridge detection

---

### 3. HalfNes (Alternative Emulator)

**Responsibility**: Secondary emulator implementation with focus on audio accuracy.

**Components**:
- `CPU.cs` - 6502 CPU implementation
- `CPURAM.cs` - Memory management
- `NES.cs` - Main emulator controller
- `Com/` - Communication interfaces

**Design Philosophy**: Simpler, more maintainable codebase focused on audio chip expansion support.

---

### 4. Interfaces (Core Abstractions)

**Responsibility**: Define contracts for inter-component communication.

**Key Interfaces**:

```csharp
// Editor Control
interface IEditor {
    IEditorController Controller;
    IEditorUndoRedo UndoRedo;
    IEditorLocator Locator;
}

// Remote Procedure Call
interface IRPC {
    void Invoke(string method, object[] parameters);
}

// Command Expansion
interface IMacroExpander {
    string Expand(string template, Dictionary<string, string> variables);
}

// Custom Commands
interface ICustomizeCommand {
    string FileName { get; }
    string Arguments { get; }
    void Execute();
}

// Global Events
static class GlobalEvents {
    event EventHandler<ICustomizeCommand> CommandLaunchOccurred;
    event EventHandler SaveFileOccurred;
    event EventHandler<Exception> ExceptionOccurred;
}
```

**Purpose**: Provides a plugin-like architecture allowing components to communicate without tight coupling.

---

### 5. JSONRPC (Communication Layer)

**Responsibility**: Inter-process communication using JSON-RPC 2.0 protocol.

**Components**:
- `JsonRPC.cs` - Base RPC message structures
- `JsonRPCServer.cs` - Server implementation (listens)
- `JsonRPCClient.cs` - Client implementation (sends)
- `JsonRPCBuilder.cs` - Factory for creating RPC endpoints

**Use Cases**:
- Communication between IDE and emulator process
- External tool integration
- Future language server protocol support
- Plugin architecture for extensions

**Protocol Example**:
```json
// Request
{
  "jsonrpc": "2.0",
  "method": "emulator.loadROM",
  "params": { "path": "/path/to/game.nes" },
  "id": 1
}

// Response
{
  "jsonrpc": "2.0",
  "result": { "status": "loaded" },
  "id": 1
}
```

---

### 6. Serializers (Configuration Management)

**Responsibility**: Application settings persistence and macro expansion.

**Components**:
- `Utility.cs` - JSON serialization helpers
- Configuration file I/O

**Managed Data**:
- User preferences
- Custom command definitions
- Include paths for compiler
- Recent file lists
- Window layout states

---

### 7. clangd (Language Server - Incomplete)

**Responsibility**: Language Server Protocol implementation for advanced code features.

**Current Status**: Skeleton implementation

**Planned Features**:
- Code completion
- Go to definition
- Symbol references
- Syntax checking
- Code refactoring

**Note**: This component is currently under development and not fully functional.

---

## Core Technologies

### UI & Windowing
| Technology | Purpose | Version |
|------------|---------|---------|
| Windows Forms | Primary UI framework | .NET 4.5 |
| DockPanelSuite | Docking panel system | 3.0.6 |
| ScintillaNET | Code editor control | 3.6.3 |

### Emulation & Graphics
| Technology | Purpose | Version |
|------------|---------|---------|
| Custom NesCore | NES hardware emulation | - |
| SharpDX | DirectX rendering | Latest |
| SDL | Cross-platform multimedia | - |
| NAudio | Audio playback | Latest |

### Scripting & Automation
| Technology | Purpose | Version |
|------------|---------|---------|
| Jint | JavaScript interpreter | 2.11.58 |
| Newtonsoft.Json | JSON serialization | 12.0.2 |

### Development Tools
| Technology | Purpose |
|------------|---------|
| cc65 | C compiler for 6502 |
| ca65 | Macro assembler |
| ld65 | Linker |
| FamiTracker | Music composition |

---

## Data Flow & Interactions

### 1. Compilation Workflow

```
User Action: Build Command
    ↓
MainForm.GlobalEvents_CommandLaunchOccurred
    ↓
MainForm.ProcessLaunchICustomizeCommand
    ↓
IMacroExpander.Expand(command template)
    ↓
Process.Start(expanded command)
    ↓
OutputWindow.Append(stdout/stderr)
    ↓
Success → Load ROM to Emulator
```

### 2. JavaScript Automation Flow

```
User: Execute JS Script
    ↓
MainForm.JSLauncherCustomizeCommands
    ↓
Jint.Engine.Execute(script)
    ↓
JS calls IDE functions via exposed API
    ↓
    ├─> Open/Save Files
    ├─> Execute Commands
    ├─> Modify Editor Content
    └─> Control Emulator
```

### 3. Emulator Integration Flow

```
NesDev Main Window
    ↓
Embedded SharpNES Control
    ↓
IRenderer (SharpDX or SDL)
    ↓
NesCore (CPU/PPU/APU Emulation)
    ↓
Game Execution
    ↓
    ├─> Video → IRenderer → Display
    ├─> Audio → ApuAudioProvider → Sound Card
    └─> Input ← GameController ← User
```

### 4. JSONRPC Communication

```
IDE Process (Client)         Emulator Process (Server)
    │                               │
    ├── JSONRPC Request ──────────>│
    │   (method, params)            │
    │                               ├── Execute Method
    │                               ├── Process Parameters
    │<─────── JSONRPC Response ─────┤
    │   (result/error)              │
```

---

## Key Design Patterns

### 1. **Observer Pattern**
- `GlobalEvents` provides application-wide event broadcasting
- Components subscribe to events without tight coupling
- Example: Save file events, command launch events

### 2. **Strategy Pattern**
- `IRenderer` allows switching between rendering backends
- `SharpDXRenderer` and `SDLMMControl` are interchangeable strategies

### 3. **Factory Pattern**
- `JsonRPCBuilder` creates RPC server/client instances
- Centralizes creation logic and configuration

### 4. **Interface Segregation**
- `IEditor` is split into `IEditorController`, `IEditorUndoRedo`, `IEditorLocator`
- Clients depend only on methods they use

### 5. **Macro Expansion Pattern**
- `IMacroExpander` enables template-based command generation
- Variables like `$(FileName)`, `$(ProjectDir)` expanded at runtime

### 6. **Docking Panel Architecture**
- `DockContent` base class for all dockable windows
- `DockPanel` manager controls layout persistence

---

## Build & Development Workflow

### Prerequisites
- Visual Studio 2013 or later
- .NET Framework 4.5
- cc65 toolchain (for NES compilation)
- Windows OS (for Windows Forms)

### Build Steps

1. **Open Solution**
   ```
   Open NesDev.sln in Visual Studio
   ```

2. **Restore NuGet Packages**
   ```
   Right-click Solution → Restore NuGet Packages
   ```

3. **Build Solution**
   ```
   Build → Build Solution (Ctrl+Shift+B)
   ```

4. **Run IDE**
   ```
   Debug → Start Debugging (F5)
   ```

### Project Build Order
1. Interfaces (no dependencies)
2. Serializers → Interfaces
3. JSONRPC → Interfaces
4. NesCore (no dependencies)
5. SharpNES → NesCore
6. HalfNes (no dependencies)
7. clangd → Interfaces
8. NesDev → All above

### Configuration Files
- `App.config` - Application settings
- `packages.config` - NuGet dependencies
- `*.csproj` - Project configurations

---

## Extension Points

### 1. Custom Commands
**Location**: `NesDev/Config/CustomCommand.cs`

Add new build commands, tools, or workflows:
```csharp
CustomCommand cmd = new CustomCommand {
    Name = "My Tool",
    FileName = "tool.exe",
    Arguments = "$(FileName)",
    WorkingDirectory = "$(ProjectDir)"
};
```

### 2. JavaScript API
**Location**: `MainForm.JSEngine.cs`

Expose new IDE functions to JavaScript:
```csharp
jsEngine.SetValue("myFunction", new Action<string>((param) => {
    // Custom logic
}));
```

### 3. Macro Variables
**Location**: `IMacroExpander` implementation

Add custom variables for command expansion:
```csharp
macros.Add("$(MyVar)", GetMyValue());
```

### 4. Rendering Backends
**Location**: `SharpNES/IRenderer.cs`

Implement new rendering backends:
```csharp
public class MyRenderer : IRenderer {
    public void Render(byte[] frameBuffer) {
        // Custom rendering logic
    }
}
```

### 5. RPC Methods
**Location**: `JSONRPC/JsonRPCServer.cs`

Register new RPC methods for external communication:
```csharp
rpcServer.RegisterMethod("myMethod", (params) => {
    // Handle RPC call
    return result;
});
```

### 6. Editor Integrations
**Location**: `Interfaces/IEditor.cs`

Extend editor capabilities via interface:
```csharp
public interface IEditorFormatter {
    void Format();
    void BeautifyCode();
}
```

---

## Future Enhancements

### Planned Features
1. **Language Server Protocol** - Complete clangd implementation
2. **Debugger Integration** - Breakpoints, step execution, variable inspection
3. **Sprite Editor** - Visual sprite and tileset editing
4. **Music Editor** - Integrated FamiTracker/FamiTone composer
5. **Cross-Platform** - Avalonia UI or .NET MAUI migration
6. **Git Integration** - Version control within IDE
7. **Project Templates** - Quick-start templates for common game types
8. **Performance Profiler** - CPU/PPU cycle counting and optimization tools

### Architecture Improvements
1. Dependency injection container (e.g., Autofac)
2. MVVM pattern for better testability
3. Plugin system with dynamic loading
4. Asynchronous build system with progress reporting
5. Unit test coverage for core components

---

## Troubleshooting & Known Issues

### Common Issues

1. **Build Errors with cc65**
   - Ensure cc65 is in PATH or configure absolute paths
   - Check include paths in custom commands

2. **Emulator Performance**
   - Try switching renderers (SharpDX vs SDL)
   - Reduce window size for better performance

3. **DockPanel Layout Corruption**
   - Delete layout configuration file
   - Restart IDE to reset to default layout

### Debug Mode
- Launch with Visual Studio debugger attached
- Check OutputWindow for exception details
- Enable verbose logging in App.config

---

## Contributing

### Code Style
- Follow C# naming conventions (PascalCase for public, camelCase for private)
- Use meaningful variable names
- Add XML documentation comments for public APIs
- Keep methods focused and under 50 lines when possible

### Commit Guidelines
- Write descriptive commit messages
- Reference issue numbers when applicable
- Keep commits atomic and focused

### Testing
- Add unit tests for new features
- Ensure existing tests pass before submitting PR
- Test on clean Windows installation when possible

---

## License

This project incorporates multiple open-source components with their respective licenses:
- ScintillaNET (MIT License)
- DockPanelSuite (MIT License)
- Jint (BSD License)
- Json.NET (MIT License)
- NAudio (MIT License)
- cc65 (zlib License)

See LICENSE file and individual component licenses for details.

---

## Contact & Support

- **Repository**: https://github.com/sunneo/NesDev
- **Issues**: https://github.com/sunneo/NesDev/issues
- **NES SDK**: https://github.com/sunneo/nes-sdk

---

*Last Updated: 2026-01-19*
