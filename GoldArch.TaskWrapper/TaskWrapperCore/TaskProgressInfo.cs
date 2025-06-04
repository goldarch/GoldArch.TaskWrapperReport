namespace GoldArch.TaskWrapperReport.TaskWrapperCore
{
    /// <summary>
    /// Defines the severity or type of a progress report message.
    /// </summary>
    public enum ReportLevel
    {
        /// <summary>
        /// Standard informational message.
        /// </summary>
        Information,
        /// <summary>
        /// A warning message that doesn't halt the process but indicates potential issues.
        /// </summary>
        Warning,
        /// <summary>
        /// An error message indicating a failure in part of the process.
        /// </summary>
        Error,
        /// <summary>
        /// A success message, often used for completion status.
        /// </summary>
        Success,
        /// <summary>
        /// Detailed or verbose information, potentially for debugging or fine-grained logging.
        /// </summary>
        Detail,
        /// <summary>
        /// General status update, often for the progress bar text.
        /// </summary>
        StatusUpdate,
        /// <summary>
        /// Indicates the start of a process or phase.
        /// </summary>
        ProcessStart,
        /// <summary>
        /// Indicates the end or completion of a process or phase.
        /// </summary>
        ProcessEnd,
        /// <summary>
        /// Indicates a process was cancelled.
        /// </summary>
        ProcessCancelled
    }

    /// <summary>
    /// Represents the data associated with a progress update from a task.
    /// This class is UI-agnostic and carries semantic information about the progress.
    /// </summary>
    public class TaskProgressInfo
    {
        /// <summary>
        /// Gets the main content or message of the progress update.
        /// Can be null if the update is purely for progress value/text.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the text to be displayed on a progress indicator (e.g., a progress bar).
        /// Can be null if the update is purely for content.
        /// </summary>
        public string ProgressText { get; }

        /// <summary>
        /// Gets the current progress value (e.g., percentage complete).
        /// Can be null if not applicable.
        /// </summary>
        public int? ProgressValue { get; }

        /// <summary>
        /// Gets the <see cref="ReportLevel"/> indicating the nature of this progress update.
        /// </summary>
        public ReportLevel Level { get; }

        /// <summary>
        /// Gets an optional tag for custom message typing or categorization.
        /// </summary>
        public string MessageTypeTag { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProgressInfo"/> class with all parameters.
        /// </summary>
        /// <param name="content">The main content or message.</param>
        /// <param name="progressText">The text for the progress indicator.</param>
        /// <param name="progressValue">The current progress value.</param>
        /// <param name="level">The report level of this update.</param>
        /// <param name="messageTypeTag">An optional tag for custom message typing.</param>
        public TaskProgressInfo(string content, string progressText, int? progressValue, ReportLevel level, string messageTypeTag = null)
        {
            Content = content;
            ProgressText = progressText;
            ProgressValue = progressValue;
            Level = level;
            MessageTypeTag = messageTypeTag;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProgressInfo"/> class, primarily for content updates.
        /// </summary>
        /// <param name="content">The main content or message.</param>
        /// <param name="level">The report level of this update.</param>
        /// <param name="messageTypeTag">An optional tag for custom message typing.</param>
        public TaskProgressInfo(string content, ReportLevel level, string messageTypeTag = null)
            : this(content, null, null, level, messageTypeTag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProgressInfo"/> class, primarily for progress bar updates.
        /// </summary>
        /// <param name="progressText">The text for the progress indicator.</param>
        /// <param name="progressValue">The current progress value.</param>
        /// <param name="level">The report level of this update (defaults to <see cref="ReportLevel.StatusUpdate"/>).</param>
        /// <param name="messageTypeTag">An optional tag for custom message typing.</param>
        public TaskProgressInfo(string progressText, int? progressValue, ReportLevel level = ReportLevel.StatusUpdate, string messageTypeTag = null)
            : this(null, progressText, progressValue, level, messageTypeTag)
        {
        }
    }
}