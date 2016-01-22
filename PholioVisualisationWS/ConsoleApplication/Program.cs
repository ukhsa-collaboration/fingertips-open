
namespace ConsoleApplication
{
    /// <summary>
    /// Intended to run one IConsoleTask implementation at a time.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            new ProcessFile().Do();
        }
    }
}
