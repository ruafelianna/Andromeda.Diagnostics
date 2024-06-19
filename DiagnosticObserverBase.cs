using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Andromeda.Diagnostics
{
    /// <summary>
    /// Base observer for diagnostic listener events
    /// </summary>
    public abstract class DiagnosticObserverBase :
        IObserver<DiagnosticListener>
    {
        /// <summary>
        /// Creates <see cref="DiagnosticObserverBase"/> instance
        /// </summary>
        ///
        /// <param name="observer">
        /// Some <see cref="KeyValuePair{string, object?}"/> observer,
        /// for example, <see cref="DiagnosticLoggerBase"/>
        /// </param>
        public DiagnosticObserverBase(
            IObserver<KeyValuePair<string, object?>> observer)
        {
            _syncRoot = new();
            _observer = observer;
            _subscriptions = [];
        }

        /// <summary>
        /// Whether observer is enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// List of the listeners names which should be listened to
        /// </summary>
        public abstract IReadOnlyList<string> ShouldListenTo { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        ///
        /// <param name="diagnosticListener"><inheritdoc/></param>
        public void OnNext(DiagnosticListener diagnosticListener)
        {
            if (!IsEnabled)
            {
                return;
            }

            lock (_syncRoot)
            {
                if (ShouldListenTo.Contains(diagnosticListener.Name))
                {
                    _subscriptions.Add(
                        diagnosticListener.Subscribe(_observer)
                    );
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void OnCompleted()
        {
            lock (_syncRoot)
            {
                _subscriptions.ForEach(x => x.Dispose());
                _subscriptions.Clear();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="error"></param>
        public void OnError(Exception error) { }

        /// <summary>
        /// List of subscriptions to dispose on completion
        /// </summary>
        protected readonly List<IDisposable> _subscriptions;

        /// <summary>
        /// Subscriptions syncronization root
        /// </summary>
        protected readonly object _syncRoot;

        /// <summary>
        /// Events observer
        /// </summary>
        protected readonly IObserver<KeyValuePair<string, object?>> _observer;
    }
}
