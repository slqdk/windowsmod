using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;


namespace WindowsMod
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static class GlobalData
        {
           
            public static string DetectedKeyEntry;
            public static string DetectedKeyString;
            public static string DetectedKeyValue;
            public static string DetectedWindowsVersion;
            public static string DetectedWindowsBuildVersion;
            public static string DetectedCurrentWindowsMajorBuildVersion;
            public static bool Windows12Detected = false;
            public static bool Windows11Detected = false;
            public static bool Windows10Detected = false;
            public static bool Windows8Detected = false;
            public static bool Windows7Detected = false;
            public static bool WindowsXPDetected = false;
           
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            textBox_OutputWindow.Text = Environment.NewLine + "Welcome to Windows Mod." + Environment.NewLine + Environment.NewLine + "This app can be used to quickly restore your trust in Windows 10/11." + Environment.NewLine + "Click on your desired action, and reboot your PC to see the changes." + Environment.NewLine + Environment.NewLine + "This app can be downloaded at http://WindowsMod.com, and are an open source project made by SLN, just to make life sweeter." ;

            GlobalData.DetectedWindowsVersion = Regedit_GetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            GlobalData.DetectedWindowsBuildVersion = Regedit_GetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
            GlobalData.DetectedCurrentWindowsMajorBuildVersion = Regedit_GetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber");


            // DetectWindowsVersion
            if (GlobalData.DetectedCurrentWindowsMajorBuildVersion == "10")
            {
                if (Convert.ToInt16(GlobalData.DetectedWindowsBuildVersion) >= 22000)
                {
                    GlobalData.Windows11Detected = true;

                }else
                {
                    GlobalData.Windows10Detected = true;
                }

            }

            if (GlobalData.Windows11Detected)
            {
                label_Windows11.Visible = true;
            }

            if (GlobalData.Windows10Detected)
            {
                label_Windows10.Visible = true;
            }

            

        }

        /// <summary>
        /// Sets a registry value. Creates the key and/or value if they do not exist.
        /// </summary>
        /// <param name="rootKey">The Registry Hive (e.g., HKey_Current_User)</param>
        /// <param name="subKeyPath">The path to the key (e.g., @"Software\MyApplication")</param>
        /// <param name="valueName">The name of the value to set</param>
        /// <param name="valueData">The data to store</param>
        public static string Regedit_SetValue(RegistryKey rootKey, string subKeyPath, string valueName, string valueData)
        {
            try
            {
                // CreateSubKey opens the key if it exists or creates it if it doesn't.
                using (RegistryKey key = rootKey.CreateSubKey(subKeyPath))
                {
                    if (key == null)
                        return $"Error: Could not open or create key at {subKeyPath}";
                  
                    // 1. Check if it's an Integer (DWORD)
                    if (int.TryParse(valueData, out int intValue))
                    {
                        key.SetValue(valueName, intValue, RegistryValueKind.DWord);
                        return $"Success: Set {valueName} as DWORD with value {intValue}";
                    }

                    // 2. Check if it's a Boolean (Convert to 0 or 1)
                    if (bool.TryParse(valueData, out bool boolValue))
                    {
                        int toggle = boolValue ? 1 : 0;
                        key.SetValue(valueName, toggle, RegistryValueKind.DWord);
                        return $"Success: Set {valueName} as Boolean (DWORD) with value {toggle}";
                    }

                    // 3. Default to String (REG_SZ)
                    key.SetValue(valueName, valueData, RegistryValueKind.String);
                    return $"Success: Set {valueName} as String with value \"{valueData}\"";
                }
            }
            catch (UnauthorizedAccessException)
            {
                return "Error: Access Denied. Try running the application as Administrator.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


        /// <summary>
        /// Reads a registry value as a string. Returns null if not found.
        /// </summary>
        /// <param name="rootKey">The Registry Hive</param>
        /// <param name="subKeyPath">The path to the key</param>
        /// <param name="valueName">The name of the value to read</param>
        public static string Regedit_GetValue(RegistryKey rootKey, string subKeyPath, string valueName)
        {
            try
            {
                // Open the subkey. 'false' means we are opening it for Read-Only access.
                using (RegistryKey key = rootKey.OpenSubKey(subKeyPath, false))
                {
                    if (key != null)
                    {
                        // GetValue returns an object; .ToString() converts it for us.
                        object value = key.GetValue(valueName);
                        if (value != null)
                        {
                            return value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
             
            }

            return null; // Return null if the key or value doesn't exist
        }


        /// <summary>
        /// Deletes a registry key and all its subkeys/values.
        /// </summary>
        /// <param name="rootKey">The Registry Hive</param>
        /// <param name="subKeyPath">The path to the key to be deleted</param>
        /// <param name="throwOnMissing">If false, no error is raised if the key doesn't exist</param>
        public static void Regedit_DeleteKey(RegistryKey rootKey, string subKeyPath, bool throwOnMissing = false)
        {
            try
            {
                // DeleteSubKeyTree removes the key and all children.
                // Setting the second parameter to 'false' prevents an exception if the key is already gone.
                rootKey.DeleteSubKeyTree(subKeyPath, throwOnMissing);
               
            }
            catch (Exception ex)
            {
                //
            }
        }


        public static void SetUserPreferencesMask()
        {
            try
            {
                // Get the registry key
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);

                if (key == null)
                {
                    
                    return;
                }

                // Convert the hex string to a byte array
                byte[] data = new byte[] { 0x90, 0x12, 0x01, 0x80, 0x10, 0x00, 0x00, 0x00 };

                // Set the value
                key.SetValue("UserPreferencesMask", data, RegistryValueKind.Binary);

                

                // Close the key
                key.Close();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void button_SpeedUpWindows_Click(object sender, EventArgs e)
        {
            textBox_OutputWindow.Clear();

            string returnvalue;

            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Desktop","MenuShowDelay", "20"); // Windows has a built-in delay (400ms) before a menu or submenu appears. Reducing this makes the OS feel immediate.
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Desktop\WindowMetrics", "MinAnimate", "0"); // This stops windows from "sliding" or "fading" when you minimize or maximize them, making the transition instant.
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Desktop", "WaitToKillAppTimeout", "2000"); // If you’re tired of Windows waiting for apps to close during shutdown, you can shorten the "wait" timer.
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Desktop", "HungAppTimeout", "2000"); // If you’re tired of Windows waiting for apps to close during shutdown, you can shorten the "wait" timer.
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Serialize", "StartupDelayInMSec", "0"); // Windows intentionally delays startup apps by a few seconds to ensure the core system is stable. On modern SSDs, this isn't necessary.
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableFirstLogonAnimation", "0");
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);


            // Visual settings
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", "3"); // Removes all animations in windows
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);


            SetUserPreferencesMask();



            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Desktop\WindowMetrics", "MinAnimate", "0"); // content of a window is shown while dragging
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAnimations", "0"); // content of a window is shown while dragging
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Desktop", "MenuFade", "0"); // content of a window is shown while dragging
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Desktop", "MenuAnimation", "No"); // content of a window is shown while dragging
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Desktop", "TooltipAnimation", "No"); // content of a window is shown while dragging
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            // 0 = Show Thumbnails, 1 = Always show icons
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "IconsOnly", "0");
            textBox_OutputWindow.AppendText("Enable Thumbnails: " + returnvalue + Environment.NewLine);

            // 2 = Enabled (Smooth edges), 0 = Disabled
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Desktop", "FontSmoothing", "2");

            // Set the smoothing type to 2 (ClearType / Antialiasing)
            Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Desktop", "FontSmoothingType", "2");

            textBox_OutputWindow.AppendText("Enable Smooth Fonts: " + returnvalue + Environment.NewLine);

            
            Console.WriteLine("Restart the computer to apply the changes");
        }

        private void button_RemoveMSDataCollection_Click(object sender, EventArgs e)
        {
            textBox_OutputWindow.Clear();
            string returnvalue;

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", "0"); // Disable Windows Telemetry (Diagnostic Data) Note: On Windows Home and Pro, setting this to 0 often defaults back to 1
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities", "0"); // This prevents Windows from tracking the apps you use and files you open to sync them across devices.
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", "0"); // This prevents Windows from tracking the apps you use and files you open to sync them across devices.
            textBox_OutputWindow.AppendText(returnvalue + Environment.NewLine);

        }

        private void button_SwitchToWin7_10Navigation_Click(object sender, EventArgs e)
        {
            textBox_OutputWindow.Clear();
            string returnvalue;

            // Restore Windows 10/7 Style Classic Context Menu
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32", "", "");
            textBox_OutputWindow.AppendText("Classic Context Menu: " + returnvalue + Environment.NewLine);

            // Disable Taskbar Grouping / Collapse (0 = Always, 1 = When full, 2 = Never)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarGlomLevel", "2");
            textBox_OutputWindow.AppendText("Taskbar Un-combine: " + returnvalue + Environment.NewLine);

            // Disable "Suggestions" in Start Menu
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\UserProfileEngagement", "ScoobeSystemSettingEnabled", "0");

            // Disable "Tips and Suggestions" Ads
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SubscribedContent-338389Enabled", "0");

            // Disable Search Highlights (the icons/ads in the Taskbar Search bar)
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "EnableAllowedPhrase", "0");

            // Disable Widgets (Taskbar)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarDa", "0");

            // Disable Chat (Teams icon)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarMn", "0");

            // Align Taskbar to the Left (0 = Left, 1 = Center)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAl", "0");

            // Disable Bing Search in Start Menu
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", "1");
            textBox_OutputWindow.AppendText("Disable Bing Search: " + returnvalue + Environment.NewLine);

            // Remove News and Interests (Feeds) - 2 is Disabled
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Feeds", "EnableFeeds", "0");
            textBox_OutputWindow.AppendText("Remove News and Interests: " + returnvalue + Environment.NewLine);

            // Remove Notification Center / Action Center
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Policies\Microsoft\Windows\Explorer", "DisableNotificationCenter", "1");
            textBox_OutputWindow.AppendText("Remove Notification Center: " + returnvalue + Environment.NewLine);

            // Force Scrollbars to always stay visible (0 = Always Visible, 1 = Auto-hide)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Accessibility", "DynamicScrollbars", "0");
            textBox_OutputWindow.AppendText("Disable Hiding Scrollbars: " + returnvalue + Environment.NewLine);

            // Disable Meet Now / Chat icon
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "HideSCAMeetNow", "1");
            textBox_OutputWindow.AppendText("Disable Meet Now: " + returnvalue + Environment.NewLine);

            // Hide Task View button (0 = Hidden, 1 = Visible)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowTaskViewButton", "0");
            textBox_OutputWindow.AppendText("Remove Task View: " + returnvalue + Environment.NewLine);

            // Hide Search on Taskbar (0 = Hidden, 1 = Icon only, 2 = Search Box)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Search", "SearchboxTaskbarMode", "0");
            textBox_OutputWindow.AppendText("Hide Search Box: " + returnvalue + Environment.NewLine);

            // Remove Search Highlights/Icons on Taskbar
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "EnableAllowedPhrase", "0");
            textBox_OutputWindow.AppendText("Disable Search Highlights: " + returnvalue + Environment.NewLine);

            // Open File Explorer to 'This PC' (1 = This PC, 2 = Quick Access)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo", "1");
            textBox_OutputWindow.AppendText("Set Explorer to This PC: " + returnvalue + Environment.NewLine);

            // Disable Online Tips in Settings
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer", "AllowOnlineTips", "0");
            textBox_OutputWindow.AppendText("Disable Settings Tips: " + returnvalue + Environment.NewLine);

            // Disable UAC Screen Dimming (Prompt remains, but screen doesn't freeze)
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", "0");
            textBox_OutputWindow.AppendText("Disable UAC Dimming: " + returnvalue + Environment.NewLine);

            // Enable Classic Volume Control (This varies by Windows build)
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\MTCUVC", "EnableMtcUvc", "0");
            textBox_OutputWindow.AppendText("Classic Volume Mixer: " + returnvalue + Environment.NewLine);

            // Disable Lock Screen (0 = Enabled, 1 = Disabled)
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Personalization", "NoLockScreen", "1");
            textBox_OutputWindow.AppendText("Disable Lock Screen: " + returnvalue + Environment.NewLine);

            // Disable Automatic Folder Type Discovery
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell", "FolderType", "NotSpecified");
            textBox_OutputWindow.AppendText("Disable Folder Type Guessing: " + returnvalue + Environment.NewLine);

            // Hide "Gallery" from File Explorer Sidebar (Windows 11)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Classes\CLSID\{e88865ad-085e-4da3-aa6a-b5139483011c}", "System.IsPinnedToNameSpaceTree", "0");

            // Hide "Home" (Quick Access) from Sidebar
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Classes\CLSID\{f8743c1a-aa48-44d5-8f6e-695a75b058a9}", "System.IsPinnedToNameSpaceTree", "0");

            textBox_OutputWindow.AppendText("Cleaned up File Explorer Sidebar: " + returnvalue + Environment.NewLine);

            // 1 = Show all folders, 0 = Hide non-pinned folders
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "NavPaneShowAllFolders", "1");
            textBox_OutputWindow.AppendText("Show All Folders in Sidebar: " + returnvalue + Environment.NewLine);

            // Disable Sticky Keys Popup
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Accessibility\StickyKeys", "Flags", "506");
           
            // Disable Filter Keys Popup
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Control Panel\Accessibility\Keyboard Response", "Flags", "122");

            // Disable "Search for app in Store" for unknown files
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "NoUseStoreOpenWith", "1");
            textBox_OutputWindow.AppendText("Disable Store for Unknown Files: " + returnvalue + Environment.NewLine);

            // Hide 3D Objects
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{31C0DD25-9139-4ed8-1054-466F031256A2}\PropertyBag", "ThisPCPolicy", "Hide");

            // Hide Music
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{a0c69a99-21c2-4671-8703-7934162fcf1d}\PropertyBag", "ThisPCPolicy", "Hide");

            // Hide Videos
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{f86fa3ab-70d2-4fc7-9c99-1055e597afc4}\PropertyBag", "ThisPCPolicy", "Hide");

            // Hide Pictures
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{0ddd015d-b06c-45d5-8c4c-f59713854639}\PropertyBag", "ThisPCPolicy", "Hide");

            textBox_OutputWindow.AppendText("Cleaned This PC Folders: " + returnvalue + Environment.NewLine);

            // Disable Built-in CD/DVD Burning Features
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoCDBurning", "1");
            textBox_OutputWindow.AppendText("Disable CD Burning: " + returnvalue + Environment.NewLine);

            // Disable Aero Shake (0 = Disabled, 1 = Enabled)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "DisallowShaking", "1");
            textBox_OutputWindow.AppendText("Disable Aero Shake: " + returnvalue + Environment.NewLine);

            // Re-enabling the engine for the old Photo Viewer
            string photoViewerPath = @"Software\Classes\Applications\photoviewer.dll\shell\open";
            Regedit_SetValue(Registry.CurrentUser, photoViewerPath + @"\command", "", "rundll32.exe \"%ProgramFiles%\\Windows Photo Viewer\\PhotoViewer.dll\", ImageView_Fullscreen %1");

            // Disable 'Enhanced' Search Mode (0 = Classic, 1 = Enhanced)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Search\Flighting\IndexSearch", "Enabled", "0");
            textBox_OutputWindow.AppendText("Classic Search Mode: " + returnvalue + Environment.NewLine);

            
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", "0");
            textBox_OutputWindow.AppendText("Disable UserAccount Promt 1: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", "0");
            textBox_OutputWindow.AppendText("Disable UserAccount Promt 2: " + returnvalue + Environment.NewLine);


            Console.WriteLine("Restart the computer to apply the changes");
        }

        

        private void button_EnableAdvancedNavigation_Click(object sender, EventArgs e)
        {
            textBox_OutputWindow.Clear();
            string returnvalue;

            // Show File Extensions (0 = Show, 1 = Hide)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", "0");
            textBox_OutputWindow.AppendText("Show File Extensions: " + returnvalue + Environment.NewLine);

            // Show Full Path in Title Bar (1 = Show, 0 = Hide)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "FullPath", "1");
            textBox_OutputWindow.AppendText("Show Full Path in Explorer: " + returnvalue + Environment.NewLine);

            // Enable Verbose Status Messages
            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "verbosestatus", "1");
            textBox_OutputWindow.AppendText("Enable Verbose Status: " + returnvalue + Environment.NewLine);

            // Launch Folder Windows in a Separate Process
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "SeparateProcess", "1");
            textBox_OutputWindow.AppendText("Separate Folder Processes: " + returnvalue + Environment.NewLine);

            // Show Protected Operating System Files (1 = Show, 0 = Hide)
            // Note: Logic is inverted for this specific key
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSuperHidden", "1");
            textBox_OutputWindow.AppendText("Show System Files: " + returnvalue + Environment.NewLine);

            // 1 = Enabled (Expand to open folder), 0 = Disabled
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "NavPaneExpandToSmart", "1");
            textBox_OutputWindow.AppendText("Enable Sidebar Auto-Expand: " + returnvalue + Environment.NewLine);

            // Show full path in the title bar (Classical behavior)
            returnvalue = Regedit_SetValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\CabinetState", "FullPath", "1");
            textBox_OutputWindow.AppendText("Explorer Classic Pathing: " + returnvalue + Environment.NewLine);

            // Define the base path for the shell command
            string takeOwnPath = @"Software\Classes\*\shell\runas";

            // Add the Menu Label and Icon
            Regedit_SetValue(Registry.CurrentUser, takeOwnPath, "", "Take Ownership");
            Regedit_SetValue(Registry.CurrentUser, takeOwnPath, "HasLUAShield", "");
            Regedit_SetValue(Registry.CurrentUser, takeOwnPath, "NoWorkingDirectory", "");

            // The command that performs the ownership change and permission grant
            Regedit_SetValue(Registry.CurrentUser, takeOwnPath + @"\command", "", "cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F");
            Regedit_SetValue(Registry.CurrentUser, takeOwnPath + @"\command", "IsolatedCommand", "cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F");

            string takeOwnFolderPath = @"Software\Classes\Directory\shell\runas";

            Regedit_SetValue(Registry.CurrentUser, takeOwnFolderPath, "", "Take Ownership");
            Regedit_SetValue(Registry.CurrentUser, takeOwnFolderPath, "HasLUAShield", "");

            // Note: /r is added here for recursive ownership of subfolders
            Regedit_SetValue(Registry.CurrentUser, takeOwnFolderPath + @"\command", "", "cmd.exe /c takeown /f \"%1\" /r /d y && icacls \"%1\" /grant administrators:F /t");

            Console.WriteLine("Restart the computer to apply the changes");
        }

        private void button_DisableWinUpdateAndDefender_Click(object sender, EventArgs e)
        {
            textBox_OutputWindow.Clear();
            string returnvalue;

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoUpdate", "1");
            textBox_OutputWindow.AppendText("Disable Automatic updates: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\wuauserv", "Start", "4");
            textBox_OutputWindow.AppendText("Disable Update Service: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", "1");
            textBox_OutputWindow.AppendText("Disable Windows Defender: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection\rtpPath", "DisableRealtimeMonitoring", "1");
            textBox_OutputWindow.AppendText("Disable RealTime Monitoring: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection\rtpPath", "DisableBehaviorMonitoring", "1");
            textBox_OutputWindow.AppendText("Disable Behavior Monitoring: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection\rtpPath", "DisableOnAccessProtection", "1");
            textBox_OutputWindow.AppendText("Disable OnAccess Protection: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection\rtpPath", "DisableScanOnRealtimeEnable", "1");
            textBox_OutputWindow.AppendText("Disable ScanOnRealtime Monitoring: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", "SmartScreenEnabled", "Off");
            textBox_OutputWindow.AppendText("Disable SmartScreen Monitoring: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "EnableSmartScreen", "0");
            textBox_OutputWindow.AppendText("Disable SmartScreen Monitoring: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender Security Center\Notifications", "DisableNotifications", "1");
            textBox_OutputWindow.AppendText("Disable SmartScreen Monitoring: " + returnvalue + Environment.NewLine);

            Console.WriteLine("Restart the computer to apply the changes");

        }

        private void button_EnableWinUpdateAndDefender_Click(object sender, EventArgs e)
        {
            textBox_OutputWindow.Clear();
            string returnvalue;

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoUpdate", "0");
            textBox_OutputWindow.AppendText("Enable Automatic updates: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\wuauserv", "Start", "2");
            textBox_OutputWindow.AppendText("Enable Update Service: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", "0");
            textBox_OutputWindow.AppendText("Enable Windows Defender: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection\rtpPath", "DisableRealtimeMonitoring", "0");
            textBox_OutputWindow.AppendText("Enable RealTime Monitoring: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection\rtpPath", "DisableBehaviorMonitoring", "0");
            textBox_OutputWindow.AppendText("Enable Behavior Monitoring: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection\rtpPath", "DisableOnAccessProtection", "0");
            textBox_OutputWindow.AppendText("Enable OnAccess Protection: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection\rtpPath", "DisableScanOnRealtimeEnable", "0");
            textBox_OutputWindow.AppendText("Enable ScanOnRealtime Monitoring: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", "SmartScreenEnabled", "On");
            textBox_OutputWindow.AppendText("Enable SmartScreen Monitoring: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "EnableSmartScreen", "1");
            textBox_OutputWindow.AppendText("Enable SmartScreen Monitoring: " + returnvalue + Environment.NewLine);

            returnvalue = Regedit_SetValue(Registry.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender Security Center\Notifications", "DisableNotifications", "0");
            textBox_OutputWindow.AppendText("Enable SmartScreen Monitoring: " + returnvalue + Environment.NewLine);

            Console.WriteLine("Restart the computer to apply the changes");
        }

        public void RestartExplorer()
        {
            try
            {
                textBox_OutputWindow.AppendText("Restarting Explorer to apply changes..." + Environment.NewLine);

                foreach (System.Diagnostics.Process exe in System.Diagnostics.Process.GetProcessesByName("explorer"))
                {
                    exe.Kill();
                    exe.WaitForExit(); // Ensure it is fully closed
                }
            }
            catch (Exception ex)
            {
                textBox_OutputWindow.AppendText("Error restarting explorer: " + ex.Message + Environment.NewLine);
            }
        }

        private void toolStripStatusLabelChangelog_Click(object sender, EventArgs e)
        {
            var messageLines = new string[]
    {

        "v 1.1 - xxxx",
        "- Change Nr 1",
        "- Change Nr 2",
        "- Change Nr 3",
        "",

        "v 1.0 - 31/12-2025",
        "- Initial Release",

        "",
     };

            MessageBox.Show(string.Join(Environment.NewLine, messageLines), "ChangeLog");
        }
    }
}
