namespace DbgLib;
public abstract class GameBase
{
    public abstract PlayerBase[] _Players { get; }
    public abstract Pile[] _CommonPiles { get; }
}
