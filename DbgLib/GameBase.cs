namespace DbgLib;
public abstract class GameBase
{
    public abstract PlayerBase[] _Players { get; }
    public abstract KeyValuePair<string, Pile>[] _CommonPiles { get; }
}
