namespace DbgLib;
public class Pile
{
    public List<CardBase> Cards { get; set; } = new List<CardBase>();
    public Visibility Visibility { get; set; } = Visibility.AllVisible;
    public List<PlayerBase> Viewers { get; set; } = new List<PlayerBase>();
}
