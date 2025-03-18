using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
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
        private Button btnOption;  // Bouton Option ajouté
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

        // *** Utilisation d'un sous-dossier dédié dans le dossier temporaire ***
        public readonly string tempPath = Path.Combine(Path.GetTempPath(), "OmniTools");

        // *** Nouveaux champs pour la gestion de l'annulation ***
        private CancellationTokenSource cancellationTokenSource;
        private bool isOperationInProgress = false;

        public MainForm()
        {
            InitializeComponent();

            // Lier l'instance de MainForm au Logger
            Logger.MainFormInstance = this;

            // Création du dossier temporaire dédié s'il n'existe pas
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

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
            this.btnOption = new Button(); // Initialisation du bouton Option
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

            // btnExit
            this.btnExit.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.btnExit.Text = "Exit";
            this.btnExit.Location = new Point(424, 27);
            this.btnExit.Size = new Size(80, 35);
            this.btnExit.Click += BtnExit_Click;

            // btnSystemInfo
            this.btnSystemInfo.Font = new Font("Segoe UI", 10F);
            this.btnSystemInfo.Text = "System Info";
            this.btnSystemInfo.Location = new Point(310, 65);
            this.btnSystemInfo.Size = new Size(110, 35);
            this.btnSystemInfo.Click += BtnSystemInfo_Click;

            // btnOption (Nouveau bouton Option)
            this.btnOption.Font = new Font("Segoe UI", 10F);
            this.btnOption.Text = "Plus";
            this.btnOption.Location = new Point(424, 65);
            this.btnOption.Size = new Size(80, 35);
            this.btnOption.Click += BtnOption_Click;

            // checkBoxRestart
            this.checkBoxRestart.AutoSize = true;
            this.checkBoxRestart.Font = new Font("Segoe UI", 10F);
            this.checkBoxRestart.Text = "Restart PC after execution - INSTABLE";
            this.checkBoxRestart.Location = new Point(15, 70);
            this.checkBoxRestart.Size = new Size(200, 23);

            // checkBoxDisableDefender
            this.checkBoxDisableDefender.AutoSize = true;
            this.checkBoxDisableDefender.Font = new Font("Segoe UI", 10F);
            this.checkBoxDisableDefender.Text = "Disable Windows Defender before execution";
            this.checkBoxDisableDefender.Location = new Point(15, 100);
            this.checkBoxDisableDefender.Size = new Size(300, 23);

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
            this.groupBoxActions.Controls.Add(this.btnOption); // Ajout du bouton Option
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
        /// Tente de charger une icône depuis les ressources.
        /// </summary>
        public void LoadIconFromResources()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = "OmniTools.Resources.images.icon.png";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (Bitmap bitmap = new Bitmap(stream))
                        {
                            this.Icon = Icon.FromHandle(bitmap.GetHicon());
                        }
                    }
                    else
                    {
                        MessageBox.Show($"La ressource {resourceName} n'a pas été trouvée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading icon: {ex.Message}");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string dotNetVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            comboBoxScripts.DataSource = ScriptConfig.Scripts;
            comboBoxScripts.DisplayMember = "DisplayName";
            comboBoxScripts.SelectedIndex = -1;
            btnExecute.Enabled = false; // Le bouton est desactiver dès le départ

            comboBoxScripts.DrawMode = DrawMode.OwnerDrawFixed;
            comboBoxScripts.DrawItem += ComboBoxScripts_DrawItem;

            bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator);

            string adminStatus = isAdmin 
                ? "                                                                                      Exécuté en tant qu'administrateur" 
                : "                                                                             Non exécuté en tant qu'administrateur";
            Color statusColor = isAdmin ? Color.Green : Color.Red;

            labelVersion.Text = $"{dotNetVersion}";
            labelVersion.ForeColor = Color.Black;
            labelVersion.AutoSize = true;

            Label adminLabel = new Label();
            adminLabel.Text = adminStatus;
            adminLabel.ForeColor = statusColor;
            adminLabel.AutoSize = true;
            int spacing = 10;
            adminLabel.Location = new Point(labelVersion.Location.X + labelVersion.Width + spacing, labelVersion.Location.Y);
            int maxRight = this.ClientSize.Width - 20;
            int totalWidth = adminLabel.Location.X + adminLabel.Width;
            if (totalWidth > maxRight)
            {
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

            ToolTip toolTip = new ToolTip();
            if (!isAdmin)
            {
                Logger.LogWarning("Les droits d'administrateur sont nécessaires.");
                toolTip.SetToolTip(adminLabel, "Vous n'êtes pas en mode administrateur.\nCliquez ici pour tenter d'élever vos privilèges.");
                adminLabel.Cursor = Cursors.Hand;
                adminLabel.Click += LblPrivilege_Click;
            }

            labelTitle.Text = $"OmniTools v{currentVersion}";

            CheckInternetAndNotifyAsync();
        }

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
                        Verb = "runas"
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
                Logger.LogSuccess("Application démarrée et prête.");
            }
        }

        // Déclaration de la variable membre pour le panel superposé
        private TransparentPanel overlayPanel;

        // Déclaration du ToolTip (au niveau de la classe)
        private ToolTip toolTip = new ToolTip();

        private void ComboBoxScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedScript = comboBoxScripts.SelectedItem as ScriptConfig.ScriptItem;
            btnExecute.Enabled = selectedScript != null && selectedScript.IsEnabled;
            RemoveOverlayPanel();

            if (selectedScript != null && selectedScript.DefenderDisabler && !Program.OverrideDefenderDisabler)
            {
                // Forcer l'affichage de l'option désactivation de Defender (comme dans le code existant)
                checkBoxDisableDefender.Checked = true;
                checkBoxDisableDefender.AutoCheck = false;
                checkBoxDisableDefender.Enabled = false;
                overlayPanel = new TransparentPanel();
                overlayPanel.Size = checkBoxDisableDefender.Size;
                overlayPanel.Location = checkBoxDisableDefender.Location;
                overlayPanel.Cursor = Cursors.Hand;
                checkBoxDisableDefender.Parent.Controls.Add(overlayPanel);
                overlayPanel.BringToFront();
                toolTip.SetToolTip(overlayPanel, "Cette option est verrouillée, car Windows Defender\nl’identifie systématiquement comme un crack.");
                overlayPanel.MouseEnter += OverlayPanel_MouseEnter;
                overlayPanel.MouseLeave += OverlayPanel_MouseLeave;
            }
            else
            {
                checkBoxDisableDefender.AutoCheck = true;
                checkBoxDisableDefender.Enabled = true;
                checkBoxDisableDefender.Cursor = Cursors.Default;
            }
        }

        // Méthode pour retirer et nettoyer le panel superposé s'il existe
        private void RemoveOverlayPanel()
        {
            if (overlayPanel != null)
            {
                overlayPanel.MouseEnter -= OverlayPanel_MouseEnter;
                overlayPanel.MouseLeave -= OverlayPanel_MouseLeave;
                checkBoxDisableDefender.Parent.Controls.Remove(overlayPanel);
                overlayPanel.Dispose();
                overlayPanel = null;
            }
        }

        private void OverlayPanel_MouseEnter(object sender, EventArgs e)
        {
            overlayPanel.Cursor = Cursors.Hand;
        }

        private void OverlayPanel_MouseLeave(object sender, EventArgs e)
        {
            overlayPanel.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Gestion du clic sur le bouton Execute / Cancel.
        /// Lorsque l'opération n'est pas en cours, démarre le téléchargement et exécution du script.
        /// Si l'opération est en cours (bouton affichant "Cancel"), l'annule.
        /// </summary>
        private async void BtnExecute_Click(object sender, EventArgs e)
        {
            // Si une opération est en cours, le bouton agit en mode "Cancel"
            if (isOperationInProgress)
            {
                cancellationTokenSource?.Cancel();
                Logger.DownloadCanceledLog(" Téléchargement annulé");
                Logger.LogWarning("Téléchargement annulé par l'utilisateur");
                return;
            }

            var selectedScript = comboBoxScripts.SelectedItem as ScriptConfig.ScriptItem;
            if (selectedScript == null || !selectedScript.IsEnabled)
            {
                Logger.LogWarning("Ce script est désactivé et ne peut pas être exécuté.");
                return;
            }

            // Si la case "Disable Defender" est cochée, avertir l'utilisateur
            if (checkBoxDisableDefender.Checked)
            {
                DialogResult warnprompt = MessageBox.Show(
                    "Attention !\nNous allons procéder à la désactivation de Windows Defender. Une fois Windows Defender désactivé, un message de confirmation apparaîtra pour lancer le programme.",
                    "Attention",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                );

                if (warnprompt == DialogResult.Cancel)
                {
                    Logger.LogInfo("Opération annulée par l'utilisateur.");
                    return;
                }
                
                Logger.LogInfo("Désactivation de Windows Defender avant l'exécution...");
                await ExecuteRegistryCommands("disable");
            }

            // Demander confirmation à l'utilisateur
            DialogResult dr = MessageBox.Show(
                $"Voulez-vous vraiment exécuter '{selectedScript.DisplayName}' ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dr == DialogResult.No)
            {
                Logger.LogInfo("Opération annulée par l'utilisateur.");
                return;
            }

            // Mise à jour de l'interface
            comboBoxScripts.Enabled = false;
            btnExecute.Text = "Cancel";
            isOperationInProgress = true;
            cancellationTokenSource = new CancellationTokenSource();

            // Chemin local de sauvegarde du script téléchargé
            string scriptLocalPath = Path.Combine(tempPath, selectedScript.LocalFileName);

            // Téléchargement du script avec gestion de l'annulation
            bool downloadSuccess = await DownloadFileWithProgressAsync(selectedScript.DownloadUrl, scriptLocalPath, cancellationTokenSource.Token);
            if (!downloadSuccess)
            {
                if (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Logger.LogError($"Échec du téléchargement de '{selectedScript.DisplayName}'.");
                }
                isOperationInProgress = false;
                comboBoxScripts.Enabled = true;
                btnExecute.Text = "Execute";
                return;
            }

            // Vérifier si le fichier téléchargé est un ZIP à extraire
            if (selectedScript.ZipFile)
            {
                try
                {
                    // Détermine le dossier d'extraction en se basant sur le nom du fichier ZIP (sans extension)
                    string extractionFolder = Path.Combine(tempPath, Path.GetFileNameWithoutExtension(selectedScript.LocalFileName));
                    
                    // Supprime le dossier existant s'il existe afin de remplacer les fichiers existants
                    if (Directory.Exists(extractionFolder))
                    {
                        Directory.Delete(extractionFolder, true);
                    }
                    Directory.CreateDirectory(extractionFolder);

                    // Extraction du fichier ZIP dans le dossier d'extraction
                    System.IO.Compression.ZipFile.ExtractToDirectory(scriptLocalPath, extractionFolder);
                    Logger.LogInfo($"Unpacking....");

                    // Supprimer le fichier ZIP après extraction
                    if (File.Exists(scriptLocalPath))
                    {
                        File.Delete(scriptLocalPath);
                        Logger.LogSuccess("Done.");
                    }

                    // Vérifier que le point d'entrée est défini
                    if (string.IsNullOrEmpty(selectedScript.EntryPoint))
                    {
                        Logger.LogError("Aucun point d'entrée spécifié pour le fichier ZIP.");
                        return;
                    }
                    
                    // Construire le chemin complet du fichier à exécuter
                    string entryFullPath = Path.Combine(extractionFolder, selectedScript.EntryPoint);
                    if (!File.Exists(entryFullPath))
                    {
                        Logger.LogError($"Le fichier spécifié comme point d'entrée n'existe pas : {entryFullPath}");
                        return;
                    }
                    
                    // Exécuter le fichier extrait
                    bool scriptExecuted = await ExecuteScriptAsync(entryFullPath, selectedScript.DefaultArguments, selectedScript.DisplayName);
                    if (!scriptExecuted)
                    {
                        Logger.LogError($"Échec de l'exécution de '{selectedScript.DisplayName}' (ZIP).");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Erreur lors de l'extraction ou de l'exécution du ZIP : {ex.Message}");
                }
            }
            else
            {
                // Traitement classique pour un fichier non-ZIP
                bool scriptExecuted = await ExecuteScriptAsync(scriptLocalPath, selectedScript.DefaultArguments, selectedScript.DisplayName);
                if (!scriptExecuted)
                {
                    Logger.LogError($"Échec de l'exécution de '{selectedScript.DisplayName}'.");
                }
            }

            // Si Windows Defender a été désactivé, le réactiver après l'exécution
            if (checkBoxDisableDefender.Checked)
            {
                DialogResult warnprompt2 = MessageBox.Show(
                    "Attention !\nNous allons procéder à la réactivation de Windows Defender.",
                    "Attention",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                
                Logger.LogInfo("Réactivation de Windows Defender après l'exécution...");
                await ExecuteRegistryCommands("enable");
            }

            // Si l'option de redémarrage est activée, initier le redémarrage du système
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

            // Réinitialisation de l'interface
            isOperationInProgress = false;
            comboBoxScripts.Enabled = true;
            btnExecute.Text = "Execute";
        }


        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async Task ExecuteRegistryCommands(string action)
        {
            string[] commands;
            if (action == "disable")
            {
                commands = new string[]
                {
                    @"powershell -Command ""Start-Process powershell -Verb RunAs -ArgumentList '-NoProfile -ExecutionPolicy Bypass -Command \""New-Item -ItemType Directory -Path $env:TEMP\OmniTools\DefenderManager -Force; Invoke-WebRequest -Uri ''https://raw.githubusercontent.com/danbenba/OmniTools/refs/heads/project/Scripts/OmniTools.DefenderDisabler.bat'' -OutFile $env:TEMP\OmniTools\DefenderManager\OmniTools.DefenderDisabler.bat; Start-Process $env:TEMP\OmniTools\DefenderManager\OmniTools.DefenderDisabler.bat\""'"""
                };
            }
            else // enable
            {
                commands = new string[]
                {
                    @"powershell -Command ""Start-Process powershell -Verb RunAs -ArgumentList '-NoProfile -ExecutionPolicy Bypass -Command \""New-Item -ItemType Directory -Path $env:TEMP\OmniTools\DefenderManager -Force; Invoke-WebRequest -Uri ''https://raw.githubusercontent.com/danbenba/OmniTools/refs/heads/project/Scripts/OmniTools.DefenderEnabler.bat'' -OutFile $env:TEMP\OmniTools\DefenderManager\OmniTools.DefenderEnabler.bat; Start-Process $env:TEMP\OmniTools\DefenderManager\OmniTools.DefenderEnabler.bat\""'"""
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
        /// Télécharge un fichier en affichant une barre de progression dans le RichTextBox.
        /// La méthode prend désormais en charge l'annulation via CancellationToken.
        /// </summary>
        private async Task<bool> DownloadFileWithProgressAsync(string url, string destinationPath, CancellationToken cancellationToken)
        {
            try
            {
                // Effacer les logs pour éviter les interférences
                Logger.Clear();

                var selectedScript = comboBoxScripts.SelectedItem as ScriptConfig.ScriptItem;
                Logger.CoreLog($"Selected: '{selectedScript.DisplayName}'");

                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }

                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                long? totalBytes = response.Content.Headers.ContentLength;
                if (totalBytes == null)
                {
                    Logger.LogError("Impossible de récupérer la taille du fichier.");
                    return false;
                }

                using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using FileStream fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);

                byte[] buffer = new byte[8192];
                long totalRead = 0;
                int bytesRead;
                string progressBarTemplate = "Downloading : [---------------------------] 0% ";
                AddLog(progressBarTemplate, Color.Blue, newLine: false);

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken); // Ajout de l'écriture dans le fichier
                    totalRead += bytesRead;
                    int progress = (int)(totalRead * 100 / totalBytes.Value);
                    if (progress >= lastProgress + 2) // Mise à jour toutes les 2%
                    {
                        UpdateProgressBar(progress);
                        lastProgress = progress;
                    }
                }

                UpdateProgressBar(100);
                AddLog(" Done !\n", Color.Green, newLine: true);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
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
            string progressLine = $"Downloading : [{progressBar}] {progress}% ";

            if (richTextBoxLogs.Lines.Length > 0)
            {
                int lastLineIndex = richTextBoxLogs.GetFirstCharIndexFromLine(richTextBoxLogs.Lines.Length - 1);
                richTextBoxLogs.Select(lastLineIndex, richTextBoxLogs.Lines.Last().Length);
                richTextBoxLogs.SelectedText = progressLine;
            }
        }

        private async Task<bool> ExecuteScriptAsync(string scriptPath, string arguments, string displayName)
        {
            try
            {
                Logger.LogInfo($"Starting...");
                Process process = new Process();

                if (Path.GetExtension(scriptPath).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = $"/c start \"\" \"{scriptPath}\" {arguments}";
                }
                else
                {
                    process.StartInfo.FileName = scriptPath;
                    process.StartInfo.Arguments = arguments;
                }

                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = false;

                process.Start();
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

        private void BtnSystemInfo_Click(object sender, EventArgs e)
        {
            SystemInfoForm infoForm = new SystemInfoForm();
            infoForm.ShowDialog();
        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

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

                richTextBoxLogs.Select(start, end - start);
                richTextBoxLogs.SelectionColor = color;

                richTextBoxLogs.SelectionLength = 0;
                richTextBoxLogs.SelectionStart = richTextBoxLogs.Text.Length;
                richTextBoxLogs.ScrollToCaret();
            }
        }

        // Gestion du clic sur le bouton Option
        private void BtnOption_Click(object sender, EventArgs e)
        {
            OptionsForm optionsForm = new OptionsForm();
            optionsForm.ShowDialog();
        }
    }

} 