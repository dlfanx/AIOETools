using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AIOETools.Views;

namespace AIOETools
{
    public partial class MainWindow : Window
    {
        private TextBlock? _logBox;
        private static string? _lastOpenedFolder;
        private BarcodeGeneratorWindow? _barcodeGeneratorWindow;
        private static MainWindow? _instance; // Track window instance
        private TrayIcon? _trayIcon;

        public MainWindow()
        {
            InitializeComponent();
            _instance = this;
            
            // Register app to startup if user wants it
            if (!StartupManager.IsStartupEnabled())
            {
                StartupManager.RegisterStartup();
            }

            // Minimize on startup if launched with "/minimized"
            if (Environment.GetCommandLineArgs().Any(arg => arg == "/minimized"))
            {
                WindowState = WindowState.Minimized;
            }

            // Find UI elements
            var captureButton = this.FindControl<Button>("CaptureButton");
            var cmdButton = this.FindControl<Button>("OpenCmdButton");
            var minimizeButton = this.FindControl<Button>("MinimizeButton");
            var openResultButton = this.FindControl<Button>("OpenFolderButton");
            var openCalculatorButton = this.FindControl<Button>("OpenCalcButton");
            var openNotepadButton = this.FindControl<Button>("OpenNotepadButton");
            var openWordPadButton = this.FindControl<Button>("OpenWordPadButton");
            var openExplorerButton = this.FindControl<Button>("OpenExplorerButton");
            var openGpeditButton = this.FindControl<Button>("OpenGpeditButton");
            _logBox = this.FindControl<TextBlock>("LogBox");
            var isStartupToggle = this.FindControl<ToggleSwitch>("IsStartupButton");
            
            if (isStartupToggle != null)
            {
                isStartupToggle.IsCheckedChanged += (_, _) =>
                {
                    if (isStartupToggle.IsChecked == true)
                        StartupManager.RegisterStartup();
                    else
                        StartupManager.UnregisterStartup();
                };
            }
            
            // Attach event handlers
            if (captureButton != null)
                captureButton.Click += (_, _) => CaptureAndSaveScreenshot();
            
            if (cmdButton != null)
                cmdButton.Click += (_, _) => StartProcessAsAdmin("cmd.exe");

            if (minimizeButton != null)
                minimizeButton.Click += (_, _) => WindowState = WindowState.Minimized;

            if (openResultButton != null)
                openResultButton.Click += (_, _) => OpenResultFolder();

            if (openCalculatorButton != null)
                openCalculatorButton.Click += (_, _) => StartProcess("calc.exe");

            if (openNotepadButton != null)
                openNotepadButton.Click += (_, _) => StartProcess("notepad.exe");

            if (openWordPadButton != null)
                openWordPadButton.Click += (_, _) => StartProcess("write.exe");
            
            if (openExplorerButton != null)
                openExplorerButton.Click += (_, _) => StartProcess("explorer.exe");

            if (openGpeditButton != null)
                openGpeditButton.Click += (_, _) => StartProcessAsAdmin("mmc.exe", "gpedit.msc");
            
            this.Closing += OnWindowClosing;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void OpenBarcodeGenButton_Click(object sender, RoutedEventArgs e)
        {
            // var barcodeGeneratorWindow = new BarcodeGeneratorWindow();
            // barcodeGeneratorWindow.Show();
            if (_barcodeGeneratorWindow != null)
            {
                if (!_barcodeGeneratorWindow.IsVisible) // If hidden, show it again
                {
                    _barcodeGeneratorWindow.Show();
                }
                else
                {
                    LogMessage("BarcodeGenerator window is already active.");
                    _barcodeGeneratorWindow.Activate(); // Bring it to the front
                }
                return;
            }

            _barcodeGeneratorWindow = new BarcodeGeneratorWindow();
    
            // Clear reference when closed
            _barcodeGeneratorWindow.Closed += (_, _) => _barcodeGeneratorWindow = null;
    
            _barcodeGeneratorWindow.Show();
        }

        private void UnlockAllButton_Click(object sender, RoutedEventArgs e)
        {
            string batchFilePath = Path.Combine(AppContext.BaseDirectory, "safetorun.bat"); // Your script file

            try
            {
                if (File.Exists(batchFilePath))
                {
                    // Run the batch file as Administrator
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = batchFilePath,
                        UseShellExecute = true,
                        Verb = "runas" // Request admin privileges
                    };

                    Process.Start(psi);
                }
                else
                {
                    LogMessage("Script file not found!");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error: {ex.Message}");
            }
        }

        private void CaptureAndSaveScreenshot()
        {
            try
            {
                var screenBounds = new Rectangle(0, 0, 1920, 1080);
                using var screenshot = new Bitmap(screenBounds.Width, screenBounds.Height);
                using (var g = Graphics.FromImage(screenshot))
                    g.CopyFromScreen(screenBounds.Location, Point.Empty, screenBounds.Size);

                string outputDir = Path.Combine(Path.GetTempPath(), "Screenshots");
                Directory.CreateDirectory(outputDir);
                string filePath = Path.Combine(outputDir, $"screenshot_{DateTime.Now:HHmmss}.png");

                screenshot.Save(filePath, ImageFormat.Png);
                _lastOpenedFolder = outputDir;
                LogMessage($"Saved: {filePath}");
            }
            catch (Exception ex)
            {
                LogMessage($"Error: {ex.Message}");
            }
        }

        private void OpenResultFolder()
        {
            if (!string.IsNullOrEmpty(_lastOpenedFolder) && Directory.Exists(_lastOpenedFolder))
                StartProcess("explorer.exe", _lastOpenedFolder);
            else
                LogMessage("No recent folder found.");
        }

        private void StartProcess(string fileName, string arguments = "")
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = fileName, Arguments = arguments, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                LogMessage($"Failed to start {fileName}: {ex.Message}");
            }
        }

        private void StartProcessAsAdmin(string fileName, string arguments = "")
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = fileName, Arguments = arguments, UseShellExecute = true, Verb = "runas" });
            }
            catch (Exception ex)
            {
                LogMessage($"Failed to start {fileName} as admin: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            if (_logBox != null) _logBox.Text = message;
        }
        
        private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
        {
            e.Cancel = true; // Prevent app from closing
            this.WindowState = WindowState.Minimized;
            // this.Hide(); // Hide window instead of closing
        }
    }
}
