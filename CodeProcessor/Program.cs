using CodeProcessor;

try
{
    string input = File.ReadAllText(@"res/sample.txt");
    Processor.Process(input);
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex);
}
