using Antlr4.Runtime.Misc;

namespace CodeProcessor.Grammar
{
    public class MyDbgGrammarVisitor : DbgGrammarBaseVisitor<object>
    {
        DbgSemanticController ctlr = new DbgSemanticController();

        public override object VisitBlock([NotNull] DbgGrammarParser.BlockContext context)
        {
            ctlr.PushNewScope();
            var result = base.VisitBlock(context);
            ctlr.PopScope();
            return result;
        }

        public override object VisitStatement([NotNull] DbgGrammarParser.StatementContext context)
        {
            ctlr.AddVerifyVariableFromVarDefinition(context);
            return base.VisitStatement(context);
        }

        public override object VisitCommandDeclaration([NotNull] DbgGrammarParser.CommandDeclarationContext context)
        {
            ctlr.AddVerifyCommandDeclaration(context);
            return base.VisitCommandDeclaration(context);
        }

        public override object VisitCommand([NotNull] DbgGrammarParser.CommandContext context)
        {
            ctlr.VerifyCommand(context);
            return base.VisitCommand(context);
        }

        public override object VisitVarRef([NotNull] DbgGrammarParser.VarRefContext context)
        {
            ctlr.VerifyVarRef(context);
            return base.VisitVarRef(context);
        }

        public override object VisitNumericExpression([NotNull] DbgGrammarParser.NumericExpressionContext context)
        {
            ctlr.VerifyNumericExpression(context);
            return base.VisitNumericExpression(context);
        }

        public override object VisitBooleanExpression([NotNull] DbgGrammarParser.BooleanExpressionContext context)
        {
            ctlr.VerifyBooleanExpression(context);
            return base.VisitBooleanExpression(context);
        }

        public override object VisitEnumIsExpression([NotNull] DbgGrammarParser.EnumIsExpressionContext context)
        {
            ctlr.VerifyEnumIsExpression(context);
            return base.VisitEnumIsExpression(context);
        }

        public override object VisitListHasExpression([NotNull] DbgGrammarParser.ListHasExpressionContext context)
        {
            ctlr.VerifyListHasExpression(context);
            return base.VisitListHasExpression(context);
        }

        public override object VisitNumberPredicate([NotNull] DbgGrammarParser.NumberPredicateContext context)
        {
            ctlr.PushNewPredicateScope(SymbolType.NUMBER);
            var result = base.VisitNumberPredicate(context);
            ctlr.PopScope();
            return result;
        }

        public override object VisitCardPredicate([NotNull] DbgGrammarParser.CardPredicateContext context)
        {
            ctlr.PushNewPredicateScope(SymbolType.CARD);
            var result = base.VisitCardPredicate(context);
            ctlr.PopScope();
            return result;
        }
    }
}
