using CodeProcessor;
using CodeProcessor.Grammar;
using Xunit.Abstractions;

namespace CodeProcessor.Tests;

public class MyDbgGrammarVisitorTest
{
    private readonly ITestOutputHelper _testOutput;
    public MyDbgGrammarVisitorTest(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
    }

    [Fact]
    public void TestCodeSematicsFromFile()
    {
        var fileNameBase = "SemanticTest";
        int i = 1;
        string fileName = @$"..\..\..\testfiles\{fileNameBase}{i}.dbg";
        while (File.Exists(fileName))
        {
            var code = File.ReadAllText(fileName);
            var parser = CodeProcessor.Processor.GetParser(code);
            var visitor = new MyDbgGrammarVisitor();
            var errors = visitor.VisitProgram(parser.program());

            _testOutput.WriteLine($"_____{Environment.NewLine}Test file {fileName}");
            foreach (var item in errors)
            {
                _testOutput.WriteLine(item);
            }
            Assert.Empty(errors);

            fileName = @$"..\..\..\testfiles\{fileNameBase}{++i}.dbg";
        }
    }
}
