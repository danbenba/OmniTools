using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmniTools
{
    public class SplashForm : Form
    {
        private Label lblTitle;
        private Label lblStatus;
        private Label lblVersion;
        private Button btnExit;

        private const int CornerRadius = 20; // pour les bords arrondis

        public SplashForm()
        {
            InitializeComponent();
            this.Shown += SplashForm_Shown;
            this.FormClosing += SplashForm_FormClosing;

            // Charger l'icon depuis le MainFrom
            LoadIconFromResources();
        }

        private async void SplashForm_Shown(object sender, EventArgs e)
        {
            await StartUpProcessAsync();
        }

        /// <summary>
        /// Processus de démarrage asynchrone : vérification de connexion, mise à jour, etc.
        /// </summary>
        private async Task StartUpProcessAsync()
        {
            try
            {
                UpdateStatus("Loading, Please Wait...");
                // Simule le chargement (ou charger réellement des ressources)
                await Task.Delay(1000);

                UpdateStatus("Checking internet connection...");
                bool isConnected = await Program.IsInternetConnectionAvailable();
                if (!isConnected)
                {
                    UpdateStatus("No internet connection detected.", Color.Red);
                    await Program.CheckInternetAndNotifyAsync();
                    await Task.Delay(2000);
                }

                UpdateStatus("Checking for updates...", Color.Black);
                bool canLaunch = await Program.CheckForUpdates();
                if (!canLaunch)
                {
                    return;
                }

                UpdateStatus("Starting application...");
                await Task.Delay(500);

                // Ouvre la fenêtre principale
                this.Hide();
                MainForm mainForm = new MainForm();
                mainForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred during startup: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                this.Close();
            }
        }

        public void UpdateStatus(string message, Color? color = null)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() =>
                {
                    lblStatus.Text = message;
                    if (color.HasValue)
                    {
                        lblStatus.ForeColor = color.Value;
                    }
                }));
            }
            else
            {
                lblStatus.Text = message;
                if (color.HasValue)
                {
                    lblStatus.ForeColor = color.Value;
                }
            }
        }


        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 20);
            this.lblTitle.AutoSize = false;
            this.lblTitle.Width = 350;
            this.lblTitle.Height = 50;
            this.lblTitle.Text = "OmniTools";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Location = new Point(25, 0);
            this.lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular);
            this.lblStatus.Location = new System.Drawing.Point(20, 69);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(360, 30);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Initializing...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.lblVersion.Location = new System.Drawing.Point(0, 130);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(400, 20);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "Version " + Program.Version;
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnExit.Location = new System.Drawing.Point(350, 10);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(40, 30);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "X";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // SplashForm
            // 
            this.ClientSize = new System.Drawing.Size(400, 160);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.ResumeLayout(false);

            // Une fois que la fenêtre est chargée, on applique des bords arrondis
            this.Load += new System.EventHandler(this.SplashForm_Load);
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            // Dessin des bords arrondis du formulaire
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, CornerRadius, CornerRadius), 180, 90);
            path.AddLine(CornerRadius, 0, this.Width - CornerRadius, 0);
            path.AddArc(new Rectangle(this.Width - CornerRadius, 0, CornerRadius, CornerRadius), 270, 90);
            path.AddLine(this.Width, CornerRadius, this.Width, this.Height - CornerRadius);
            path.AddArc(new Rectangle(this.Width - CornerRadius, this.Height - CornerRadius, CornerRadius, CornerRadius), 0, 90);
            path.AddLine(this.Width - CornerRadius, this.Height, CornerRadius, this.Height);
            path.AddArc(new Rectangle(0, this.Height - CornerRadius, CornerRadius, CornerRadius), 90, 90);
            path.CloseFigure();
            this.Region = new Region(path);
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        private void SplashForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        // Permettre le déplacement de la fenêtre en cliquant n'importe où
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        // API Win32 pour bouger la fenêtre en l'absence de barre de titre
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
    }
}
