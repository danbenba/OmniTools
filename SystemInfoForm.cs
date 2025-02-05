using System;
using System.Drawing;
using System.Net;
using System.Security.Principal;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;

public class SystemInfoForm : Form
{
    // Contrôles d'affichage
    private Label lblOS;
    private Label lblDotNet;
    private Label lblPrivilege;
    private PictureBox pbOS;
    private PictureBox pbDotNet;
    private PictureBox pbPrivilege;
    private ToolTip toolTip;

    // Informations système
    private bool isAdmin;
    private string systemVersion;
    private string dotNetVersion;
    
    public SystemInfoForm()
    {
        // Configuration de la fenêtre
        this.Text = "Informations Système";
        this.Size = new Size(500, 300);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false;

        // Initialisation de l'infobulle
        toolTip = new ToolTip();

        // Récupération des informations système
        systemVersion = GetWindowsVersion();
        dotNetVersion = RuntimeInformation.FrameworkDescription;
        isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent())
                      .IsInRole(WindowsBuiltInRole.Administrator);

        // Création et configuration des contrôles
        // Label et image pour le système d'exploitation
        pbOS = new PictureBox
        {
            Size = new Size(64, 64),
            Location = new Point(20, 10),
            SizeMode = PictureBoxSizeMode.StretchImage
        };
        // Exemple d'URL pour le logo Windows (remplacez par l'URL de votre choix)
        LoadImageFromUrl(pbOS, "https://img.icons8.com/?size=64&id=TuXN3JNUBGOT&format=png");

        lblOS = new Label
        {
            Text = $"Système d'exploitation : {systemVersion}",
            AutoSize = true,
            Location = new Point(120, 20)
        };

        // Label et image pour le .NET Runtime
        pbDotNet = new PictureBox
        {
            Size = new Size(64, 64),
            Location = new Point(20, 80),
            SizeMode = PictureBoxSizeMode.StretchImage
        };
        // Exemple d'URL pour le logo .NET (remplacez par l'URL de votre choix)
        LoadImageFromUrl(pbDotNet, "https://i.ibb.co/H5npFvw/NET-Core-Logo-svg.png");

        lblDotNet = new Label
        {
            Text = $".NET Runtime : {dotNetVersion}",
            AutoSize = true,
            Location = new Point(120, 100)
        };

        // Label et image pour les privilèges
        pbPrivilege = new PictureBox
        {
            Size = new Size(64, 64),
            Location = new Point(20, 150),
            SizeMode = PictureBoxSizeMode.StretchImage
        };
        // Exemple d'URL pour un logo représentant la sécurité (remplacez par l'URL de votre choix)
        LoadImageFromUrl(pbPrivilege, "https://img.icons8.com/?size=64&id=YztviYmQLfhl&format=png");

        lblPrivilege = new Label
        {
            Text = $"Privilège : {(isAdmin ? "Administrateur" : "Non-administrateur")}",
            AutoSize = true,
            Location = new Point(120, 170),
            ForeColor = isAdmin ? Color.Green : Color.Red
        };

        // Si l'utilisateur n'est pas admin, on ajoute une infobulle et un clic pour proposer l'élévation
        if (!isAdmin)
        {
            toolTip.SetToolTip(lblPrivilege, "Vous n'êtes pas en mode administrateur.\nCliquez ici pour tenter d'élever vos privilèges.");
            lblPrivilege.Cursor = Cursors.Hand;
            lblPrivilege.Click += LblPrivilege_Click;
        }

        // Ajout des contrôles à la fenêtre
        this.Controls.Add(pbOS);
        this.Controls.Add(lblOS);
        this.Controls.Add(pbDotNet);
        this.Controls.Add(lblDotNet);
        this.Controls.Add(pbPrivilege);
        this.Controls.Add(lblPrivilege);
    }

    /// <summary>
    /// Charge une image depuis une URL dans le PictureBox.
    /// En cas d'erreur, le PictureBox affiche un fond gris.
    /// </summary>
    private void LoadImageFromUrl(PictureBox pictureBox, string url)
    {
        try
        {
            using (WebClient wc = new WebClient())
            {
                byte[] imageBytes = wc.DownloadData(url);
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    pictureBox.Image = Image.FromStream(ms);
                }
            }
        }
        catch
        {
            pictureBox.BackColor = Color.Gray;
        }
    }

    /// <summary>
    /// Récupérer la version actuelle de windows
    /// </summary>
    public static string GetWindowsVersion()
    {
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
        {
            if (key != null)
            {
                string productName = key.GetValue("ProductName")?.ToString() ?? "Unknown";
                // Sur Windows 11, 'DisplayVersion' est souvent présente, sinon 'ReleaseId'
                string version = key.GetValue("DisplayVersion")?.ToString() 
                                ?? key.GetValue("ReleaseId")?.ToString() 
                                ?? "";
                string build = key.GetValue("CurrentBuild")?.ToString() ?? "";
                
                if (!string.IsNullOrEmpty(version))
                {
                    return $"{productName} {version} (Build {build})";
                }
                else
                {
                    return $"{productName} (Build {build})";
                }
            }
        }
        return "Unknown";
    }

    /// <summary>
    /// Gestion du clic sur le label des privilèges : si l'utilisateur n'est pas administrateur,
    /// il est invité à relancer l'application en mode administrateur.
    /// </summary>
    private void LblPrivilege_Click(object sender, EventArgs e)
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
}
