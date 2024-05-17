using LoggingSystem.Destinations;
using LoggingSystem.Levels;

namespace LoggingSystem
{
    internal class LogManager
    {
        public static AbstractLogger DoChaining()
        {
            AbstractLogger infoLogger = new InfoLogger(LoggerLevel.Info);
            AbstractLogger errorLogger = new ErrorLogger(LoggerLevel.Error);
            AbstractLogger debugLogger = new DebugLogger(LoggerLevel.Debug);

            infoLogger.SetNextLevelLogger(errorLogger);
            errorLogger.SetNextLevelLogger(debugLogger);

            return infoLogger;
        }

        public static LoggerTarget AddObservers()
        {
            LoggerTarget loggerTarget = new LoggerTarget();

            ConsoleLogger consoleLogger = new ConsoleLogger();
            loggerTarget.AddObserver(LoggerLevel.Info, consoleLogger);
            loggerTarget.AddObserver(LoggerLevel.Error, consoleLogger);
            loggerTarget.AddObserver(LoggerLevel.Debug, consoleLogger);

            FileLogger fileLogger = new FileLogger();
            loggerTarget.AddObserver(LoggerLevel.Error, fileLogger);

            return loggerTarget;
        }
    }
}
