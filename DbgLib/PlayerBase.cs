namespace DbgLib;
public abstract class PlayerBase
{
    private static int nextId = 0;

    public int _Id { get; } = nextId++;
    public abstract KeyValuePair<string, Pile>[] _Piles { get; }
}
