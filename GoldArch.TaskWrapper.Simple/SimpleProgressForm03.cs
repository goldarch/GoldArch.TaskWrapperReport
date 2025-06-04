using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoldArch.TaskWrapperReport.Common;
using GoldArch.TaskWrapperReport.TaskReportControl;
using GoldArch.TaskWrapperReport.TaskWrapperCore;

// For StatusProgressBar

namespace GoldArch.TaskWrapperReport.Simple
{
    /// <summary>
    /// An example form demonstrating the use of the <see cref="StatusProgressBar"/> control,
    /// which consolidates all status and progress information into a single progress bar.
    /// </summary>
    public class SimpleProgressForm03 : Form
{
    private Button btnStartSimpleTask;
    private Button btnStartErrorTask;
    private Button btnCancelSimpleTask;
    private StatusProgressBar statusProgressBarControl;

    private TaskWrapperReport.TaskWrapperCore.TaskWrapper _taskWrapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleProgressForm03"/> class.
    /// </summary>
    public SimpleProgressForm03()
    {
        InitializeComponent();
        InitializeTaskWrapperAndDisplay();
    }

    private void InitializeComponent()
    {
            this.btnStartSimpleTask = new System.Windows.Forms.Button();
            this.btnStartErrorTask = new System.Windows.Forms.Button();
            this.btnCancelSimpleTask = new System.Windows.Forms.Button();
            this.statusProgressBarControl = new GoldArch.TaskWrapperReport.TaskReportControl.StatusProgressBar();
            this.SuspendLayout();
            // 
            // btnStartSimpleTask
            // 
            this.btnStartSimpleTask.Location = new System.Drawing.Point(13, 55);
            this.btnStartSimpleTask.Name = "btnStartSimpleTask";
            this.btnStartSimpleTask.Size = new System.Drawing.Size(120, 21);
            this.btnStartSimpleTask.TabIndex = 1;
            this.btnStartSimpleTask.Text = "启动正常任务";
            this.btnStartSimpleTask.UseVisualStyleBackColor = true;
            this.btnStartSimpleTask.Click += new System.EventHandler(this.BtnStartSimpleTask_Click);
            // 
            // btnStartErrorTask
            // 
            this.btnStartErrorTask.Location = new System.Drawing.Point(139, 55);
            this.btnStartErrorTask.Name = "btnStartErrorTask";
            this.btnStartErrorTask.Size = new System.Drawing.Size(120, 21);
            this.btnStartErrorTask.TabIndex = 2;
            this.btnStartErrorTask.Text = "启动错误任务";
            this.btnStartErrorTask.UseVisualStyleBackColor = true;
            this.btnStartErrorTask.Click += new System.EventHandler(this.BtnStartErrorTask_Click);
            // 
            // btnCancelSimpleTask
            // 
            this.btnCancelSimpleTask.Enabled = false;
            this.btnCancelSimpleTask.Location = new System.Drawing.Point(265, 55);
            this.btnCancelSimpleTask.Name = "btnCancelSimpleTask";
            this.btnCancelSimpleTask.Size = new System.Drawing.Size(75, 21);
            this.btnCancelSimpleTask.TabIndex = 3;
            this.btnCancelSimpleTask.Text = "取消";
            this.btnCancelSimpleTask.UseVisualStyleBackColor = true;
            this.btnCancelSimpleTask.Click += new System.EventHandler(this.BtnCancelSimpleTask_Click);
            // 
            // statusProgressBarControl
            // 
            this.statusProgressBarControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusProgressBarControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.statusProgressBarControl.Location = new System.Drawing.Point(14, 15);
            this.statusProgressBarControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.statusProgressBarControl.Name = "statusProgressBarControl";
            this.statusProgressBarControl.ProgressBarCancelledColor = System.Drawing.Color.Silver;
            this.statusProgressBarControl.ProgressBarCancellingColor = System.Drawing.Color.SandyBrown;
            this.statusProgressBarControl.ProgressBarCompletedColor = System.Drawing.Color.MediumSeaGreen;
            this.statusProgressBarControl.ProgressBarFaultedColor = System.Drawing.Color.Salmon;
            this.statusProgressBarControl.ProgressBarIdleColor = System.Drawing.Color.LightGray;
            this.statusProgressBarControl.ProgressBarRunningColor = System.Drawing.Color.SkyBlue;
            this.statusProgressBarControl.ProgressBarRunningErrorColor = System.Drawing.Color.OrangeRed;
            this.statusProgressBarControl.ProgressBarRunningWarningColor = System.Drawing.Color.Orange;
            this.statusProgressBarControl.ProgressBarStartingColor = System.Drawing.Color.LightSteelBlue;
            this.statusProgressBarControl.Size = new System.Drawing.Size(570, 29);
            this.statusProgressBarControl.TabIndex = 0;
            this.statusProgressBarControl.TextColor = System.Drawing.Color.DarkSlateBlue;
            this.statusProgressBarControl.TextFormatCancelled = "已取消";
            this.statusProgressBarControl.TextFormatCancelling = "正在取消...";
            this.statusProgressBarControl.TextFormatCompleted = "已完成!";
            this.statusProgressBarControl.TextFormatFaulted = "错误: {0} - 请查看日志";
            this.statusProgressBarControl.TextFormatIdle = "状态: 空闲";
            this.statusProgressBarControl.TextFormatRunning = "运行中... {0}% - {1}";
            this.statusProgressBarControl.TextFormatStarting = "正在启动...";
            // 
            // SimpleProgressForm03
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 88);
            this.Controls.Add(this.statusProgressBarControl);
            this.Controls.Add(this.btnCancelSimpleTask);
            this.Controls.Add(this.btnStartErrorTask);
            this.Controls.Add(this.btnStartSimpleTask);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SimpleProgressForm03";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "超简洁进度条 V3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimpleProgressForm03_FormClosing);
            this.ResumeLayout(false);

    }

    private void InitializeTaskWrapperAndDisplay()
    {
        _taskWrapper = new TaskWrapperReport.TaskWrapperCore.TaskWrapper
        {
            TextProgressBarMinimum = 0,
            TextProgressBarMaximum = 100
        };

        statusProgressBarControl.ProgressBarMinimum = _taskWrapper.TextProgressBarMinimum;
        statusProgressBarControl.ProgressBarMaximum = _taskWrapper.TextProgressBarMaximum;
        statusProgressBarControl.ResetDisplay(_taskWrapper.TextProgressBarMinimum, _taskWrapper.TextProgressBarMaximum);

        _taskWrapper.ProgressReporter.ProgressChanged += TaskWrapper_ProgressChanged;
        _taskWrapper.StateChanged += TaskWrapper_StateChanged;
        _taskWrapper.AfterCompletionAction = () => { /* Optional post-completion logic */ };
    }

    private void TaskWrapper_ProgressChanged(object sender, TaskProgressInfo e)
    {
        statusProgressBarControl.ReportProgress(e, _taskWrapper.CurrentState);
    }

    private void TaskWrapper_StateChanged(object sender, TaskStateChangedEventArgs e)
    {
        ControlInvokeHelper.ControlInvoke(this, () =>
        {
            statusProgressBarControl.SetVisualState(e.NewState, e.Exception);

            bool canCancel = (e.NewState == TaskExecutionState.Running || e.NewState == TaskExecutionState.Starting || e.NewState == TaskExecutionState.Cancelling);
            btnCancelSimpleTask.Enabled = canCancel;

            bool canStart = (e.NewState == TaskExecutionState.Idle || e.NewState == TaskExecutionState.Completed || e.NewState == TaskExecutionState.Faulted || e.NewState == TaskExecutionState.Cancelled);
            btnStartSimpleTask.Enabled = canStart;
            btnStartErrorTask.Enabled = canStart;
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
                progress.Report(new TaskProgressInfo(null, $"步骤 {i / 10 + 1}", i, level));
            }
            progress.Report(new TaskProgressInfo(null, "全部完成", 100, ReportLevel.Success));
            return null; // Success
        };
        StartTaskWrapperInternal();
    }

    private void BtnStartErrorTask_Click(object sender, EventArgs e)
    {
        if (!IsTaskRunnable()) return;

        _taskWrapper.DoWorkFuncAsync = async (token, progress) =>
        {
            progress.Report(new TaskProgressInfo(null, "启动中", 0, ReportLevel.Information));
            await Task.Delay(500, token).ConfigureAwait(false);
            for (int i = 0; i <= 50; i += 10)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(200, token).ConfigureAwait(false);
                progress.Report(new TaskProgressInfo(null, $"处理中 {i}%", i, ReportLevel.Information));
            }
            progress.Report(new TaskProgressInfo(null, "准备出错", 50, ReportLevel.Warning));
            await Task.Delay(500, token).ConfigureAwait(false);
            throw new InvalidOperationException("这是一个来自 SimpleProgressForm03 的故意测试异常!");
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
        statusProgressBarControl.ResetDisplay(_taskWrapper.TextProgressBarMinimum, _taskWrapper.TextProgressBarMaximum);
        // statusProgressBarControl.SetVisualState(TaskExecutionState.Starting); // TaskWrapper will set this
        _taskWrapper.StartTaskAsync();
    }

    private void BtnCancelSimpleTask_Click(object sender, EventArgs e)
    {
        _taskWrapper.RequestCancel();
    }

    private void SimpleProgressForm03_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_taskWrapper != null)
        {
            if (_taskWrapper.CurrentState == TaskExecutionState.Running ||
                _taskWrapper.CurrentState == TaskExecutionState.Starting ||
                _taskWrapper.CurrentState == TaskExecutionState.Cancelling)
            {
                _taskWrapper.RequestCancel();
            }
            _taskWrapper.ProgressReporter.ProgressChanged -= TaskWrapper_ProgressChanged;
            _taskWrapper.StateChanged -= TaskWrapper_StateChanged;
            _taskWrapper.Dispose();
            _taskWrapper = null;
        }
    }
}
}

