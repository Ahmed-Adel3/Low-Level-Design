namespace LoggingSystem.Destinations
{
    internal class ConsoleLogger : ILogObserver
    {
        public void Log(string message)
        {
            Console.WriteLine("Writing to Console " + message);
        }
    }
}
