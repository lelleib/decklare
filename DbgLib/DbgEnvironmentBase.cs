namespace DbgLib;

using CardPredicate = Predicate<CardBase>;
using Number = Int32;
using TakeCommand = Func<PileBase?, PileBase?>;
using PutCommand = Action<PileBase?, PileBase?>;
using Effect = Action;

public abstract class DbgEnvironmentBase
{
    protected class BreakException : Exception { }

    private static Random rng = new Random();

    private IDbgRuntime runtime;

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

    public CardBase? _1From2And3To4(TakeCommand? takeCommand, PileBase? fromPile, PutCommand? putCommand, PileBase? toPile)
    {
        if (takeCommand is null || fromPile is null || putCommand is null || toPile is null)
            return null;

        var pileToMove = takeCommand(fromPile);
        putCommand(fromPile, toPile);
        return default!;
    }

    public PileBase? _Pop12(Number? popCount, PileBase? fromPile)
    {
        if (popCount is null || fromPile is null)
            return null;

        return default!;
    }

    public PileBase? _Let1Choose23(PlayerBase? player, Number? chooseCount, PileBase? fromPile)
    {
        if (player is null || chooseCount is null || fromPile is null)
            return null;

        return default!;
    }

    public PileBase? _Let1Choose2Where34(PlayerBase? player, Number? chooseCount, CardPredicate? wherePredicate, PileBase? fromPile)
    {
        if (player is null || chooseCount is null || wherePredicate is null || fromPile is null)
            return null;

        return default!;
    }

    public PileBase? _TakeAll1(PileBase? fromPile)
    {
        if (fromPile is null)
            return null;

        return default!;
    }

    public PileBase? _TakeAllWhere12(CardPredicate? wherePredicate, PileBase? fromPile)
    {
        if (wherePredicate is null || fromPile is null)
            return null;

        return default!;
    }

    public void _Put12(PileBase? pile, PileBase? toPile)
    {
        if (pile is null || toPile is null)
            return;

        // TODO implementation
    }

    public void _Let1Put2Anywhere3(PlayerBase? player, PileBase? pile, PileBase? toPile)
    {
        if (player is null || pile is null || toPile is null)
            return;

        // TODO implementation
    }

    public void _Let1Arrange2(PlayerBase? player, PileBase? pile)
    {
        if (player is null || pile is null)
            return;

        // TODO implementation
        runtime.ArrangePile(pile, player);
    }

    public void _Shuffle1(PileBase? pile)
    {
        if (pile is null)
            return;

        pile.Cards.OrderBy(c => rng.Next());
    }

    public void _Rotate1(PileBase? pile)
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

    public void _For12(List<PlayerBase>? players, Effect? effect)
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

    public PileBase _NewPile()
    {
        return new PileBase();
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