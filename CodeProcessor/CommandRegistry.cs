using System;
using System.Collections.Generic;

namespace CodeProcessor
{
    public class CommandSignature
    {
        public SymbolType CommandType {get; set;}
        public SymbolType[] Arguments {get; set;}

        public CommandSignature(SymbolType commandType, SymbolType[] arguments)
        {
            CommandType = commandType;
            Arguments = arguments;
        }
    }

    public class CommandRegistry
    {
        Dictionary<string, CommandSignature> commandTable = new Dictionary<string, CommandSignature>();

        public CommandSignature this[string name]
        {
            get
            {
                if (!this.commandTable.ContainsKey(name))
                    throw new Exception("Command does not exist");
                return this.commandTable[name];
            }
            set
            {
                if (this.commandTable.ContainsKey(name))
                    throw new Exception("Command already exists");
                this.commandTable[name] = value;
            }
        }
    }
}
