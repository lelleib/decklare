namespace DbgLib.Tests.DominionDbgSampleGenerated;

using CardPredicate = Predicate<Card>;
using Number = Int32;

public class DbgEnvironment : DbgEnvironmentBase
{
    private Gameplay gamePlay = new Gameplay();
    public DbgEnvironment(IDbgRuntime runtime) : base(runtime)
    { }

    public void InitDominion(int playerCount)
    { // AllPlayers; Supply, Trash, CenterPile
      // AllOtherPlayers, LeftPlayer, RightPlayer; Action, Buy, Coin, Discount, Victory; Deck, Hand, Discard, InPlay
        gamePlay = new Gameplay();
        for (int i = 0; i < playerCount; i++)
        {
            gamePlay.AllPlayers.Add(new Player
            {
                Action = 1,
                Buy = 1,
                Coin = 0,
                Discount = 0,
                Victory = 0,
                Deck = new Pile(), // TODO starting deck
                Hand = new Pile(),
                Discard = new Pile(),
                InPlay = new Pile()
            });
        }

        gamePlay.Supply = new Supply();
        gamePlay.Trash = new Pile();
        gamePlay.CenterPile = new Pile();

        // Populating supply
        // ...

        // Setting player relation properties
        for (int i = 0; i < playerCount; i++)
        {
            var player = gamePlay.AllPlayers[playerCount];
            player.AllOtherPlayers = gamePlay.AllPlayers.Where((p, j) => j != i).ToList();
            player.RightPlayer = playerCount == 0 ? gamePlay.Players.Last() : gamePlay.Players[playerCount - 1];
            player.LeftPlayer = playerCount == gamePlay.Players.Count - 1 ? gamePlay.Players.First() : gamePlay.Players[playerCount + 1];
        }
    }
}

public class Card : CardBase
{ }

public class Game
{ }

public class Gameplay
{
    public List<Player> AllPlayers { get; set; } = new List<Player>();
    public Supply Supply { get; set; } = new Supply();
    public Pile Trash { get; set; } = new Pile();
    public Pile CenterPile { get; set; } = new Pile();
}

public class Pile : PileBase
{ }

public class Player : PlayerBase
{
    public List<Player> AllOtherPlayers { get; set; } = new List<Player>();
    public Player LeftPlayer { get; set; } = new Player();
    public Player RightPlayer { get; set; } = new Player();
    public Number Action { get; set; } = 0;
    public Number Buy { get; set; } = 0;
    public Number Coin { get; set; } = 0;
    public Number Discount { get; set; } = 0;
    public Number Victory { get; set; } = 0;
    public Pile Deck { get; set; } = new Pile();
    public Pile Hand { get; set; } = new Pile();
    public Pile Discard { get; set; } = new Pile();
    public Pile InPlay { get; set; } = new Pile();
}

public class Supply : SupplyBase
{ }
