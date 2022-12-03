using System.Collections.Generic;

namespace CodeProcessor
{
    public class SymbolType
    {
        public enum SymbolTypeEnum
        {
            Number,
            Pile
        }

        public static SymbolType VOID;
        public static SymbolType CARD;
        public static SymbolType PILE;
        public static SymbolType SUPPLY;
        public static SymbolType NUMBER;
        public static SymbolType ENUM;
        public static SymbolType PLAYER;
        public static SymbolType LIST;


        public SymbolTypeEnum MainType { get; private set; }
        public SymbolTypeEnum SubType { get; private set; }

        public SymbolType(string mainType, string subType = null)
        {
            return null;
        }

        public SymbolType GetMemberType(string[] path)
        {
            return this;
        }
    }

    public class TypeSystem
    {
        //private HashSet<SymbolType> types = new HashSet<SymbolType>{new SymbolType{MainType=Number, SubType = Pile}};

        public SymbolType this[string name]
        {
            get
            {
                // if (this.types.ContainsKey(name))
                //     return types[name];
                return null;
            }
        }
    }
}
