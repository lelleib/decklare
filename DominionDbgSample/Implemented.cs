namespace DominionDbgSample.Implemented;

using DbgLib;
using DominionDbgSample.Generated;

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
        PrintGameForPlayer(player, 0);
        PrintIndented("=== ARRANGE PILE ===", 0);
        PrintIndented("The pile:", 1);
        PrintPileForPlayer(new KeyValuePair<string, Pile>("unnamed", pile), player, 1);
        var cards = new CardBase[pile.Count];
        for (int i = 0; i < pile.Count; i++)
        {
            PrintIndented($"The {i}. card (index in pile of unchosen cards): ", 1);
            int result;
            while (!int.TryParse(Console.ReadLine(), out result) || !(result < pile.Count - i))
            {
                PrintIndented("Not a valid choice, try again: ", 1);
            }
            cards[i] = pile._Cards[result];
        }
        pile._Cards = cards.ToList();
    }

    public void PutPileAnywhereToAnotherPile(PlayerBase player, Pile pile, Pile anotherPile)
    {
        PrintGameForPlayer(player, 0);
        PrintIndented("=== PUT PILE ANYWHERE TO ANOTHER PILE ===", 0);
        PrintIndented("The pile:", 1);
        PrintPileForPlayer(new KeyValuePair<string, Pile>("unnamed1", pile), player, 1);
        PrintIndented("The another pile:", 1);
        PrintPileForPlayer(new KeyValuePair<string, Pile>("unnamed2", anotherPile), player, 1);
        for (int i = 0; i < pile._Cards.Count; i++)
        {
            PrintIndented($"The {i}. card's position in the another pile (between 0 and {anotherPile.Count + 1}): ", 1);
            int position;
            while (!int.TryParse(Console.ReadLine(), out position) || !(position < anotherPile.Count + 1 && position >= 0))
            {
                PrintIndented("Not a valid choice, try again: ", 1);
            }
            anotherPile._Cards.Insert(position, pile._Cards[i]);
        }
        pile._Cards.Clear();
    }

    public int[] ChooseFromPile(PlayerBase player, Pile pile, int choiceCount, Predicate<CardBase> predicate)
    {
        PrintGameForPlayer(player, 0);
        PrintIndented("=== CHOOSE FROM PILE ===", 0);
        PrintIndented("The pile to choose from:", 1);
        PrintPileForPlayer(new KeyValuePair<string, Pile>("unnamed", pile), player, 1);
        int optionCount = pile.Count;
        int[] choices = Enumerable.Repeat(-1, choiceCount).ToArray();
        for (int i = 0; i < choiceCount; i++)
        {
            PrintIndented($"Choose the {i}. card out of {choiceCount} (between 0 and {optionCount - 1}): ", 1);
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || !(choice < optionCount && choice >= 0) || choices.Contains(choice) || !predicate(pile._Cards[choice]))
            {
                PrintIndented("Not a valid choice, try again: ", 1);
            }
            choices[i] = choice;
        }
        return choices;
    }

    public int[] ChooseFromOptions(PlayerBase player, int optionsCount, int choiceCount)
    {
        PrintGameForPlayer(player, 0);
        PrintIndented("=== CHOOSE FROM OPTIONS ===", 0);
        int[] choices = Enumerable.Repeat(-1, choiceCount).ToArray();
        for (int i = 0; i < choiceCount; i++)
        {
            PrintIndented($"Choose the {i}. option out of {choiceCount} (between 0 and {optionsCount - 1}): ", 1);
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || !(choice < optionsCount && choice >= 0))
            {
                PrintIndented("Not a valid choice, try again: ", 1);
            }
            choices[i] = choice;
        }
        return choices;
    }

    public int[] ChooseDistinctFromOptions(PlayerBase player, int optionCount, int choiceCount)
    {
        PrintGameForPlayer(player, 0);
        PrintIndented("=== CHOOSE DISTINCT FROM OPTIONS ===", 0);
        int[] choices = Enumerable.Repeat(-1, choiceCount).ToArray();
        for (int i = 0; i < choiceCount; i++)
        {
            PrintIndented($"Choose the {i}. option out of {choiceCount} (between 0 and {optionCount - 1}): ", 1);
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || !(choice < optionCount && choice >= 0) || choices.Contains(choice))
            {
                PrintIndented("Not a valid choice, try again: ", 1);
            }
            choices[i] = choice;
        }
        return choices;
    }

    public int ChooseNumber(PlayerBase player, Predicate<int> predicate)
    {
        PrintGameForPlayer(player, 0);
        PrintIndented("=== CHOOSE NUMBER ===", 0);
        PrintIndented("Choose a number: ", 1);
        int result;
        while (!int.TryParse(Console.ReadLine(), out result) || !predicate(result))
        {
            PrintIndented("Not a valid choice, try again: ", 1);
        }
        return result;
    }

    private void PrintGameForPlayer(PlayerBase activePlayer, int indentLevel)
    {
        if (game is null)
            return;

        PrintIndented("=== GAME STATE ===", indentLevel);
        PrintIndented("COMMON PILES:", indentLevel);
        foreach (var pile in game._CommonPiles)
        {
            PrintPileForPlayer(pile, activePlayer, indentLevel + 1);
        }
        int playerIndex = 1;
        foreach (var player in game._Players)
        {
            PrintIndented($"Player#{playerIndex++}:", indentLevel);
            foreach (var pile in player._Piles)
            {
                PrintPileForPlayer(pile, activePlayer, indentLevel + 1);
            }
        }
    }

    private void PrintPileForPlayer(KeyValuePair<string, Pile> namedPile, PlayerBase player, int indentLevel)
    {
        var pileName = namedPile.Key;
        var pile = namedPile.Value;
        if (pile.Count == 0)
        {
            PrintIndented($"Pile '{pileName}' (empty)", indentLevel);
        }
        else
        {
            PrintIndented($"Pile '{pileName}':", indentLevel);
            int cardIndex = 0;
            foreach (var card in pile._Cards)
            {
                if (pile.Viewers.Contains(player) &&
                    (pile.Visibility == VISIBILITY.AllVisible ||
                        (pile.Visibility == VISIBILITY.TopVisible && cardIndex == 0)))
                {
                    PrintIndented($"Card{cardIndex++}:", indentLevel + 1);
                    foreach (var prop in card._Properties)
                    {
                        PrintIndented(string.Format("{0,-10}:{1,10}", prop.Key, prop.Value), indentLevel + 2);
                    }
                }
                else
                {
                    PrintIndented($"Card{cardIndex++} (not visible)", indentLevel + 1);
                }
            }
        }
    }

    private static void PrintIndented(string line, int indentLevel)
    {
        Console.WriteLine(line.PadLeft(line.Length + indentLevel * indentSpan));
    }
}
