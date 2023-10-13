namespace DbgLib;

using Number = Int32;

public class Pile
{
    public List<CardBase> _Cards { get; set; } = new List<CardBase>();

    public CardBase? _TopCard
    {
        get
        {
            return _Cards.FirstOrDefault();
        }
    }

    public VISIBILITY Visibility { get; set; } = VISIBILITY.AllVisible;

    public List<PlayerBase> Viewers { get; set; } = new List<PlayerBase>();

    public Number Count
    {
        get
        {
            return _Cards.Count;
        }
    }
}
