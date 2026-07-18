# Boilerplates for HoloKit Unity SDK

This repository contains the carrier project for [HoloKit Unity SDK](https://github.com/holokit/holokit-unity-sdk). 

For how to use this project, see the section [How To Work With The SDK](https://github.com/holokit/holokit-unity-sdk/blob/main/CONTRIBUTING.md#How-To-Work-With-The-SDK) in the SDK's `CONTRIBUTING.md` file.

## System requirements

- Unity 6
- Xcode 16
- iOS 18

## Local compile gate

Run both the macOS Editor and iOS script-compilation checks before opening a pull request:

```bash
UNITY_EDITOR_PATH="/path/to/Unity.app/Contents/MacOS/Unity" \
  ./scripts/verify-unity-compile.sh
```

The script opens the project in a separate batch-mode process for each target. It compiles and imports the project but does not create a macOS player or Xcode build.
