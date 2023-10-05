namespace DbgLib;
public class Pile
{
    public List<CardBase> Cards { get; set; } = new List<CardBase>();
    public VISIBILITY Visibility { get; set; } = VISIBILITY.AllVisible;
    public List<PlayerBase> Viewers { get; set; } = new List<PlayerBase>();
}
