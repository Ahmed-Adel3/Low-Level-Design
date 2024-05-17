namespace LoggingSystem
{
    internal class Logger
    {
        private static Logger _logger;
        private static AbstractLogger _chainOfLogger;
        private static LoggerTarget _loggerTarget;

        private Logger()
        {
            if (_logger != null)
                throw new InvalidOperationException("Object already created");
        }

        public static Logger GetLogger()
        {
            if (_logger == null)
            {
                lock (typeof(Logger))
                {
                    if (_logger == null)
                    {
                        _logger = new Logger();
                        _chainOfLogger = LogManager.DoChaining();
                        _loggerTarget = LogManager.AddObservers();
                    }
                }
            }
            return _logger;
        }

        public void Info(string message)
        {
            CreateLog(LoggerLevel.Info, message);
        }

        public void Error(string message)
        {
            CreateLog(LoggerLevel.Error, message);
        }

        public void Debug(string message)
        {
            CreateLog(LoggerLevel.Debug, message);
        }

        private void CreateLog(LoggerLevel level, string message)
        {
            _chainOfLogger.LogMessage(level, message, _loggerTarget);
        }
    }
}
