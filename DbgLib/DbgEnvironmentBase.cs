namespace DbgLib;

using CardPredicate = Predicate<CardBase>;
using NumberPredicate = Predicate<Int32>;
using Number = Int32;
using TakeCommand = Func<Pile?, Pile?>;
using PutCommand = Action<Pile?, Pile?>;
using Effect = Action;

public abstract class DbgEnvironmentBase
{
    private class BreakException : Exception { }

    private static readonly Random rng = new();

    private readonly IDbgRuntime runtime;

    public DbgEnvironmentBase(IDbgRuntime runtime)
    {
        this.runtime = runtime;
    }

    protected abstract void Program();
    protected abstract void SetPlayerContext(PlayerBase playerBase);
    protected abstract void UnsetPlayerContext();

    public void Run()
    {
        try
        {
            Program();
        }
        catch (BreakException)
        { }
    }

    protected CardBase? _1From2And3To4(TakeCommand? takeCommand, Pile? fromPile, PutCommand? putCommand, Pile? toPile)
    {
        if (takeCommand is null || fromPile is null || putCommand is null || toPile is null)
            return null;

        var pileToMove = takeCommand(fromPile);
        putCommand(pileToMove, toPile);
        return pileToMove?._TopCard;
    }

    protected Pile? _Pop12(Number? popCount, Pile? fromPile)
    {
        if (popCount is null || fromPile is null)
            return null;

        var popped = fromPile._Cards.GetRange(0, (int)popCount);
        fromPile._Cards.RemoveRange(0, (int)popCount);
        return new Pile { _Cards = popped };
    }

    protected Pile? _Let1Choose23(PlayerBase? player, Number? choiceCount, Pile? fromPile)
    {
        if (player is null || choiceCount is null || fromPile is null)
            return null;

        int[] chosenIndices = runtime.ChooseFromPile(player, fromPile, (int)choiceCount, (_) => true);
        var newPile = new Pile() { _Cards = chosenIndices.Select((i) => fromPile._Cards[i]).ToList() };
        fromPile._Cards.RemoveAll((c) => newPile._Cards.Contains(c));
        return newPile;
    }

    protected Pile? _Let1Choose2Where34(PlayerBase? player, Number? choiceCount, CardPredicate? wherePredicate, Pile? fromPile)
    {
        if (player is null || choiceCount is null || wherePredicate is null || fromPile is null)
            return null;

        int[] chosenIndices = runtime.ChooseFromPile(player, fromPile, (int)choiceCount, wherePredicate);
        var newPile = new Pile() { _Cards = chosenIndices.Select((i) => fromPile._Cards[i]).ToList() };
        fromPile._Cards.RemoveAll((c) => newPile._Cards.Contains(c));
        return newPile;
    }

    protected Pile? _TakeAll1(Pile? fromPile)
    {
        if (fromPile is null)
            return null;

        List<CardBase> taken = new(fromPile._Cards);
        fromPile._Cards.Clear();
        return new Pile { _Cards = taken };
    }

    protected Pile? _TakeAllWhere12(CardPredicate? wherePredicate, Pile? fromPile)
    {
        if (wherePredicate is null || fromPile is null)
            return null;

        var results = fromPile._Cards.ToLookup((x) => wherePredicate(x));
        fromPile._Cards = results[false].ToList();
        return new Pile { _Cards = results[true].ToList() };
    }

    protected void _Put12(Pile? pile, Pile? toPile)
    {
        if (pile is null || toPile is null)
            return;

        toPile._Cards.InsertRange(0, pile._Cards);
    }

    protected void _Let1Put2Anywhere3(PlayerBase? player, Pile? pile, Pile? toPile)
    {
        if (player is null || pile is null || toPile is null)
            return;

        runtime.PutPileAnywhereToAnotherPile(player, pile, toPile);
    }

    protected void _Let1Arrange2(PlayerBase? player, Pile? pile)
    {
        if (player is null || pile is null)
            return;

        runtime.ArrangePile(player, pile);
    }

    protected void _Shuffle1(Pile? pile)
    {
        if (pile is null)
            return;

        pile._Cards = pile._Cards.OrderBy(c => rng.Next()).ToList();
    }

    protected void _Rotate1(Pile? pile)
    {
        if (pile is null)
            return;

        var first = pile._Cards[0];
        pile._Cards.RemoveAt(0);
        pile._Cards.Add(first);
    }

    protected void _Execute1(Effect? effect)
    {
        if (effect is null)
            return;

        effect.Invoke();
    }

    protected void _Break()
    {
        throw new BreakException();
    }

    protected void _Repeat1(Effect effect)
    {
        try
        {
            while (true)
            {
                effect.Invoke();
            }
        }
        catch (BreakException)
        { }
    }

    protected void _For12(PlayerBase[]? players, Effect? effect)
    {
        if (players is null || effect is null)
            return;

        try
        {
            foreach (var player in players)
            {
                SetPlayerContext(player);
                effect.Invoke();
            }
        }
        catch (BreakException)
        { }
        finally
        {
            UnsetPlayerContext();
        }
    }

    protected void _Let1ChooseIf2(PlayerBase? player, Effect? effect)
    {
        if (player is null || effect is null)
            return;

        int choice = runtime.ChooseFromOptions(player, 2, 1)[0];
        if (choice == 0)
        {
            effect.Invoke();
        }
    }

    protected void _Let1Choose2From3And4(PlayerBase? player, Number? choiceNumber, Effect? effect1, Effect? effect2)
    {
        if (player is null || choiceNumber is null || effect1 is null || effect2 is null)
            return;

        int choice = runtime.ChooseFromOptions(player, 2, 1)[0];
        if (choice == 0)
        {
            effect1.Invoke();
        }
        else
        {
            effect2.Invoke();
        }
    }

    protected void _While12(Boolean? condition, Effect? effect)
    {
        if (condition is null || effect is null)
            return;

        try
        {
            while ((bool)condition)
            {
                effect.Invoke();
            }
        }
        catch (BreakException)
        { }
    }

    protected Pile _NewPile()
    {
        return new Pile();
    }

    protected void _If12(Boolean? condition, Effect? effect)
    {
        if (condition is null || effect is null)
            return;

        if ((bool)condition)
        {
            effect.Invoke();
        }
    }

    protected Number? _Let1ChooseNumberWhere2(PlayerBase? player, NumberPredicate? wherePredicate)
    {
        if (player is null || wherePredicate is null)
            return null;

        return runtime.ChooseNumber(player, wherePredicate);
    }
}