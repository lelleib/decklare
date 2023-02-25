using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace CodeProcessor.Grammar
{
    public class DbgSemanticController
    {
        private readonly (string, CommandSignature)[] builtInCommands =
        {
            ("Let1Arrange2", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.PLAYER,SymbolType.PILE}}),
            ("SET (variable: &ENUM<T>) TO (value: ENUM<T>)", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.CARD}}),
            ("Set1To2", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.NUMBER,SymbolType.NUMBER}}),
            ("Shuffle1", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.PILE}}),
            ("Rotate1", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.PILE}}),
            ("Execute1", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.EFFECT}}),
            ("Clone1", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.EFFECT}}),
            ("Detach1FromCard", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.EFFECT}}),
            ("InitDominion", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{}})
        };
        private readonly ScopeSymbol[] builtInVars =
        {
            new ScopeSymbol("Hand", "PILE"),
            new ScopeSymbol("Deck", "PILE"),
            new ScopeSymbol("Discard", "PILE"),
            new ScopeSymbol("InPlay", "PILE"),
            new ScopeSymbol("This", "PILE"),
            new ScopeSymbol("CenterPile", "PILE"),
            new ScopeSymbol("Action", "NUMBER"),
            new ScopeSymbol("Buy", "NUMBER"),
            new ScopeSymbol("Coin", "NUMBER"),
            new ScopeSymbol("Victory", "NUMBER"),
            new ScopeSymbol("Discount", "NUMBER"),
            new ScopeSymbol("Me", "PLAYER"),
            new ScopeSymbol("ActivePlayer", "PLAYER"),
            new ScopeSymbol("LeftPlayer", "PLAYER"),
            new ScopeSymbol("RightPlayer", "PLAYER"),
            new ScopeSymbol("AllPlayers", "LIST<PLAYER>"),
            new ScopeSymbol("AllOtherPlayers", "LIST<PLAYER>")
        };

        private TypeSystem typeSystem;
        private Scope currentScope;
        private CommandRegistry commandRegistry;
        private List<(string, SymbolType)> nextBlockArguments;

        public DbgSemanticController()
        {
            // create default scope and add built-in variables
            this.currentScope = new Scope();
            foreach (var item in builtInVars)
            {
                this.currentScope[item.Name] = item.Type;
            }
            this.commandRegistry = new CommandRegistry();
            foreach (var item in builtInCommands)
            {
                this.commandRegistry[item.Item1] = item.Item2;
            }
            nextBlockArguments = new List<(string, SymbolType)>();
        }

        public void PushNewScope()
        {
            // create nested scope and add arguments
            this.currentScope = new Scope(this.currentScope);
            foreach (var item in nextBlockArguments)
            {
                this.currentScope[item.Item1] = item.Item2;
            }
            nextBlockArguments.Clear();
        }

        public void PopScope()
        {
            this.currentScope = this.currentScope.Parent;
        }

        public void AddVerifyVariableFromVarDefinition(DbgGrammarParser.StatementContext context)
        {
            // add variable if variable definition
            var varDefinition = context.varDefinition();
            if (varDefinition != null)
            {
                var varName = varDefinition.varName.Text;
                var varType = GetAssignorType(context);
                if (varType == SymbolType.VOID)
                {
                    Console.WriteLine($"Error at 51: cannot create assign a command without return value to a variable");
                    varType = null;
                }
                AddVerifyVariable(varName, varType);
            }
        }

        public void VerifyAssignment(DbgGrammarParser.StatementContext context)
        {
            // check validity if assignment
            var assignment = context.assignment();
            if (assignment != null)
            {
                var assigneeType = GetVariableType(assignment.varRef());
                var assignorType = GetAssignorType(context);
                if (assigneeType != assignorType)
                {
                    Console.WriteLine($"Error at 52: types in assignment ({assigneeType} and {assignorType}) do not match");
                }
            }
        }

        public void VerifyCommand(DbgGrammarParser.CommandContext context)
        {
            var signature = GetCommandSignature(context);
            if (signature == null)
            {
                Console.WriteLine($"Error at 45: command does not exist");
            }
            else
            {
                var arguments = context.expression();
                for (int i = 0; i < arguments.Length; i++)
                {
                    var type = GetExpressionType(arguments[i]);
                    var expectedType = signature.Arguments[i];
                    if (type != expectedType) // TODO type compatibility check
                    {
                        Console.WriteLine($"Error at 46: The command's {i + 1}. argument have a wrong type ({type} instead of {expectedType})");
                    }
                }
            }
        }

        public void VerifyVarRef(DbgGrammarParser.VarRefContext context)
        {
            var type = GetVariableType(context);
            if (type == null)
            {
                Console.WriteLine($"Error at 43: variable '{context.GetText()}' does not exist");
            }
        }

        public void VerifyNumericExpression(DbgGrammarParser.NumericExpressionContext context)
        {
            var varRef = context.varRef();
            if (varRef != null)
            {
                var type = GetVariableType(varRef);
                if (type != null && type != SymbolType.NUMBER) // TODO type compatibility check
                {
                    Console.WriteLine($"Error at 48: variable '{varRef.GetText()}' used in numeric expression but is not of type NUMBER");
                }
            }
        }

        public void VerifyBooleanExpression(DbgGrammarParser.BooleanExpressionContext context)
        {
            var varRef = context.varRef();
            if (varRef != null)
            {
                var type = GetVariableType(varRef);
                if (type != null && type != SymbolType.BOOLEAN) // TODO type compatibility check
                {
                    Console.WriteLine($"Error at 49: variable '{varRef.GetText()}' used in boolean expression but is not of type BOOLEAN");
                }
            }
        }

        public void VerifyEnumIsExpression(DbgGrammarParser.EnumIsExpressionContext context)
        {
            var varRefs = context.varRef(); // TODO implement enum literal syntax
        }

        public void VerifyListHasExpression(DbgGrammarParser.ListHasExpressionContext context)
        {
            var varRefs = context.varRef();
            var firstOperandType = GetVariableType(varRefs[0]);
            var secondOperandType = GetVariableType(varRefs[1]);
            if (firstOperandType != SymbolType.LIST)
            {
                Console.WriteLine($"Error at 50: {ex.Message}");
            }
            if (secondOperandType != firstOperandType.SubType) // TODO fix type system representation
            {
                Console.WriteLine($"Error at 50: {ex.Message}");
            }
        }

        public void AddVerifyCommandDeclaration(DbgGrammarParser.CommandDeclarationContext context)
        {
            var commandId = GetCommandIdFromCWs(context.CW());

            var argumentNames = context.argumentDeclaration().Select(ctx => ctx.name.Text).ToArray();
            var arguments = context.argumentDeclaration().Select(ctx => GetVerifyTypeDefinition(ctx.typeDefinition())).ToArray();
            var signature = new CommandSignature { CommandType = SymbolType.VOID, Arguments = arguments };

            nextBlockArguments.AddRange(argumentNames.Zip(arguments));

            try
            {
                this.commandRegistry[commandId] = signature;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at 44: {ex.Message}");
            }
        }

        public void PushNewPredicateScope(SymbolType symbolType)
        {
            // create nested scope and add input variable
            this.currentScope = new Scope(this.currentScope);
            this.currentScope["x"] = symbolType;
        }


        private void AddVerifyVariable(string name, SymbolType type)
        {
            try
            {
                this.currentScope[name] = type;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at 42: {ex.Message}");
            }
        }

        private SymbolType GetVariableType(DbgGrammarParser.VarRefContext context)
        {
            string varName = context.ID().First().Symbol.Text;
            string[] memberPath = context.ID().Skip(1).Select(id => id.Symbol.Text).ToArray();
            var type = this.currentScope[varName];
            if (type == null || memberPath == null) // TODO memberPath==null === no members?
            {
                return type;
            }
            else
            {
                return type.GetMemberType(memberPath);
            }
        }

        private CommandSignature GetCommandSignature(DbgGrammarParser.CommandContext context)
        {
            var commandId = GetCommandIdFromCWs(context.CW());
            return this.commandRegistry[commandId];
        }

        private SymbolType GetCommandType(DbgGrammarParser.CommandContext context)
        {
            return GetCommandSignature(context)?.CommandType;
        }

        private SymbolType GetExpressionType(DbgGrammarParser.ExpressionContext context)
        {
            var varRef = context.varRef();
            if (varRef != null)
            {
                return GetVariableType(varRef);
            }
            var block = context.block();
            if (block != null)
            {
                return SymbolType.EFFECT;
            }
            var numericExpression = context.numericExpression();
            if (numericExpression != null)
            {
                return SymbolType.NUMBER;
            }
            var booleanExpression = context.booleanExpression();
            if (booleanExpression != null)
            {
                return SymbolType.BOOLEAN;
            }
            var numberPredicate = context.numberPredicate();
            if (numberPredicate != null)
            {
                return SymbolType.NUMBERPREDICATE;
            }
            return SymbolType.CARDPREDICATE;
        }

        private SymbolType GetAssignorType(DbgGrammarParser.StatementContext context)
        {
            var command = context.command();
            if (command != null)
            {
                return GetCommandType(command);
            }
            else
            {
                return GetExpressionType(context.expression());
            }
        }

        private SymbolType GetVerifyTypeDefinition(DbgGrammarParser.TypeDefinitionContext context)
        {
            var type = new SymbolType(context.mainType.Text, context.subType?.Text);
            if (type == null)
            {
                Console.WriteLine($"Error at 47: Type {context.GetText()} does not exist");
            }
            return type;
        }

        private string ToTitle(string text)
        {
            return Char.ToUpperInvariant(text[0]) + text.Substring(1).ToLowerInvariant();
        }

        private string GetCommandIdFromCWs(Antlr4.Runtime.Tree.ITerminalNode[] cws)
        {
            StringBuilder result = new StringBuilder();
            result.Append(ToTitle(cws[0].Symbol.Text));
            int paramIdx = 0;
            for (int i = 1; i < cws.Length; i++)
            {
                for (int j = 0; j < cws[i].Symbol.TokenIndex - cws[i - 1].Symbol.TokenIndex - 1; j++)
                {
                    result.Append(++paramIdx);
                }
                result.Append(ToTitle(cws[i].Symbol.Text));
            }
            return result.ToString();
        }
    }
}