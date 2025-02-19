
/// <summary>
/// Ajoutez une hitbox transparente pour afficher un message sur la combobox lorsque la case DisableDefender est désactivée.
/// </summary>
namespace OmniTools
{
    public class TransparentPanel : Panel
    {
        public TransparentPanel()
        {
            // Indique que ce contrôle supporte une couleur d'arrière-plan transparente
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
                // Ne rien peindre ici pour conserver la transparence
        }
    }
}

