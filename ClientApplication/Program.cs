namespace ClientApplication
{
    public sealed class Program : System.Object
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Contact Application has been launched]");

            Console.ForegroundColor = ConsoleColor.White;

            try { StartupBase.StartApplication<StartupApplication>(args); }
            catch (StartupException error)
            {
                Console.WriteLine($"[{error.StartupType.Name}]: {error.Message}");
            }
        }
    }
}