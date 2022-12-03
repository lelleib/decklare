using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeProcessorTest
{
    [TestClass]
    public class DbgParserTest
    {
        /*private DbgGrammarParser Setup(string text)
    {
        CodeProcessor. AntlrInputStream inputStream = new AntlrInputStream(text);
        SpeakLexer speakLexer = new SpeakLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
        SpeakParser speakParser = new SpeakParser(commonTokenStream);
        return speakParser;   
    }*/
        [TestMethod]
        public void TestMethod1()
        {
            var parser = CodeProcessor.Processor.GetParser(" EXAMPLE COMMAND WITH 1 ARGUMENT");
            var visitor = new MyDbgGrammarVisitor();
            visitor.
        }
    }
}
