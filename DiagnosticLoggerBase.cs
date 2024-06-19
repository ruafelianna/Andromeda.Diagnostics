using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Andromeda.Diagnostics
{
    /// <summary>
    /// Observer which logs events to <see cref="ILogger"/>
    /// </summary>
    public abstract class DiagnosticLoggerBase :
        IObserver<KeyValuePair<string, object?>>
    {
        /// <summary>
        /// Creates <see cref="DiagnosticLoggerBase"/> instance
        /// </summary>
        ///
        /// <param name="logger">Logger instance</param>
        public DiagnosticLoggerBase(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"><inheritdoc/></param>
        public void OnNext(KeyValuePair<string, object?> value)
        {
            if (value.Value is null)
            {
                return;
            }

            if (value.Value is DiagnosticEventBase deb)
            {
                Log(value.Key, deb, GetAdditionals(value.Key, deb));
            }
            else
            {
                Log(value.Key, value.Value);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void OnCompleted() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="error"><inheritdoc/></param>
        public void OnError(Exception error) { }

        /// <summary>
        /// Logger instance
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Log message with base information
        /// </summary>
        protected static StringBuilder BaseLogMsg => new StringBuilder()
            .AppendLine("Timestamp = {ts}")
            .AppendLine("EventId = {cmd}")
            .AppendLine("MethodName = {op}")
            .AppendLine("Guid = {opId}");

        /// <summary>
        /// Get additional information fields
        /// </summary>
        ///
        /// <param name="eventName">Name of the event</param>
        ///
        /// <param name="eventObj">Event instance</param>
        ///
        /// <returns>A dictionary of information fields and objects that describe those fields</returns>
        protected abstract IDictionary<string, object?> GetAdditionals(
            string eventName,
            DiagnosticEventBase eventObj
        );

        /// <summary>
        /// Logs message which is not <see cref="DiagnosticEventBase"/> or it's child
        /// </summary>
        ///
        /// <param name="eventName">Name of the event</param>
        ///
        /// <param name="eventObj">Event instance</param>
        protected abstract void Log(string eventName, object? eventObj);

        /// <summary>
        /// Logs message which is <see cref="DiagnosticEventBase"/> or it's child
        /// </summary>
        ///
        /// <param name="eventName">Name of the event</param>
        ///
        /// <param name="eventObj">Event instance</param>
        ///
        /// <param name="additionals">Dictionary which is returned by <see cref="GetAdditionals"/></param>
        protected void Log(
            string eventName,
            DiagnosticEventBase eventObj,
            IDictionary<string, object?> additionals
        )
        {
            List<object?> args = [
                eventObj.Timestamp.ToLocalTime().ToString(),
                eventName,
                eventObj.Operation,
                eventObj.OperationId,
                .. additionals.Values,
            ];

            var logMsg = BaseLogMsg
                .Append(
                    string.Join(
                        Environment.NewLine,
                        additionals.Keys
                    )
                );

            if (eventObj.StackTrace is not null)
            {
                args.Add(eventObj.StackTrace.ToString());
                logMsg
                    .AppendLine()
                    .AppendLine("StackTrace =")
                    .Append("{stTr}");
            }
#pragma warning disable CA2254 // Template should be a static expression

#warning Should probably be not only trace level
            _logger.LogTrace(logMsg.ToString(), [.. args]);

#pragma warning restore CA2254 // Template should be a static expression
        }
    }
}
