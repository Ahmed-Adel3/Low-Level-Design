namespace LoggingSystem
{
    internal abstract class AbstractLogger
    {
        protected LoggerLevel Levels;

        private AbstractLogger _nextLevelLogger;

        public void SetNextLevelLogger(AbstractLogger nextLevelLogger)
        {
            _nextLevelLogger = nextLevelLogger;
        }

        public void LogMessage(LoggerLevel level, string msg, LoggerTarget loggerTarget)
        {
            if (Levels == level)
            {
                Display(msg, loggerTarget);
            }

            if (_nextLevelLogger != null)
            {
                _nextLevelLogger.LogMessage(level, msg, loggerTarget);
            }
        }

        protected abstract void Display(string msg, LoggerTarget loggerTarget);
    }
}
