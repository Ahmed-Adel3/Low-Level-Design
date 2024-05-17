namespace LoggingSystem.Levels
{
    internal class InfoLogger : AbstractLogger
    {
        public InfoLogger(LoggerLevel levels)
        {
            this.Levels = levels;
        }

        protected override void Display(string msg, LoggerTarget loggerTarget)
        {
            loggerTarget.NotifyAllObservers(LoggerLevel.Info, "INFO: " + msg);
        }
    }
}
