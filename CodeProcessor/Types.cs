using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeProcessor
{
    public enum SymbolTypeEnum
    {
        Boolean,
        Card,
        CardPredicate,
        Effect,
        Enum,
        List,
        Number,
        NumberPredicate,
        Pile,
        Player,
        Supply,
        Void,
        ErrorType
    }

    public class SymbolType
    {

        public static SymbolType BOOLEAN = new SymbolType(SymbolTypeEnum.Boolean);
        public static SymbolType CARD = new SymbolType(SymbolTypeEnum.Card);
        public static SymbolType CARDPREDICATE = new SymbolType(SymbolTypeEnum.CardPredicate);
        public static SymbolType EFFECT = new SymbolType(SymbolTypeEnum.Effect);
        public static SymbolType NUMBER = new SymbolType(SymbolTypeEnum.Number);
        public static SymbolType NUMBERPREDICATE = new SymbolType(SymbolTypeEnum.NumberPredicate);
        public static SymbolType PILE = new SymbolType(SymbolTypeEnum.Pile);
        public static SymbolType PLAYER = new SymbolType(SymbolTypeEnum.Player);
        public static SymbolType PLAYERLIST = new SymbolType(SymbolTypeEnum.List, PLAYER);
        public static SymbolType SUPPLY = new SymbolType(SymbolTypeEnum.Supply);
        public static SymbolType VOID = new SymbolType(SymbolTypeEnum.Void);
        public static SymbolType ERRORTYPE = new SymbolType(SymbolTypeEnum.ErrorType);


        public SymbolTypeEnum MainType { get; private set; }
        public SymbolType SubType { get; private set; }
        public string EnumSubType { get; private set; }

        public SymbolType(SymbolTypeEnum mainType, SymbolType subType = null, string enumSubType = null)
        {
            this.MainType = mainType;
            this.SubType = subType;
            this.EnumSubType = enumSubType;
        }
    }

    public class TypeSystem
    {
        private Dictionary<string, string[]> enumDefinitions = new Dictionary<string, string[]>{
            {"CARDTYPES", new string[]{"Action, Attack, Reaction, Treasure, Victory"}},
            {"VISIBILITY", new string[]{"AllVisible, TopVisible, NoneVisible"}}
        };

        public SymbolType this[string[] typeChain]
        {
            get
            {
                switch (typeChain[0])
                {
                    case "BOOLEAN":
                        return new SymbolType(SymbolTypeEnum.Boolean);
                    case "CARD":
                        return new SymbolType(SymbolTypeEnum.Card);
                    case "CARDPREDICATE":
                        return new SymbolType(SymbolTypeEnum.CardPredicate);
                    case "EFFECT":
                        return new SymbolType(SymbolTypeEnum.Effect);
                    case "ENUM":
                        if (typeChain.Length < 2)
                        {
                            throw new Exception("The enum type must be specified");
                        }
                        if (!enumDefinitions.ContainsKey(typeChain[1]))
                        {
                            throw new Exception($"The enum type '{typeChain[1]}' does not exist");
                        }
                        return new SymbolType(SymbolTypeEnum.Enum, null, typeChain[1]);
                    case "LIST":
                        if (typeChain.Length < 2)
                        {
                            throw new Exception("The type of list's elements must be specified");
                        }
                        var subSubType = this[typeChain.Skip(1).ToArray()];
                        return new SymbolType(SymbolTypeEnum.List, subSubType);
                    case "NUMBER":
                        return new SymbolType(SymbolTypeEnum.Number);
                    case "NUMBERPREDICATE":
                        return new SymbolType(SymbolTypeEnum.NumberPredicate);
                    case "PILE":
                        return new SymbolType(SymbolTypeEnum.Pile);
                    case "PLAYER":
                        return new SymbolType(SymbolTypeEnum.Player);
                    case "SUPPLY":
                        return new SymbolType(SymbolTypeEnum.Supply);
                    default:
                        throw new Exception($"Unknown type name '{typeChain[0]}'");
                }
            }
        }

        public bool IsConvertibleTo(SymbolType sourceType, SymbolType targetType)
        {
            if (sourceType == targetType || (targetType.MainType == SymbolTypeEnum.List && targetType.SubType == sourceType) || (sourceType == SymbolType.ERRORTYPE || targetType == SymbolType.ERRORTYPE))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public SymbolType GetMemberType(SymbolType rootType, string[] path)
        {
            if (path.Length == 0 || rootType == SymbolType.ERRORTYPE)
            {
                return rootType;
            }

            switch (rootType.MainType)
            {
                case SymbolTypeEnum.Card:
                    switch (path[0])
                    {
                        case "Name":
                            return GetMemberType(this[new string[] { "ENUM", "CARDS" }], path.Skip(1).ToArray());
                        case "Types":
                            return GetMemberType(this[new string[] { "LIST", "ENUM", "CARDTYPES" }], path.Skip(1).ToArray());
                        case "Cost":
                            return GetMemberType(SymbolType.NUMBER, path.Skip(1).ToArray());
                        default:
                            throw new Exception($"Type 'CARD' does not have member '{path[0]}'");
                    }
                case SymbolTypeEnum.Pile:
                    switch (path[0])
                    {
                        case "Count":
                            return GetMemberType(SymbolType.NUMBER, path.Skip(1).ToArray());
                        case "Visibility":
                            return GetMemberType(this[new string[] { "ENUM", "VISIBILITY" }], path.Skip(1).ToArray());
                        case "Viewers":
                            return GetMemberType(SymbolType.PLAYERLIST, path.Skip(1).ToArray());
                        default:
                            throw new Exception($"Type 'PILE' does not have member '{path[0]}'");
                    }
                default:
                    throw new Exception($"Type '{rootType}' does not have any members");
            }
        }

        public void AssertEnumLiteralValidity(string enumType, string variant)
        {
            if (!enumDefinitions.ContainsKey(enumType))
            {
                throw new Exception($"Enum type '{enumType}' does not exist");
            }
            if (!enumDefinitions[enumType].Contains(variant))
            {
                throw new Exception($"Enum type '{enumType}' does not have a variant of '{variant}'");
            }
        }
    }
}
