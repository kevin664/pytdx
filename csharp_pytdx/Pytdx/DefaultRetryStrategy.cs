// C# Translation of the DefaultRetryStrategy class from pytdx/base_socket_client.py

using System;
using System.Collections.Generic;

namespace Pytdx
{
    /// <summary>
    /// The default retry strategy. Retries 4 times with increasing delays.
    /// </summary>
    public class DefaultRetryStrategy : IRetryStrategy
    {
        /// <summary>
        /// Returns the default intervals: 0.1, 0.5, 1, and 2 seconds.
        /// </summary>
        public IEnumerable<TimeSpan> GetIntervals()
        {
            yield return TimeSpan.FromSeconds(0.1);
            yield return TimeSpan.FromSeconds(0.5);
            yield return TimeSpan.FromSeconds(1.0);
            yield return TimeSpan.FromSeconds(2.0);
        }
    }
}
