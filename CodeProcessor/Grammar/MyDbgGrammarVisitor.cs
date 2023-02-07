using System;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Misc;

namespace CodeProcessor.Grammar
{
    public class MyDbgGrammarVisitor : DbgGrammarBaseVisitor<object>
    {
        private readonly (string, CommandSignature)[] builtInCommands =
        {
            ("LET (player: &PLAYER) ARRANGE (pile: &PILE)", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.CARD}}),
            ("SET (variable: &ENUM<T>) TO (value: ENUM<T>)", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.CARD}}),
            ("SET (variable: &NUMBER) TO (value: NUMBER)", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.CARD}}),
            ("SHUFFLE (pile: &PILE)", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.CARD}}),
            ("ROTATE (pile: &PILE)", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.CARD}}),
            ("EXECUTE (effect: EFFECT)", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.CARD}}),
            ("CLONE (effect: EFFECT)", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.CARD}}),
            ("DETACH (effect: EFFECT) FROM CARD", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.CARD}}),
            ("INIT DOMINION", new CommandSignature{CommandType = SymbolType.VOID, Arguments = new SymbolType[]{SymbolType.CARD}})
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

        public override object VisitProgram([NotNull] DbgGrammarParser.ProgramContext context)
        {
            // create default scope and add built-in variables
            this.currentScope = new Scope();
            foreach (var item in builtInVars)
            {
                this.currentScope[item.Name] = item.Type;
            }
            return base.VisitProgram(context);
        }

        public override object VisitBlock([NotNull] DbgGrammarParser.BlockContext context)
        {
            // create nested scope
            this.currentScope = new Scope(this.currentScope);
            var result = base.VisitBlock(context);
            this.currentScope = this.currentScope.Parent;
            return result;
        }

        public override object VisitStatement([NotNull] DbgGrammarParser.StatementContext context)
        {
            // create variable for assignment
            var assignment = context.assignment();
            if (assignment != null)
            {
                // add newly created variable to scope
                AddVariable(assignment.varName.Text, GetAssigneeType(context));
            }
            return base.VisitStatement(context);
        }

        public override object VisitCommandDefinition([NotNull] DbgGrammarParser.CommandDefinitionContext context)
        {
            var arguments = context.commandDeclaration().argumentDeclaration().Select(ctx => GetVerifyTypeDefinition(ctx.typeDefinition())).ToArray();
            AddCommand(GetCommandIdFromCWs(context.commandDeclaration().CW()), new CommandSignature { CommandType = null, Arguments = arguments });
            return base.VisitCommandDefinition(context);
        }

        public override object VisitCommand([NotNull] DbgGrammarParser.CommandContext context)
        {
            VerifyCommand(context);
            return base.VisitCommand(context);
        }

        public override object VisitVarRef([NotNull] DbgGrammarParser.VarRefContext context)
        {
            VerifyVarRef(context);
            return base.VisitVarRef(context);
        }

        public override object VisitNumericExpression([NotNull] DbgGrammarParser.NumericExpressionContext context)
        {
            VerifyNumericExpression(context);
            return base.VisitNumericExpression(context);
        }
        
        public override object VisitBooleanExpression([NotNull] DbgGrammarParser.BooleanExpressionContext context)
        {
            VerifyBooleanExpression(context);
            return base.VisitBooleanExpression(context);
        }
        
        public override object VisitNumberPredicate([NotNull] DbgGrammarParser.NumberPredicateContext context)
        {
            // create nested scope and add input variable
            this.currentScope = new Scope(this.currentScope);
            AddVariable("x", SymbolType.NUMBER);
            var result = base.VisitNumberPredicate(context);
            this.currentScope = this.currentScope.Parent;
            return result;
        }
        
        public override object VisitCardPredicate([NotNull] DbgGrammarParser.CardPredicateContext context)
        {
            // create nested scope and add input variable
            this.currentScope = new Scope(this.currentScope);
            AddVariable("x", SymbolType.CARD);
            var result = base.VisitCardPredicate(context);
            this.currentScope = this.currentScope.Parent;
            return result;
        }

        private void VerifyCommand([NotNull] DbgGrammarParser.CommandContext context)
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
                    if (type != expectedType)
                    {
                        Console.WriteLine($"Error at 46: The command's {i + 1}. argument have a wrong type ({type} instead of {expectedType})");
                    }
                }
            }
        }

        private void VerifyVarRef([NotNull] DbgGrammarParser.VarRefContext context)
        {
            var type = GetVariableType(context);
            if (type == null)
            {
                Console.WriteLine($"Error at 43: variable '{context.GetText()}' does not exist");
            }
        }

        private void VerifyNumericExpression([NotNull] DbgGrammarParser.NumericExpressionContext context)
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
        

        private void VerifyBooleanExpression([NotNull] DbgGrammarParser.BooleanExpressionContext context)
        {
            var varRef = context.varRef();
            var type = GetVariableType(varRef);
            if (type != null && type != SymbolType.BOOLEAN) // TODO type compatibility check
            {
                Console.WriteLine($"Error at 48: variable '{varRef.GetText()}' used in numeric expression but is not of type NUMBER");
            }
        }

        private void AddCommand(string commandId, CommandSignature signature)
        {
            try
            {
                this.commandRegistry[commandId] = signature;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error at 44: {ex.Message}");
            }
        }

        private void AddVariable(string name, SymbolType type)
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

        private SymbolType GetVariableType([NotNull] DbgGrammarParser.VarRefContext context)
        {
            string varName = context.ID().First().Symbol.Text;
            string[] memberPath = context.ID().Skip(1).Select(id => id.Symbol.Text).ToArray();
            var type = this.currentScope[varName];
            if (type == null || memberPath == null)
            {
                return type;
            }
            else
            {
                return type.GetMemberType(memberPath);
            }
        }

        private CommandSignature GetCommandSignature([NotNull] DbgGrammarParser.CommandContext context)
        {
            var signature = this.commandRegistry[GetCommandIdFromCWs(context.CW())];
            return signature;
        }

        private SymbolType GetCommandType([NotNull] DbgGrammarParser.CommandContext context)
        {
            return GetCommandSignature(context)?.CommandType;
        }

        private SymbolType GetExpressionType([NotNull] DbgGrammarParser.ExpressionContext context)
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

        private SymbolType GetAssigneeType([NotNull] DbgGrammarParser.StatementContext context)
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

        private SymbolType GetVerifyTypeDefinition([NotNull] DbgGrammarParser.TypeDefinitionContext context)
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
