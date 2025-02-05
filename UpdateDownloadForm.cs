using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmniTools
{
    public class UpdateDownloadForm : Form
    {
        private Label lblStatus;
        private ProgressBar progressBar;
        private string _downloadUrl;
        private string _currentExePath;
        private string _tempExePath;
        private WebClient _webClient; // Stocke le WebClient pour éviter une disposition prématurée

        // Constructeur : on reçoit l’URL de téléchargement et le chemin de l’EXE actuel
        public UpdateDownloadForm(string downloadUrl, string currentExePath)
        {
            _downloadUrl = downloadUrl;
            _currentExePath = currentExePath;
            _tempExePath = Path.Combine(Path.GetDirectoryName(_currentExePath), "OmniTools_Update.exe");

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Mise à jour en cours";
            this.Size = new Size(400, 150);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            this.Controls.Add(mainPanel);

            lblStatus = new Label
            {
                Text = "Téléchargement en cours…",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12, FontStyle.Regular)
            };
            mainPanel.Controls.Add(lblStatus, 0, 0);

            progressBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };
            mainPanel.Controls.Add(progressBar, 0, 1);

            // Démarrage du téléchargement à l’affichage du formulaire
            this.Load += UpdateDownloadForm_Load;
        }

        /// <summary>
        /// Lance le téléchargement du nouveau fichier quand la fenêtre apparaît.
        /// </summary>
        private void UpdateDownloadForm_Load(object sender, EventArgs e)
        {
            StartDownload();
        }

        private void StartDownload()
        {
            try
            {
                // Supprime l’ancien fichier temporaire, s’il existe déjà
                if (File.Exists(_tempExePath))
                {
                    File.Delete(_tempExePath);
                }

                _webClient = new WebClient();
                _webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                _webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

                // Lance le téléchargement en mode asynchrone
                _webClient.DownloadFileAsync(new Uri(_downloadUrl), _tempExePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du téléchargement : {ex.Message}",
                    "Erreur de mise à jour", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblStatus.Text = $"Téléchargement en cours… {e.ProgressPercentage}%";
        }

        /// <summary>
        /// Une fois le téléchargement terminé, met à jour l’interface,
        /// attend 2 secondes sans bloquer l’UI puis lance le script de mise à jour.
        /// </summary>
        private async void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Téléchargement annulé.", "Mise à jour", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            if (e.Error != null)
            {
                MessageBox.Show($"Erreur lors du téléchargement : {e.Error.Message}",
                    "Mise à jour", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Téléchargement terminé
            progressBar.Value = 100;
            lblStatus.Text = "Extraction des fichiers…";

            // Attente asynchrone de 2 secondes pour laisser le temps au label de se mettre à jour
            await Task.Delay(2000);

            // Génération et exécution du script .bat pour remplacer l’EXE actuel
            CreateAndRunUpdateScript();

            // Ferme la fenêtre et termine l’application
            this.Close();
            Application.Exit();
        }

        /// <summary>
        /// Crée un fichier .bat qui, après quelques secondes :
        /// 1) Supprime l’EXE actuel
        /// 2) Renomme le nouveau fichier en OmniTools.exe
        /// 3) Lance la nouvelle version
        /// 4) Supprime le .bat
        /// </summary>
        private void CreateAndRunUpdateScript()
        {
            string batFilePath = Path.Combine(Path.GetTempPath(), "OmniTools_Updater.bat");
            string batContent = $@"
@echo off
ping 127.0.0.1 -n 2 > nul
del ""{_currentExePath}""
move ""{_tempExePath}"" ""{_currentExePath}""
start """" ""{_currentExePath}""
del ""%~f0""
";

            try
            {
                File.WriteAllText(batFilePath, batContent);

                var psi = new ProcessStartInfo
                {
                    FileName = batFilePath,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible de remplacer l’application : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
