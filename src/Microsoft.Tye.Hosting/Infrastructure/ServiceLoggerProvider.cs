// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;

namespace Microsoft.Tye.Hosting;

internal class ServiceLoggerProvider(Subject<string> logs) : ILoggerProvider
{
    private readonly Subject<string> _logs = logs;

    public ILogger CreateLogger(string categoryName)
    {
        return new ServiceLogger(categoryName, _logs);
    }

    public void Dispose()
    {
    }

    private class ServiceLogger(string categoryName, Subject<string> logs) : ILogger
    {
        private readonly string _categoryName = categoryName;
        private readonly Subject<string> _logs = logs;

#pragma warning disable CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
        public IDisposable BeginScope<TState>(TState state)
#pragma warning restore CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
        {
            return null!;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string>? formatter)
        {
            if (exception != null)
            {
                if (formatter != null)
                {
                    _logs.OnNext($"[{logLevel}]: {formatter(state, exception)}");
                }

                _logs.OnNext(exception.ToString());
            }
        }
    }
}
