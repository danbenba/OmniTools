using System;
using System.Drawing;

namespace OmniTools
{
    /// <summary>
    /// Classe utilitaire pour la gestion des logs.
    /// </summary>
    public static class Logger
    {
        public static MainForm MainFormInstance { get; set; }

        /// <summary>
        /// Génère un timestamp au format [HH:mm:ss].
        /// </summary>
        private static string GetTimestamp()
        {
            return $"[{DateTime.Now:HH:mm:ss}]";
        }

        /// <summary>
        /// Log un message d'information.
        /// </summary>
        public static void LogInfo(string message)
        {
            MainFormInstance?.AddLog($"{GetTimestamp()} [INFO] {message}", Color.Black);
        }

        /// <summary>
        /// Log un message du core.
        /// </summary>
        public static void CoreLog(string message)
        {
            MainFormInstance?.AddLog($"{GetTimestamp()} [+] {message}", Color.Black);
        }

        /// <summary>
        /// Log un message d'information générique (LogOut).
        /// </summary>
        public static void LogOut(string message)
        {
            MainFormInstance?.AddLog($"{GetTimestamp()} {message}", Color.Black);
        }

        /// <summary>
        /// Log un message de succès (en vert).
        /// </summary>
        public static void LogSuccess(string message)
        {
            MainFormInstance?.AddLog($"{GetTimestamp()} [SUCCESS] {message}", Color.Green);
        }

        /// <summary>
        /// Log un message d'avertissement (en orange).
        /// </summary>
        public static void LogWarning(string message)
        {
            MainFormInstance?.AddLog($"{GetTimestamp()} [WARNING] {message}", Color.DarkOrange);
        }

        /// <summary>
        /// Log un message d'erreur (en rouge).
        /// </summary>
        public static void LogError(string message)
        {
            MainFormInstance?.AddLog($"{GetTimestamp()} [ERROR] {message}", Color.Red);
        }

        /// <summary>
        /// Balise d'annulation de téléchargement
        /// </summary>
        public static void DownloadCanceledLog(string message)
        {
            MainFormInstance?.AddLog($" {message}", Color.Red);
        }

        /// <summary>
        /// Efface le contenu du RichTextBox des logs.
        /// </summary>
        public static void Clear()
        {
            if (MainFormInstance?.richTextBoxLogs.InvokeRequired == true)
            {
                MainFormInstance.richTextBoxLogs.Invoke(new Action(() => MainFormInstance.richTextBoxLogs.Clear()));
            }
            else
            {
                MainFormInstance?.richTextBoxLogs.Clear();
            }
        }
    }
}
