using gui.Model.Managers.CardManager;
using System.Windows.Media;

public class CardImageViewModel
{
    public Card Card { get; }
    public ImageSource Image { get; }

    public CardImageViewModel(Card card, ImageSource image)
    {
        Card = card;
        Image = image;
    }
}
