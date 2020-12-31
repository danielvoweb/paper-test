using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace PaperTest
{
    public class PaperLogger : ILogger, IDisposable
    {
        private readonly Action<string> _output = Console.WriteLine;

        public void Dispose()
        {
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter) => _output(formatter(state, exception));

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => this;
    }

    public static class LoggingBuilderExtensions
    {
        public static void AddPaperLogger(this ILoggingBuilder builder)
        {
            builder.AddProvider(new PaperLoggerProvider());
        }
    }

    public sealed class PaperLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, PaperLogger> _loggers = new();

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new PaperLogger());

        public void Dispose() => _loggers.Clear();
    }
}