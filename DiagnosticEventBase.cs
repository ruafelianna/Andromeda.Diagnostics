using System;
using System.Diagnostics;

namespace Andromeda.Diagnostics
{
    /// <summary>
    /// Diagnostic event with base info
    /// </summary>
    public record DiagnosticEventBase
    {
        /// <summary>
        /// Timestamp in UTC
        /// </summary>
        public required DateTime Timestamp { get; init; }

        /// <summary>
        /// Operation name
        /// </summary>
        public required string Operation { get; init; }

        /// <summary>
        /// Operation ID
        /// </summary>
        public required Guid OperationId { get; init; }

        /// <summary>
        /// Stack trace
        /// </summary>
        public StackTrace? StackTrace { get; init; }

        /// <summary>
        /// Timestamp in local timezone
        /// </summary>
        public DateTime TimestampLocal => Timestamp.ToLocalTime();
    }
}
