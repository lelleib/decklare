using Antlr4.Runtime;
using CodeProcessor.Grammar;

namespace CodeProcessor
{
    public class Processor
    {
        public static void Process(string sourceCode)
        {
            DbgGrammarParser DbgGrammarParser = GetParser(sourceCode);
            DbgGrammarParser.ProgramContext tree = DbgGrammarParser.program();
            MyDbgGrammarVisitor visitor = new MyDbgGrammarVisitor();
            visitor.Visit(tree);
        }

        public static DbgGrammarParser GetParser(string sourceCode)
        {
            AntlrInputStream inputStream = new AntlrInputStream(sourceCode);
            DbgGrammarLexer DbgGrammarLexer = new DbgGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(DbgGrammarLexer);
            DbgGrammarParser DbgGrammarParser = new DbgGrammarParser(commonTokenStream);
            return DbgGrammarParser;
        }
    }
}