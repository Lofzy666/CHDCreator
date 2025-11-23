# CHD Creator

A Windows desktop application that batch-converts disc image files to CHD (Compressed Hunted Drive) format, commonly used for retro gaming emulation.

## Features

- **Batch Processing**: Convert multiple disc images at once
- **Archive Support**: Automatically extracts `.7z` and `.zip` archives containing disc images
- **Multiple Formats**: Support for `.cue`, `.iso`, `.7z`, and `.zip` files
- **Recursive Search**: Option to search subdirectories for files
- **Progress Tracking**: Real-time progress bar and detailed logging
- **Flexible Options**:
  - Process compressed archives
  - Process direct disc image files
  - Keep or delete original files after conversion
  - Enable file-based logging

## Installation

1. Download the latest release from the [Releases](https://github.com/Lofzy666/CHDCreator/releases) page
2. Extract the `.exe` file
3. Run `CHDCreator.exe`

No additional installation required - all required tools are embedded.

## Usage

1. **Select a Folder**: Click "Browse..." to choose a directory containing your disc images or archives
2. **Configure Options**:
   - Check "Search Recursively" to include subdirectories
   - Check "Keep Original Archives" to preserve `.7z`/`.zip` files
   - Check "Enable Logging" to save a `conversion.log` file
   - Check "Process Compressed Archives" to extract and convert from archives
   - Check "Process .cue .bin .iso Files" to convert direct disc images
3. **Start Conversion**: Click "Start Conversion" and monitor progress in the log window

## Requirements

- Windows 7 or later
- .NET Framework 4.5+

## License

Licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
