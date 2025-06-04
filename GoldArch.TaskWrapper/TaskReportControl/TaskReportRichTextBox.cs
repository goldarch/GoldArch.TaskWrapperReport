using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GoldArch.TaskWrapperReport.Common;
using GoldArch.TaskWrapperReport.TaskWrapperCore;

// For TextProgressBar, RichTextBoxEx02

// For TaskWrapper, TaskProgressInfo etc.

namespace GoldArch.TaskWrapperReport.TaskReportControl
{
    /// <summary>
    /// A user control that combines a <see cref="TextProgressBar"/> and a <see cref="RichTextBoxEx02"/>
    /// to display detailed progress and log messages from a <see cref="TaskWrapper"/> instance.
    /// </summary>
    public partial class TaskReportRichTextBox : UserControl
    {
        private readonly Progress<TaskProgressInfo> _directProgressReporter = new Progress<TaskProgressInfo>();

        /// <summary>
        /// Gets the instance of <see cref="TaskWrapper"/> used by this control to manage and report task progress.
        /// </summary>
        [Browsable(false)] // Not typically set in designer, managed internally or by consuming code.
        public TaskWrapper TaskWrapperInstance { get; } = new TaskWrapper();

        #region Appearance Properties for RichTextBox Content
        /// <summary>Gets or sets the default text color for informational messages in the RichTextBox.</summary>
        [Category("Appearance - RichTextBox")]
        [Description("Default text color for informational messages in the RichTextBox.")]
        public Color InfoTextColor { get; set; } = SystemColors.ControlText;

        /// <summary>Gets or sets the text color for warning messages in the RichTextBox.</summary>
        [Category("Appearance - RichTextBox")]
        [Description("Text color for warning messages in the RichTextBox.")]
        public Color WarningTextColor { get; set; } = Color.DarkOrange;

        /// <summary>Gets or sets the text color for error messages in the RichTextBox.</summary>
        [Category("Appearance - RichTextBox")]
        [Description("Text color for error messages in the RichTextBox.")]
        public Color ErrorTextColor { get; set; } = Color.Red;

        /// <summary>Gets or sets the text color for success messages in the RichTextBox.</summary>
        [Category("Appearance - RichTextBox")]
        [Description("Text color for success messages in the RichTextBox.")]
        public Color SuccessTextColor { get; set; } = Color.Green;

        /// <summary>Gets or sets the text color for detailed or debug messages in the RichTextBox.</summary>
        [Category("Appearance - RichTextBox")]
        [Description("Text color for detailed or debug messages in the RichTextBox.")]
        public Color DetailTextColor { get; set; } = Color.Gray;

        /// <summary>Gets or sets the text color for process start messages in the RichTextBox.</summary>
        [Category("Appearance - RichTextBox")]
        [Description("Text color for process start messages in the RichTextBox.")]
        public Color ProcessStartTextColor { get; set; } = Color.Blue;

        /// <summary>Gets or sets the text color for process end messages in the RichTextBox.</summary>
        [Category("Appearance - RichTextBox")]
        [Description("Text color for process end messages in the RichTextBox.")]
        public Color ProcessEndTextColor { get; set; } = Color.DarkBlue;

        /// <summary>Gets or sets the text color for process cancelled messages in the RichTextBox.</summary>
        [Category("Appearance - RichTextBox")]
        [Description("Text color for process cancelled messages in the RichTextBox.")]
        public Color ProcessCancelledTextColor { get; set; } = Color.OrangeRed;
        #endregion

        #region Appearance Properties for ProgressBar
        /// <summary>Gets or sets the progress bar color for normal operation.</summary>
        [Category("Appearance - ProgressBar")]
        [Description("Progress bar color for normal operation.")]
        public Color ProgressBarDefaultColor { get; set; } = Color.LightGreen;

        /// <summary>Gets or sets the progress bar color for warnings.</summary>
        [Category("Appearance - ProgressBar")]
        [Description("Progress bar color for warnings.")]
        public Color ProgressBarWarningColor { get; set; } = Color.Orange;

        /// <summary>Gets or sets the progress bar color for errors.</summary>
        [Category("Appearance - ProgressBar")]
        [Description("Progress bar color for errors.")]
        public Color ProgressBarErrorColor { get; set; } = Color.Red;

        /// <summary>Gets or sets the progress bar color for success or completion.</summary>
        [Category("Appearance - ProgressBar")]
        [Description("Progress bar color for success or completion.")]
        public Color ProgressBarSuccessColor { get; set; } = Color.Green;
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether the RichTextBox should always scroll to the end after new content is reported.
        /// </summary>
        [Category("Behavior")]
        [Description("Indicates whether the RichTextBox should always scroll to the end after new content is reported.")]
        [DefaultValue(true)]
        public bool IsScrollToEndAfterReport { get; set; } = true;

        /// <summary>
        /// Gets the underlying <see cref="TextProgressBar"/> control.
        /// </summary>
        [Browsable(false)]
        public TextProgressBar TextProgressBarControl => textProgressBar1;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="TaskWrapperInstance"/> should show stack traces in its error reports.
        /// </summary>
        [Category("Behavior")]
        [Description("Indicates whether the TaskWrapper should include stack traces in its error reports displayed in this control.")]
        [DefaultValue(true)]
        public bool ShowErrorStackTraceInWrapper
        {
            get => TaskWrapperInstance.ShowErrorStackTrace;
            set => TaskWrapperInstance.ShowErrorStackTrace = value;
        }

        private bool _showCancelButton = true;
        /// <summary>
        /// Gets or sets a value indicating whether the Cancel button is visible.
        /// </summary>
        [Category("Appearance")]
        [Description("Determines whether the Cancel button is visible.")]
        [DefaultValue(true)]
        public bool ShowCancelButton
        {
            get => _showCancelButton;
            set
            {
                if (_showCancelButton != value)
                {
                    _showCancelButton = value;
                    ControlInvokeHelper.ControlInvoke(this, () => { button取消.Visible = _showCancelButton; });
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskReportRichTextBoxV3"/> class.
        /// </summary>
        public TaskReportRichTextBox()
        {
            InitializeComponent();
            checkBox报告是否总是滚动到最后.Checked = IsScrollToEndAfterReport;
            button取消.Visible = _showCancelButton; // Initialize based on property
        }

        private void ProgressControl_Load(object sender, EventArgs e)
        {
            TaskWrapperInstance.ProgressReporter.ProgressChanged += HandleProgressInfoReceived;
            _directProgressReporter.ProgressChanged += HandleProgressInfoReceived;
            TaskWrapperInstance.StateChanged += TaskWrapper_StateChanged;

            UpdateCancelButtonState(TaskWrapperInstance.CurrentState); // Initial state
        }

        private void TaskWrapper_StateChanged(object sender, TaskStateChangedEventArgs e)
        {
            ControlInvokeHelper.ControlInvoke(this, () => UpdateCancelButtonState(e.NewState));
        }

        private void UpdateCancelButtonState(TaskExecutionState currentState)
        {
            if (button取消 != null && !button取消.IsDisposed && button取消.Visible)
            {
                button取消.Enabled = (currentState == TaskExecutionState.Running ||
                                     currentState == TaskExecutionState.Starting ||
                                     currentState == TaskExecutionState.Cancelling);
            }
        }

        /// <summary>
        /// Reports progress information directly to this control's UI without going through the TaskWrapper.
        /// </summary>
        /// <param name="progressInfo">The progress information to report.</param>
        public void ReportDirect(TaskProgressInfo progressInfo)
        {
            if (progressInfo == null) throw new ArgumentNullException(nameof(progressInfo));
            ((IProgress<TaskProgressInfo>)_directProgressReporter).Report(progressInfo);
        }

        /// <summary>
        /// Reports content directly to this control's RichTextBox.
        /// </summary>
        /// <param name="content">The content to report.</param>
        /// <param name="level">The report level, influencing the text color.</param>
        /// <param name="messageTypeTag">An optional message type tag.</param>
        public void ReportDirectContent(string content, ReportLevel level, string messageTypeTag = null)
        {
            ReportDirect(new TaskProgressInfo(content, level, messageTypeTag));
        }

        /// <summary>
        /// Reports progress directly to this control's ProgressBar.
        /// </summary>
        /// <param name="progressText">The text for the progress bar.</param>
        /// <param name="progressValue">The value for the progress bar.</param>
        /// <param name="level">The report level, influencing the progress bar color.</param>
        public void ReportDirectProgress(string progressText, int? progressValue, ReportLevel level = ReportLevel.StatusUpdate)
        {
            ReportDirect(new TaskProgressInfo(progressText, progressValue, level));
        }

        private void HandleProgressInfoReceived(object sender, TaskProgressInfo progressInfo)
        {
            ReportInfoToUI(progressInfo);
        }

        private void ReportInfoToUI(TaskProgressInfo progressInfo)
        {
            if (progressInfo == null) return; // Should not happen if ReportDirect validates

            ControlInvokeHelper.ControlInvoke(this, () =>
            {
                Color contentColor = GetContentColorForLevel(progressInfo.Level);
                Color progressBarColor = GetProgressBarColorForLevel(progressInfo.Level);

                // Update ProgressBar
                if (progressInfo.ProgressValue.HasValue)
                {
                    int val = progressInfo.ProgressValue.Value;
                    if (textProgressBar1.Minimum <= val && val <= textProgressBar1.Maximum)
                        textProgressBar1.Value = val;
                    else if (val < textProgressBar1.Minimum)
                        textProgressBar1.Value = textProgressBar1.Minimum;
                    else if (val > textProgressBar1.Maximum)
                        textProgressBar1.Value = textProgressBar1.Maximum;
                }

                if (progressInfo.ProgressText != null) // Allow clearing text by passing null
                {
                    textProgressBar1.CustomText = progressInfo.ProgressText;
                }

                textProgressBar1.ProgressColor = progressBarColor;

                // Update RichTextBox
                if (!string.IsNullOrEmpty(progressInfo.Content)) // Only append if there's content
                {
                    // RichTextBoxLog is RichTextBoxEx02 which handles its own SelectionColor
                    RichTextBoxLog.SelectionColor = contentColor;
                    RichTextBoxLog.AppendText(progressInfo.Content);
                    // RichTextBoxLog.SelectionColor = originalColor; // RichTextBoxEx02 should manage this internally

                    if (IsScrollToEndAfterReport)
                    {
                        ScrollToEnd();
                    }
                }
            });
        }

        private Color GetContentColorForLevel(ReportLevel level)
        {
            switch (level)
            {
                case ReportLevel.Information: return InfoTextColor;
                case ReportLevel.Warning: return WarningTextColor;
                case ReportLevel.Error: return ErrorTextColor;
                case ReportLevel.Success: return SuccessTextColor;
                case ReportLevel.Detail: return DetailTextColor;
                case ReportLevel.ProcessStart: return ProcessStartTextColor;
                case ReportLevel.ProcessEnd: return ProcessEndTextColor;
                case ReportLevel.ProcessCancelled: return ProcessCancelledTextColor;
                case ReportLevel.StatusUpdate: // Status updates usually don't have RichTextBox content
                default:
                    return InfoTextColor;
            }
        }

        private Color GetProgressBarColorForLevel(ReportLevel level)
        {
            switch (level)
            {
                case ReportLevel.Warning: return ProgressBarWarningColor;
                case ReportLevel.Error: return ProgressBarErrorColor;
                case ReportLevel.ProcessCancelled: return ProgressBarWarningColor; // Or a specific cancel color
                case ReportLevel.Success:
                case ReportLevel.ProcessEnd:
                    return ProgressBarSuccessColor;
                case ReportLevel.ProcessStart: // Use default for start, or a specific start color
                case ReportLevel.Information:
                case ReportLevel.StatusUpdate:
                case ReportLevel.Detail:
                default:
                    return ProgressBarDefaultColor;
            }
        }

        /// <summary>
        /// Scrolls the RichTextBox to the end of its content.
        /// </summary>
        public void ScrollToEnd()
        {
            ControlInvokeHelper.ControlInvoke(this, () =>
            {
                if (RichTextBoxLog != null && !RichTextBoxLog.IsDisposed && RichTextBoxLog.IsHandleCreated)
                {
                    RichTextBoxLog.SelectionStart = RichTextBoxLog.TextLength;
                    RichTextBoxLog.ScrollToCaret();
                }
            });
        }

        /// <summary>
        /// Resets the control to its initial state, clearing the log and progress bar.
        /// If a task is running via <see cref="TaskWrapperInstance"/>, it attempts to cancel it.
        /// </summary>
        public void ResetReportControl()
        {
            ControlInvokeHelper.ControlInvoke(this, () =>
            {
                textProgressBar1.ProgressColor = ProgressBarDefaultColor;
                textProgressBar1.Value = TaskWrapperInstance.TextProgressBarMinimum; // Use TaskWrapper's min
                textProgressBar1.CustomText = "Ready"; // Or an empty string
                textProgressBar1.Maximum = TaskWrapperInstance.TextProgressBarMaximum; // Align max

                RichTextBoxLog.Clear();
                RichTextBoxLog.SelectionColor = InfoTextColor; // Default text color

                if (TaskWrapperInstance.CurrentState == TaskExecutionState.Running ||
                    TaskWrapperInstance.CurrentState == TaskExecutionState.Starting ||
                    TaskWrapperInstance.CurrentState == TaskExecutionState.Cancelling)
                {
                    TaskWrapperInstance.RequestCancel();
                }
                // TaskWrapper's state will naturally transition or be reset on next StartTaskAsync.
            });
        }

        private void button取消_Click(object sender, EventArgs e)
        {
            TaskWrapperInstance.RequestCancel();
            // The UI state of the button (enabled/disabled) is managed by TaskWrapper_StateChanged.
        }

        private void checkBox报告是否总是滚动到最后_CheckedChanged(object sender, EventArgs e)
        {
            IsScrollToEndAfterReport = checkBox报告是否总是滚动到最后.Checked;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (TaskWrapperInstance != null)
                {
                    TaskWrapperInstance.ProgressReporter.ProgressChanged -= HandleProgressInfoReceived;
                    TaskWrapperInstance.StateChanged -= TaskWrapper_StateChanged;
                    TaskWrapperInstance.Dispose(); // Dispose the owned TaskWrapper instance
                }
                if (_directProgressReporter != null)
                {
                    // Progress<T> doesn't typically need explicit unsubscription if it's going out of scope with this control.
                    // However, if it were a static or long-lived event source, unsubscription would be vital.
                    // For local Progress<T> instances, this is less critical but doesn't hurt.
                    _directProgressReporter.ProgressChanged -= HandleProgressInfoReceived;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}