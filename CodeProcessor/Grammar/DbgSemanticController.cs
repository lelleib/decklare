using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace CodeProcessor.Grammar
{
    public class DbgSemanticController
    {
        private TypeSystem typeSystem;
        private Scope currentScope;
        private CommandRegistry commandRegistry;
        private List<(string, SymbolType)> nextBlockArguments;

        public List<string> Errors { get; } = new List<string>();

        public DbgSemanticController()
        {
            (string, SymbolType)[] builtInVars =
            {
                ("Supply", SymbolType.PILE),
                ("Trash", SymbolType.PILE),
                ("CenterPile", SymbolType.PILE),
                ("Hand", SymbolType.PILE),
                ("Deck", SymbolType.PILE),
                ("Discard", SymbolType.PILE),
                ("InPlay", SymbolType.PILE),
                ("This", SymbolType.PILE),
                ("Action", SymbolType.NUMBER),
                ("Buy", SymbolType.NUMBER),
                ("Coin", SymbolType.NUMBER),
                ("Victory", SymbolType.NUMBER),
                ("Discount", SymbolType.NUMBER),
                ("Me", SymbolType.PLAYER),
                ("ActivePlayer", SymbolType.PLAYER),
                ("LeftPlayer", SymbolType.PLAYER),
                ("RightPlayer", SymbolType.PLAYER),
                ("AllPlayers", SymbolType.PLAYERLIST),
                ("AllOtherPlayers", SymbolType.PLAYERLIST)
            };

            (string, CommandSignature)[] builtInCommands =
            {
                ("1From2And3To4", new CommandSignature(SymbolType.CARD, new SymbolType[]{SymbolType.TAKEEXPRESSION,SymbolType.PILE,SymbolType.PUTEXPRESSION,SymbolType.PILE})),
                ("Pop12", new CommandSignature(SymbolType.PILE, new SymbolType[]{SymbolType.NUMBER,SymbolType.PILE})),
                ("Let1Choose23", new CommandSignature(SymbolType.PILE, new SymbolType[]{SymbolType.PLAYER,SymbolType.NUMBER,SymbolType.PILE})),
                ("Let1Choose2Where34", new CommandSignature(SymbolType.PILE, new SymbolType[]{SymbolType.PLAYER,SymbolType.NUMBER,SymbolType.CARDPREDICATE,SymbolType.PILE})),
                ("TakeAll1", new CommandSignature(SymbolType.PILE, new SymbolType[]{SymbolType.PILE})),
                ("TakeAllWhere12", new CommandSignature(SymbolType.PILE, new SymbolType[]{SymbolType.CARDPREDICATE,SymbolType.PILE})),
                ("Put12", new CommandSignature(SymbolType.PILE, new SymbolType[]{SymbolType.CARD,SymbolType.PILE})),
                ("Let1Put2Anywhere3", new CommandSignature(SymbolType.PILE, new SymbolType[]{SymbolType.PLAYER,SymbolType.PILE})),
                ("Let1Arrange2", new CommandSignature(SymbolType.PILE, new SymbolType[]{SymbolType.PLAYER,SymbolType.PILE})),
                ("Shuffle1", new CommandSignature(SymbolType.VOID, new SymbolType[]{SymbolType.PILE})),
                ("Rotate1", new CommandSignature(SymbolType.VOID, new SymbolType[]{SymbolType.PILE})),
                ("Execute1", new CommandSignature(SymbolType.VOID, new SymbolType[]{SymbolType.EFFECT})),
                ("Clone1", new CommandSignature(SymbolType.VOID, new SymbolType[]{SymbolType.EFFECT})),
                ("Detach1FromCard", new CommandSignature(SymbolType.VOID, new SymbolType[]{SymbolType.EFFECT})),
                ("InitDominion", new CommandSignature(SymbolType.VOID, new SymbolType[]{}))
            };

            typeSystem = new TypeSystem();

            // create default scope and add built-in variables
            this.currentScope = new Scope();
            foreach (var item in builtInVars)
            {
                this.currentScope[item.Item1] = item.Item2;
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
            this.currentScope = this.currentScope.Parent ?? this.currentScope;
        }

        public void AddVerifyVariableFromVarDefinition(DbgGrammarParser.StatementContext context)
        {
            // add variable if variable definition
            var varDefinition = context.varDefinition();
            if (varDefinition is not null)
            {
                var varName = context.varDefinition().varName.Text;
                var varType = GetAssignorType(context);
                if (varType == SymbolType.VOID)
                {
                    SignalError("Cannot assign a command without return value to a variable", context);
                    varType = SymbolType.ERRORTYPE;
                }

                try
                {
                    this.currentScope[varName] = varType;
                }
                catch (Exception ex)
                {
                    SignalError($"{ex.Message}", context);
                }
            }
        }

        public void VerifyAssignment(DbgGrammarParser.StatementContext context)
        {
            // check validity if assignment
            var assignment = context.assignment();
            if (assignment is not null)
            {
                var assigneeType = GetVariableType(context.assignment().varRef());
                var assignorType = GetAssignorType(context);
                if (!typeSystem.IsConvertibleTo(assignorType, assigneeType))
                {
                    SignalError($"Types in assignment ('{assigneeType}' and '{assignorType}') do not match", context);
                }
            }
        }

        public void VerifyCommandCall(DbgGrammarParser.CommandContext context)
        {
            try
            {
                var signature = GetCommandSignature(context);
                var arguments = context.expression();
                VerifyCommandArguments(signature, context);
            }
            catch (Exception ex)
            {
                SignalError($"{ex.Message}", context);
            }
        }

        public void VerifyVarRef(DbgGrammarParser.VarRefContext context)
        {
            string varName = context.varName.Text;
            string[] memberPath = GetMemberPath(context);
            var type = this.currentScope[varName];

            if (type == SymbolType.ERRORTYPE)
            {
                SignalError($"Variable '{context.varName.Text}' does not exist in the current context", context);
            }

            try
            {
                typeSystem.GetMemberType(type, memberPath);
            }
            catch (Exception ex)
            {
                SignalError(ex.Message, context);
            }
        }

        public void VerifyNumericExpression(DbgGrammarParser.NumericExpressionContext context)
        {
            var varRef = context.varRef();
            if (varRef is not null)
            {
                var type = GetVariableType(varRef);
                if (!typeSystem.IsConvertibleTo(type, SymbolType.NUMBER))
                {
                    SignalError($"Variable '{VarRefToString(varRef)}' used in numeric expression but is of type '{type}'", context);
                }
            }
        }

        public void VerifyBooleanExpression(DbgGrammarParser.BooleanExpressionContext context)
        {
            var varRef = context.varRef();
            if (varRef is not null)
            {
                var type = GetVariableType(varRef);
                if (!typeSystem.IsConvertibleTo(type, SymbolType.BOOLEAN))
                {
                    SignalError($"Variable '{VarRefToString(varRef)}' used in boolean expression but is of type '{type}'", context);
                }
            }
        }

        public void VerifyEnumIsExpression(DbgGrammarParser.EnumIsExpressionContext context)
        {
            var varRefs = context.varRef();
            var firstOperandType = GetVariableType(varRefs[0]);
            var secondOperandType = GetVariableType(varRefs[1]);
            if (firstOperandType.MainType != SymbolTypeEnum.Enum || secondOperandType.MainType != SymbolTypeEnum.Enum)
            {
                SignalError("'IS' expressions must have enums as their operands", context);
            }
            else if (!typeSystem.IsConvertibleTo(secondOperandType, firstOperandType))
            {
                SignalError("Enum types of operands in 'IS' expression do not match", context);
            }
        }

        public void VerifyListHasExpression(DbgGrammarParser.ListHasExpressionContext context)
        {
            var varRefs = context.varRef();
            var firstOperandType = GetVariableType(varRefs[0]);
            var secondOperandType = GetVariableType(varRefs[1]);
            if (firstOperandType.MainType != SymbolTypeEnum.List)
            {
                SignalError("'HAS' expressions must have a list as their first operand", context);
            }
            if (!typeSystem.IsConvertibleTo(secondOperandType, firstOperandType.SubType!))
            {
                SignalError("The second operand's type in 'HAS' expression does not match the item type of the list", context);
            }
        }

        public void AddVerifyCommandDeclaration(DbgGrammarParser.CommandDeclarationContext context)
        {
            var argPositions = Enumerable.Range(0, context.ChildCount).Where(i => context.GetChild(i).GetType() == typeof(DbgGrammarParser.ArgumentDeclarationContext)).ToArray();
            var commandId = GetCommandIdFromCWs(context.CW(), argPositions);

            var argumentNames = context.argumentDeclaration().Select(ctx => ctx.name.Text).ToArray();
            var arguments = context.argumentDeclaration().Select(ctx => GetVerifyTypeDefinition(ctx.typeDefinition())).ToArray();
            var signature = new CommandSignature(SymbolType.VOID, arguments);

            nextBlockArguments.AddRange(argumentNames.Zip(arguments));

            var nextBlockArgumentsDistinct = nextBlockArguments.GroupBy(x => x.Item1).Select(x => x.First()).ToList();
            if (nextBlockArguments.Count > nextBlockArgumentsDistinct.Count)
            {
                SignalError("There are multiple arguments with the same name in command declaration", context);
                nextBlockArguments = nextBlockArgumentsDistinct;
            }

            try
            {
                this.commandRegistry[commandId] = signature;
            }
            catch (Exception ex)
            {
                SignalError($"{ex.Message}", context);
            }
        }

        public void PushNewPredicateScope(SymbolType symbolType)
        {
            // create nested scope and add input variable
            this.currentScope = new Scope(this.currentScope);
            this.currentScope["X"] = symbolType;
        }

        public void VerifyEnumLiteral(DbgGrammarParser.EnumLiteralContext context)
        {
            var enumType = context.enumType.Text;
            var variant = context.variant.Text;

            try
            {
                typeSystem.AssertEnumLiteralValidity(enumType, variant);
            }
            catch (Exception ex)
            {
                SignalError($"{ex.Message}", context);
            }
        }

        public void VerifyPileCommand(DbgGrammarParser.CommandContext context)
        {
            // check if the embedded command exists (with PILE as last parameter) and returns PILE
            var cws = context.CW();
            var argPositions = Enumerable.Range(0, context.ChildCount).Where(i => context.GetChild(i).GetType() == typeof(DbgGrammarParser.ExpressionContext)).ToList();
            argPositions.Add(cws.Length + argPositions.Count);
            var commandId = GetCommandIdFromCWs(cws, argPositions.ToArray());

            try
            {
                var pileCommandSignature = this.commandRegistry[commandId];

                if (pileCommandSignature.Arguments.Last() != SymbolType.PILE)
                {
                    throw new Exception($"Command must have last parameter of type '{SymbolType.PILE}'");
                }
                if (pileCommandSignature.CommandType != SymbolType.PILE)
                {
                    throw new Exception($"Command must return value of type '{SymbolType.PILE}'");
                }

                pileCommandSignature.Arguments = pileCommandSignature.Arguments.SkipLast(1).ToArray();
                VerifyCommandArguments(pileCommandSignature, context);
            }
            catch (Exception ex)
            {
                SignalError($"Problem with pile command: {ex.Message}", context);
            }
        }

        public void PushNewPutExpressionScope()
        {
            // create nested scope and add input variable
            this.currentScope = new Scope(this.currentScope);
            this.currentScope["IT"] = SymbolType.PILE;
        }

        public void VerifyCardDefinition(DbgGrammarParser.CardDefinitionContext context)
        {
            int propertyCount = context.propertyDefinition().Length;
            int distinctPropertyCount = context.propertyDefinition().GroupBy(pd => pd.varDefinition().varName.Text).Count();
            if (propertyCount > distinctPropertyCount)
            {
                SignalError("Card definition's properties must be unique", context);
            }
        }

        private SymbolType GetVariableType(DbgGrammarParser.VarRefContext context)
        {
            string varName = context.varName.Text;
            string[] memberPath = GetMemberPath(context);
            var type = this.currentScope[varName];
            try
            {
                return typeSystem.GetMemberType(type, memberPath);
            }
            catch (Exception)
            {
                return SymbolType.ERRORTYPE;
            }
        }

        private string[] GetMemberPath(DbgGrammarParser.VarRefContext context)
        {
            return context.varMemberPath().ID().Select(id => id.Symbol.Text).ToArray();
        }

        private CommandSignature GetCommandSignature(DbgGrammarParser.CommandContext context)
        {
            var argPositions = Enumerable.Range(0, context.ChildCount).Where(i => context.GetChild(i).GetType() == typeof(DbgGrammarParser.ExpressionContext)).ToArray();
            var commandId = GetCommandIdFromCWs(context.CW(), argPositions);
            return this.commandRegistry[commandId];
        }

        private SymbolType GetCommandType(DbgGrammarParser.CommandContext context)
        {
            try
            {
                return GetCommandSignature(context).CommandType;
            }
            catch (Exception)
            {
                return SymbolType.ERRORTYPE;
            }
        }

        private SymbolType GetExpressionType(DbgGrammarParser.ExpressionContext context)
        {
            var varRef = context.varRef();
            if (varRef is not null)
            {
                return GetVariableType(varRef);
            }
            var block = context.block();
            if (block is not null)
            {
                return SymbolType.EFFECT;
            }
            var numericExpression = context.numericExpression();
            if (numericExpression is not null)
            {
                return SymbolType.NUMBER;
            }
            var booleanExpression = context.booleanExpression();
            if (booleanExpression is not null)
            {
                return SymbolType.BOOLEAN;
            }
            var numberPredicate = context.numberPredicate();
            if (numberPredicate is not null)
            {
                return SymbolType.NUMBERPREDICATE;
            }
            var cardPredicate = context.cardPredicate();
            if (cardPredicate is not null)
            {
                return SymbolType.CARDPREDICATE;
            }
            var enumLiteral = context.enumLiteral();
            if (enumLiteral is not null)
            {
                return GetEnumLiteralType(enumLiteral);
            }
            var takeExpression = context.takeExpression();
            if (takeExpression is not null)
            {
                return SymbolType.TAKEEXPRESSION;
            }
            var putExpression = context.putExpression();
            if (putExpression is not null)
            {
                return SymbolType.PUTEXPRESSION;
            }
            return SymbolType.ERRORTYPE;
        }

        private SymbolType GetAssignorType(DbgGrammarParser.StatementContext context)
        {
            var command = context.command();
            if (command is not null)
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
            var typeChain = TDContextToTypeChain(context);
            try
            {
                return typeSystem[typeChain];
            }
            catch (Exception ex)
            {
                SignalError($"Type {context.GetText()} does not exist: {ex.Message}", context);
                return SymbolType.ERRORTYPE;
            }
        }

        private void VerifyCommandArguments(CommandSignature signature, DbgGrammarParser.CommandContext context)
        {
            var arguments = context.expression();
            for (int i = 0; i < arguments.Length; i++)
            {
                var inputType = GetExpressionType(arguments[i]);
                var expectedType = signature.Arguments[i];
                if (!typeSystem.IsConvertibleTo(inputType, expectedType))
                {
                    SignalError($"The command's {i + 1}. argument have a wrong type ('{inputType}' but '{expectedType}' is expected)", context);
                }
            }
        }

        private string ToTitle(string text)
        {
            return Char.ToUpperInvariant(text[0]) + text.Substring(1).ToLowerInvariant();
        }

        private string GetCommandIdFromCWs(Antlr4.Runtime.Tree.ITerminalNode[] cws, int[] argPositions)
        {
            StringBuilder result = new StringBuilder();
            int cwIdx = 0;
            int argIdx = 0;
            int argsLength = argPositions.Length;
            int cmdLength = cws.Length + argsLength;
            for (int i = 0; i < cmdLength; i++)
            {
                if (argIdx < argsLength && argPositions[argIdx] == i)
                {
                    result.Append(argIdx + 1);
                    argIdx++;
                }
                else
                {
                    result.Append(ToTitle(cws[cwIdx].Symbol.Text));
                    cwIdx++;
                }
            }
            return result.ToString();
        }

        private string[] TDContextToTypeChain(DbgGrammarParser.TypeDefinitionContext context)
        {
            return TDContextToTypeChain(context, new List<string>());
        }

        private string[] TDContextToTypeChain(DbgGrammarParser.TypeDefinitionContext context, List<string> acc)
        {
            acc.Add(context.mainType.Text);

            if (context.subType is null)
            {
                return acc.ToArray();
            }
            else
            {
                return TDContextToTypeChain(context.subType, acc);
            }
        }

        private string VarRefToString(DbgGrammarParser.VarRefContext context)
        {
            var ids = new List<string>{context.varName.Text};
            ids.AddRange(context.varMemberPath().ID().Select(t => t.Symbol.Text));
            return string.Join("'s ", ids);
        }

        private SymbolType GetEnumLiteralType(DbgGrammarParser.EnumLiteralContext context)
        {
            var enumType = context.enumType.Text;
            try
            {
                return typeSystem[new string[] { "ENUM", enumType }];
            }
            catch (Exception)
            {
                return SymbolType.ERRORTYPE;
            }
        }

        private void SignalError(string message, Antlr4.Runtime.ParserRuleContext context)
        {
            Errors.Add($"Error at line {context.Start.Line}: {message}");
        }
    }
}