using System;
using System.Diagnostics;
using System.Globalization;

// For ToString("F2", CultureInfo.InvariantCulture)

namespace GoldArch.TaskWrapperReport.TaskWrapperCore
{
    /// <summary>
    /// Utility class for task reporting, primarily for formatting time and date.
    /// </summary>
    public static class TaskReportUtil
    {
        /// <summary>
        /// Gets the current date and time formatted as "yyyy-MM-dd HH:mm:ss".
        /// </summary>
        /// <returns>A string representing the current date and time.</returns>
        public static string Now年月日时分秒() // Consider renaming to English for consistency if publishing widely, e.g., GetCurrentTimestamp
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Stops the specified stopwatch and returns the elapsed time in seconds, formatted to two decimal places (e.g., "1.23s").
        /// </summary>
        /// <param name="stopwatch">The <see cref="Stopwatch"/> instance to stop and measure.</param>
        /// <returns>A string representing the elapsed time in seconds.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stopwatch"/> is null.</exception>
        public static string StopAndgetSecondF2(Stopwatch stopwatch) // Consider renaming, e.g., GetElapsedSecondsFormatted
        {
            if (stopwatch == null) throw new ArgumentNullException(nameof(stopwatch));

            stopwatch.Stop();
            // Using InvariantCulture to ensure '.' is used as decimal separator regardless of system culture.
            return (stopwatch.ElapsedMilliseconds / 1000.0).ToString("F2", CultureInfo.InvariantCulture) + "s";
        }
    }
}