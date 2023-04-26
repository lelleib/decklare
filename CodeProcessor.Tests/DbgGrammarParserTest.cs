namespace CodeProcessor.Tests;

public class DbgGrammarParserTest
{
    [Fact]
    public void TestCodeSyntaxFromFile()
    {

        var fileNameBase = "SyntacticTest";
        int i = 1;
        string fileName = @$"..\..\..\testfiles\{fileNameBase}{i}.dbg";
        while (File.Exists(fileName))
        {
            var code = File.ReadAllText(fileName);
            var parser = CodeProcessor.Processor.GetParser(code);

            fileName = @$"..\..\..\testfiles\{fileNameBase}{++i}.dbg";
        }
    }
}
