using System;
using System.Collections.Generic;

namespace CodeProcessor
{
    public class CommandSignature
    {
        public SymbolType CommandType {get; set;}
        public SymbolType[] Arguments {get; set;}
    }

    public class CommandRegistry
    {
        Dictionary<string, CommandSignature> commandTable = new Dictionary<string, CommandSignature>();

        public CommandSignature this[string name]
        {
            get
            {
                return this.commandTable.GetValueOrDefault(name, null);
            }
            set
            {
                if (this.commandTable.ContainsKey(name))
                    throw new Exception($"name '{name}' already exists");
                this.commandTable[name] = value;
            }
        }
    }
}
