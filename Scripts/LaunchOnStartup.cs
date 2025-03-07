using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace NnUtils.Scripts
{
    public static class LaunchOnStartup
    {
        public static void Toggle(bool launchOnStartup)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (!_init) Init();
            if (launchOnStartup) AddToStartup();
            else RemoveFromStartup();
#else
            Debug.LogWarning("Launch On Startup is available only on Windows standalone builds.");
#endif
        }

#if UNITY_STANDALONE_WIN
        private const string RunRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private static bool _init;
        private static string _applicationName;
        private static string _applicationPath;

        // P/Invoke for direct Windows Registry access
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegOpenKeyEx(IntPtr hKey, string lpSubKey, uint ulOptions, int samDesired, out IntPtr phkResult);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegSetValueEx(IntPtr hKey, string lpValueName, uint Reserved, uint dwType, string lpData, int cbData);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegDeleteValue(IntPtr hKey, string lpValueName);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegCloseKey(IntPtr hKey);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, IntPtr lpReserved, out uint lpType, IntPtr lpData,
            ref uint lpcbData);

        // Constants for RegOpenKeyEx
        private const int HKEY_CURRENT_USER = unchecked((int)0x80000001);
        private const int KEY_ALL_ACCESS = 0xF003F;
        private const uint REG_SZ = 1;

        private static void Init()
        {
            // Get the application name (without extension)
            _applicationName = Application.productName;
            // Get the full path to the executable
            _applicationPath = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(_applicationPath))
            {
                // Fallback if MainModule is not accessible
                _applicationPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Application.companyName,
                    Application.productName,
                    Path.GetFileName(Application.dataPath).Replace("_Data", ".exe"));
            }

            _init = true;
            Debug.Log($"Initialized LaunchOnStartup with app path: {_applicationPath}");
        }

        /// <summary>
        /// Adds the current application to Windows startup registry using P/Invoke
        /// </summary>
        private static void AddToStartup()
        {
            try
            {
                IntPtr hKey = IntPtr.Zero;
                int result = RegOpenKeyEx((IntPtr)HKEY_CURRENT_USER, RunRegistryPath, 0, KEY_ALL_ACCESS, out hKey);

                if (result != 0)
                {
                    Debug.LogError($"Cannot access the registry key for startup applications. Error code: {result}");
                    return;
                }

                // Check if already exists
                if (ValueExists(hKey, _applicationName))
                {
                    Debug.Log($"{_applicationName} is already in startup registry.");
                    RegCloseKey(hKey);
                    return;
                }

                // Set the registry value
                int dataSize = (_applicationPath.Length + 1) * 2; // Unicode string
                result = RegSetValueEx(hKey, _applicationName, 0, REG_SZ, _applicationPath, dataSize);

                if (result != 0)
                {
                    Debug.LogError($"Error setting registry value. Error code: {result}");
                }
                else
                {
                    Debug.Log($"Added {_applicationName} to startup registry with path {_applicationPath}");
                }

                RegCloseKey(hKey);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error adding to startup: {ex}");
            }
        }

        /// <summary>
        /// Removes the current application from Windows startup registry using P/Invoke
        /// </summary>
        private static void RemoveFromStartup()
        {
            try
            {
                IntPtr hKey = IntPtr.Zero;
                int result = RegOpenKeyEx((IntPtr)HKEY_CURRENT_USER, RunRegistryPath, 0, KEY_ALL_ACCESS, out hKey);

                if (result != 0)
                {
                    Debug.LogError($"Cannot access the registry key for startup applications. Error code: {result}");
                    return;
                }

                // Check if exists before trying to delete
                if (!ValueExists(hKey, _applicationName))
                {
                    Debug.Log($"{_applicationName} is not in startup registry.");
                    RegCloseKey(hKey);
                    return;
                }

                // Delete the registry value
                result = RegDeleteValue(hKey, _applicationName);

                if (result != 0)
                {
                    Debug.LogError($"Error removing registry value. Error code: {result}");
                }
                else
                {
                    Debug.Log($"Removed {_applicationName} from startup registry.");
                }

                RegCloseKey(hKey);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error removing from startup: {ex}");
            }
        }

        /// <summary>
        /// Checks if a value exists in the registry
        /// </summary>
        private static bool ValueExists(IntPtr hKey, string valueName)
        {
            uint type = 0;
            uint dataSize = 0;

            // First call to get the size of the data
            int result = RegQueryValueEx(hKey, valueName, IntPtr.Zero, out type, IntPtr.Zero, ref dataSize);

            // If the key doesn't exist, ERROR_FILE_NOT_FOUND (2) is returned
            return result != 2;
        }
#endif
    }
}