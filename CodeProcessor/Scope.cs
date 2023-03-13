using System;
using System.Collections.Generic;

namespace CodeProcessor
{
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
                if (this.symbolTable.ContainsKey(name))
                {
                    return this.symbolTable[name];
                }
                else
                {
                    if (Parent == null)
                        return SymbolType.ERRORTYPE;
                    return Parent[name];
                }
            }
            set
            {
                if (this.symbolTable.ContainsKey(name))
                    throw new Exception($"Name '{name}' is already in scope");
                this.symbolTable[name] = value;
            }
        }
    }
}
