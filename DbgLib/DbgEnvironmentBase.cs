namespace DbgLib;

using CardPredicate = Predicate<CardBase>;
using Number = Int32;
using TakeCommand = Func<PileBase?, PileBase?>;
using PutCommand = Action<PileBase?, PileBase?>;

public abstract class DbgEnvironmentBase
{
    private static Random rng = new Random();

    private IDbgRuntime runtime;

    public DbgEnvironmentBase(IDbgRuntime runtime)
    {
        this.runtime = runtime;
    }

    public CardBase? _1From2And3To4(TakeCommand? takeCommand, PileBase? fromPile, PutCommand? putCommand, PileBase? toPile)
    {
        if (takeCommand is null || fromPile is null || putCommand is null || toPile is null)
            return null;

        var pileToMove = takeCommand(fromPile);
        putCommand(fromPile, toPile);
        return default!;
    }

    public PileBase? Pop12(Number? popCount, PileBase? fromPile)
    {
        if (popCount is null || fromPile is null)
            return null;

        return default!;
    }

    public PileBase? Let1Choose23(PlayerBase? player, Number? chooseCount, PileBase? fromPile)
    {
        if (player is null || chooseCount is null || fromPile is null)
            return null;

        return default!;
    }

    public PileBase? Let1Choose2Where34(PlayerBase? player, Number? chooseCount, CardPredicate? wherePredicate, PileBase? fromPile)
    {
        if (player is null || chooseCount is null || wherePredicate is null || fromPile is null)
            return null;

        return default!;
    }

    public PileBase? TakeAll1(PileBase? fromPile)
    {
        if (fromPile is null)
            return null;

        return default!;
    }

    public PileBase? TakeAllWhere12(CardPredicate? wherePredicate, PileBase? fromPile)
    {
        if (wherePredicate is null || fromPile is null)
            return null;

        return default!;
    }

    public void Put12(PileBase? pile, PileBase? toPile)
    {
        if (pile is null || toPile is null)
            return;

        // TODO implementation
    }

    public void Let1Put2Anywhere3(PlayerBase? player, PileBase? pile, PileBase? toPile)
    {
        if (player is null || pile is null || toPile is null)
            return;

        // TODO implementation
    }

    public void Let1Arrange2(PlayerBase? player, PileBase? pile)
    {
        if (player is null || pile is null)
            return;

        // TODO implementation
        runtime.ArrangePile(pile, player);
    }

    public void Shuffle1(PileBase? pile)
    {
        if (pile is null)
            return;

        pile.Cards.OrderBy(c => rng.Next());
    }

    public void Rotate1(PileBase? pile)
    {
        if (pile is null)
            return;

        var first = pile.Cards[0];
        pile.Cards.RemoveAt(0);
        pile.Cards.Add(first);
    }

    public void Execute1(Action? effect)
    {
        if (effect is null)
            return;

        effect.Invoke();
    }

    public void Repeat1(Action effect)
    {
        while (true)
        {
            effect.Invoke();
        }
    }

    public void For12(List<PlayerBase>? players, Action? effect)
    {
        if (players is null || effect is null)
            return;

        foreach (var player in players)
        {
            player.setPlayerContext();
            effect.Invoke();
        }
    }

    public void Let1ChooseIf2(PlayerBase? player, Action? effect)
    {
        if (player is null || effect is null)
            return;

        // TODO
    }

    public void Let1Choose2From3And4(PlayerBase? player, Number? choiceNumber, Action? effect1, Action? effect2)
    {
        if (player is null || choiceNumber is null || effect1 is null|| effect2 is null)
            return;

        // TODO
    }

    public void While12(Boolean? condition, Action? effect)
    {
        if (condition is null || effect is null)
            return;

        while ((bool)condition)
        {
            effect.Invoke();
        }
    }

    public PileBase NewPile()
    {
        return new PileBase();
    }

    public void If12(Boolean? condition, Effect? effect)
    {
        if (condition is null || effect is null)
            return;

        if ((bool)condition)
        {
            effect.Invoke();
        }
    }
}