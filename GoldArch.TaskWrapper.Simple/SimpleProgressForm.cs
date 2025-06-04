using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoldArch.TaskWrapperReport.Common;
using GoldArch.TaskWrapperReport.TaskReportControl;
using GoldArch.TaskWrapperReport.TaskWrapperCore;
using Label = System.Windows.Forms.Label;

namespace GoldArch.TaskWrapperReport.Simple
{
    public class SimpleProgressForm : Form
{
    private TextProgressBar textProgressBarTask;
    private Button btnStartSimpleTask;
    private Button btnStartErrorTask;
    private Button btnCancelSimpleTask;
    private Label lblStatus;

    private TaskWrapperReport.TaskWrapperCore.TaskWrapper _taskWrapper;

    // UI Color Scheme
    private Color ProgressBarDefaultColor { get; } = Color.SkyBlue; 
    private Color ProgressBarWarningColor { get; } = Color.Gold;
    private Color ProgressBarErrorColor { get; } = Color.Salmon;
    private Color ProgressBarSuccessColor { get; } = Color.MediumSeaGreen;
    private Color ProgressBarStartingColor { get; } = Color.LightSteelBlue;
    private Color ProgressBarCancellingColor { get; } = Color.OrangeRed;
    private Color ProgressBarCancelledColor { get; } = Color.Gray;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleProgressForm"/> class.
    /// </summary>
    public SimpleProgressForm()
    {
        InitializeComponent();
        InitializeTaskWrapper();
    }

    private void InitializeComponent()
    {
        this.textProgressBarTask = new TextProgressBar();
        this.btnStartSimpleTask = new System.Windows.Forms.Button();
        this.btnStartErrorTask = new System.Windows.Forms.Button();
        this.btnCancelSimpleTask = new System.Windows.Forms.Button();
        this.lblStatus = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // textProgressBarTask
        // 
        this.textProgressBarTask.CustomText = "准备就绪";
        this.textProgressBarTask.Location = new System.Drawing.Point(12, 50);
        this.textProgressBarTask.Name = "textProgressBarTask";
        //this.textProgressBarTask.ProgressColor = ProgressBarDefaultColor; // Use defined color
        this.textProgressBarTask.Size = new System.Drawing.Size(440, 23);
        this.textProgressBarTask.TabIndex = 0;
        this.textProgressBarTask.TextColor = System.Drawing.Color.Black;
        this.textProgressBarTask.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.textProgressBarTask.VisualMode = ProgressBarDisplayMode.CustomText;
        // 
        // btnStartSimpleTask
        // 
        this.btnStartSimpleTask.Location = new System.Drawing.Point(12, 90);
        this.btnStartSimpleTask.Name = "btnStartSimpleTask";
        this.btnStartSimpleTask.Size = new System.Drawing.Size(120, 23); // Adjusted size
        this.btnStartSimpleTask.TabIndex = 1;
        this.btnStartSimpleTask.Text = "启动正常任务";
        this.btnStartSimpleTask.UseVisualStyleBackColor = true;
        this.btnStartSimpleTask.Click += new System.EventHandler(this.BtnStartSimpleTask_Click);
        // 
        // btnStartErrorTask
        // 
        this.btnStartErrorTask.Location = new System.Drawing.Point(138, 90);
        this.btnStartErrorTask.Name = "btnStartErrorTask";
        this.btnStartErrorTask.Size = new System.Drawing.Size(120, 23); // Adjusted size
        this.btnStartErrorTask.TabIndex = 2;
        this.btnStartErrorTask.Text = "启动错误任务";
        this.btnStartErrorTask.UseVisualStyleBackColor = true;
        this.btnStartErrorTask.Click += new System.EventHandler(this.BtnStartErrorTask_Click);
        // 
        // btnCancelSimpleTask
        // 
        this.btnCancelSimpleTask.Enabled = false;
        this.btnCancelSimpleTask.Location = new System.Drawing.Point(264, 90);
        this.btnCancelSimpleTask.Name = "btnCancelSimpleTask";
        this.btnCancelSimpleTask.Size = new System.Drawing.Size(75, 23);
        this.btnCancelSimpleTask.TabIndex = 3;
        this.btnCancelSimpleTask.Text = "取消";
        this.btnCancelSimpleTask.UseVisualStyleBackColor = true;
        this.btnCancelSimpleTask.Click += new System.EventHandler(this.BtnCancelSimpleTask_Click);
        //
        // lblStatus
        //
        this.lblStatus.AutoSize = true;
        this.lblStatus.Location = new System.Drawing.Point(12, 20);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new System.Drawing.Size(64, 13); // Example size
        this.lblStatus.TabIndex = 4;
        this.lblStatus.Text = "状态: 空闲";
        // 
        // SimpleProgressForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(464, 141);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.btnCancelSimpleTask);
        this.Controls.Add(this.btnStartErrorTask);
        this.Controls.Add(this.btnStartSimpleTask);
        this.Controls.Add(this.textProgressBarTask);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.Name = "SimpleProgressForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "简单进度条示例 (含错误测试)";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimpleProgressForm_FormClosing);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void InitializeTaskWrapper()
    {
        _taskWrapper = new TaskWrapperReport.TaskWrapperCore.TaskWrapper
        {
            TextProgressBarMinimum = 0,
            TextProgressBarMaximum = 100
        };

        _taskWrapper.ProgressReporter.ProgressChanged += TaskWrapper_ProgressChanged;
        _taskWrapper.StateChanged += TaskWrapper_StateChanged;
        _taskWrapper.AfterCompletionAction = () => HandleTaskCompletion();
    }

    private void HandleTaskCompletion()
    {
        // This method is called by AfterCompletionAction.
        // UI updates related to final states (Completed, Faulted, Cancelled) are primarily handled
        // in TaskWrapper_StateChanged. This is for any additional logic.
        // For example, re-enabling start buttons is handled in TaskWrapper_StateChanged.
    }


    private void TaskWrapper_ProgressChanged(object sender, TaskProgressInfo e)
    {
        ControlInvokeHelper.ControlInvoke(this, () =>
        {
            if (e.ProgressValue.HasValue)
            {
                int val = e.ProgressValue.Value;
                textProgressBarTask.Value = Math.Max(textProgressBarTask.Minimum, Math.Min(val, textProgressBarTask.Maximum));
            }
            if (e.ProgressText != null)
            {
                textProgressBarTask.CustomText = e.ProgressText;
            }

            if (_taskWrapper.CurrentState == TaskExecutionState.Running)
            {
                switch (e.Level)
                {
                    case ReportLevel.Warning: textProgressBarTask.ProgressColor = ProgressBarWarningColor; break;
                    case ReportLevel.Error: textProgressBarTask.ProgressColor = ProgressBarErrorColor; break; // Error during running
                    case ReportLevel.Success: textProgressBarTask.ProgressColor = ProgressBarSuccessColor; break; // Step success
                    default:
                        // Only revert to default running if not already a specific warning/error color
                        if (textProgressBarTask.ProgressColor != ProgressBarWarningColor && textProgressBarTask.ProgressColor != ProgressBarErrorColor)
                        {
                            textProgressBarTask.ProgressColor = ProgressBarDefaultColor;
                        }
                        break;
                }
            }
        });
    }

    private void TaskWrapper_StateChanged(object sender, TaskStateChangedEventArgs e)
    {
        ControlInvokeHelper.ControlInvoke(this, () =>
        {
            lblStatus.Text = $"状态: {e.NewState}";
            bool canCancel = (e.NewState == TaskExecutionState.Running || e.NewState == TaskExecutionState.Starting || e.NewState == TaskExecutionState.Cancelling);
            btnCancelSimpleTask.Enabled = canCancel;

            bool canStart = (e.NewState == TaskExecutionState.Idle || e.NewState == TaskExecutionState.Completed || e.NewState == TaskExecutionState.Faulted || e.NewState == TaskExecutionState.Cancelled);
            btnStartSimpleTask.Enabled = canStart;
            btnStartErrorTask.Enabled = canStart;

            switch (e.NewState)
            {
                case TaskExecutionState.Idle:
                    textProgressBarTask.ProgressColor = ProgressBarDefaultColor;
                    textProgressBarTask.CustomText = "准备就绪";
                    textProgressBarTask.Value = _taskWrapper.TextProgressBarMinimum;
                    break;
                case TaskExecutionState.Starting:
                    textProgressBarTask.ProgressColor = ProgressBarStartingColor;
                    textProgressBarTask.CustomText = "正在启动...";
                    break;
                case TaskExecutionState.Running:
                    textProgressBarTask.ProgressColor = ProgressBarDefaultColor; // Initial running color
                    // CustomText will be updated by ProgressChanged
                    break;
                case TaskExecutionState.Cancelling:
                    textProgressBarTask.ProgressColor = ProgressBarCancellingColor;
                    textProgressBarTask.CustomText = "正在取消...";
                    break;
                case TaskExecutionState.Completed:
                    textProgressBarTask.ProgressColor = ProgressBarSuccessColor;
                    textProgressBarTask.CustomText = "任务完成!";
                    textProgressBarTask.Value = _taskWrapper.TextProgressBarMaximum;
                    break;
                case TaskExecutionState.Faulted:
                    textProgressBarTask.ProgressColor = ProgressBarErrorColor;
                    textProgressBarTask.CustomText = $"任务失败: {e.Exception?.Message.Split('\n')[0]}"; // Show first line of exception
                    // Log full exception if needed
                    if (e.Exception != null) System.Diagnostics.Debug.WriteLine($"Task Faulted in SimpleProgressForm: {e.Exception}");
                    break;
                case TaskExecutionState.Cancelled:
                    textProgressBarTask.ProgressColor = ProgressBarCancelledColor;
                    textProgressBarTask.CustomText = "任务已取消";
                    break;
                default:
                    textProgressBarTask.ProgressColor = ProgressBarDefaultColor; // Fallback
                    break;
            }
        });
    }

    private void BtnStartSimpleTask_Click(object sender, EventArgs e)
    {
        if (!IsTaskRunnable()) return;

        _taskWrapper.DoWorkFuncAsync = async (token, progress) =>
        {
            for (int i = 0; i <= 100; i += 10)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(300, token).ConfigureAwait(false);
                ReportLevel level = (i > 70) ? ReportLevel.Warning : ReportLevel.Information;
                progress.Report(new TaskProgressInfo(null, $"正常任务处理中... {i}%", i, level));
            }
            progress.Report(new TaskProgressInfo(null, "正常任务完成!", 100, ReportLevel.Success));
            return null; // Success
        };
        StartTaskWrapperInternal();
    }

    private void BtnStartErrorTask_Click(object sender, EventArgs e)
    {
        if (!IsTaskRunnable()) return;

        _taskWrapper.DoWorkFuncAsync = async (token, progress) =>
        {
            progress.Report(new TaskProgressInfo(null, "错误任务启动...", 0, ReportLevel.Information));
            await Task.Delay(500, token).ConfigureAwait(false);
            for (int i = 0; i <= 50; i += 10)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(200, token).ConfigureAwait(false);
                progress.Report(new TaskProgressInfo(null, $"错误任务进展... {i}%", i, ReportLevel.Information));
            }
            progress.Report(new TaskProgressInfo(null, "准备抛出异常...", 50, ReportLevel.Warning));
            await Task.Delay(500, token).ConfigureAwait(false);
            throw new InvalidOperationException("这是一个来自 SimpleProgressForm 的故意测试异常!");
        };
        StartTaskWrapperInternal();
    }

    private bool IsTaskRunnable()
    {
        if (_taskWrapper.CurrentState == TaskExecutionState.Idle ||
            _taskWrapper.CurrentState == TaskExecutionState.Completed ||
            _taskWrapper.CurrentState == TaskExecutionState.Faulted ||
            _taskWrapper.CurrentState == TaskExecutionState.Cancelled)
        {
            return true;
        }
        MessageBox.Show("任务正在运行或处理中。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return false;
    }

    private void StartTaskWrapperInternal()
    {
        // UI updates for starting are handled by TaskWrapper_StateChanged when state becomes Starting
        _taskWrapper.StartTaskAsync();
    }

    private void BtnCancelSimpleTask_Click(object sender, EventArgs e)
    {
        _taskWrapper.RequestCancel();
    }

    private void SimpleProgressForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_taskWrapper != null)
        {
            if (_taskWrapper.CurrentState == TaskExecutionState.Running ||
                _taskWrapper.CurrentState == TaskExecutionState.Starting ||
                _taskWrapper.CurrentState == TaskExecutionState.Cancelling)
            {
                _taskWrapper.RequestCancel();
                // Consider if a brief wait or more robust shutdown is needed for critical tasks
            }
            // Unsubscribe events and dispose
            _taskWrapper.ProgressReporter.ProgressChanged -= TaskWrapper_ProgressChanged;
            _taskWrapper.StateChanged -= TaskWrapper_StateChanged;
            _taskWrapper.Dispose();
            _taskWrapper = null; // Help GC
        }
    }
}
}

/// <summary>
    /// A simple form demonstrating direct usage of <see cref="TaskWrapper"/>
    /// with a <see cref="TextProgressBar"/> and a <see cref="Label"/> for status display.
    /// </summary>
