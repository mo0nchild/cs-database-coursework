namespace ClientApplication
{
    using Config = ClientApplication.Configurations;
    public sealed class Program : System.Object
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Contact Application has been launched]");

            Console.ForegroundColor = ConsoleColor.White;

            try { Config::StartupBase.StartApplication<Config::StartupApplication>(args).Wait(); }
            catch (AggregateException error) when (error.InnerException is Config::StartupException)
            {
                var startup_error = error.InnerException as Config::StartupException;
                Console.WriteLine($"[{startup_error?.StartupType.Name}]: {error.Message}");
            }
        }
    }
}