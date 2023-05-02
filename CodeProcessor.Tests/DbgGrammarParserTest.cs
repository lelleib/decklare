namespace CodeProcessor.Tests;

public class DbgGrammarParserTest
{
    [Fact]
    public void TestCodeSyntaxFromFile()
    {
        foreach (var fileName in Directory.GetFiles(@"..\..\..\testfiles\"))
        {
            var code = File.ReadAllText(fileName);
            var parser = CodeProcessor.Processor.GetParser(code);
        }
    }
}
