// C# Translation of the RetryStrategy concept from pytdx/base_socket_client.py

using System;
using System.Collections.Generic;

namespace Pytdx
{
    /// <summary>
    /// Defines a strategy for retrying a failed operation.
    /// </summary>
    public interface IRetryStrategy
    {
        /// <summary>
        /// Gets the sequence of time intervals to wait between retries.
        /// </summary>
        /// <returns>An enumerable of TimeSpan representing the wait intervals.</returns>
        IEnumerable<TimeSpan> GetIntervals();
    }
}
