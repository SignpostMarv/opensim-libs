using System;

namespace HttpServer
{
    public enum LogPrio
    {
        /// <summary>
        /// Very detailed logs to be able to follow the flow of the program.
        /// </summary>
        Trace,

        /// <summary>
        /// Logs to help debug errors in the application
        /// </summary>
        Debug,

        /// <summary>
        /// Information to be able to keep track of state changes etc.
        /// </summary>
        Info,

        /// <summary>
        /// Something did not go as we expected, but it's no problem.
        /// </summary>
        Warning,

        /// <summary>
        /// Something that should not fail failed, but we can still keep
        /// on going.
        /// </summary>
        Error,

        /// <summary>
        /// Something failed, and we cannot handle it properly.
        /// </summary>
        Fatal
    }

    /// <summary>
    /// A delegate that helps us keep track of errors in the system.
    /// </summary>
    /// <param name="source">class that writes the log entry.</param>
    /// <param name="priority">priority for the message</param>
    /// <param name="message">log message</param>
    public delegate void WriteLogHandler(object source, LogPrio priority, string message);

    public class ConsoleLogWriter
    {
        public static void Logwriter(object source, LogPrio prio, string message)
        {
            switch (prio)
            {
                case LogPrio.Trace:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case LogPrio.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogPrio.Info:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case LogPrio.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogPrio.Error:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogPrio.Fatal:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine(source.GetType().Name + " " + prio + ": " + message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }        
    }
}
