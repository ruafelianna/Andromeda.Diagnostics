using System;
using System.Diagnostics;

namespace Andromeda.Diagnostics
{
    public static class DiagnosticListenerExt
    {
        /// <summary>
        /// Registers event if it is enabled, and assigns a GUID to it
        /// </summary>
        ///
        /// <param name="listener">Events listener instance</param>
        ///
        /// <param name="eventName">Name of the event</param>
        ///
        /// <param name="operation">Operation name</param>
        ///
        /// <param name="createEvent">
        /// Function, which can construct required event from the base event.
        /// If <paramref name="createEvent"/> is null,
        /// <see cref="DiagnosticEventBase"/> is used instead
        /// </param>
        ///
        /// <returns>Newly assigned event GUID</returns>
        public static Guid RegisterEventWithId(
            this DiagnosticListener listener,
            string eventName,
            string operation,
            Func<DiagnosticEventBase, object?>? createEvent = null
        ) => listener.RegisterEventInternal(
            eventName, operation, null, createEvent
        );

        /// <summary>
        /// Registers event if it is enabled, with a given GUID
        /// </summary>
        ///
        /// <param name="listener">Events listener instance</param>
        ///
        /// <param name="eventName">Name of the event</param>
        ///
        /// <param name="operation">Operation name</param>
        ///
        /// <param name="operationId">
        /// Operation GUID. If <paramref name="operationId"/> is null,
        /// new GUID is created for the event
        /// </param>
        ///
        /// <param name="createEvent">
        /// Function, which can construct required event from the base event.
        /// If <paramref name="createEvent"/> is null,
        /// <see cref="DiagnosticEventBase"/> is used instead
        /// </param>
        public static void RegisterEvent(
            this DiagnosticListener listener,
            string eventName,
            string operation,
            Guid? operationId = null,
            Func<DiagnosticEventBase, object?>? createEvent = null
        ) => listener.RegisterEventInternal(
            eventName, operation, operationId, createEvent
        );

        /// <summary>
        /// Registers event if it is enabled
        /// </summary>
        ///
        /// <param name="listener">Events listener instance</param>
        ///
        /// <param name="eventName">Name of the event</param>
        ///
        /// <param name="operation">Operation name</param>
        ///
        /// <param name="operationId">
        /// Operation GUID. If <paramref name="operationId"/> is null,
        /// new GUID is created for the event
        /// </param>
        ///
        /// <param name="createEvent">
        /// Function, which can construct required event from the base event.
        /// If <paramref name="createEvent"/> is null,
        /// <see cref="DiagnosticEventBase"/> is used instead
        /// </param>
        ///
        /// <returns>
        /// If event should not be registered returns <code>default(Guid)</code>
        /// Else returns
        /// <paramref name="operationId"/>, if it is not null,
        /// or newly assigned GUID in other case
        /// </returns>
        private static Guid RegisterEventInternal(
            this DiagnosticListener listener,
            string eventName,
            string operation,
            Guid? operationId = null,
            Func<DiagnosticEventBase, object?>? createEvent = null
        )
        {
            if (listener.IsEnabled(eventName))
            {
                operationId ??= Guid.NewGuid();

                var baseInfo = new DiagnosticEventBase
                {
                    OperationId = operationId.Value,
                    Operation = operation,
                    Timestamp = DateTime.UtcNow,
                };

                createEvent ??= x => x;

                listener.Write(eventName, createEvent(baseInfo));

                return operationId.Value;
            }
            return default;
        }
    }
}
