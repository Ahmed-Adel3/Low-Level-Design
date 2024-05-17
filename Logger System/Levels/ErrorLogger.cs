namespace LoggingSystem.Levels
{
    internal class ErrorLogger : AbstractLogger
    {
        public ErrorLogger(LoggerLevel levels)
        {
            this.Levels = levels;
        }

        protected override void Display(string msg, LoggerTarget loggerTarget)
        {
            loggerTarget.NotifyAllObservers(LoggerLevel.Error, "ERROR: " + msg);
        }
    }
}
