using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmniTools
{
    internal static class Program
    {
        // Version locale de l’application
        private const string CurrentVersion = "0.6";
        // Langue courante
        private const string CurrentLanguage = "FR-fr";

        // URL où se trouve la dernière version sous forme de texte (par ex. "0.7")
        private const string VersionUrl = "https://raw.githubusercontent.com/danbenba/OmniTools/refs/heads/project/version";
        
        // URL pointant vers le nouvel exécutable (fichier .exe) à télécharger
        // À adapter selon votre hébergement (GitHub Releases, serveur privé, etc.)
        private const string DownloadExeUrl = "https://github.com/danbenba/OmniTools/releases/download/lasted/OmniTools.exe";
        
        // URL de la page release (si vous voulez rediriger l’utilisateur en cas d’erreur ou autre)
        private const string ReleaseUrl = "https://github.com/danbenba/OmniTools/releases/latest";

        // URL pour la veriffication de la connection internet
        private const string CheckURL = "https://www.google.com";

        // Expose également la version et la langue en public
        public const string Version = CurrentVersion;
        public const string Language = CurrentLanguage;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Lance la SplashForm
            Application.Run(new SplashForm());
        }

        /// <summary>
        /// Vérifie s’il existe une nouvelle version en comparant avec un fichier distant.
        /// Si oui, propose de télécharger la nouvelle version via un message personnalisé.
        /// </summary>
        public static async Task<bool> CheckForUpdates()
        {
            try
            {
                using HttpClient client = new HttpClient();
                string latestVersion = await client.GetStringAsync(VersionUrl);
                latestVersion = latestVersion.Trim();

                if (latestVersion == CurrentVersion)
                {
                    // Déjà à jour
                    return true;
                }
                else
                {
                    // Nouvelle version trouvée
                    using (var form = new UpdateForm(latestVersion))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            // L’utilisateur a cliqué sur "Mettre à jour"
                            string currentExePath = Application.ExecutablePath;

                            // Ouvrir la fenêtre de progression
                            using (var frmUpdate = new UpdateDownloadForm(DownloadExeUrl, currentExePath))
                            {
                                frmUpdate.ShowDialog();
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de la vérification des mises à jour : {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return true;
            }
        }



        /// <summary>
        /// Vérifie si une URL est joignable en envoyant une requête HEAD.
        /// </summary>
        public static async Task<bool> IsUrlReachable(string url)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Vérifie la connexion Internet et affiche un MessageBox si aucune connexion n’est détectée.
        /// </summary>
        public static async Task CheckInternetAndNotifyAsync()
        {
            bool isConnected = await IsInternetConnectionAvailable();
            if (!isConnected)
            {
                MessageBox.Show(
                    "Aucune connexion Internet détectée. Veuillez vérifier votre connexion.",
                    "Connexion Internet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>
        /// Vérifie s’il y a une connexion internet (ping d’un site fiable).
        /// </summary>
        public static async Task<bool> IsInternetConnectionAvailable()
        {
            try
            {
                using var client = new HttpClient();
                using var response = await client.GetAsync(CheckURL);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Télécharge le nouvel exécutable depuis <paramref name="downloadUrl" />,
        /// remplace l’EXE actuel, puis lance la nouvelle version.
        /// </summary>
        private static async Task DownloadAndReplaceExe(string downloadUrl)
        {
            // Chemin de l’EXE courant
            string currentExePath = Application.ExecutablePath;
            string currentFolder = Path.GetDirectoryName(currentExePath);

            // On télécharge d’abord le nouveau fichier sous un nom temporaire
            string tempExeName = "OmniTools_Update.exe";
            string tempExePath = Path.Combine(currentFolder, tempExeName);

            try
            {
                using HttpClient client = new HttpClient();
                byte[] newExeBytes = await client.GetByteArrayAsync(downloadUrl);

                // On écrit le nouveau fichier à côté de l’EXE actuel
                await File.WriteAllBytesAsync(tempExePath, newExeBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du téléchargement de la mise à jour : {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Génération d’un script .bat pour remplacer l’EXE actuel
            // (Impossible de supprimer ou de remplacer un fichier en cours d’utilisation)
            // On va donc lancer ce script .bat qui va :
            // 1) Attendre 1-2 secondes
            // 2) Supprimer l’EXE actuel
            // 3) Renommer le nouveau fichier
            // 4) Lancer le nouveau .exe
            // 5) Supprimer le .bat
            string batFilePath = Path.Combine(Path.GetTempPath(), "OmniTools_Updater.bat");

            // Petite temporisation via `ping 127.0.0.1 -n 2`
            // Le `-n 2` attend environ 2 secondes.
            string batContent = $@"
            @echo off
            ping 127.0.0.1 -n 2 > nul
            del ""{currentExePath}""
            move ""{tempExePath}"" ""{currentExePath}""
            start """" ""{currentExePath}""
            del ""%~f0""
            ";
            File.WriteAllText(batFilePath, batContent);

            // On lance le script
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = batFilePath,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du démarrage de la nouvelle version : {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Ferme l’application en cours
            Application.Exit();
        }
    }
}
