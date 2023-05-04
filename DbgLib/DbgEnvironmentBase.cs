namespace DbgLib;

using CardPredicate = Predicate<CardBase>;
using Number = Int32;

public abstract class DbgEnvironmentBase
{
    private static Random rng = new Random();

    private IDbgRuntime runtime;

    public DbgEnvironmentBase(IDbgRuntime runtime)
    {
        this.runtime = runtime;
    }

    public Card 1From2And3To4()
    {
        return default!;
    }

    public PileBase Pop12(Number popCount, PileBase fromPile)
    {
        return default!;
    }

    public PileBase Let1Choose23(PlayerBase player, Number chooseCount, PileBase fromPile)
    {
        return default!;
    }

    public PileBase Let1Choose2Where34(PlayerBase player, Number chooseCount, CardPredicate wherePredicate, PileBase fromPile)
    {
        return default!;
    }

    public PileBase TakeAll1(PileBase fromPile)
    {
        return default!;
    }

    public PileBase TakeAllWhere12(CardPredicate wherePredicate, PileBase fromPile)
    {
        return default!;
    }

    public PileBase Put12(PileBase pile, PileBase toPile)
    {
        return default!;
    }

    public PileBase Let1Put2Anywhere3(PlayerBase player, PileBase pile, PileBase toPile)
    {
        return default!;
    }

    public void Let1Arrange2(PlayerBase player, PileBase pile)
    {
        runtime.ArrangePile(pile, player);
    }

    public void Shuffle1(PileBase pile)
    {
        pile.Cards.OrderBy(c => rng.Next());
    }

    public void Rotate1(PileBase pile)
    {
        var first = pile.Cards[0];
        pile.Cards.RemoveAt(0);
        pile.Cards.Add(first);
    }

    public void Execute1(Action effect)
    {
        effect.Invoke();
    }
}