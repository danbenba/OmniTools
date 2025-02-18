using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OmniTools
{
    /// <summary>
    /// Classe de configuration pour les scripts utilisés dans l'application.
    /// </summary>
    public static class ScriptConfig
    {
        // Classe interne décrivant un script
        public class ScriptItem
        {
            public string DisplayName { get; set; }      // Nom affiché dans l'interface (ComboBox)
            public string DownloadUrl { get; set; }      // URL de téléchargement
            public string LocalFileName { get; set; }    // Nom du fichier local
            public string DefaultArguments { get; set; } = ""; // Arguments par défaut à passer
            public bool IsEnabled { get; set; } = true;  
        }

        /// <summary>
        /// Liste de tous les scripts disponibles.
        /// </summary>
        public static List<ScriptItem> Scripts { get; private set; } = new List<ScriptItem>();

        // Constructeur statique : chargé une fois au démarrage de l'application
        static ScriptConfig()
        {
            LoadScripts();
        }

        /// <summary>
        /// Charge les scripts depuis un fichier de configuration JSON local, sinon charge une liste intégrée.
        /// </summary>
        private static void LoadScripts()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "playloads.json");
            if (File.Exists(configPath))
            {
                try
                {
                    string json = File.ReadAllText(configPath);
                    Scripts = JsonSerializer.Deserialize<List<ScriptItem>>(json);
                    Logger.LogSuccess("Scripts loaded successfully from playloads.json.");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading scripts from playloads.json: {ex.Message}");
                    LoadDefaultScripts();
                }
            }
            else
            {
                LoadDefaultScripts();
            }
        }

        /// <summary>
        /// Charge une liste par défaut de scripts intégrée dans le code.
        /// Appelée si le fichier scripts.json n’est pas présent ou pose problème.
        /// </summary>
        private static void LoadDefaultScripts()
        {
            Scripts = new List<ScriptItem>
            {
                new ScriptItem
                {
                    DisplayName = "--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-- ",
                    DownloadUrl = "https://raw.githubusercontent.com/danbenba/OmniTools/refs/heads/project/webFiles/Playloads/other.cmd",
                    LocalFileName = "Disabled.OmniTools.bat",
                    IsEnabled = false  // Désactiver cet élément
                },

                // Installers
                new ScriptItem
                {
                    DisplayName = "Microsoft Installer (Win 1.01 - 11)",
                    DownloadUrl = "https://github.com/danbenba/MediaWinDownloader/releases/download/lasted/MediaWinDownloader.exe",
                    LocalFileName = "MediaWinDownloader.OmniTools.exe"
                },
                new ScriptItem
                {
                    DisplayName = "Office Installer (2016 - 2024)",
                    DownloadUrl = "https://github.com/danbenba/OmniTools.OfficeInstaller/raw/refs/heads/project/Deploy-Office.exe",
                    LocalFileName = "OfficeInstaller.OmniTools.exe"
                },
                new ScriptItem
                {
                    DisplayName = "Edition Changer (Windows - Office)",
                    DownloadUrl = "https://github.com/danbenba/OmniTools.EditionChanger/raw/refs/heads/project/EditionChanger.exe",
                    LocalFileName = "EditionChanger.OmniTools.exe"
                },
                new ScriptItem
                {
                    DisplayName = "UniGetUI (Package Installer)",
                    DownloadUrl = "https://github.com/marticliment/UniGetUI/releases/download/3.1.6/UniGetUI.Installer.exe",
                    LocalFileName = "UniGetUI.OmniTools.exe"
                },
                new ScriptItem
                {
                    DisplayName = "Rufus (4.6.2208)",
                    DownloadUrl = "https://github.com/pbatard/rufus/releases/download/v4.6/rufus-4.6p.exe",
                    LocalFileName = "Rufus.OmniTools.exe"
                },

                // Activators
                new ScriptItem
                {
                    DisplayName = "--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-- ",
                    DownloadUrl = "https://raw.githubusercontent.com/danbenba/OmniTools/refs/heads/project/webFiles/Playloads/other.cmd",
                    LocalFileName = "Disabled.OmniTools.bat",
                    IsEnabled = false  // Désactiver cet élément
                },
                new ScriptItem
                {
                    DisplayName = "Activators (Windows / Office)",
                    DownloadUrl = "https://github.com/danbenba/OmniTools.Activators/raw/refs/heads/project/WindowsActivators.exe",
                    LocalFileName = "Microsoft-Activators.OmniTools.exe"
                },
                new ScriptItem
                {
                    DisplayName = "Windows KeyGen (Win98 - WinXP)",
                    DownloadUrl = "https://github.com/Endermanch/XPKeygen/releases/download/v2.7/XPKeygen.exe",
                    LocalFileName = "XPKeygen.OmniTools.exe"
                },
                new ScriptItem
                {
                    DisplayName = "--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-- ",
                    DownloadUrl = "https://raw.githubusercontent.com/danbenba/OmniTools/refs/heads/project/webFiles/Playloads/other.cmd",
                    LocalFileName = "Disabled.OmniTools.bat",
                    IsEnabled = false  // Désactiver cet élément
                },
                
                // Tweak, UiGetUI and Troubleshoot
                new ScriptItem
                {
                    DisplayName = "Windows Tweak",
                    DownloadUrl = "https://github.com/danbenba/OmniTools.WindowsTweaks/raw/refs/heads/project/Windows%20Tweaker.exe",
                    LocalFileName = "WindowsTweaker.OmniTools.exe"
                },
                new ScriptItem
                {
                    DisplayName = "Defender Tool",
                    DownloadUrl = "https://github.com/danbenba/DefenderTool/raw/refs/heads/project/publish/DefenderTool.exe",
                    LocalFileName = "DefenderTool.OmniTools.exe"
                },
                new ScriptItem
                {
                    DisplayName = "Driver Installer",
                    DownloadUrl = "https://github.com/danbenba/OmniTools.DriverInstaller/releases/download/lasted/DriverInstaller.exe",
                    LocalFileName = "DriverInstaller.OmniTools.exe"
                },
                new ScriptItem
                {
                    DisplayName = "Troubleshoot",
                    DownloadUrl = "https://github.com/danbenba/OmniTools.Troubleshoot/releases/download/lasted/RepairKit.exe",
                    LocalFileName = "RepairKit.OmniTools.exe",
                    DefaultArguments = ""
                },
                new ScriptItem
                {
                    DisplayName = "--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-- ",
                    DownloadUrl = "https://raw.githubusercontent.com/danbenba/OmniTools/refs/heads/project/webFiles/Playloads/other.cmd",
                    LocalFileName = "Disabled.OmniTools.bat",
                    IsEnabled = false  // Désactiver cet élément
                },

                //Other Tools
                new ScriptItem
                {
                    DisplayName = "Command Prompt (TrustedInstaller)",
                    DownloadUrl = "https://github.com/danbenba/ElevationExploit/releases/download/0.4/elevation.exe",
                    LocalFileName = "ElevationExploit.OmniTools.exe",
                    DefaultArguments = "cmd.exe -t"
                },
                new ScriptItem
                {
                    DisplayName = "Windows Update Blocker",
                    DownloadUrl = "https://github.com/danbenba/OmniTools.WinUpdatesBlocker/raw/refs/heads/project/Wub_x64.exe",
                    LocalFileName = "WinUpdateBLocker.OmniTools.exe",
                    DefaultArguments = ""
                },
                new ScriptItem
                {
                    DisplayName = "Other Tool (Optimizer)",
                    DownloadUrl = "https://github.com/hellzerg/optimizer/releases/download/16.7/Optimizer-16.7.exe",
                    LocalFileName = "Optimizer.OmniTools.exe",
                },
                new ScriptItem
                {
                    DisplayName = "--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-- ",
                    DownloadUrl = "https://raw.githubusercontent.com/danbenba/OmniTools/refs/heads/project/webFiles/Playloads/other.cmd",
                    LocalFileName = "Disabled.OmniTools.bat",
                    IsEnabled = false  // Désactiver cet élément
                },
            };
            Logger.CoreLog("All Add-On Loaded !");
        }

        /// <summary>
        /// Recherche un script dans la liste Scripts en fonction de son DisplayName.
        /// </summary>
        public static ScriptItem GetScriptByDisplayName(string displayName)
        {
            return Scripts.Find(script => script.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
