using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace OmniTools
{
    public class AboutFrom : Form
    {
        private PictureBox pictureBoxLogo;
        private Label lblTitle;
        private Label lblDescription;
        private Label lblPrecaution;
        private Button btnOk;
        private Button btnLearnMore;
        private TableLayoutPanel mainLayout;
        private TableLayoutPanel buttonLayout;

        // Récupération des informations de version et de langue depuis la classe Program
        string CurrentVersion = Program.Version;
        string Language = Program.Language;

        public AboutFrom()
        {
            InitializeComponent();
            LoadLogoImage();
        }

        private void InitializeComponent()
        {
            // Paramètres de base de la fenêtre "AboutFrom"
            this.Text = "À propos de OmniTools";
            this.Size = new Size(600, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Création d'un TableLayoutPanel principal (mainLayout)
            mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.RowCount = 1;
            // Première colonne : 30% de la largeur pour l'image
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            // Deuxième colonne : 70% de la largeur pour le texte et les boutons
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            this.Controls.Add(mainLayout);

            // PictureBox pour le logo
            pictureBoxLogo = new PictureBox();
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxLogo.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(pictureBoxLogo, 0, 0);

            // Création d'un TableLayoutPanel pour stocker les labels et les boutons
            var textAndButtonsPanel = new TableLayoutPanel();
            textAndButtonsPanel.Dock = DockStyle.Fill;
            textAndButtonsPanel.ColumnCount = 1;
            textAndButtonsPanel.RowCount = 3;
            // Répartition en pourcentage pour laisser la place au texte, aux boutons et au label de précaution
            textAndButtonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F)); // zone texte
            textAndButtonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // zone boutons
            textAndButtonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F)); // zone "précaution"
            mainLayout.Controls.Add(textAndButtonsPanel, 1, 0);

            // Panel pour les textes (un sous-TableLayoutPanel supplémentaire)
            var textsPanel = new TableLayoutPanel();
            textsPanel.Dock = DockStyle.Fill;
            textsPanel.ColumnCount = 1;
            textsPanel.RowCount = 2;
            textsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F)); // Titre
            textsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F)); // Description
            textAndButtonsPanel.Controls.Add(textsPanel, 0, 0);

            // Label de Titre (lblTitle)
            lblTitle = new Label();
            lblTitle.Text = $"OmniTools";
            lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Dock = DockStyle.Fill;
            textsPanel.Controls.Add(lblTitle, 0, 0);

            // Label de Description (lblDescription)
            lblDescription = new Label();
            lblDescription.Text = $"Développé par danbenba.\n\nOmniTools est un utilitaire Windows qui facilite l’exécution\nde scripts variés.\nVersion {CurrentVersion} ({Language} Version) - Copyright © 2025 danbenba";
            lblDescription.Font = new Font("Arial", 10);
            lblDescription.AutoSize = true;
            lblDescription.TextAlign = ContentAlignment.MiddleLeft;
            lblDescription.Dock = DockStyle.Fill;
            textsPanel.Controls.Add(lblDescription, 0, 1);

            // TableLayoutPanel pour les boutons
            buttonLayout = new TableLayoutPanel();
            buttonLayout.Dock = DockStyle.Fill;
            buttonLayout.ColumnCount = 2;
            buttonLayout.RowCount = 1;
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            textAndButtonsPanel.Controls.Add(buttonLayout, 0, 1);

            // Bouton "Learn More"
            btnLearnMore = new Button();
            btnLearnMore.Text = "Learn More";
            btnLearnMore.Size = new Size(100, 30);
            btnLearnMore.Anchor = AnchorStyles.Right;
            btnLearnMore.Click += BtnLearnMore_Click;
            buttonLayout.Controls.Add(btnLearnMore, 0, 0);

            // Bouton "OK"
            btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Size = new Size(100, 30);
            btnOk.Anchor = AnchorStyles.Left;
            btnOk.Click += BtnOk_Click;
            buttonLayout.Controls.Add(btnOk, 1, 0);

            // Label d’avertissement "précaution" (lblPrecaution)
            lblPrecaution = new Label();
            lblPrecaution.Text = "     Attention : utilisez ce programme avec prudence.   ";
            lblPrecaution.Font = new Font("Arial", 10, FontStyle.Bold);
            lblPrecaution.ForeColor = Color.Red;
            lblPrecaution.Dock = DockStyle.Fill;
            lblPrecaution.TextAlign = ContentAlignment.MiddleCenter;
            textAndButtonsPanel.Controls.Add(lblPrecaution, 0, 2);
        }

        /// <summary>
        /// Tente de charger une image de logo intégrée dans les ressources de l'assembly.
        /// </summary>
        private void LoadLogoImage()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                // Chemin dans les ressources intégrées, par ex. "OmniTools.Resources.images.icon.png"
                using (Stream imageStream = assembly.GetManifestResourceStream("OmniTools.Resources.images.icon.png"))
                {
                    if (imageStream != null)
                    {
                        pictureBoxLogo.Image = Image.FromStream(imageStream);
                    }
                    else
                    {
                        MessageBox.Show("L'image du logo n'a pas été trouvée dans les ressources intégrées.", "Erreur",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible de charger le logo JetBrains : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Événement appelé lorsque l'utilisateur clique sur le bouton "OK".
        /// Ferme simplement la fenêtre "À propos".
        /// </summary>
        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Événement appelé lorsque l'utilisateur clique sur "Learn More".
        /// Ouvre un navigateur web pointant vers la page GitHub.
        /// </summary>
        private void BtnLearnMore_Click(object sender, EventArgs e)
        {
            string githubUrl = "https://github.com/danbenba/OmniTools";
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = githubUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'ouvrir le lien GitHub : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
