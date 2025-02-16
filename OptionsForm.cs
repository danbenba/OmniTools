namespace OmniTools
{
    public class OptionsForm : Form
    {
        private Button btnClearTemp;
        private Button ClearLogBtn;
        private MainForm mainForm;

        // *** Utilisation d'un sous-dossier dédié dans le dossier temporaire ***
        public readonly string tempPath = Path.Combine(Path.GetTempPath(), "OmniTools");

        public OptionsForm()
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(350, 170);
            this.Text = "Plus d'options";

            btnClearTemp = new Button();
            btnClearTemp.Font = new Font("Segoe UI", 10F);
            btnClearTemp.Text = "Nettoyer les fichiers temporaires";
            btnClearTemp.Size = new Size(300, 40);
            btnClearTemp.Location = new Point(20, 20);
            btnClearTemp.Click += BtnClearTemp_Click;

            ClearLogBtn = new Button();
            ClearLogBtn.Font = new Font("Segoe UI", 10F);
            ClearLogBtn.Text = "Effacer les logs";
            ClearLogBtn.Size = new Size(300, 40);
            ClearLogBtn.Location = new Point(20, 62);
            ClearLogBtn.Click += ClearLogBtn_Click;

            this.Controls.Add(ClearLogBtn);
            this.Controls.Add(btnClearTemp);
        }

        private void BtnClearTemp_Click(object sender, EventArgs e)
        {
            ClearTemporaryFiles();
            this.Close();
        }

        private void ClearLogBtn_Click(object sender, EventArgs e)
        {
            Logger.Clear();
            this.Close();
        }

    // Méthode pour effacer tous les fichiers temporaires de l'application
    public void ClearTemporaryFiles()
    {
        try
        {
            var files = Directory.GetFiles(tempPath);
            foreach (var file in files)
            {
                File.Delete(file);
            }
            Logger.LogSuccess("Effacement des fichiers réussi :)");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Erreur lors de l'effacement des fichiers temporaires: {ex.Message}");
        }
    }

    }
}