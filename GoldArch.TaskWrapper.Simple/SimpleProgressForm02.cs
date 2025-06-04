using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoldArch.TaskWrapperReport.Common;
using GoldArch.TaskWrapperReport.TaskReportControl;
using GoldArch.TaskWrapperReport.TaskWrapperCore;

// For StatusProgressDisplay

namespace GoldArch.TaskWrapperReport.Simple
{/// <summary>
 /// An example form demonstrating the use of the <see cref="StatusProgressDisplay"/> control
 /// to show task progress and status.
 /// </summary>
    public class SimpleProgressForm02 : Form
{
    private Button btnStartSimpleTask;
    private Button btnStartErrorTask;
    private Button btnCancelSimpleTask;
    private StatusProgressDisplay statusProgressDisplayControl;

    private TaskWrapperReport.TaskWrapperCore.TaskWrapper _taskWrapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleProgressForm02"/> class.
    /// </summary>
    public SimpleProgressForm02()
    {
        InitializeComponent();
        InitializeTaskWrapperAndDisplay();
    }

    private void InitializeComponent()
    {
        this.btnStartSimpleTask = new System.Windows.Forms.Button();
        this.btnStartErrorTask = new System.Windows.Forms.Button();
        this.btnCancelSimpleTask = new System.Windows.Forms.Button();
        this.statusProgressDisplayControl = new StatusProgressDisplay();
        this.SuspendLayout();
        // 
        // statusProgressDisplayControl
        // 
        this.statusProgressDisplayControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
        this.statusProgressDisplayControl.Location = new System.Drawing.Point(12, 12);
        this.statusProgressDisplayControl.Name = "statusProgressDisplayControl";
        this.statusProgressDisplayControl.Size = new System.Drawing.Size(440, 45); // Default size
        this.statusProgressDisplayControl.TabIndex = 0;
        // Example customizations for StatusProgressDisplay
        this.statusProgressDisplayControl.ProgressBarRunningWarningColor = Color.DarkOrange;
        this.statusProgressDisplayControl.StatusTextFaulted = "发生错误!";
        // 
        // btnStartSimpleTask
        // 
        this.btnStartSimpleTask.Location = new System.Drawing.Point(12, 70);
        this.btnStartSimpleTask.Name = "btnStartSimpleTask";
        this.btnStartSimpleTask.Size = new System.Drawing.Size(120, 23);
        this.btnStartSimpleTask.TabIndex = 1;
        this.btnStartSimpleTask.Text = "启动正常任务";
        this.btnStartSimpleTask.UseVisualStyleBackColor = true;
        this.btnStartSimpleTask.Click += new System.EventHandler(this.BtnStartSimpleTask_Click);
        // 
        // btnStartErrorTask
        // 
        this.btnStartErrorTask.Location = new System.Drawing.Point(138, 70);
        this.btnStartErrorTask.Name = "btnStartErrorTask";
        this.btnStartErrorTask.Size = new System.Drawing.Size(120, 23);
        this.btnStartErrorTask.TabIndex = 2;
        this.btnStartErrorTask.Text = "启动错误任务";
        this.btnStartErrorTask.UseVisualStyleBackColor = true;
        this.btnStartErrorTask.Click += new System.EventHandler(this.BtnStartErrorTask_Click);
        // 
        // btnCancelSimpleTask
        // 
        this.btnCancelSimpleTask.Enabled = false;
        this.btnCancelSimpleTask.Location = new System.Drawing.Point(264, 70);
        this.btnCancelSimpleTask.Name = "btnCancelSimpleTask";
        this.btnCancelSimpleTask.Size = new System.Drawing.Size(75, 23);
        this.btnCancelSimpleTask.TabIndex = 3;
        this.btnCancelSimpleTask.Text = "取消";
        this.btnCancelSimpleTask.UseVisualStyleBackColor = true;
        this.btnCancelSimpleTask.Click += new System.EventHandler(this.BtnCancelSimpleTask_Click);
        // 
        // SimpleProgressForm02
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(464, 115);
        this.Controls.Add(this.statusProgressDisplayControl);
        this.Controls.Add(this.btnCancelSimpleTask);
        this.Controls.Add(this.btnStartErrorTask);
        this.Controls.Add(this.btnStartSimpleTask);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.Name = "SimpleProgressForm02";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "简单进度 V2 (自定义控件)";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimpleProgressForm02_FormClosing);
        this.ResumeLayout(false);
    }

    private void InitializeTaskWrapperAndDisplay()
    {
        _taskWrapper = new TaskWrapperReport.TaskWrapperCore.TaskWrapper
        {
            TextProgressBarMinimum = 0,
            TextProgressBarMaximum = 100
        };

        statusProgressDisplayControl.ProgressBarMinimum = _taskWrapper.TextProgressBarMinimum;
        statusProgressDisplayControl.ProgressBarMaximum = _taskWrapper.TextProgressBarMaximum;
        statusProgressDisplayControl.ResetDisplay(_taskWrapper.TextProgressBarMinimum, _taskWrapper.TextProgressBarMaximum);

        _taskWrapper.ProgressReporter.ProgressChanged += TaskWrapper_ProgressChanged;
        _taskWrapper.StateChanged += TaskWrapper_StateChanged;
        _taskWrapper.AfterCompletionAction = () => { /* Optional post-completion logic */ };
    }

    private void TaskWrapper_ProgressChanged(object sender, TaskProgressInfo e)
    {
        statusProgressDisplayControl.ReportProgress(e, _taskWrapper.CurrentState);
    }

    private void TaskWrapper_StateChanged(object sender, TaskStateChangedEventArgs e)
    {
        ControlInvokeHelper.ControlInvoke(this, () =>
        {
            statusProgressDisplayControl.SetVisualState(e.NewState, e.Exception);

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
            throw new InvalidOperationException("这是一个来自 SimpleProgressForm02 的故意测试异常!");
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
        statusProgressDisplayControl.ResetDisplay(_taskWrapper.TextProgressBarMinimum, _taskWrapper.TextProgressBarMaximum);
        // statusProgressDisplayControl.SetVisualState(TaskExecutionState.Starting); // TaskWrapper will set this via StateChanged
        _taskWrapper.StartTaskAsync();
    }

    private void BtnCancelSimpleTask_Click(object sender, EventArgs e)
    {
        _taskWrapper.RequestCancel();
    }

    private void SimpleProgressForm02_FormClosing(object sender, FormClosingEventArgs e)
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
