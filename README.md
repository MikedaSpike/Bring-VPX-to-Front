# Bring Visual Pinball to Front

This VB.NET program is designed to monitor the start and stop of specific Visual Pinball processes and bring the associated window to the front. It minimizes itself to the system tray and includes logging functionality to track its actions.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Functions](#functions)
- [Logging](#logging)
- [Contributing](#contributing)
- [License](#license)

## Installation

1. **Clone the repository:**
    ```sh
    git clone https://github.com/MikedaSpike/Bring-VPX-to-Front.git
    ```
2. **Open the project in Visual Studio:**
    - Navigate to the cloned directory.
    - Open the `.sln` file with Visual Studio.

3. **Build the project:**
    - Build the solution to restore the necessary NuGet packages and compile the project.

## Usage

1. **Run the application:**
    - The program will start minimized to the system tray.
    - It will monitor the start and stop of specified Visual Pinball processes.

2. **Interact with the tray icon:**
    - **Double-click** or **click** the tray icon to restore the application window.

3. **Check logs:**
    - Logs of the program’s actions are displayed in the main window’s RichTextBox.

## Functions

### Form Events

#### `Form1_Load`
- Initializes the form on load.

#### `Form1_FormClosing`
- Handles form closing event.

### Process Watchers

#### `ProcessStartWatcher_EventArrived`
- Handles the start of processes.

#### `ProcessStopWatcher_EventArrived`
- Handles the stop of processes.

### Utility Functions

#### `Delay`
- Creates an asynchronous delay.

#### `ActivateWindow`
- Activates a window by its title.

### Form Behavior

#### `Form1_Resize`
- Handles form resize events.

#### `Form1_Closing`
- Handles form closing.

#### `NotifyIcon1_DoubleClick`
- Handles system tray icon double-click.

#### `NotifyIcon1_Click`
- Handles system tray icon click.

### Logging

#### `LogAnError`
- Logs errors.

#### `UpdateLog`
- Logs actions.

### Process Helper Class

#### `GetActiveProcess`
- Retrieves the active process.

## Logging

The program logs various actions and events to the RichTextBox in the main window, including:
- Process starts and stops.
- Window activation attempts.
- Error messages.

## Contributing

Contributions are welcome! Please follow these steps to contribute:
1. **Fork the repository**
2. **Create a new branch:**
    ```sh
    git checkout -b feature-branch
    ```
3. **Make your changes**
4. **Commit your changes:**
    ```sh
    git commit -m 'Add some feature'
    ```
5. **Push to the branch:**
    ```sh
    git push origin feature-branch
    ```
6. **Submit a pull request**

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
