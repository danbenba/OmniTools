using System;
using System.Drawing;
using System.Windows.Forms;

namespace OmniTools
{
    public class DetailedLogForm : Form
    {
        private ListBox listBoxLogs;
        private Button btnRefresh;
        private bool allowClose = false;

        public DetailedLogForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Terminal - Logs détaillés";
            this.Size = new Size(600, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            listBoxLogs = new ListBox();
            listBoxLogs.Dock = DockStyle.Fill;
            listBoxLogs.Font = new Font("Consolas", 10);
            this.Controls.Add(listBoxLogs);

            btnRefresh = new Button();
            btnRefresh.Text = "Rafraîchir";
            btnRefresh.Dock = DockStyle.Bottom;
            btnRefresh.Click += BtnRefresh_Click;
            this.Controls.Add(btnRefresh);

            // Timer optionnel pour rafraîchir les logs automatiquement
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 2000;
            timer.Tick += (s, e) => RefreshLogs();
            timer.Start();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshLogs();
        }

        // Exemple de méthode de rafraîchissement (à adapter selon votre implémentation de Logger)
        public void RefreshLogs()
        {
            // Par exemple, vous pouvez récupérer des logs détaillés depuis Logger et les afficher :
            // listBoxLogs.Items.Clear();
            // foreach (var log in Logger.GetDetailedLogs())
            // {
            //     listBoxLogs.Items.Add(log);
            // }
        }

        // Méthode pour autoriser la fermeture depuis le code
        public void ForceClose()
        {
            allowClose = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (Program.DetailedLogsEnabled && !allowClose)
            {
                MessageBox.Show("Vous devez désactiver l'option 'Afficher logs détaillés en terminal' pour fermer cette fenêtre.", 
                                "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }
    }
}
