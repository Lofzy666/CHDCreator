# CHD Creator

A WinForms application to convert disc images and compressed archives to CHD (Compressed Hunks of Data) format.

## Requirements

- Visual Studio 2017 or later
- .NET Framework 4.7.2 or later
- `7z.exe` - For archive extraction (embedded in project)
- `chdman.exe` - For CHD conversion (embedded in project)

## Setup

1. **Clone/open this project in Visual Studio**

2. **Add embedded resources:**
   - Copy `7z.exe` and `chdman.exe` to the `Resources` folder
   - In Solution Explorer, right-click each file
   - Set **Properties** → **Build Action** → **Embedded Resource**

3. **Build the project:**
   ```
   Build → Build Solution (Ctrl+Shift+B)
   ```

4. **Run:**
   - Press F5 to debug or Ctrl+F5 to run without debugging
   - Or run `CHDCreator.exe` from `bin\Debug\` or `bin\Release\`

## Features

- **Folder Selection**: Browse and select a folder containing archives or disc images
- **Recursive Search**: Option to search subdirectories recursively
- **Archive Processing**: Extract and convert `.7z` and `.zip` archives
- **Direct File Processing**: Convert `.cue` and `.iso` disc images directly to CHD
- **Progress Tracking**: Real-time progress bar for conversions
- **Logging**: Optional file logging to `conversion.log` in the selected folder
- **Keep Archives**: Option to preserve original files after conversion

## Usage

1. Click **Browse...** to select a folder
2. Choose options:
   - ☑ **Search Recursively** - Search subdirectories
   - ☑ **Keep Original Archives** - Don't delete source files
   - ☑ **Enable Logging** - Write log to conversion.log
   - ☑ **Process Compressed Archives** - Handle .7z/.zip files
   - ☑ **Process .cue .bin .iso Files** - Handle disc images
3. Click **Start Conversion**
4. Monitor progress in the log area

## Project Structure

```
CHDCreator/
├── CHDCreator.csproj          # Project file with embedded resources
├── CHDCreator.cs              # Main application code
├── Properties/
│   └── AssemblyInfo.cs        # Assembly metadata
├── Resources/
│   ├── 7z.exe                 # 7-Zip executable (embedded)
│   └── chdman.exe             # MAME CHD tool (embedded)
└── README.md                  # This file
```

## Building for Distribution

1. Build in Release mode: **Build** → **Release**
2. Executable and resources are in `bin\Release\CHDCreator.exe`
3. The embedded executables extract on first run to the app directory

## Troubleshooting

- **"Tool not found" warning**: Ensure `7z.exe` and `chdman.exe` are in the `Resources` folder and set as Embedded Resources
- **Conversion fails**: Check that source files are valid disc images or archives
- **Permission denied**: Run as Administrator if file deletion fails
