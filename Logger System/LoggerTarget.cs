namespace LoggingSystem
{
    internal class LoggerTarget
    {
        private readonly Dictionary<LoggerLevel, List<ILogObserver>> _logObservers = new Dictionary<LoggerLevel, List<ILogObserver>>();

        public void AddObserver(LoggerLevel level, ILogObserver logObserver)
        {
            if (!_logObservers.ContainsKey(level))
            {
                _logObservers[level] = new List<ILogObserver>();
            }
            _logObservers[level].Add(logObserver);
        }

        public void RemoveObserver(ILogObserver logObserver)
        {
            foreach (var logObservers in _logObservers.Values)
            {
                logObservers.Remove(logObserver);
            }
        }

        public void NotifyAllObservers(LoggerLevel level, string message)
        {
            if (_logObservers.ContainsKey(level))
            {
                foreach (var logObserver in _logObservers[level])
                {
                    logObserver.Log(message);
                }
            }
        }
    }
}
