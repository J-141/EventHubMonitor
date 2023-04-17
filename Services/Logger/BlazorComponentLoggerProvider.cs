using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace EventHubMonitor.Services.Logger;
public class BlazorComponentLoggerProvider : ILoggerProvider {
    public ConcurrentQueue<string> LogMessages { get; } = new ConcurrentQueue<string>();
    public event EventHandler<string> OnMessageLogged;

    public ILogger CreateLogger(string categoryName) {
        return new ComponentLogger(this);
    }

    public void Dispose() {
    }

    internal void LogMessage(string message) {
        LogMessages.Enqueue(message);
        OnMessageLogged?.Invoke(this, message);
    }
}


public class ComponentLogger : ILogger {
    private readonly BlazorComponentLoggerProvider _provider;

    public ComponentLogger(BlazorComponentLoggerProvider provider) {
        _provider = provider;
    }

    public IDisposable BeginScope<TState>(TState state) {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel) {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
        if (!IsEnabled(logLevel)) {
            return;
        }

        string message = formatter(state, exception);
        _provider.LogMessage(message);
    }
}