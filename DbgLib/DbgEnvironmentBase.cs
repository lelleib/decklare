namespace DbgLib;

using CardPredicate = Predicate<CardBase>;
using Number = Int32;
using TakeCommand = Func<Pile?, Pile?>;
using PutCommand = Action<Pile?, Pile?>;
using Effect = Action;

public abstract class DbgEnvironmentBase
{
    private class BreakException : Exception { }

    private static readonly Random rng = new();

    private readonly IDbgRuntime runtime;

    private Effect? program;

    public DbgEnvironmentBase(IDbgRuntime runtime)
    {
        this.runtime = runtime;
    }

    protected void InitProgram(Effect program)
    {
        this.program = program;
    }

    public void Run()
    {
        if (program is null)
            return;

        try
        {
            program();
        }
        catch (BreakException)
        { }
    }

    public CardBase? _1From2And3To4(TakeCommand? takeCommand, Pile? fromPile, PutCommand? putCommand, Pile? toPile)
    {
        if (takeCommand is null || fromPile is null || putCommand is null || toPile is null)
            return null;

        var pileToMove = takeCommand(fromPile);
        putCommand(fromPile, toPile);
        return default!;
    }

    public Pile? _Pop12(Number? popCount, Pile? fromPile)
    {
        if (popCount is null || fromPile is null)
            return null;

        return default!;
    }

    public Pile? _Let1Choose23(PlayerBase? player, Number? chooseCount, Pile? fromPile)
    {
        if (player is null || chooseCount is null || fromPile is null)
            return null;

        return default!;
    }

    public Pile? _Let1Choose2Where34(PlayerBase? player, Number? chooseCount, CardPredicate? wherePredicate, Pile? fromPile)
    {
        if (player is null || chooseCount is null || wherePredicate is null || fromPile is null)
            return null;

        return default!;
    }

    public Pile? _TakeAll1(Pile? fromPile)
    {
        if (fromPile is null)
            return null;

        return default!;
    }

    public Pile? _TakeAllWhere12(CardPredicate? wherePredicate, Pile? fromPile)
    {
        if (wherePredicate is null || fromPile is null)
            return null;

        return default!;
    }

    public void _Put12(Pile? pile, Pile? toPile)
    {
        if (pile is null || toPile is null)
            return;

        // TODO implementation
    }

    public void _Let1Put2Anywhere3(PlayerBase? player, Pile? pile, Pile? toPile)
    {
        if (player is null || pile is null || toPile is null)
            return;

        // TODO implementation
    }

    public void _Let1Arrange2(PlayerBase? player, Pile? pile)
    {
        if (player is null || pile is null)
            return;

        // TODO implementation
        runtime.ArrangePile(pile, player);
    }

    public void _Shuffle1(Pile? pile)
    {
        if (pile is null)
            return;

        pile.Cards.OrderBy(c => rng.Next());
    }

    public void _Rotate1(Pile? pile)
    {
        if (pile is null)
            return;

        var first = pile.Cards[0];
        pile.Cards.RemoveAt(0);
        pile.Cards.Add(first);
    }

    public void _Execute1(Effect? effect)
    {
        if (effect is null)
            return;

        effect.Invoke();
    }

    public void _Break()
    {
        throw new BreakException();
    }

    public void _Repeat1(Effect effect)
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

    public void _For12(PlayerBase[]? players, Effect? effect)
    {
        if (players is null || effect is null)
            return;

        try
        {
            foreach (var player in players)
            {
                player.SetPlayerContext();
                effect.Invoke();
            }
        }
        catch (BreakException)
        { }
        finally
        {
            // TODO switch context back
        }
    }

    public void _Let1ChooseIf2(PlayerBase? player, Effect? effect)
    {
        if (player is null || effect is null)
            return;

        // TODO
    }

    public void _Let1Choose2From3And4(PlayerBase? player, Number? choiceNumber, Effect? effect1, Effect? effect2)
    {
        if (player is null || choiceNumber is null || effect1 is null || effect2 is null)
            return;

        // TODO
    }

    public void _While12(Boolean? condition, Effect? effect)
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

    public Pile _NewPile()
    {
        return new Pile();
    }

    public void _If12(Boolean? condition, Effect? effect)
    {
        if (condition is null || effect is null)
            return;

        if ((bool)condition)
        {
            effect.Invoke();
        }
    }
}