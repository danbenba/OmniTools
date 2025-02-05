using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace OmniTools
{
    public class UpdateForm : Form
    {
        private Label lblTitle;
        private Label lblMessage;
        private Button btnUpdate;
        private Button btnCancel;

        // Vous pouvez récupérer la version à afficher en paramètre
        private readonly string _latestVersion;

        public UpdateForm(string latestVersion)
        {
            _latestVersion = latestVersion;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Paramètres de base de la fenêtre "UpdateForm"
            this.Text = "Mise à jour disponible";
            this.Size = new Size(500, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Création d'un panel principal
            var mainPanel = new TableLayoutPanel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.ColumnCount = 1;
            mainPanel.RowCount = 3;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // pour le titre
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // pour le message
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // pour les boutons
            this.Controls.Add(mainPanel);

            // Label de titre
            lblTitle = new Label();
            lblTitle.Text = "Nouvelle version disponible !";
            lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Navy;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            mainPanel.Controls.Add(lblTitle, 0, 0);

            // Label de message
            lblMessage = new Label();
            lblMessage.Text = $"Une nouvelle version ({_latestVersion}) d’OmniTools est disponible.\n" +
                              "Voulez-vous la télécharger et l’installer maintenant ?";
            lblMessage.Font = new Font("Arial", 11);
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            lblMessage.Dock = DockStyle.Fill;
            lblMessage.AutoSize = false;
            mainPanel.Controls.Add(lblMessage, 0, 1);

            // Panel pour les boutons
            var buttonsPanel = new FlowLayoutPanel();
            buttonsPanel.Dock = DockStyle.Fill;
            buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonsPanel.Padding = new Padding(10);
            mainPanel.Controls.Add(buttonsPanel, 0, 2);

            // Bouton "Mettre à jour"
            btnUpdate = new Button();
            btnUpdate.Text = "Mettre à jour";
            btnUpdate.Size = new Size(120, 35);
            btnUpdate.Click += BtnUpdate_Click;
            buttonsPanel.Controls.Add(btnUpdate);

            // Bouton "Non merci"
            btnCancel = new Button();
            btnCancel.Text = "Non merci";
            btnCancel.Size = new Size(120, 35);
            btnCancel.Click += BtnCancel_Click;
            // On l’ajoute après le bouton "Mettre à jour" (mais l’affichage est inversé à cause de FlowDirection)
            buttonsPanel.Controls.Add(btnCancel);
        }

        /// <summary>
        /// L’utilisateur clique sur "Mettre à jour"
        /// </summary>
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // On renvoie un DialogResult.OK pour signaler qu’on veut mettre à jour
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// L’utilisateur clique sur "Non merci"
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // On renvoie un DialogResult.Cancel
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
