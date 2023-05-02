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
        foreach (var fileName in Directory.GetFiles(@"..\..\..\testfiles\"))
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
        }
    }
}
