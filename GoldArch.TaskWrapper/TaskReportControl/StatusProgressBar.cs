using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GoldArch.TaskWrapperReport.Common;
using GoldArch.TaskWrapperReport.TaskWrapperCore;
// For TaskExecutionState
// For TaskProgressInfo, ReportLevel
// For ControlInvokeHelper

// For TextProgressBar

namespace GoldArch.TaskWrapperReport.TaskReportControl
{
    /// <summary>
    /// A UserControl that uses a <see cref="TextProgressBar"/> to display both task status and progress information.
    /// All information is consolidated into the <see cref="TextProgressBar.CustomText"/> property.
    /// </summary>
    public partial class StatusProgressBar : UserControl
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

        // ProgressBar Text Formats
        /// <summary>Gets or sets the text format for the Idle state. Example: '状态: 空闲'</summary>
        [Category("Appearance - ProgressBar Texts")]
        [Description("Text format for the Idle state displayed in the progress bar. Example: '状态: 空闲'")]
        public string TextFormatIdle { get; set; } = "状态: 空闲";

        /// <summary>Gets or sets the text format for the Starting state. Example: '正在启动...'</summary>
        [Category("Appearance - ProgressBar Texts")]
        [Description("Text format for the Starting state displayed in the progress bar. Example: '正在启动...'")]
        public string TextFormatStarting { get; set; } = "正在启动...";

        /// <summary>Gets or sets the text format for the Running state. Example: '运行中... {0}% - {1}' where {0} is percentage, {1} is specific progress text.</summary>
        [Category("Appearance - ProgressBar Texts")]
        [Description("Text format for the Running state. {0} = percentage, {1} = progress message.")]
        public string TextFormatRunning { get; set; } = "运行中... {0}% - {1}";

        /// <summary>Gets or sets the text format for the Completed state. Example: '已完成!'</summary>
        [Category("Appearance - ProgressBar Texts")]
        [Description("Text format for the Completed state displayed in the progress bar. Example: '已完成!'")]
        public string TextFormatCompleted { get; set; } = "已完成!";

        /// <summary>Gets or sets the text format for the Faulted state. Example: '失败: {0}' where {0} is the error message.</summary>
        [Category("Appearance - ProgressBar Texts")]
        [Description("Text format for the Faulted state. {0} = error message.")]
        public string TextFormatFaulted { get; set; } = "失败: {0}";

        /// <summary>Gets or sets the text format for the Cancelling state. Example: '正在取消...'</summary>
        [Category("Appearance - ProgressBar Texts")]
        [Description("Text format for the Cancelling state displayed in the progress bar. Example: '正在取消...'")]
        public string TextFormatCancelling { get; set; } = "正在取消...";

        /// <summary>Gets or sets the text format for the Cancelled state. Example: '已取消'</summary>
        [Category("Appearance - ProgressBar Texts")]
        [Description("Text format for the Cancelled state displayed in the progress bar. Example: '已取消'")]
        public string TextFormatCancelled { get; set; } = "已取消";
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusProgressBar"/> class.
        /// </summary>
        public StatusProgressBar()
        {
            InitializeComponent();
            SetVisualState(TaskExecutionState.Idle); // Set initial appearance
        }

        /// <summary>
        /// Updates the visual appearance of the control based on the provided task execution state.
        /// All information is displayed within the <see cref="TextProgressBar"/>.
        /// </summary>
        /// <param name="state">The current <see cref="TaskExecutionState"/> of the task.</param>
        /// <param name="ex">The exception, if the state is <see cref="TaskExecutionState.Faulted"/>; otherwise, null.</param>
        public void SetVisualState(TaskExecutionState state, Exception ex = null)
        {
            ControlInvokeHelper.ControlInvoke(this, () =>
            {
                string displayText;
                Color progressBarColor;

                switch (state)
                {
                    case TaskExecutionState.Idle:
                        displayText = TextFormatIdle;
                        progressBarColor = ProgressBarIdleColor;
                        textProgressBar.Value = textProgressBar.Minimum;
                        break;
                    case TaskExecutionState.Starting:
                        displayText = TextFormatStarting;
                        progressBarColor = ProgressBarStartingColor;
                        textProgressBar.Value = textProgressBar.Minimum;
                        break;
                    case TaskExecutionState.Running:
                        // Default running text, ReportProgress will update with specifics
                        displayText = string.Format(System.Globalization.CultureInfo.CurrentCulture, TextFormatRunning, textProgressBar.Value, "...");
                        progressBarColor = ProgressBarRunningColor;
                        break;
                    case TaskExecutionState.Completed:
                        displayText = TextFormatCompleted;
                        progressBarColor = ProgressBarCompletedColor;
                        textProgressBar.Value = textProgressBar.Maximum;
                        break;
                    case TaskExecutionState.Faulted:
                        displayText = string.Format(System.Globalization.CultureInfo.CurrentCulture, TextFormatFaulted, ex?.Message ?? "未知错误");
                        progressBarColor = ProgressBarFaultedColor;
                        break;
                    case TaskExecutionState.Cancelling:
                        displayText = TextFormatCancelling;
                        progressBarColor = ProgressBarCancellingColor;
                        break;
                    case TaskExecutionState.Cancelled:
                        displayText = TextFormatCancelled;
                        progressBarColor = ProgressBarCancelledColor;
                        break;
                    default:
                        displayText = "未知状态";
                        progressBarColor = ProgressBarIdleColor;
                        break;
                }
                textProgressBar.CustomText = displayText;
                textProgressBar.ProgressColor = progressBarColor;
                textProgressBar.Invalidate(); // Ensure repaint
            });
        }

        /// <summary>
        /// Reports detailed progress information, updating the <see cref="TextProgressBar"/>.
        /// This method is typically called when the associated task is in the <see cref="TaskExecutionState.Running"/> state.
        /// </summary>
        /// <param name="progressInfo">The <see cref="TaskProgressInfo"/> containing details of the progress.</param>
        /// <param name="currentState">The current <see cref="TaskExecutionState"/> of the task.</param>
        public void ReportProgress(TaskProgressInfo progressInfo, TaskExecutionState currentState)
        {
            if (progressInfo == null) throw new ArgumentNullException(nameof(progressInfo));

            ControlInvokeHelper.ControlInvoke(this, () =>
            {
                int currentProgressValue = textProgressBar.Value; // Keep track of value for text formatting
                if (progressInfo.ProgressValue.HasValue)
                {
                    int val = progressInfo.ProgressValue.Value;
                    if (textProgressBar.Minimum <= val && val <= textProgressBar.Maximum)
                        currentProgressValue = val;
                    else if (val < textProgressBar.Minimum)
                        currentProgressValue = textProgressBar.Minimum;
                    else
                        currentProgressValue = textProgressBar.Maximum;
                    textProgressBar.Value = currentProgressValue;
                }

                // Format the running text using the current progress value and the specific progress message
                string progressSpecificMessage = progressInfo.ProgressText ?? "...";
                textProgressBar.CustomText = string.Format(System.Globalization.CultureInfo.CurrentCulture, TextFormatRunning, currentProgressValue, progressSpecificMessage);

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
                        case ReportLevel.Success: // Success of a step
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
                SetVisualState(TaskExecutionState.Idle); // This will set text, color, and value
            });
        }

        /// <summary>Gets or sets the minimum value of the range of the progress bar.</summary>
        [Category("Behavior")]
        [Description("The minimum value of the range of the progress bar.")]
        [DefaultValue(0)]
        public int ProgressBarMinimum
        {
            get => textProgressBar.Minimum;
            set { ControlInvokeHelper.ControlInvoke(this, () => textProgressBar.Minimum = value); }
        }

        /// <summary>Gets or sets the maximum value of the range of the progress bar.</summary>
        [Category("Behavior")]
        [Description("The maximum value of the range of the progress bar.")]
        [DefaultValue(100)]
        public int ProgressBarMaximum
        {
            get => textProgressBar.Maximum;
            set { ControlInvokeHelper.ControlInvoke(this, () => textProgressBar.Maximum = value); }
        }

        /// <summary>
        /// Gets or sets the font used to display text within the progress bar.
        /// This overrides the base <see cref="UserControl.Font"/> property to target the internal <see cref="TextProgressBar"/>.
        /// </summary>
        [Category("Appearance")]
        [Description("The font used to display text in the progress bar.")]
        public override Font Font
        {
            get => textProgressBar.TextFont; // Get from the internal TextProgressBar
            set
            {
                base.Font = value; // Set UserControl's Font (good practice)
                if (textProgressBar != null) // textProgressBar might be null during initial base constructor calls
                {
                    textProgressBar.TextFont = value; // Set the TextProgressBar's specific font property
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the text displayed within the progress bar.
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the text displayed in the progress bar.")]
        public Color TextColor
        {
            get => textProgressBar.TextColor;
            set
            {
                if (textProgressBar != null)
                {
                    textProgressBar.TextColor = value;
                }
            }
        }
    }
}