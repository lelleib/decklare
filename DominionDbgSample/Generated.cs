namespace DominionDbgSample.Generated;

using DbgLib;

using Number = Int32;
using Effect = Action;
using TakeCommand = Func<DbgLib.Pile?, DbgLib.Pile?>;
using PutCommand = Action<DbgLib.Pile?, DbgLib.Pile?>;
using System.Collections.Generic;

public class DbgEnvironment : DbgEnvironmentBase
{
    // Gameplay context
    private Player[]? AllPlayers
    {
        get => _game.AllPlayers;
        set => _game.AllPlayers = value;
    }
    public Player? ActivePlayer
    {
        get => _game.ActivePlayer;
        set => _game.ActivePlayer = value;
    }
    // public Supply? Supply; // TODO impl
    private Pile? Trash
    {
        get => _game.Trash;
        set => _game.Trash = value;
    }
    private Pile? CenterPile
    {
        get => _game.CenterPile;
        set => _game.CenterPile = value;
    }

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
    public Player? LeftPlayer;
    public Player? RightPlayer;
    public Player[]? AllOtherPlayers;
    // Card context
    public Card? ThisCard;

    private readonly Dictionary<CARDNAME, Card> _cards;
    private Game _game;

    // GENERATED
    public DbgEnvironment(IDbgRuntime _runtime) : base(_runtime)
    {
        var cardArray = new Card[]
        {
            new() {
                Name = CARDNAME.Estate,
                Cost = 2,
                Types = new CARDTYPE[]{ CARDTYPE.Victory },
                VictoryEffect = () =>
                {
                    _Plus12
                    (
                    1,
                    Victory
                    );
                }
            },
            new() {
                Name = CARDNAME.Copper,
                Cost = 0,
                Types = new CARDTYPE[]{ CARDTYPE.Treasure },
                TreasureEffect = () =>
                {
                    _Plus12
                    (
                    1,
                    Coin
                    );
                }
            },
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
        _cards = new();
        foreach (var card in cardArray)
        {
            _cards.Add(card.Name, card);
        }

        _game = new();
        _runtime.SetGame(_game);
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
                        var card = _1From2And3To4
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
        _game.AllPlayers = new Player[playerCount];

        // Initializing players
        for (int i = 0; i < playerCount; i++)
        {
            _game.AllPlayers[i] = new()
            {
                Action = 1,
                Buy = 1,
                Coin = 0,
                Discount = 0,
                Victory = 0,
                Deck = new Pile()
            };
            var player = _game.AllPlayers[i];
            player.Deck = new()
            {
                Viewers = new() { player },
                Visibility = VISIBILITY.AllVisible,
                _Cards =
                    Enumerable.Repeat(_cards[CARDNAME.Estate], 3).Concat
                    (
                        Enumerable.Repeat(_cards[CARDNAME.Copper], 7)
                    ).ToList<CardBase>()
            };
            _Shuffle1(player.Deck);
        }

        // Populating supply
        // ...

        // Setting player relation properties
        var players = _game.AllPlayers;
        for (int i = 0; i < playerCount; i++)
        {
            var player = players[i];
            player.AllOtherPlayers = players.Where((p, j) => j != i).ToArray();
            player.Me = player;
            player.RightPlayer = i == 0 ? players.Last() : players[i - 1];
            player.LeftPlayer = i == players.Length - 1 ? players.First() : players[i + 1];
        }
    }

    private new Card? _1From2And3To4(TakeCommand? takeCommand, Pile? fromPile, PutCommand? putCommand, Pile? toPile)
    {
        return (Card?)base._1From2And3To4(takeCommand, fromPile, putCommand, toPile);
    }

    protected override void SetPlayerContext(PlayerBase playerBase)
    {
        var player = (Player)playerBase;

        Me = player.Me;
        AllOtherPlayers = player.AllOtherPlayers;
        LeftPlayer = player.LeftPlayer;
        RightPlayer = player.RightPlayer;

        Action = player.Action;
        Buy = player.Buy;
        Coin = player.Coin;
        Discount = player.Discount;
        Victory = player.Victory;

        Deck = player.Deck;
        Hand = player.Hand;
        Discard = player.Discard;
        InPlay = player.InPlay;
    }

    protected override void UnsetPlayerContext()
    {
        Me = null;
        AllOtherPlayers = null;
        LeftPlayer = null;
        RightPlayer = null;

        Action = null;
        Buy = null;
        Coin = null;
        Discount = null;
        Victory = null;

        Deck = null;
        Hand = null;
        Discard = null;
        InPlay = null;
    }
}

// GENERATED
public enum CARDNAME
{
    Estate,
    Copper,
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
    public override Dictionary<string, object> _Properties =>
        new KeyValuePair<string, object>[]
        {
            new(nameof(Name), Name!),
            new(nameof(Types), Types!),
            new(nameof(Cost), Cost!),
            new(nameof(ActionEffect), ActionEffect!),
            new(nameof(TreasureEffect), TreasureEffect!),
            new(nameof(ReactionEffect), ReactionEffect!),
            new(nameof(VictoryEffect), VictoryEffect!)
        }
        .Where((o) => o.Value is not null)
        .ToDictionary(o => o.Key, o => o.Value);

    public CARDNAME Name { get; set; }
    public CARDTYPE[] Types { get; set; } = new CARDTYPE[0];
    public Number Cost { get; set; }
    public Effect ActionEffect { get; set; } = () => { };
    public Effect TreasureEffect { get; set; } = () => { };
    public Effect ReactionEffect { get; set; } = () => { };
    public Effect VictoryEffect { get; set; } = () => { };

}

public class Game : GameBase
{
    public override PlayerBase[] _Players => AllPlayers ?? Array.Empty<PlayerBase>();

    public override KeyValuePair<string, Pile>[] _CommonPiles =>
        new KeyValuePair<string, Pile>[]
        {
            new(nameof(Trash), Trash!),
            new(nameof(CenterPile), CenterPile!)
        }
        .Where((o) => o.Value is not null)
        .ToArray();

    public Player[]? AllPlayers { get; set; }
    public Player? ActivePlayer { get; set; }
    public Pile? Trash { get; set; }
    public Pile? CenterPile { get; set; }
}

public class Player : PlayerBase
{
    public override KeyValuePair<string, Pile>[] _Piles =>
        new KeyValuePair<string, Pile>[]
        {
            new(nameof(Deck), Deck!),
            new(nameof(Hand), Hand!),
            new(nameof(Discard), Discard!),
            new(nameof(InPlay), InPlay!)
        }
        .Where((o) => o.Value is not null)
        .ToArray();

    public Player[]? AllOtherPlayers { get; set; }
    public Player? Me { get; set; }
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
}

public class Supply : SupplyBase
{ }
