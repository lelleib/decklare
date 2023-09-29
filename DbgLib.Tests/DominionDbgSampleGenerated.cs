namespace DbgLib.Tests.DominionDbgSampleGenerated;

using CardPredicate = Predicate<Card>;
using Number = Int32;
using Effect = Action;

public class DbgEnvironment : DbgEnvironmentBase
{
    private Gameplay gamePlay = new Gameplay();

    // public Supply? Supply; // TODO impl
    public Pile? Trash;
    public Pile? CenterPile;
    public Pile? Hand;
    public Pile? Deck;
    public Pile? Discard;
    public Pile? InPlay;
    public Pile? This;
    public Number? Action;
    public Number? Buy;
    public Number? Coin;
    public Number? Victory;
    public Number? Discount;
    public Player? Me;
    public Player? ActivePlayer;
    public Player? LeftPlayer;
    public Player? RightPlayer;
    public Player[]? AllPlayers;
    public Player[]? AllOtherPlayers;

    public DbgEnvironment(IDbgRuntime _runtime) : base(_runtime)
    {
        void program()
        {
            _InitDominion
            (
            3
            );

            _Repeat1
            (
            () =>
            {
                _For12
                (
                AllPlayers,
                () =>
                {
                    _While12
                    (
                    Action > 0,
                    () =>
                    {
                        _Let1Choose2From3And4
                        (
                        Me,
                        1,
                        () =>
                        {
                            var card_to_play = _NewPile();

                            _1From2And3To4
                            (
                            (_fromPile) =>
                                _Let1Choose2Where34
                                (
                                Me,
                                1,
                                (_x) =>
                                    ((Card)_x).Types.Contains(CardType.Action),
                                _fromPile
                                ),
                            Hand,
                            (_it, _toPile) =>
                                _Put12
                                (
                                _it,
                                _toPile
                                ),
                            card_to_play
                            );
                        },
                        () =>
                        {
                            _Break();
                        }
                        );
                    }
                    );

                    _While12
                    (
                    Buy > 0,
                    () =>
                    {
                        _Let1Choose2From3And4
                        (
                        Me,
                        1,
                        () =>
                        {
                            var card = (Card?)_1From2And3To4
                            (
                            (_fromPile) =>
                                _Let1Choose2Where34
                                (
                                Me,
                                1,
                                (_x) =>
                                    ((Card)_x).Cost - Discount < Coin,
                                _fromPile
                                ),
                            Hand,
                            (_it, _toPile) =>
                                _Put12
                                (
                                _it,
                                _toPile
                                ),
                            Discard
                            );

                            Coin = (Coin - (card?.Cost - Discount));
                        },
                        () =>
                        {
                            _Break();
                        }
                        );
                    }
                    );

                    _1From2And3To4
                    (
                    (_fromPile) =>
                        _TakeAll1
                        (
                        _fromPile
                        ),
                    Hand,
                    (_it, _toPile) =>
                        _Put12
                        (
                        _it,
                        _toPile
                        ),
                    Discard
                    );

                    _1From2And3To4
                    (
                    (_fromPile) =>
                        _TakeAll1
                        (
                        _fromPile
                        ),
                    InPlay,
                    (_it, _toPile) =>
                        _Put12
                        (
                        _it,
                        _toPile
                        ),
                    Discard
                    );

                    _1From2And3To4
                    (
                    (_fromPile) =>
                        _Pop12
                        (
                        5,
                        _fromPile
                        ),
                    Deck,
                    (_it, _toPile) =>
                        _Put12
                        (
                        _it,
                        _toPile
                        ),
                    Hand
                    );

                    Action = 1;

                    Buy = 1;

                    Coin = 0;

                    Discount = 0;

                    _If12
                    (
                    true /*TODO win condition*/,
                    () =>
                    {
                        _Break();
                    }
                    );
                }
                );

                _If12
                (
                true /*TODO win condition*/,
                () =>
                {
                    _Break();
                }
                );
            }
            );
        }

        InitProgram(program);
    }

    private void _InitDominion(int playerCount)
    { // AllPlayers; Supply, Trash, CenterPile
      // AllOtherPlayers, LeftPlayer, RightPlayer; Action, Buy, Coin, Discount, Victory; Deck, Hand, Discard, InPlay
        gamePlay = new Gameplay();
        for (int i = 0; i < playerCount; i++)
        {
            gamePlay.AllPlayers.Add(new Player(this)
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
            player.AllOtherPlayers = gamePlay.AllPlayers.Where((p, j) => j != i).ToArray();
            player.RightPlayer = playerCount == 0 ? gamePlay.AllPlayers.Last() : gamePlay.AllPlayers[playerCount - 1];
            player.LeftPlayer = playerCount == gamePlay.AllPlayers.Length - 1 ? gamePlay.AllPlayers.First() : gamePlay.AllPlayers[playerCount + 1];
        }
    }
}

public class Card : CardBase
{
    public CardName Name { get; set; }
    public CardType[] Types { get; set; } = new CardType[0];
    public Number Cost { get; set; }
    public Effect ActionEffect { get; set; } = () => { };
    public Effect TreasureEffect { get; set; } = () => { };
    public Effect ReactionEffect { get; set; } = () => { };
    public Effect VictoryEffect { get; set; } = () => { };
}

public class Game
{ }

public class Gameplay
{
    public Player[] AllPlayers { get; set; } = new Player[0];
    public Supply Supply { get; set; } = new Supply();
    public Pile Trash { get; set; } = new Pile();
    public Pile CenterPile { get; set; } = new Pile();
}

public class Pile : PileBase
{ }

public class Player : PlayerBase
{
    private DbgEnvironment environment;

    public Player[]? AllOtherPlayers { get; set; }
    public Player? LeftPlayer { get; set; }
    public Player? RightPlayer { get; set; }
    public Number? Action { get; set; }
    public Number? Buy { get; set; }
    public Number? Coin { get; set; }
    public Number? Discount { get; set; }
    public Number? Victory { get; set; }
    public Pile? Deck { get; set; }
    public Pile? Hand { get; set; }
    public Pile? Discard { get; set; }
    public Pile? InPlay { get; set; }

    public Player(DbgEnvironment environment)
    {
        this.environment = environment;
    }

    public override void SetPlayerContext()
    {
        environment.AllOtherPlayers = AllOtherPlayers;
        environment.LeftPlayer = LeftPlayer;
        environment.RightPlayer = LeftPlayer;

        environment.Action = Action;
        environment.Buy = Buy;
        environment.Coin = Coin;
        environment.Discount = Discount;
        environment.Victory = Victory;

        environment.Deck = Deck;
        environment.Hand = Hand;
        environment.Discard = Discard;
        environment.InPlay = InPlay;
    }
}

public class Supply : SupplyBase
{ }

public enum CardType
{
    Action,
    Attack,
    Reaction,
    Treasure,
    Victory
}

public enum CardName
{ }

public enum Visibility
{
    AllVisible,
    TopVisible,
    NoneVisible
}