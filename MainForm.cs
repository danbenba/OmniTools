using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.VisualBasic.Logging;
using ScriptItem = OmniTools.ScriptConfig.ScriptItem;

namespace OmniTools
{
    public class MainForm : Form
    {
        private Label labelTitle;
        private GroupBox groupBoxActions;
        private ComboBox comboBoxScripts;
        private Button btnExecute;
        private CheckBox checkBoxRestart;
        private CheckBox checkBoxDisableDefender;
        private Label labelVersion;
        private Button btnSystemInfo;
        private Button btnAbout;
        private Button btnExit;
        internal RichTextBox richTextBoxLogs;  // accessible dans Logger

        // Constantes et variables pour la barre de progression
        private const int ProgressBarWidth = 27; 
        private int lastProgress = 0;

        string systemVersion = Environment.OSVersion.VersionString;
        string dotNetVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        string currentVersion = Program.Version;

        // Chemin vers le dossier temporaire
        private readonly string tempPath = Path.GetTempPath();

        public MainForm()
        {
            InitializeComponent();

            // Lier l'instance de MainForm au Logger
            Logger.MainFormInstance = this;

            // Charger l'icône depuis les ressources
            LoadIconFromResources();
        }

        private void InitializeComponent()
        {
            // Configuration initiale de la fenêtre
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.FormClosing += MainForm_FormClosing;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Text = "OmniTools";

            // Création des contrôles
            this.labelTitle = new Label();
            this.groupBoxActions = new GroupBox();
            this.comboBoxScripts = new ComboBox();
            this.btnExecute = new Button();
            this.btnExit = new Button();
            this.checkBoxDisableDefender = new CheckBox();
            this.checkBoxRestart = new CheckBox();
            this.labelVersion = new Label();
            this.btnSystemInfo = new Button();
            this.btnAbout = new Button();
            this.richTextBoxLogs = new RichTextBox();

            // Label Titre
            this.labelTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.labelTitle.Location = new Point(12, 9);
            this.labelTitle.Size = new Size(400, 40);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Text = "OmniTools";

            // groupBoxActions (regroupe les boutons et contrôles liés aux actions)
            this.groupBoxActions.Text = "Actions";
            this.groupBoxActions.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.groupBoxActions.Location = new Point(12, 60);
            this.groupBoxActions.Size = new Size(510, 150);

            // comboBoxScripts
            this.comboBoxScripts.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxScripts.Font = new Font("Segoe UI", 12F);
            this.comboBoxScripts.Location = new Point(15, 30);
            this.comboBoxScripts.Size = new Size(280, 29);
            this.comboBoxScripts.Name = "comboBoxScripts";
            this.comboBoxScripts.SelectedIndex = -1;
            this.comboBoxScripts.SelectedIndexChanged += ComboBoxScripts_SelectedIndexChanged;

            // btnExecute
            this.btnExecute.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnExecute.Text = "Execute";
            this.btnExecute.Location = new Point(310, 27);
            this.btnExecute.Size = new Size(110, 35);
            this.btnExecute.Click += BtnExecute_Click;
            this.btnExecute.Enabled = false;

            // btnExit
            this.btnExit.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            this.btnExit.Text = "Exit";
            this.btnExit.Location = new Point(424, 27);
            this.btnExit.Size = new Size(80, 35);
            this.btnExit.Click += BtnExit_Click;

            // checkBoxRestart
            this.checkBoxRestart.AutoSize = true;
            this.checkBoxRestart.Font = new Font("Segoe UI", 10F);
            this.checkBoxRestart.Text = "Restart PC after execution";
            this.checkBoxRestart.Location = new Point(15, 70);
            this.checkBoxRestart.Size = new Size(200, 23);

            // checkBoxDisableDefender
            this.checkBoxDisableDefender.AutoSize = true;
            this.checkBoxDisableDefender.Font = new Font("Segoe UI", 10F);
            this.checkBoxDisableDefender.Text = "Disable Windows Defender before execution";
            this.checkBoxDisableDefender.Location = new Point(15, 100);
            this.checkBoxDisableDefender.Size = new Size(300, 23);

            // btnSystemInfo
            this.btnSystemInfo.Font = new Font("Segoe UI", 10F);
            this.btnSystemInfo.Text = "System Info";
            this.btnSystemInfo.Location = new Point(310, 65);
            this.btnSystemInfo.Size = new Size(110, 35);
            this.btnSystemInfo.Click += BtnSystemInfo_Click;

            // labelVersion
            this.labelVersion.Font = new Font("Segoe UI", 9F);
            this.labelVersion.Location = new Point(12, 375);
            this.labelVersion.Size = new Size(400, 20);

            // btnAbout
            this.btnAbout.Font = new Font("Segoe UI", 10F);
            this.btnAbout.Text = "About";
            this.btnAbout.Location = new Point(420, 9);
            this.btnAbout.Size = new Size(90, 35);
            this.btnAbout.Click += BtnAbout_Click;

            // richTextBoxLogs
            this.richTextBoxLogs.Font = new Font("Consolas", 10F);
            this.richTextBoxLogs.Location = new Point(12, 220);
            this.richTextBoxLogs.Size = new Size(510, 150);
            this.richTextBoxLogs.ReadOnly = true;
            this.richTextBoxLogs.BackColor = Color.White;

            // Ajout des contrôles au groupBox
            this.groupBoxActions.Controls.Add(this.comboBoxScripts);
            this.groupBoxActions.Controls.Add(this.btnExecute);
            this.groupBoxActions.Controls.Add(this.btnExit);
            this.groupBoxActions.Controls.Add(this.checkBoxRestart);
            this.groupBoxActions.Controls.Add(this.btnSystemInfo);
            this.groupBoxActions.Controls.Add(this.checkBoxDisableDefender);

            // Ajout des contrôles à la Form
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.groupBoxActions);
            this.Controls.Add(this.richTextBoxLogs);

            // Réglage de la taille de la fenêtre
            this.ClientSize = new Size(534, 401);

            // Événement lors du chargement du Form
            this.Load += MainForm_Load;
        }

        /// <summary>
        /// Tente de charger une icône depuis le dossier temporaire (icon.ico).
        /// </summary>
        public void LoadIconFromResources()
        {
            try
            {
                // Obtenez l'assembly courant
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Spécifiez le nom complet de la ressource
                string resourceName = "OmniTools.Resources.images.icon.png";

                // Obtenez le flux de la ressource
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        // Chargez l'image depuis le flux
                        using (Bitmap bitmap = new Bitmap(stream))
                        {
                            this.Icon = Icon.FromHandle(bitmap.GetHicon());
                        }
                    }
                    else
                    {
                        // Gérer le cas où la ressource n'est pas trouvée
                        MessageBox.Show($"La ressource {resourceName} n'a pas été trouvée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading icon: {ex.Message}");
            }
        }

        /// <summary>
        /// Événement déclenché lors du chargement de la fenêtre principale.
        /// Prépare le ComboBox des scripts, vérifie si l'exécution est en mode admin, etc.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            string dotNetVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

            // Charger la liste des scripts dans la comboBox
            comboBoxScripts.DataSource = ScriptConfig.Scripts;
            comboBoxScripts.DisplayMember = "DisplayName";
            comboBoxScripts.SelectedIndex = -1;
            btnExecute.Enabled = false;

            // Configurer le ComboBox pour dessiner les éléments personnalisés
            comboBoxScripts.DrawMode = DrawMode.OwnerDrawFixed;
            comboBoxScripts.DrawItem += ComboBoxScripts_DrawItem;

            // Vérifier si l'application tourne en mode administrateur
            bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator);

            string adminStatus = isAdmin 
                ? "                                                                                      Exécuté en tant qu'administrateur" 
                : "                                                                             Non exécuté en tant qu'administrateur";
            Color statusColor = isAdmin ? Color.Green : Color.Red;

            // Afficher la version .NET
            labelVersion.Text = $"{dotNetVersion}";
            labelVersion.ForeColor = Color.Black;
            labelVersion.AutoSize = true;

            // Création d'un label pour afficher le statut administrateur
            Label adminLabel = new Label();
            adminLabel.Text = adminStatus;
            adminLabel.ForeColor = statusColor;
            adminLabel.AutoSize = true;

            // Positionnement de adminLabel
            int spacing = 10;
            adminLabel.Location = new Point(labelVersion.Location.X + labelVersion.Width + spacing, labelVersion.Location.Y);

            // Vérifier que le label rentre dans la fenêtre
            int maxRight = this.ClientSize.Width - 20; 
            int totalWidth = adminLabel.Location.X + adminLabel.Width;
            if (totalWidth > maxRight)
            {
                // Si ça dépasse, on réduit la police
                using (Graphics g = this.CreateGraphics())
                {
                    while (g.MeasureString(adminLabel.Text, adminLabel.Font).Width > (maxRight - adminLabel.Location.X) 
                           && adminLabel.Font.Size > 6)
                    {
                        adminLabel.Font = new Font(adminLabel.Font.FontFamily, adminLabel.Font.Size - 1, adminLabel.Font.Style);
                    }
                }
            }
            this.Controls.Add(adminLabel);

            // S'il n’est pas admin, ajouter un message d'avertissement
            ToolTip toolTip = new ToolTip();
            if (!isAdmin)
            {
                Logger.LogWarning("Les droits d'administrateur sont nécessaires.");
                toolTip.SetToolTip(adminLabel, "Vous n'êtes pas en mode administrateur.\nCliquez ici pour tenter d'élever vos privilèges.");
                adminLabel.Cursor = Cursors.Hand;
                adminLabel.Click += LblPrivilege_Click;
            }

            // Titre principal
            labelTitle.Text = $"OmniTools v{currentVersion}";

            CheckInternetAndNotifyAsync();
        }

        /// <summary>
        /// Dessine les éléments du ComboBox en grisant ceux qui sont désactivés.
        /// </summary>
        private void ComboBoxScripts_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0)
                return;

            var script = (ScriptItem)comboBoxScripts.Items[e.Index];
            Brush brush = script.IsEnabled ? Brushes.Black : Brushes.Gray;

            e.Graphics.DrawString(script.DisplayName, e.Font, brush, e.Bounds);
            e.DrawFocusRectangle();
        }

        /// <summary>
        /// Gestion du clic sur le label des privilèges : si l'utilisateur n'est pas administrateur,
        /// il est invité à relancer l'application en mode administrateur.
        /// </summary>
        public void LblPrivilege_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Vous n'êtes pas en mode administrateur.\nVoulez-vous relancer l'application en mode administrateur ?",
                "Élévation des privilèges",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    ProcessStartInfo procInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = Application.ExecutablePath,
                        Verb = "runas" // Demande d'élévation
                    };
                    Process.Start(procInfo);
                    Application.Exit();
                }
                catch
                {
                    MessageBox.Show("L'élévation des privilèges a échoué.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Vérifie la connexion Internet et affiche un message dans les logs si aucune connexion n'est détectée.
        /// </summary>
        public static async Task CheckInternetAndNotifyAsync()
        {
            bool isConnected = await Program.IsInternetConnectionAvailable();
            if (!isConnected)
            {
                Logger.LogWarning("Aucune connexion Internet détectée.");
                Logger.LogSuccess("Application démarrée et prête (avec ERREURS).");
            }
            else
            {
                // Message de log
                Logger.LogSuccess("Application démarrée et prête.");
            }
        }

        /// <summary>
        /// Événement appelé lorsqu’on change la sélection du ComboBox des scripts.
        /// Active ou désactive le bouton d'exécution en conséquence.
        /// </summary>
        private void ComboBoxScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedScript = comboBoxScripts.SelectedItem as ScriptConfig.ScriptItem;
            btnExecute.Enabled = selectedScript != null && selectedScript.IsEnabled;
        }

        /// <summary>
        /// Événement appelé quand on clique sur "Execute".
        /// Télécharge le script, désactive (si coché) Defender, exécute le script, puis réactive Defender et éventuellement redémarre.
        /// </summary>
        private async void BtnExecute_Click(object sender, EventArgs e)
        {
            var selectedScript = comboBoxScripts.SelectedItem as ScriptConfig.ScriptItem;
            if (selectedScript == null || !selectedScript.IsEnabled)
            {
                Logger.LogWarning("Ce script est désactivé et ne peut pas être exécuté.");
                return;
            }

            if (checkBoxDisableDefender.Checked)
            {
                Logger.LogInfo("Désactivation de Windows Defender avant l'exécution...");
                await ExecuteRegistryCommands("disable");
            }

            // Demander confirmation à l'utilisateur
            DialogResult dr = MessageBox.Show(
                $"Voulez-vous vraiment exécuter '{selectedScript.DisplayName}'?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dr == DialogResult.No)
            {
                Logger.LogInfo("Opération annulée par l'utilisateur.");
                return;
            }

            btnExecute.Enabled = false; // Empêcher les doublons

            string scriptLocalPath = Path.Combine(tempPath, selectedScript.LocalFileName);

            // 1) Télécharger le script avec une barre de progression
            bool downloadSuccess = await DownloadFileWithProgressAsync(selectedScript.DownloadUrl, scriptLocalPath);
            if (!downloadSuccess)
            {
                Logger.LogError($"Échec du téléchargement de '{selectedScript.DisplayName}'.");
                btnExecute.Enabled = true;
                return;
            }

            // 2) Exécuter le script
            bool scriptExecuted = await ExecuteScriptAsync(scriptLocalPath, selectedScript.DefaultArguments, selectedScript.DisplayName);
            if (!scriptExecuted)
            {
                Logger.LogError($"Échec de l'exécution de '{selectedScript.DisplayName}'.");
            }
            else
            {
            }

            // Si on avait coché la case "Disable Windows Defender before execution", on réactive
            if (checkBoxDisableDefender.Checked)
            {
                Logger.LogInfo("Réactivation de Windows Defender après l'exécution...");
                await ExecuteRegistryCommands("enable");
            }

            // Si on a coché "Restart PC after execution"
            if (checkBoxRestart.Checked)
            {
                Logger.LogInfo("Redémarrage du système initié...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "shutdown",
                    Arguments = "-r -t 2",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }

            btnExecute.Enabled = true;
        }

        /// <summary>
        /// Ferme l'application.
        /// </summary>
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Active ou désactive Windows Defender via des clés de registre.
        /// </summary>
        private async Task ExecuteRegistryCommands(string action)
        {
            string[] commands;
            if (action == "disable")
            {
                commands = new string[]
                {
                    "reg add \"HKLM\\Software\\Microsoft\\Windows Defender Security Center\\Notifications\" /v \"DisableNotifications\" /t REG_DWORD /d \"1\" /f",
                    "reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows Defender\" /v \"DisableAntiSpyware\" /t REG_DWORD /d \"1\" /f",
                    "reg add \"HKLM\\System\\CurrentControlSet\\Services\\WinDefend\" /v \"Start\" /t REG_DWORD /d \"4\" /f"
                };
            }
            else // enable
            {
                commands = new string[]
                {
                    "reg delete \"HKLM\\Software\\Policies\\Microsoft\\Windows Defender\" /f",
                    "reg add \"HKLM\\System\\CurrentControlSet\\Services\\WinDefend\" /v \"Start\" /t REG_DWORD /d \"2\" /f"
                };
            }

            foreach (var command in commands)
            {
                Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {command}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Logger.LogError(error);
                }
                else
                {
                    Logger.LogInfo(output);
                }
            }
        }

        /// <summary>
        /// Télécharge un fichier en montrant une barre de progression dans le RichTextBox.
        /// </summary>
        private async Task<bool> DownloadFileWithProgressAsync(string url, string destinationPath)
        {
            try
            {

                // Effacer les logs pour eviter les interferences
                Logger.Clear();

                // Defini le nom du script à exécuter
                var selectedScript = comboBoxScripts.SelectedItem as ScriptConfig.ScriptItem;

                Logger.CoreLog($"Selected: '{selectedScript.DisplayName}'");

                // Supprime toujours l'ancien fichier s'il existe
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath); // Supprime le fichier existant
                }

                // Si le fichier existe déjà, on n'a pas besoin de le re-télécharger
                if (File.Exists(destinationPath))
                {
                    UpdateProgressBar(100);
                    AddLog(" Done !\n", Color.Green, newLine: true);
                    return true;
                }

                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                long? totalBytes = response.Content.Headers.ContentLength;
                if (totalBytes == null)
                {
                    Logger.LogError("Impossible de récupérer la taille du fichier.");
                    return false;
                }

                using Stream contentStream = await response.Content.ReadAsStreamAsync();
                using FileStream fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);

                byte[] buffer = new byte[8192];
                long totalRead = 0;
                int bytesRead;

                // Préparation de la barre de progression
                string progressBarTemplate = "Downloading : [---------------------------] 0% ";
                AddLog(progressBarTemplate, Color.Blue, newLine: false);

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalRead += bytesRead;

                    int progress = (int)(totalRead * 100 / totalBytes.Value);
                    if (progress >= lastProgress + 2) // Mettre à jour toutes les 2%
                    {
                        UpdateProgressBar(progress);
                        lastProgress = progress;
                    }
                }

                // Mise à jour finale à 100%
                UpdateProgressBar(100);
                AddLog(" Done !\n", Color.Green, newLine: true);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Erreur lors du téléchargement : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Met à jour la barre de progression textuelle dans le RichTextBox.
        /// </summary>
        private void UpdateProgressBar(int progress)
        {
            int filledBars = (progress * ProgressBarWidth) / 100;
            string progressBar = new string('#', filledBars) + new string('-', ProgressBarWidth - filledBars);
            string progressLine = $"Downloading : [{progressBar}] {progress}% ";

            if (richTextBoxLogs.Lines.Length > 0)
            {
                int lastLineIndex = richTextBoxLogs.GetFirstCharIndexFromLine(richTextBoxLogs.Lines.Length - 1);
                richTextBoxLogs.Select(lastLineIndex, richTextBoxLogs.Lines.Last().Length);
                richTextBoxLogs.SelectedText = progressLine;
            }
        }

        /// <summary>
        /// Désactive Windows Defender via un script (si configuré).
        /// </summary>
        private async Task<bool> DisableDefenderAsync()
        {
            var disableScript = ScriptConfig.GetScriptByDisplayName("Disable All Security Mitigations");
            if (disableScript == null)
            {
                Logger.LogError("Script de désactivation non trouvé dans la configuration.");
                return false;
            }

            string scriptLocalPath = Path.Combine(tempPath, disableScript.LocalFileName);
            if (!File.Exists(scriptLocalPath))
            {
                Logger.LogError($"Le script '{disableScript.LocalFileName}' n'a pas été trouvé dans le répertoire temporaire.");
                return false;
            }

            Logger.LogInfo("Désactivation temporaire de Windows Defender...");
            bool success = await ExecuteScriptAsync(scriptLocalPath, disableScript.DefaultArguments, disableScript.DisplayName);
            if (success)
                Logger.LogSuccess("Windows Defender a été désactivé.");
            return success;
        }

        /// <summary>
        /// Réactive Windows Defender via un script (si configuré).
        /// </summary>
        private async Task<bool> EnableDefenderAsync()
        {
            var enableScript = ScriptConfig.GetScriptByDisplayName("Enable W-Defender");
            if (enableScript == null)
            {
                Logger.LogError("Script d'activation non trouvé dans la configuration.");
                return false;
            }

            string scriptLocalPath = Path.Combine(tempPath, enableScript.LocalFileName);
            if (!File.Exists(scriptLocalPath))
            {
                Logger.LogError($"Le script '{enableScript.LocalFileName}' n'a pas été trouvé dans le répertoire temporaire.");
                return false;
            }

            Logger.LogInfo("Réactivation de Windows Defender...");
            bool success = await ExecuteScriptAsync(scriptLocalPath, enableScript.DefaultArguments, enableScript.DisplayName);
            if (success)
                Logger.LogSuccess("Windows Defender a été réactivé.");
            return success;
        }

        /// <summary>
        /// Exécute le fichier ou script spécifié avec des arguments.
        /// </summary>
        private async Task<bool> ExecuteScriptAsync(string scriptPath, string arguments, string displayName)
        {
            try
            {
                Logger.LogInfo($"Starting...");
                Process process = new Process();

                if (Path.GetExtension(scriptPath).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    // Utilisation de "start" pour ouvrir le .bat dans une nouvelle fenêtre
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = $"/c start \"\" \"{scriptPath}\" {arguments}";
                }
                else
                {
                    // Pour les .exe ou autres types, on lance directement le fichier
                    process.StartInfo.FileName = scriptPath;
                    process.StartInfo.Arguments = arguments;
                }

                // IMPORTANT : Pour lancer le processus comme un double-clic, on active UseShellExecute
                process.StartInfo.UseShellExecute = true;
                // On laisse CreateNoWindow à false pour que la nouvelle fenêtre s'ouvre
                process.StartInfo.CreateNoWindow = false;

                // Démarrer le processus
                process.Start();

                // Optionnel : attendre la fin du processus de façon asynchrone (selon votre besoin)
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode == 0)
                {
                    Logger.LogSuccess($"'{displayName}' exécuté :)");
                    return true;
                }
                else
                {
                    Logger.LogError($"'{displayName}' a échoué avec le code de sortie {process.ExitCode}.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception lors de l'exécution de '{displayName}': {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Affiche la fenêtre d'information système personnalisée.
        /// </summary>
        private void BtnSystemInfo_Click(object sender, EventArgs e)
        {
            SystemInfoForm infoForm = new SystemInfoForm();
            infoForm.ShowDialog();
        }

        /// <summary>
        /// Affiche la fenêtre "À propos" (AboutForm).
        /// </summary>
        private void BtnAbout_Click(object sender, EventArgs e)
        {
            AboutFrom aboutForm = new AboutFrom();
            aboutForm.ShowDialog(); // Modal
        }

        /// <summary>
        /// Ferme l'application si l’utilisateur ferme le MainForm.
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Ajoute un message coloré dans la zone de logs (RichTextBox).
        /// </summary>
        public void AddLog(string message, Color color, bool newLine = true)
        {
            if (this.richTextBoxLogs.InvokeRequired)
            {
                this.richTextBoxLogs.Invoke(new Action(() => AddLog(message, color, newLine)));
            }
            else
            {
                int start = richTextBoxLogs.TextLength;
                if (newLine)
                {
                    richTextBoxLogs.AppendText(message.Trim() + "\n");
                }
                else
                {
                    richTextBoxLogs.AppendText(message + " ");
                }
                int end = richTextBoxLogs.TextLength;

                // Sélection de la zone pour lui appliquer la couleur
                richTextBoxLogs.Select(start, end - start);
                richTextBoxLogs.SelectionColor = color;

                // Désélection et défilement automatique
                richTextBoxLogs.SelectionLength = 0;
                richTextBoxLogs.SelectionStart = richTextBoxLogs.Text.Length;
                richTextBoxLogs.ScrollToCaret();
            }
        }
    }
}
