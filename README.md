# ARCore Unity App

Unity 6 AR application with ARCore and ARKit support for cross-platform mobile AR experiences.

## Overview

This project uses Unity's AR Foundation framework to create augmented reality experiences that work on both Android (ARCore) and iOS (ARKit) devices. It includes an interactive onboarding flow, object placement, manipulation, and plane detection visualization.

## Features

- **Cross-platform AR**: Support for both ARCore (Android) and ARKit (iOS)
- **Interactive Onboarding**: Progressive tutorial system to guide users through AR setup
- **Object Placement**: Tap to place 3D objects on detected surfaces
- **Object Manipulation**: Move, scale, and rotate placed objects using touch gestures
- **Plane Visualization**: Toggle visualization of detected AR planes with fade effects
- **Debug Menu**: Built-in debugging tools for AR development

## Requirements

### Development
- Unity 6000.3.3f1 or later
- JetBrains Rider or Visual Studio (recommended)
- macOS for iOS builds

### Runtime
- **Android**: ARCore-compatible device (Android 7.0+)
- **iOS**: ARKit-compatible device (iPhone 6s or later, iOS 11.0+)

## Key Dependencies

- AR Foundation 6.3.1
- ARCore XR Plugin 6.3.1
- ARKit XR Plugin 6.3.1
- XR Interaction Toolkit 3.3.0
- Universal Render Pipeline 17.3.0
- Unity Input System 1.17.0

## Getting Started

1. Open the project in Unity 6000.3.3f1
2. Open the `SampleScene` in `Assets/Scenes/`
3. Configure build settings for your target platform:
   - **Android**: File > Build Settings > Android
   - **iOS**: File > Build Settings > iOS
4. Enable the appropriate XR Plugin in Project Settings > XR Plug-in Management
5. Build and deploy to a physical device (AR requires device camera)

## Project Structure

- `Assets/MobileARTemplateAssets/` - AR template scripts, UI, and prefabs
- `Assets/Samples/XR Interaction Toolkit/` - XRI sample scripts and assets
- `Assets/Scenes/` - Main AR scene
- `Assets/XR/` - XR rig configuration
- `ProjectSettings/` - Build and platform settings

## CI/CD Pipeline

This project includes a complete CI/CD setup for automated building, testing, and deployment:

### Android (GitHub Actions)
- ✅ Automated APK and AAB builds
- ✅ Unity Test Runner (Edit Mode & Play Mode)
- ✅ C# code linting and validation
- ✅ Automatic GitHub Releases

### iOS (Xcode Cloud)
- ✅ Xcode project generation from Unity
- ✅ TestFlight deployment
- ✅ Automatic distribution to testers

**Setup Guide**: See [CI-CD-SETUP.md](CI-CD-SETUP.md) for complete configuration instructions.

**Workflows**:
- `test-runner.yml` - Fast testing on PRs
- `android-build.yml` - Full Android build pipeline
- `generate-xcode-project.yml` - iOS Xcode project generation

## Documentation

- [CLAUDE.md](CLAUDE.md) - Architecture and development guidance
- [CI-CD-SETUP.md](CI-CD-SETUP.md) - CI/CD configuration and usage

## Platform-Specific Notes

### Android (ARCore)
- Requires Google Play Services for AR
- Camera permission is automatically requested
- Test on ARCore-supported devices only

### iOS (ARKit)
- Camera usage permission required (configured in Info.plist)
- Requires Xcode for final build and deployment
- Test on ARKit-supported devices only

## License

[Add your license information here]