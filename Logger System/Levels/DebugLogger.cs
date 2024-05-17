namespace LoggingSystem.Levels
{
    internal class DebugLogger : AbstractLogger
    {
        public DebugLogger(LoggerLevel level)
        {
            this.Levels = level;
        }

        protected override void Display (string msg, LoggerTarget loggerTarget)
        {
             loggerTarget.NotifyAllObservers(LoggerLevel.Debug, "DEBUG: " + msg);
        }
    }
}
