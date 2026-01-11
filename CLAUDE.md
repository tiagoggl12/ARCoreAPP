# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 6 (version 6000.3.3f1) AR application built with ARCore and ARKit support. The project uses the Mobile AR Template and XR Interaction Toolkit to provide cross-platform mobile AR experiences for both Android (ARCore) and iOS (ARKit).

## Architecture

### Core AR Stack
- **AR Foundation 6.3.1**: Provides platform-agnostic AR functionality
- **ARCore 6.3.1**: Android AR platform support
- **ARKit 6.3.1**: iOS AR platform support
- **XR Interaction Toolkit 3.3.0**: Handles AR interactions, object spawning, and manipulation
- **Universal Render Pipeline (URP) 17.3.0**: Graphics rendering pipeline

### Key Components

**GoalManager** (`Assets/MobileARTemplateAssets/Scripts/GoalManager.cs`):
- Manages onboarding flow with progressive tutorial steps
- Coordinates goal states: FindSurfaces, TapSurface, Hints, Scale
- Listens to ObjectSpawner events to track user progress through AR setup

**ARTemplateMenuManager** (`Assets/MobileARTemplateAssets/Scripts/ARTemplateMenuManager.cs`):
- Central UI controller for the AR experience
- Manages object creation menu, delete functionality, and debug options
- Handles AR plane visualization toggling with fade effects
- Dynamically adjusts debug menu positioning based on screen size
- Integrates with ARPlaneManager to track and visualize detected surfaces

**XR Interaction System**:
- Uses XRInteractionGroup for managing focus and selection
- ObjectSpawner handles placing objects on detected AR planes
- Touch input handled via XRInputValueReader for tap and drag gestures

### Scene Structure
- **SampleScene.unity**: Main AR scene located in `Assets/Scenes/`
- Scene includes AR session, plane detection, and XR interaction rig

### Input System
- Unity Input System 1.17.0 for modern input handling
- Touch-based gestures for object placement and manipulation
- AR-specific input readers for tap, drag, and multi-touch interactions

## Development Workflow

### Opening the Project
This project should be opened with Unity Editor 6000.3.3f1 or compatible version. You are currently working with JetBrains Rider as the IDE.

### Build and Run

**For Android (ARCore)**:
```bash
# Build from Unity Editor: File > Build Settings > Android
# Ensure ARCore is enabled in XR Plug-in Management
# Build and Run requires a physical Android device with ARCore support
```

**For iOS (ARKit)**:
```bash
# Build from Unity Editor: File > Build Settings > iOS
# Ensure ARKit is enabled in XR Plug-in Management
# Build generates Xcode project that must be built on macOS
```

### Debug Configurations

Available run configurations in Rider:
- **Attach to Unity Editor**: Debug C# scripts while running in Unity Editor Play Mode
- **Start Unity**: Launch Unity Editor
- **Unit Tests (batch mode)**: Run Unity tests in headless mode
- **Attach to**: Attach debugger to running Unity process

### Testing
```bash
# Run tests in Unity Editor
# Window > General > Test Runner

# Run tests from command line (batch mode)
# Use "Unit Tests (batch mode)" run configuration in Rider
```

## Key Dependencies

Critical packages from `Packages/manifest.json`:
- `com.unity.xr.arfoundation`: AR Foundation framework
- `com.unity.xr.arcore`: Android ARCore support
- `com.unity.xr.arkit`: iOS ARKit support
- `com.unity.xr.interaction.toolkit`: XR interaction system
- `com.unity.xr.management`: XR plugin management
- `com.unity.render-pipelines.universal`: URP rendering
- `com.unity.inputsystem`: New Unity Input System
- `com.unity.mobile.android-logcat`: Android debugging

## Platform-Specific Considerations

### Android
- Requires ARCore-compatible device
- Minimum API level and permissions must be configured in Player Settings
- ARCore settings configured in `ProjectSettings/` via ARCoreSettings

### iOS
- Requires ARKit-compatible iOS device (iPhone 6s or later)
- Camera usage permission required in Info.plist
- ARKit settings configured in `ProjectSettings/` via ARKitSettings

### AR Plane Detection
- Both platforms use AR plane detection for surface placement
- Plane visualization can be toggled via debug menu
- Planes fade in/out based on `ARPlaneMeshVisualizerFader` component
- Detected planes stored and managed by `ARPlaneManager`

## Common Patterns

### Adding New AR Objects
1. Create prefab in `Assets/MobileARTemplateAssets/` or custom folder
2. Add prefab to ObjectSpawner's `objectPrefabs` list in scene
3. Add corresponding UI button in object menu that calls `ARTemplateMenuManager.SetObjectToSpawn(index)`

### Modifying Onboarding Flow
- Edit `GoalManager.StartCoaching()` to add/remove goal steps
- Add corresponding Step objects with UI elements to `m_StepList`
- Handle new goal states in `PreprocessGoal()` method

### Custom XR Interactions
- Extend or implement `IXRSelectInteractable` or `IXRHoverInteractable`
- Add interaction affordances using XRI starter assets patterns
- Register interactables with XRInteractionGroup for focus management

## Project Structure Notes

- `Assets/MobileARTemplateAssets/`: Template-provided AR scripts and UI
- `Assets/Samples/XR Interaction Toolkit/`: XRI sample scripts and assets
- `Assets/XR/`: XR rig and settings
- `Assets/XRI/`: XR interaction-specific prefabs and configurations
- `ProjectSettings/`: Build settings, XR management, and platform configs
- Assembly definitions (`.asmdef`) are present for sample code organization
