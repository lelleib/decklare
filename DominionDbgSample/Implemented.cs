namespace DominionDbgSample.Implemented;

using DbgLib;

public class TestDbgRuntime : IDbgRuntime
{
    private static readonly int indentSpan = 4;
    private GameBase? game;

    public void SetGame(GameBase game)
    {
        this.game = game;
    }

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

    private void PrintGameForPlayer(PlayerBase activePlayer)
    {
        if (game is null)
            return;

        PrintIndented("COMMON PILES:", 0);
        foreach (var pile in game._CommonPiles)
        {
            PrintPileForPlayer(pile, activePlayer, 1);
        }
        int playerIndex = 1;
        foreach (var player in game._Players)
        {
            PrintIndented($"Player{playerIndex++}:", 0);
            foreach (var pile in player._Piles)
            {
                PrintPileForPlayer(pile, activePlayer, 1);
            }
        }
    }

    private void PrintPileForPlayer(KeyValuePair<string, Pile> namedPile, PlayerBase player, int indentLevel)
    {
        var pileName = namedPile.Key;
        var pile = namedPile.Value;
        PrintIndented($"Pile '{pileName}':", indentLevel);
        int cardIndex = 1;
        foreach (var card in pile._Cards)
        {
            // TODO write if visible
            PrintIndented($"Card{cardIndex++}:", indentLevel + 1);
            foreach (var prop in card._Properties)
            {
                PrintIndented(string.Format("{0,-10}:{1,10}", prop.Key, prop.Value), indentLevel + 2);
            }
        }
    }

    private static void PrintIndented(string line, int indentLevel)
    {
        Console.WriteLine(line.PadLeft(line.Length + indentLevel * indentSpan));
    }
}
