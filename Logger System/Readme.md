# 🛠️ Practicing Low-Level Design: Building a Logging Framework 📝

- Looking to enhance your low-level design skills using design patterns? In this tutorial, we'll embark on a fascinating journey of building a logging framework from the ground up. 
- By leveraging the power of three key design patterns
**_Singleton, Chain of Responsibility, and Observer_** we'll not only create a functional logging system but also gain valuable insights into the art of low-level design. So, roll up your sleeves, fire up your favorite IDE, and let's dive into this hands-on coding adventure! 💻

## Let's start with gathering the requirements ...
1- 📌 **Multiple Sync:** The framework should support logging in multiple places, such as console, log file, database, and distributed queue.
2- 📌 **Multiple Categories:** The framework should support logging into multiple categories, such as info, debug, and error.
3- 📌 **Configurability:** The category and logging location should be configurable.

Here is the result code we are going to implement today 
{% github danistefanovic/build-your-own-x %}

## So our main components will be:
📚 Logger Class: The main class exposed to the application for writing logs.
📚 Categories: Info, Debug, and Error.
📚 Target: Console, File, and Database.


![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/pqvnybafciyggpy10mkc.png)

## Logger class:
 - For this class, we will start with a creational design pattern to create the logger.
 - Here, we will choose `Singleton design` pattern to create just one instance from the logger all over our application

![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/vsmcj490ysotv5ooxd9g.png)

```c#
// Singleton pattern ensures that a class has only one instance and provides a global point of access to it.
internal class Logger
{
    // Private static instance of the Logger class.
    private static Logger _logger;

    // Private constructor to prevent instantiation of the Logger class from outside.
    private Logger()
    {
        // Check if an instance of Logger already exists, throw an exception if so.
        if (_logger != null)
            throw new InvalidOperationException("Object already created");
    }

    // Public static method to provide access to the single instance of the Logger class.
    public static Logger GetLogger()
    {
        // Check if _logger is null (not yet initialized).
        if (_logger == null)
        {
            // Use locking to ensure thread safety in multi-threaded environments.
            lock (typeof(Logger))
            {
                // Double-check if _logger is still null (another thread might have initialized it while waiting for the lock).
                if (_logger == null)
                {
                    // Create a new instance of Logger and assign it to _logger.
                    _logger = new Logger();
                }
            }
        }
        // Return the single instance of Logger.
        return _logger;
    }
}
```

### So this implementation:

🚫 **Prevent Multiple Instances**: The private constructor prevents the creation of multiple instances of the `Logger` class, so it's the class's responsibility to create the object and return the same instance when requested using the `GetLogger` function.

____

## Now let's move to the second section, categorization of the logs ...
 - For that we will use **_Chain of responsibility_** design pattern
 - **_Chain of Responsibility_** is a behavioral design pattern that lets you pass requests along a chain of handlers. Upon receiving a request, each handler decides either to process the request or to pass it to the next handler in the chain.


![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/aaekvhagabeetp3freci.png)

 - Let's now create a simple Enum to carry the values that will identify the log level ...

```c#
    internal enum LoggerLevel
    {
        Info = 1,
        Error = 2,
        Debug = 3
    }
```

- Now, let's implement the Chain of Responsibility pattern for categories (Info, Debug, and Error) using the AbstractLogger class and its concrete subclasses.

```c#
namespace LoggingSystem
{
    // AbstractLogger is an abstract base class for other logging classes.
    internal abstract class AbstractLogger
    {
        // Levels is a protected field that holds the logging level.
        protected LoggerLevel Levels;

        // _nextLevelLogger is a private field that holds the next logger in the chain of responsibility.
        private AbstractLogger _nextLevelLogger;

        // SetNextLevelLogger is a public method that sets the next logger in the chain of responsibility.
        public void SetNextLevelLogger(AbstractLogger nextLevelLogger)
        {
            _nextLevelLogger = nextLevelLogger;
        }

        // LogMessage is a public method that logs a message.
        public void LogMessage(LoggerLevel level, string msg, LoggerTarget loggerTarget)
        {
            // If the current logger's level matches the provided level, it displays the message.
            if (Levels == level)
            {
                Display(msg, loggerTarget);
            }

            // Regardless of whether it displayed the message, it passes the message to the next logger in the chain (if there is one).
            if (_nextLevelLogger != null)
            {
                _nextLevelLogger.LogMessage(level, msg, loggerTarget);
            }
        }

        // Display is a protected abstract method that displays a message.
        // This method has no implementation in this class and must be implemented in any non-abstract subclass.
        protected abstract void Display(string msg, LoggerTarget loggerTarget);
    }
}
```

## AbstractLogger Class:

🧩 **Chain of Responsibility**: `AbstractLogger` implements the Chain of Responsibility pattern, allowing different loggers to handle log messages based on their logging levels.

🔄 **Next Logger**: It maintains a private field `_nextLevelLogger` to hold the reference to the next logger in the chain, enabling the passing of log messages to the next logger if needed.

🔒 **Levels Field**: The `Levels` field holds the logging level for the current logger, determining which messages to handle.

📝 **LogMessage Method**: The `LogMessage` method checks if the provided logging level matches the logger's level. If it does, it displays the message; otherwise, it passes the message to the next logger in the chain.

🚧 **Display Method**: The `Display` method is a protected abstract method that must be implemented by subclasses to handle the actual display of log messages.

🔗 **SetNextLevelLogger Method**: This method allows setting the next logger in the chain, establishing the order in which loggers handle messages.

### Now let's create the concrete classes for displaying the message...

 - For Info:
``` c#
    internal class InfoLogger : AbstractLogger
    {
        public InfoLogger(LoggerLevel levels)
        {
            this.Levels = levels;
        }

        protected override void Display(string msg, LoggerTarget loggerTarget)
        {
            //temporary for now ...
            Console.WriteLine("INFO: " + msg);
        }
    }
```

 - For Error:
``` c#
    internal class ErrorLogger : AbstractLogger
    {
        public ErrorLogger(LoggerLevel levels)
        {
            this.Levels = levels;
        }

        protected override void Display(string msg, LoggerTarget loggerTarget)
        {
            //temporary for now ...
            Console.WriteLine("ERROR: " + msg);
        }
    }
```

 - For Debug:
``` c#
    internal class DebugLogger : AbstractLogger
    {
        public DebugLogger(LoggerLevel levels)
        {
            this.Levels = levels;
        }

        protected override void Display(string msg, LoggerTarget loggerTarget)
        {
            //temporary for now ...
            Console.WriteLine("DEBUG: " + msg);
        }
    }
```

### Now, let's edit or logger class ...
 - we need to use the abstract logger to create loggers for various levels.
 - So we will instantiate a new instance of the abstract logger, let's name it _chainOfLogger in the GetLogger function.

```c#
internal class Logger
{
    private static Logger _logger;
    private static AbstractLogger _chainOfLogger;
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
                    _chainOfLogger = CreateChainOfLogger();
                    //IS THIS REALLY WHERE THE CreateChainOfLogger FUNCTION BELONGS??
                }
            }
        }
        return _logger;
    }
}
```

**- We should make a slight enhancement here ... CreateChainOfLogger doesn't actually belong to the logger class, it is not it's responsibility to build the chain...**

- So we will create another class called LogManager to handle it ...

```cs
internal class LogManager
{
public static AbstractLogger DoChaining()
{
    // Create an InfoLogger with the Info level.
    AbstractLogger infoLogger = new InfoLogger(LoggerLevel.Info);

    // Create an ErrorLogger with the Error level.
    AbstractLogger errorLogger = new ErrorLogger(LoggerLevel.Error);

    // Create a DebugLogger with the Debug level.
    AbstractLogger debugLogger = new DebugLogger(LoggerLevel.Debug);

    // Set the next logger for the InfoLogger to be the ErrorLogger.
    infoLogger.SetNextLevelLogger(errorLogger);

    // Set the next logger for the ErrorLogger to be the DebugLogger.
    errorLogger.SetNextLevelLogger(debugLogger);

    // Return the first logger in the chain (InfoLogger).
    return infoLogger;
}

}
```

**🔗 Logger Chain Setup:** The DoChaining method sets up a chain of responsibility for loggers, where each logger handles messages based on its logging level.

**🔄 Chain Order:** It creates three loggers (InfoLogger, ErrorLogger, and DebugLogger) with increasing logging levels (Info < Error < Debug) and sets the next logger in the chain accordingly.

**🧩 Chain Completion:** The InfoLogger is set to pass messages to the ErrorLogger, which in turn passes messages to the DebugLogger. This completes the chain, ensuring that log messages are handled appropriately based on their levels.

- Then we will use this function to create the chain inside the Logger class.

- After that, we will implement a function to create the log itself, it a general function accepting the message and the level, it will also be a private function, it will only be called from inside the logger class by three functions for each log level...

- So the Logger class will be like below ...

```cs
internal class Logger
    {
        private static Logger _logger;
        private static AbstractLogger _chainOfLogger;

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

        // The CreateLog method is a private method that creates a log with the provided level and message.
        private void CreateLog(LoggerLevel level, string message)
        {
            // It calls the LogMessage method on the chain of loggers with the provided level, message, and logger target.
            _chainOfLogger.LogMessage(level, message, _loggerTarget);
        }
    }
```

- Let's test our application till now, let's create a simple console app to test it ...

```cs
using LoggingSystem;

Logger logger = Logger.GetLogger();

logger.Info("This is info message");
Console.WriteLine("___");
logger.Error("This is Error message");
Console.WriteLine("___");
logger.Debug("This is Debug message");
```

and after running it, we will have the result as below


![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/a0yc6d9hilwjqyklrr9m.png)

As you can see:
1- Info message prints only log
2- Error message prints info and error
3- Debug message prints info, error, and debug.

___

### Now let's move to the last part of our implementation, which is the log target, here we will be using the _**Observer design pattern**_

**_Observer _** is a behavioral design pattern that lets you define a subscription mechanism to notify multiple objects about any events that happen to the object they’re observing.


![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/gfroc45z4xvso5uhmnjh.png)

So, we will have two parts here ...
 1- The target, which will be the changing part, where the log target is changing.
 2- The constant part, which is the observer.

- So Lt's create the 2 classes, first we will write an interface for all Log observers (console, file, database, ..etc).

- This class will have only the log function, to force all its implementations to implement it.

```cs
    internal interface ILogObserver
    {
        void Log (string message);
    }
```
- Now let's assume we will have console, database and file loggers ...
- And let's ignore the implementation of the logging mechanism inside each of the logging targets for now

- Console Logger
```cs
    internal class ConsoleLogger : ILogObserver
    {
        public void Log(string message)
        {
            Console.WriteLine("Writing to Console " + message);
        }
    }
```

- File Logger
```cs
    internal class FileLogger : ILogObserver
    {
        public void Log(string message)
        {
            Console.WriteLine("Writing to File " + message);
        }
    }
```

- Database Logger
```cs
    internal class DatabaseLogger : ILogObserver
    {
        public void Log(string message)
        {
            Console.WriteLine("Writing to Database" + message);
        }
    }
```

### Now let's implement the observer class which will contain:
1- A list of observers.
2- A function to add a new observer.
3- A second method to remove an observer.
4- A third method to notify all observers.

```cs
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
```

### Now, where will we notify our observers ?🤔
- The Logger class of course...

- Same as what we did with the chain of logger, we will instantiate Log target in the logger class in the GetLogger function using the Logger manager again.

- Also the LoggerTarget needs to be passed to the LogMessage and Display function.

```cs
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
```
- And the LoggerManager will implement the new `AddObservers` function as below


```cs
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

        // AddObservers is a static method that sets up observers for different logging levels.
        public static LoggerTarget AddObservers()
        {
            // Create a new LoggerTarget. This is the target that the observers will be observing.
            LoggerTarget loggerTarget = new LoggerTarget();

            // Create a new ConsoleLogger. This logger will log messages to the console.
            ConsoleLogger consoleLogger = new ConsoleLogger();
            
            // Add the ConsoleLogger as an observer for Info level logs.
            loggerTarget.AddObserver(LoggerLevel.Info, consoleLogger);
            
            // Add the ConsoleLogger as an observer for Error level logs.
            loggerTarget.AddObserver(LoggerLevel.Error, consoleLogger);
            
            // Add the ConsoleLogger as an observer for Debug level logs.
            loggerTarget.AddObserver(LoggerLevel.Debug, consoleLogger);

            // Create a new FileLogger. This logger will log messages to a file.
            FileLogger fileLogger = new FileLogger();
            
            // Add the FileLogger as an observer for Error level logs.
            loggerTarget.AddObserver(LoggerLevel.Error, fileLogger);

            // Return the LoggerTarget with the observers added.
            return loggerTarget;
        }
}
```
👁️ **Observer Setup**: The `AddObservers` method sets up observers for different logging levels using the Observer design pattern. Observers watch for changes in the target (the `LoggerTarget`) and react accordingly.

📚 **LoggerTarget Creation**: It creates a new `LoggerTarget`, which serves as the target that observers will be observing. The `LoggerTarget` manages the list of observers and notifies them of any changes.

🖥️ **ConsoleLogger Setup**: The method creates a `ConsoleLogger`, which logs messages to the console. It then adds this `ConsoleLogger` as an observer for Info, Error, and Debug level logs.

📁 **FileLogger Setup**: Additionally, the method creates a `FileLogger`, which logs messages to a file. It adds this `FileLogger` as an observer specifically for Error level logs.

🔍 **Observer Registration**: Observers are registered with the `LoggerTarget` using the `AddObserver` method, specifying the logging level they are interested in and the corresponding logger to handle messages at that level.

📝 **Return**: Finally, the method returns the `LoggerTarget` with all the observers added, ready to observe and react to changes in logging levels.

#### Notice that in the Logger class, LoggerTarget is now sent to the LogMessage function, it will be passed to the display function for each type of logger (Debug, info, or error) to notify its observers.

- So, let's edit the Abstract Logger too, to be like below 

```cs
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
        if (Levels <= level)
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
```
- Now let's edit all implementations of the Display function..

- Info Logger
```cs
internal class InfoLogger : AbstractLogger
{
    public InfoLogger(LoggerLevel levels)
    {
        this.Levels = levels;
    }

    protected override void Display(string msg, LoggerTarget loggerTarget)
    {
        //replace console log with notify observer 
        loggerTarget.NotifyAllObservers(LoggerLevel.Info, "INFO: " + msg);
    }
}
```

- Error Logger
```cs
internal class ErrorLogger : AbstractLogger
{
    public ErrorLogger(LoggerLevel levels)
    {
        this.Levels = levels;
    }

    protected override void Display(string msg, LoggerTarget loggerTarget)
    {
        //replace console log with notify observer
        loggerTarget.NotifyAllObservers(LoggerLevel.Error, "ERROR: " + msg);
    }
}
```

- Debug Logger
```cs
    internal class DebugLogger : AbstractLogger
    {
        public DebugLogger(LoggerLevel level)
        {
            this.Levels = level;
        }

        protected override void Display (string msg, LoggerTarget loggerTarget)
        {
             //replace console log with notify observer
             loggerTarget.NotifyAllObservers(LoggerLevel.Debug, "DEBUG: " + msg);
        }
    }
```

- let's run the solution now and check the result ...

![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/h4adflgah9nqqrghp50h.png)

And it's working as expected, because according to our configuration, we are
  - printing info message to console
  - printing error message in console and file (and I will be printed in error and info levels).
  - printing debug message in console and file (and I will be printed in error,info and debug levels).

### and we can easily change this behavior...

- Let's assume we only need to print in the same level as instructed, not the level and the levels below it, then in the LogMessage function, we can easily edit it like below

```cs
        public void LogMessage(LoggerLevel level, string msg, LoggerTarget loggerTarget)
        {
            //old: if (Levels <= level)
            if (Levels == level)
            {
                Display(msg, loggerTarget);
            }
         //The rest of the code of Abstract logger
```

- Now the result will be 


![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/r9kl2bne746ssqxr6njd.png)


#### So as instructed in the LoggerManager class :
1- Console Logger will have info, debug and error.
2- File logger will have only errors.

### Of course this LoggerManager in a real logger system will read from a config file to give you the flexibility to change logging levels and targets.

____

# Wrapping Up: Embracing the Power of Design Patterns in Logging

In this tutorial, we've explored the intricate world of low-level design by building a logging framework from scratch. We've delved into the Singleton pattern, ensuring our logging class has only one instance, and the Chain of Responsibility pattern, allowing different loggers to handle log messages based on their levels. We've also embraced the Observer pattern, setting up observers to react to changes in logging levels.

By understanding and implementing these design patterns, we've not only created a functional logging system but also gained valuable insights into the art of designing flexible, extensible, and maintainable software solutions. We hope this tutorial has inspired you to apply these principles in your own projects and continue to explore the vast landscape of software design patterns. Happy coding!


Follow me on:
[Linkedin](https://www.linkedin.com/in/ahmedadel333/).