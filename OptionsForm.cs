
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmniTools
{
    public class OptionsForm : Form
    {
        // Champs pour les différents contrôles de l'interface.
        private GroupBox groupBoxTempFiles = null!;
        private GroupBox groupBoxAppOptions = null!;
        private TableLayoutPanel tempTable = null!;
        private TableLayoutPanel appTable = null!;
        private Label lblHeader = null!;

        // Boutons existants pour la gestion des fichiers temporaires et logs.
        private Button btnClearTemp = null!;
        private Button btnOpenTempFolder = null!;
        private Button btnClearLogs = null!;
        private Button btnResetSettings = null!;

        // Nouveaux contrôles pour les options supplémentaires.
        private CheckBox chkOverrideDefender = null!;

        // Dossier temporaire dédié.
        public readonly string tempPath = Path.Combine(Path.GetTempPath(), "OmniTools");
        
        public OptionsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Configuration générale de la fenêtre.
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(550, 500);
            this.Text = "Options Avancées";
            this.BackColor = Color.White;

            // En-tête.
            lblHeader = new Label();
            lblHeader.Text = "Options Avancées";
            lblHeader.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblHeader.ForeColor = Color.Black;
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            lblHeader.Dock = DockStyle.Top;
            lblHeader.Height = 60;

            // GroupBox pour la gestion des fichiers temporaires.
            groupBoxTempFiles = new GroupBox();
            groupBoxTempFiles.Text = "Gestion des fichiers temporaires";
            groupBoxTempFiles.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            groupBoxTempFiles.ForeColor = Color.Black;
            groupBoxTempFiles.BackColor = Color.White;
            groupBoxTempFiles.Dock = DockStyle.Top;
            groupBoxTempFiles.Height = 150;
            groupBoxTempFiles.Padding = new Padding(10);

            tempTable = new TableLayoutPanel();
            tempTable.ColumnCount = 2;
            tempTable.RowCount = 2;
            tempTable.Dock = DockStyle.Fill;
            tempTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tempTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tempTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tempTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tempTable.BackColor = Color.White;

            btnClearTemp = new Button();
            btnClearTemp.Font = new Font("Segoe UI", 10);
            btnClearTemp.Text = "Nettoyer";
            btnClearTemp.Dock = DockStyle.Fill;
            btnClearTemp.Click += BtnClearTemp_Click;
            ApplyRoundedStyle(btnClearTemp, 10);

            Label lblClearTempDesc = new Label();
            lblClearTempDesc.Text = "Supprime tous les fichiers du dossier temporaire.";
            lblClearTempDesc.Font = new Font("Segoe UI", 9);
            lblClearTempDesc.ForeColor = Color.Black;
            lblClearTempDesc.Dock = DockStyle.Fill;
            lblClearTempDesc.TextAlign = ContentAlignment.MiddleLeft;

            btnOpenTempFolder = new Button();
            btnOpenTempFolder.Font = new Font("Segoe UI", 10);
            btnOpenTempFolder.Text = "Ouvrir le dossier";
            btnOpenTempFolder.Dock = DockStyle.Fill;
            btnOpenTempFolder.Click += BtnOpenTempFolder_Click;
            ApplyRoundedStyle(btnOpenTempFolder, 10);

            Label lblOpenTempDesc = new Label();
            lblOpenTempDesc.Text = "Ouvre le dossier contenant les fichiers temporaires.";
            lblOpenTempDesc.Font = new Font("Segoe UI", 9);
            lblOpenTempDesc.ForeColor = Color.Black;
            lblOpenTempDesc.Dock = DockStyle.Fill;
            lblOpenTempDesc.TextAlign = ContentAlignment.MiddleLeft;

            tempTable.Controls.Add(btnClearTemp, 0, 0);
            tempTable.Controls.Add(lblClearTempDesc, 1, 0);
            tempTable.Controls.Add(btnOpenTempFolder, 0, 1);
            tempTable.Controls.Add(lblOpenTempDesc, 1, 1);
            groupBoxTempFiles.Controls.Add(tempTable);

            // GroupBox pour les options de l'application.
            groupBoxAppOptions = new GroupBox();
            groupBoxAppOptions.Text = "Options de l'application";
            groupBoxAppOptions.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            groupBoxAppOptions.ForeColor = Color.Black;
            groupBoxAppOptions.BackColor = Color.White;
            groupBoxAppOptions.Dock = DockStyle.Fill;
            groupBoxAppOptions.Padding = new Padding(10);

            // Mise à jour du TableLayoutPanel pour 4 lignes.
            appTable = new TableLayoutPanel();
            appTable.ColumnCount = 2;
            appTable.RowCount = 4;
            appTable.Dock = DockStyle.Fill;
            appTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            appTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            appTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            appTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            appTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            appTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            appTable.BackColor = Color.White;

            // Bouton "Effacer les logs".
            btnClearLogs = new Button();
            btnClearLogs.Font = new Font("Segoe UI", 10);
            btnClearLogs.Text = "Effacer les logs";
            btnClearLogs.Dock = DockStyle.Fill;
            btnClearLogs.Click += BtnClearLogs_Click;
            ApplyRoundedStyle(btnClearLogs, 10);

            Label lblClearLogsDesc = new Label();
            lblClearLogsDesc.Text = "Efface l'historique des logs de l'application.";
            lblClearLogsDesc.Font = new Font("Segoe UI", 9);
            lblClearLogsDesc.ForeColor = Color.Black;
            lblClearLogsDesc.Dock = DockStyle.Fill;
            lblClearLogsDesc.TextAlign = ContentAlignment.MiddleLeft;

            // Bouton "Redémarrer l'application".
            btnResetSettings = new Button();
            btnResetSettings.Font = new Font("Segoe UI", 10);
            btnResetSettings.Text = "Redémarrer";
            btnResetSettings.Dock = DockStyle.Fill;
            btnResetSettings.Click += BtnResetSettings_Click;
            ApplyRoundedStyle(btnResetSettings, 10);

            Label lblResetDesc = new Label();
            lblResetDesc.Text = "Redémarre l'application.";
            lblResetDesc.Font = new Font("Segoe UI", 9);
            lblResetDesc.ForeColor = Color.Black;
            lblResetDesc.Dock = DockStyle.Fill;
            lblResetDesc.TextAlign = ContentAlignment.MiddleLeft;

            // Nouvelle option 1 : Override Defender.
            chkOverrideDefender = new CheckBox();
            chkOverrideDefender.Font = new Font("Segoe UI", 10);
            chkOverrideDefender.Text = "Ne pas forcer la désactivation de Windows Defender";
            chkOverrideDefender.Dock = DockStyle.Fill;
            chkOverrideDefender.CheckedChanged += ChkOverrideDefender_CheckedChanged;

            Label lblOverrideDefenderDesc = new Label();
            lblOverrideDefenderDesc.Text = "Ignore l'obligation de désactiver Windows Defender.";
            lblOverrideDefenderDesc.Font = new Font("Segoe UI", 9);
            lblOverrideDefenderDesc.ForeColor = Color.Black;
            lblOverrideDefenderDesc.Dock = DockStyle.Fill;
            lblOverrideDefenderDesc.TextAlign = ContentAlignment.MiddleLeft;

            appTable.Controls.Add(btnClearLogs, 0, 0);
            appTable.Controls.Add(lblClearLogsDesc, 1, 0);
            appTable.Controls.Add(btnResetSettings, 0, 1);
            appTable.Controls.Add(lblResetDesc, 1, 1);
            appTable.Controls.Add(chkOverrideDefender, 0, 2);
            appTable.Controls.Add(lblOverrideDefenderDesc, 1, 2);

            groupBoxAppOptions.Controls.Add(appTable);

            // Ajout des contrôles à la fenêtre.
            this.Controls.Add(groupBoxAppOptions);
            this.Controls.Add(groupBoxTempFiles);
            this.Controls.Add(lblHeader);
        }

        private void ApplyRoundedStyle(Button btn, int radius)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.LightGray;
            btn.ForeColor = Color.Black;
            btn.Resize += (s, e) =>
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
                    path.CloseFigure();
                    btn.Region = new Region(path);
                }
            };
        }

        private async void BtnClearTemp_Click(object sender, EventArgs e)
        {
            ClearTemporaryFiles();
            string originalText = btnClearTemp.Text;
            Color originalForeColor = btnClearTemp.ForeColor;
            btnClearTemp.Text = "Fichiers nettoyés";
            btnClearTemp.ForeColor = Color.Green;
            btnClearTemp.Enabled = false;
            await Task.Delay(3000);
            btnClearTemp.Text = originalText;
            btnClearTemp.ForeColor = originalForeColor;
            btnClearTemp.Enabled = true;
        }

        private void BtnOpenTempFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                Process.Start("explorer.exe", tempPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du dossier temporaire : {ex.Message}",
                                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnClearLogs_Click(object sender, EventArgs e)
        {
            Logger.Clear();
            string originalText = btnClearLogs.Text;
            Color originalForeColor = btnClearLogs.ForeColor;
            btnClearLogs.Text = "Logs effacés";
            btnClearLogs.ForeColor = Color.Green;
            btnClearLogs.Enabled = false;
            await Task.Delay(3000);
            btnClearLogs.Text = originalText;
            btnClearLogs.ForeColor = originalForeColor;
            btnClearLogs.Enabled = true;
        }

        private async void BtnResetSettings_Click(object sender, EventArgs e)
        {
            bool restartCancelled = false;
            Form autoCloseForm = new Form();
            autoCloseForm.StartPosition = FormStartPosition.CenterScreen;
            autoCloseForm.FormBorderStyle = FormBorderStyle.None;
            autoCloseForm.BackColor = Color.Black;
            autoCloseForm.Size = new Size(300, 150);
            autoCloseForm.ShowInTaskbar = false;

            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = 20;
                path.StartFigure();
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddLine(radius, 0, autoCloseForm.Width - radius, 0);
                path.AddArc(autoCloseForm.Width - radius, 0, radius, radius, -90, 90);
                path.AddLine(autoCloseForm.Width, radius, autoCloseForm.Width, autoCloseForm.Height - radius);
                path.AddArc(autoCloseForm.Width - radius, autoCloseForm.Height - radius, radius, radius, 0, 90);
                path.AddLine(autoCloseForm.Width - radius, autoCloseForm.Height, radius, autoCloseForm.Height);
                path.AddArc(0, autoCloseForm.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();
                autoCloseForm.Region = new Region(path);
            }

            Label lblMessage = new Label();
            lblMessage.Font = new Font("Segoe UI", 10);
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            lblMessage.Dock = DockStyle.Top;
            lblMessage.Height = 60;
            lblMessage.ForeColor = Color.White;
            autoCloseForm.Controls.Add(lblMessage);

            Button btnCancel = new Button();
            btnCancel.Text = "Annuler";
            btnCancel.Font = new Font("Segoe UI", 10);
            btnCancel.Size = new Size(80, 30);
            btnCancel.BackColor = Color.LightGray;
            btnCancel.ForeColor = Color.Black;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point((autoCloseForm.Width - btnCancel.Width) / 2, autoCloseForm.Height - btnCancel.Height - 20);
            btnCancel.Click += (s, args) =>
            {
                restartCancelled = true;
                autoCloseForm.Close();
            };
            autoCloseForm.Controls.Add(btnCancel);

            autoCloseForm.Show();
            for (int i = 3; i >= 1; i--)
            {
                lblMessage.Text = $"L'application va redémarrer dans {i} seconde{(i > 1 ? "s" : "")}...";
                await Task.Delay(1000);
                if (restartCancelled)
                    break;
            }

            if (!restartCancelled)
            {
                autoCloseForm.Close();
                Application.Restart();
            }
        }

        public void ClearTemporaryFiles()
        {
            try
            {
                if (Directory.Exists(tempPath))
                {
                    var files = Directory.GetFiles(tempPath);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                    var directories = Directory.GetDirectories(tempPath);
                    foreach (var directory in directories)
                    {
                        Directory.Delete(directory, true);
                    }
                    Logger.LogSuccess("Effacement des fichiers et dossiers réussi :)");
                }
                else
                {
                    Logger.LogWarning("Le dossier temporaire n'existe pas.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Erreur lors de l'effacement des fichiers temporaires : {ex.Message}");
            }
        }

        // Gestion de l'option "Override Defender"
        private void ChkOverrideDefender_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOverrideDefender.Checked)
            {
                var result = MessageBox.Show("Voulez-vous vraiment désactiver l'obligation de désactiver Windows Defender ?",
                                             "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Program.OverrideDefenderDisabler = true;
                    Logger.LogInfo("Option 'Override Defender' activée.");
                }
                else
                {
                    chkOverrideDefender.Checked = false;
                }
            }
            else
            {
                Program.OverrideDefenderDisabler = false;
                Logger.LogInfo("Option 'Override Defender' désactivée.");
            }
        }
    }
}
