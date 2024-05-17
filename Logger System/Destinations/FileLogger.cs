namespace LoggingSystem.Destinations
{
    internal class FileLogger : ILogObserver
    {
        public void Log(string message)
        {
            Console.WriteLine("Writing to File " + message);
        }
    }
}
