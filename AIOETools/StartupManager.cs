using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AIOETools
{
    public static class StartupManager
    {
        private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private static readonly string AppName = "AIOETools"; // Change this to your app name
        private static readonly string AppPath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;

        public static void RegisterStartup()
        {
            if (string.IsNullOrEmpty(AppPath))
                return;

            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
            {
                key?.SetValue(AppName, $"\"{AppPath}\" /minimized");
            }
        }

        public static void UnregisterStartup()
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
            {
                key?.DeleteValue(AppName, false);
            }
        }

        public static bool IsStartupEnabled()
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false))
            {
                return key?.GetValue(AppName) != null;
            }
        }
    }
}