// For TaskProgressInfo, ReportLevel
// For ControlInvokeHelper

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GoldArch.TaskWrapperReport.Common;
using GoldArch.TaskWrapperReport.TaskWrapperCore;

// For TextProgressBar

namespace GoldArch.TaskWrapperReport.TaskReportControl
{
    /// <summary>
    /// A user control that displays task status using a <see cref="Label"/> and a <see cref="TextProgressBar"/>.
    /// It visually represents the state and progress of a task.
    /// </summary>
    public partial class StatusProgressDisplay : UserControl
    {
        #region Appearance Properties
        // ProgressBar Colors
        /// <summary>Gets or sets the progress bar color for the Idle state.</summary>
        [Category("Appearance - ProgressBar Colors")]
        [Description("Progress bar color for the Idle state.")]
        public Color ProgressBarIdleColor { get; set; } = Color.LightGray;

        /// <summary>Gets or sets the progress bar color for the Starting state.</summary>
        [Category("Appearance - ProgressBar Colors")]
        [Description("Progress bar color for the Starting state.")]
        public Color ProgressBarStartingColor { get; set; } = Color.LightSteelBlue;

        /// <summary>Gets or sets the progress bar color for the normal Running state.</summary>
        [Category("Appearance - ProgressBar Colors")]
        [Description("Progress bar color for the normal Running state.")]
        public Color ProgressBarRunningColor { get; set; } = Color.SkyBlue;

        /// <summary>Gets or sets the progress bar color when running and a warning is reported.</summary>
        [Category("Appearance - ProgressBar Colors")]
        [Description("Progress bar color when the task is running and a warning-level progress is reported.")]
        public Color ProgressBarRunningWarningColor { get; set; } = Color.Gold;

        /// <summary>Gets or sets the progress bar color when running and an error is reported.</summary>
        [Category("Appearance - ProgressBar Colors")]
        [Description("Progress bar color when the task is running and an error-level progress is reported.")]
        public Color ProgressBarRunningErrorColor { get; set; } = Color.OrangeRed;

        /// <summary>Gets or sets the progress bar color for the Completed state.</summary>
        [Category("Appearance - ProgressBar Colors")]
        [Description("Progress bar color for the Completed state.")]
        public Color ProgressBarCompletedColor { get; set; } = Color.MediumSeaGreen;

        /// <summary>Gets or sets the progress bar color for the Faulted state.</summary>
        [Category("Appearance - ProgressBar Colors")]
        [Description("Progress bar color for the Faulted state.")]
        public Color ProgressBarFaultedColor { get; set; } = Color.Salmon;

        /// <summary>Gets or sets the progress bar color for the Cancelling state.</summary>
        [Category("Appearance - ProgressBar Colors")]
        [Description("Progress bar color for the Cancelling state.")]
        public Color ProgressBarCancellingColor { get; set; } = Color.SandyBrown;

        /// <summary>Gets or sets the progress bar color for the Cancelled state.</summary>
        [Category("Appearance - ProgressBar Colors")]
        [Description("Progress bar color for the Cancelled state.")]
        public Color ProgressBarCancelledColor { get; set; } = Color.Silver;

        // Status Label Texts
        /// <summary>Gets or sets the status text displayed when the task is Idle.</summary>
        [Category("Appearance - Status Texts")]
        [Description("Status text displayed when the task is Idle.")]
        public string StatusTextIdle { get; set; } = "状态: 空闲";

        /// <summary>Gets or sets the status text displayed when the task is Starting.</summary>
        [Category("Appearance - Status Texts")]
        [Description("Status text displayed when the task is Starting.")]
        public string StatusTextStarting { get; set; } = "状态: 正在启动...";

        /// <summary>Gets or sets the status text displayed when the task is Running.</summary>
        [Category("Appearance - Status Texts")]
        [Description("Status text displayed when the task is Running.")]
        public string StatusTextRunning { get; set; } = "状态: 运行中...";

        /// <summary>Gets or sets the status text displayed when the task is Completed.</summary>
        [Category("Appearance - Status Texts")]
        [Description("Status text displayed when the task is Completed.")]
        public string StatusTextCompleted { get; set; } = "状态: 已完成";

        /// <summary>Gets or sets the status text displayed when the task is Faulted.</summary>
        [Category("Appearance - Status Texts")]
        [Description("Status text displayed when the task is Faulted.")]
        public string StatusTextFaulted { get; set; } = "状态: 失败";

        /// <summary>Gets or sets the status text displayed when the task is Cancelling.</summary>
        [Category("Appearance - Status Texts")]
        [Description("Status text displayed when the task is Cancelling.")]
        public string StatusTextCancelling { get; set; } = "状态: 正在取消...";

        /// <summary>Gets or sets the status text displayed when the task is Cancelled.</summary>
        [Category("Appearance - Status Texts")]
        [Description("Status text displayed when the task is Cancelled.")]
        public string StatusTextCancelled { get; set; } = "状态: 已取消";
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusProgressDisplay"/> class.
        /// </summary>
        public StatusProgressDisplay()
        {
            InitializeComponent();
            SetVisualState(TaskExecutionState.Idle); // Set initial appearance
            textProgressBar.CustomText = "准备就绪";    // Initial text for progress bar
        }

        /// <summary>
        /// Updates the visual appearance of the control based on the provided task execution state.
        /// </summary>
        /// <param name="state">The current <see cref="TaskExecutionState"/> of the task.</param>
        /// <param name="ex">The exception, if the state is <see cref="TaskExecutionState.Faulted"/>; otherwise, null.</param>
        public void SetVisualState(TaskExecutionState state, Exception ex = null)
        {
            ControlInvokeHelper.ControlInvoke(this, () =>
            {
                string statusText = GetStatusTextForState(state);
                Color progressBarColor = GetProgressBarColorForState(state);

                if (state == TaskExecutionState.Faulted && ex != null)
                {
                    statusText += $" - {ex.Message}"; // Append exception message if faulted
                }
                lblStatus.Text = statusText;
                textProgressBar.ProgressColor = progressBarColor;

                // Update progress bar text and value based on the overall state
                switch (state)
                {
                    case TaskExecutionState.Idle:
                        textProgressBar.CustomText = "准备就绪";
                        textProgressBar.Value = textProgressBar.Minimum;
                        break;
                    case TaskExecutionState.Starting:
                        textProgressBar.CustomText = "正在启动...";
                        textProgressBar.Value = textProgressBar.Minimum;
                        break;
                    case TaskExecutionState.Running:
                        // If no specific progress text yet, show generic "Running..."
                        if (string.IsNullOrEmpty(textProgressBar.CustomText) || textProgressBar.CustomText == "准备就绪" || textProgressBar.CustomText == "正在启动...")
                            textProgressBar.CustomText = "运行中...";
                        break;
                    case TaskExecutionState.Cancelling:
                        textProgressBar.CustomText = "正在取消...";
                        break;
                    case TaskExecutionState.Completed:
                        textProgressBar.CustomText = "已完成!";
                        textProgressBar.Value = textProgressBar.Maximum;
                        break;
                    case TaskExecutionState.Faulted:
                        textProgressBar.CustomText = "任务失败!";
                        // Value could be kept or reset
                        break;
                    case TaskExecutionState.Cancelled:
                        textProgressBar.CustomText = "已取消";
                        // Value could be kept or reset
                        break;
                }
                textProgressBar.Invalidate(); // Ensure changes are painted
            });
        }

        /// <summary>
        /// Reports detailed progress information to the control, updating the <see cref="TextProgressBar"/>.
        /// This method is typically called when the associated task is in the <see cref="TaskExecutionState.Running"/> state.
        /// </summary>
        /// <param name="progressInfo">The <see cref="TaskProgressInfo"/> containing details of the progress.</param>
        /// <param name="currentState">The current <see cref="TaskExecutionState"/> of the task (used to determine color logic).</param>
        public void ReportProgress(TaskProgressInfo progressInfo, TaskExecutionState currentState)
        {
            if (progressInfo == null) throw new ArgumentNullException(nameof(progressInfo));

            ControlInvokeHelper.ControlInvoke(this, () =>
            {
                // Update progress bar value
                if (progressInfo.ProgressValue.HasValue)
                {
                    int val = progressInfo.ProgressValue.Value;
                    if (textProgressBar.Minimum <= val && val <= textProgressBar.Maximum)
                        textProgressBar.Value = val;
                    else if (val < textProgressBar.Minimum)
                        textProgressBar.Value = textProgressBar.Minimum;
                    else
                        textProgressBar.Value = textProgressBar.Maximum;
                }

                // Update progress bar text
                if (!string.IsNullOrEmpty(progressInfo.ProgressText))
                {
                    textProgressBar.CustomText = progressInfo.ProgressText;
                }

                // Adjust progress bar color based on ReportLevel if the task is currently running
                if (currentState == TaskExecutionState.Running)
                {
                    switch (progressInfo.Level)
                    {
                        case ReportLevel.Warning:
                            textProgressBar.ProgressColor = ProgressBarRunningWarningColor;
                            break;
                        case ReportLevel.Error: // An error reported during a running task
                            textProgressBar.ProgressColor = ProgressBarRunningErrorColor;
                            break;
                        case ReportLevel.Information:
                        case ReportLevel.Detail:
                        case ReportLevel.StatusUpdate:
                        case ReportLevel.Success: // Success of a step, not necessarily the whole task
                        default:
                            // Revert to standard running color if not a special level for running state
                            textProgressBar.ProgressColor = ProgressBarRunningColor;
                            break;
                    }
                }
                textProgressBar.Invalidate(); // Ensure changes are painted
            });
        }

        /// <summary>
        /// Resets the display to its initial (Idle) state.
        /// </summary>
        /// <param name="minProgressValue">The minimum value for the progress bar.</param>
        /// <param name="maxProgressValue">The maximum value for the progress bar.</param>
        public void ResetDisplay(int minProgressValue = 0, int maxProgressValue = 100)
        {
            ControlInvokeHelper.ControlInvoke(this, () =>
            {
                textProgressBar.Minimum = minProgressValue;
                textProgressBar.Maximum = maxProgressValue;
                SetVisualState(TaskExecutionState.Idle); // This will set text and color
            });
        }

        private string GetStatusTextForState(TaskExecutionState state)
        {
            switch (state)
            {
                case TaskExecutionState.Idle: return StatusTextIdle;
                case TaskExecutionState.Starting: return StatusTextStarting;
                case TaskExecutionState.Running: return StatusTextRunning;
                case TaskExecutionState.Completed: return StatusTextCompleted;
                case TaskExecutionState.Faulted: return StatusTextFaulted;
                case TaskExecutionState.Cancelling: return StatusTextCancelling;
                case TaskExecutionState.Cancelled: return StatusTextCancelled;
                default: return "状态: 未知"; // Fallback
            }
        }

        private Color GetProgressBarColorForState(TaskExecutionState state)
        {
            switch (state)
            {
                case TaskExecutionState.Idle: return ProgressBarIdleColor;
                case TaskExecutionState.Starting: return ProgressBarStartingColor;
                case TaskExecutionState.Running: return ProgressBarRunningColor; // Base running color
                case TaskExecutionState.Completed: return ProgressBarCompletedColor;
                case TaskExecutionState.Faulted: return ProgressBarFaultedColor;
                case TaskExecutionState.Cancelling: return ProgressBarCancellingColor;
                case TaskExecutionState.Cancelled: return ProgressBarCancelledColor;
                default: return ProgressBarIdleColor; // Fallback
            }
        }

        /// <summary>
        /// Gets or sets the minimum value of the range of the progress bar.
        /// </summary>
        [Category("Behavior")]
        [Description("The minimum value of the range of the progress bar.")]
        [DefaultValue(0)]
        public int ProgressBarMinimum
        {
            get => textProgressBar.Minimum;
            set { ControlInvokeHelper.ControlInvoke(this, () => textProgressBar.Minimum = value); }
        }

        /// <summary>
        /// Gets or sets the maximum value of the range of the progress bar.
        /// </summary>
        [Category("Behavior")]
        [Description("The maximum value of the range of the progress bar.")]
        [DefaultValue(100)]
        public int ProgressBarMaximum
        {
            get => textProgressBar.Maximum;
            set { ControlInvokeHelper.ControlInvoke(this, () => textProgressBar.Maximum = value); }
        }
    }
}