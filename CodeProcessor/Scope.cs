using System;
using System.Collections.Generic;

namespace CodeProcessor
{
    public class ScopeSymbol
    {
        public string Name { get; set; }
        public SymbolType Type { get; set; }

        public ScopeSymbol(string name, string type) {
            if (!Enum.TryParse(type, true, out SymbolType symbolType))
            {
                throw new Exception($"type '{type}' is invalid");
            }
            this.Name = name;
            this.Type = symbolType;
        }
    }

    public class Scope
    {
        public Scope Parent { get; set; }
        private Dictionary<string, SymbolType> symbolTable;

        public Scope(Scope parent = null)
        {
            this.symbolTable = new Dictionary<string, SymbolType>();
            this.Parent = parent;
        }

        public SymbolType this[string name]
        {
            get
            {
                return this.symbolTable.ContainsKey(name) ? this.symbolTable[name] : Parent?[name];
            }
            set
            {
                if (this.symbolTable.ContainsKey(name))
                    throw new Exception($"name '{name}' is already in scope");
                this.symbolTable[name] = value;
            }
        }
    }
}
