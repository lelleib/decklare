using System;
using System.IO;

namespace CodeProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string input = File.ReadAllText(@"res/sample.txt");
                Processor.Process(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

    }
}
