namespace DbgLib.Tests.DominionDbgSampleGenerated;

using Number = Int32;
using Effect = Action;

public class DbgEnvironment : DbgEnvironmentBase
{
    // Gameplay context
    public Player[]? AllPlayers;
    // public Supply? Supply; // TODO impl
    public Pile? Trash;
    public Pile? CenterPile;
    // Player context
    public Pile? Hand;
    public Pile? Deck;
    public Pile? Discard;
    public Pile? InPlay;
    public Number? Action;
    public Number? Buy;
    public Number? Coin;
    public Number? Victory;
    public Number? Discount;
    public Player? Me;
    public Player? ActivePlayer;
    public Player? LeftPlayer;
    public Player? RightPlayer;
    public Player[]? AllOtherPlayers;
    // Card context
    public Card? ThisCard;

    private readonly Card[] _cards;

    // GENERATED
    public DbgEnvironment(IDbgRuntime _runtime) : base(_runtime)
    {
        _cards = new Card[]
        {
            new() {
                Name = CARDNAME.Cellar,
                Cost = 2,
                Types = new CARDTYPE[]{ CARDTYPE.Action },
                ActionEffect = () =>
                {
                    _Plus12
                    (
                    1,
                    Action
                    );

                    var n = _Let1ChooseNumberWhere2
                    (
                    Me,
                    (_x) =>
                        _x <= Hand?.Count
                    );

                    _Let1Choose2ToDiscard
                    (
                    Me,
                    n
                    );

                    _Draw1
                    (
                    n
                    );
                }
            },

            new() {
                Name = CARDNAME.Bureaucrat,
                Cost = 4,
                Types = new CARDTYPE[]{ CARDTYPE.Action, CARDTYPE.Attack },
                ActionEffect = () =>
                {
                    // GAIN 1 Silver:CARDNAME TO Deck

                    _For12
                    (
                    AllOtherPlayers,
                    () =>
                    {
                        _1From2And3To4
                        (
                        (_fromPile) =>
                            _Let1Choose2Where34
                            (
                            Me,
                            1,
                            (_x) =>
                                ((Card)_x).Types.Contains(CARDTYPE.Victory),
                            _fromPile
                            ),
                        Hand,
                        (_it, _toPile) =>
                            _Put12
                            (
                            _it,
                            _toPile
                            ),
                        Deck
                        );
                    }
                    );
                }
            }
        };
    }

    // GENERATED
    protected override void Program()
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
                ActivePlayer = Me;

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
                                ((Card)_x).Types.Contains(CARDTYPE.Action),
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
                false /*TODO win condition*/,
                () =>
                {
                    _Break();
                }
                );
            }
            );

            _If12
            (
            AllPlayers?.All((player) => player.Deck?.Count == 0) /*TODO win condition*/,
            () =>
            {
                _Break();
            }
            );
        }
        );
    }

    private void _Plus12(Number? n, Number? variable)
    {
        if (n is null || variable is null)
            return;

        variable = variable + n;
    }

    private void _Let1Choose2ToDiscard(Player? player, Number? n)
    {
        _1From2And3To4
        (
        (_fromPile) =>
            _Let1Choose23
            (
            player,
            n,
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
    }

    private void _Draw1(Number? n)
    {
        if (n is null)
            return;

        _1From2And3To4
        (
        (_fromPile) =>
            _Pop12
            (
            n,
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
    }

    private void _InitDominion(int playerCount)
    { // AllPlayers; Supply, Trash, CenterPile
      // AllOtherPlayers, LeftPlayer, RightPlayer; Action, Buy, Coin, Discount, Victory; Deck, Hand, Discard, InPlay
        AllPlayers = new Player[playerCount];
        // Supply = new Supply();
        Trash = new Pile();
        CenterPile = new Pile();

        // Initializing players
        for (int i = 0; i < playerCount; i++)
        {
            AllPlayers[i] = new Player(this)
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
            };
        }

        // Populating supply
        // ...

        // Setting player relation properties
        for (int i = 0; i < playerCount; i++)
        {
            var player = AllPlayers[playerCount];
            player.AllOtherPlayers = AllPlayers.Where((p, j) => j != i).ToArray();
            player.RightPlayer = playerCount == 0 ? AllPlayers.Last() : AllPlayers[playerCount - 1];
            player.LeftPlayer = playerCount == AllPlayers.Length - 1 ? AllPlayers.First() : AllPlayers[playerCount + 1];
        }
    }
}

// GENERATED
public enum CARDNAME
{
    Cellar,
    Bureaucrat
}

public enum CARDTYPE
{
    Action,
    Attack,
    Reaction,
    Treasure,
    Victory
}

public class Card : CardBase
{
    public CARDNAME Name { get; set; }
    public CARDTYPE[] Types { get; set; } = new CARDTYPE[0];
    public Number Cost { get; set; }
    public Effect ActionEffect { get; set; } = () => { };
    public Effect TreasureEffect { get; set; } = () => { };
    public Effect ReactionEffect { get; set; } = () => { };
    public Effect VictoryEffect { get; set; } = () => { };
}

public class Game
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
