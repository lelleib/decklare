namespace DominionDbgSample.Implemented;

using DbgLib;

public class TestDbgRuntime : IDbgRuntime
{
    public Game Game { init => throw new NotImplementedException(); }

    public void ArrangePile(PlayerBase player, Pile pile)
    {
        throw new NotImplementedException();
    }

    public int ChooseFromList(PlayerBase player, int[] list)
    {
        throw new NotImplementedException();
    }

    public int[] ChooseFromPile(PlayerBase player, Pile pile, int number)
    {
        throw new NotImplementedException();
    }

    public int ChooseNumber(PlayerBase player, Predicate<int> predicate)
    {
        throw new NotImplementedException();
    }
}
