using Antlr4.Runtime.Misc;

namespace CodeProcessor.Grammar
{
    public class MyDbgGrammarVisitor : DbgGrammarBaseVisitor<List<string>>
    {
        DbgSemanticController ctlr = new DbgSemanticController();

        public override List<string> VisitProgram([NotNull] DbgGrammarParser.ProgramContext context)
        {
            base.VisitProgram(context);
            return ctlr.Errors;
        }

        public override List<string> VisitBlock([NotNull] DbgGrammarParser.BlockContext context)
        {
            ctlr.PushNewScope();
            var result = base.VisitBlock(context);
            ctlr.PopScope();
            return result;
        }

        public override List<string> VisitStatement([NotNull] DbgGrammarParser.StatementContext context)
        {
            ctlr.AddVerifyVariableFromVarDefinition(context);
            ctlr.VerifyAssignment(context);
            return base.VisitStatement(context);
        }

        public override List<string> VisitCommandDeclaration([NotNull] DbgGrammarParser.CommandDeclarationContext context)
        {
            ctlr.AddVerifyCommandDeclaration(context);
            return base.VisitCommandDeclaration(context);
        }

        public override List<string> VisitCommand([NotNull] DbgGrammarParser.CommandContext context)
        {
            ctlr.VerifyCommand(context);
            return base.VisitCommand(context);
        }

        public override List<string> VisitVarRef([NotNull] DbgGrammarParser.VarRefContext context)
        {
            ctlr.VerifyVarRef(context);
            return base.VisitVarRef(context);
        }

        public override List<string> VisitNumericExpression([NotNull] DbgGrammarParser.NumericExpressionContext context)
        {
            ctlr.VerifyNumericExpression(context);
            return base.VisitNumericExpression(context);
        }

        public override List<string> VisitBooleanExpression([NotNull] DbgGrammarParser.BooleanExpressionContext context)
        {
            ctlr.VerifyBooleanExpression(context);
            return base.VisitBooleanExpression(context);
        }

        public override List<string> VisitEnumIsExpression([NotNull] DbgGrammarParser.EnumIsExpressionContext context)
        {
            ctlr.VerifyEnumIsExpression(context);
            return base.VisitEnumIsExpression(context);
        }

        public override List<string> VisitListHasExpression([NotNull] DbgGrammarParser.ListHasExpressionContext context)
        {
            ctlr.VerifyListHasExpression(context);
            return base.VisitListHasExpression(context);
        }

        public override List<string> VisitNumberPredicate([NotNull] DbgGrammarParser.NumberPredicateContext context)
        {
            ctlr.PushNewPredicateScope(SymbolType.NUMBER);
            var result = base.VisitNumberPredicate(context);
            ctlr.PopScope();
            return result;
        }

        public override List<string> VisitCardPredicate([NotNull] DbgGrammarParser.CardPredicateContext context)
        {
            ctlr.PushNewPredicateScope(SymbolType.CARD);
            var result = base.VisitCardPredicate(context);
            ctlr.PopScope();
            return result;
        }

        public override List<string> VisitEnumLiteral([NotNull] DbgGrammarParser.EnumLiteralContext context)
        {
            ctlr.VerifyEnumLiteral(context);
            return base.VisitEnumLiteral(context);
        }
    }
}
