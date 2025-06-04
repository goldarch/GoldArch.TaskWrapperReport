using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoldArch.TaskWrapperReport.Simple.MVP;
using GoldArch.TaskWrapperReport.TaskReportControl;
using GoldArch.TaskWrapperReport.TaskWrapperCore;

// Assuming SimpleProgressForm, SimpleProgressForm02, SimpleProgressForm03 are in the same assembly or referenced.
// If they are in a different namespace, add the appropriate using directive.
// For example: using YourProject.SimpleForms;

namespace GoldArch.TaskWrapperReport.Simple
{
    public class MainForm : Form
{
    private Button btnStartTask;
    private Button btnStartFailingTask;
    private Button btnReportDirectly;
    private Button btnShowSimpleForm1;
    private Button btnShowSimpleForm2;
    private Button btnShowSimpleForm3;
        private Button button1;
        private TaskReportRichTextBox taskReportControl1;

    public MainForm()
    {
        InitializeComponent();
        SetupTaskReportControl();
    }

    private void SetupTaskReportControl()
    {
        taskReportControl1.ShowCancelButton = true;
        taskReportControl1.ShowErrorStackTraceInWrapper = true;
        taskReportControl1.IsScrollToEndAfterReport = true;

        // Example of customizing colors
        taskReportControl1.WarningTextColor = Color.FromArgb(255, 165, 0); // Custom Orange
        taskReportControl1.ProgressBarWarningColor = Color.FromArgb(255, 140, 0); // Darker Orange

        taskReportControl1.TaskWrapperInstance.StateChanged += (s, e) =>
        {
            // Example: Log state changes or update other UI elements based on task state
            // this.Text = $"Task Demo - Main Wrapper State: {e.NewState}";
            if (e.NewState == TaskExecutionState.Faulted && e.Exception != null)
            {
                System.Diagnostics.Debug.WriteLine($"MainForm's TaskReportControl1 encountered an error: {e.Exception.Message}");
            }
        };
    }

    private void InitializeComponent()
    {
            this.btnStartTask = new System.Windows.Forms.Button();
            this.btnStartFailingTask = new System.Windows.Forms.Button();
            this.btnReportDirectly = new System.Windows.Forms.Button();
            this.btnShowSimpleForm1 = new System.Windows.Forms.Button();
            this.btnShowSimpleForm2 = new System.Windows.Forms.Button();
            this.taskReportControl1 = new GoldArch.TaskWrapperReport.TaskReportControl.TaskReportRichTextBox();
            this.btnShowSimpleForm3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartTask
            // 
            this.btnStartTask.Location = new System.Drawing.Point(12, 11);
            this.btnStartTask.Name = "btnStartTask";
            this.btnStartTask.Size = new System.Drawing.Size(120, 21);
            this.btnStartTask.TabIndex = 0;
            this.btnStartTask.Text = "Start Long Task";
            this.btnStartTask.UseVisualStyleBackColor = true;
            this.btnStartTask.Click += new System.EventHandler(this.BtnStartTask_Click);
            // 
            // btnStartFailingTask
            // 
            this.btnStartFailingTask.Location = new System.Drawing.Point(138, 11);
            this.btnStartFailingTask.Name = "btnStartFailingTask";
            this.btnStartFailingTask.Size = new System.Drawing.Size(130, 21);
            this.btnStartFailingTask.TabIndex = 1;
            this.btnStartFailingTask.Text = "Start Failing Task";
            this.btnStartFailingTask.UseVisualStyleBackColor = true;
            this.btnStartFailingTask.Click += new System.EventHandler(this.BtnStartFailingTask_Click);
            // 
            // btnReportDirectly
            // 
            this.btnReportDirectly.Location = new System.Drawing.Point(274, 11);
            this.btnReportDirectly.Name = "btnReportDirectly";
            this.btnReportDirectly.Size = new System.Drawing.Size(130, 21);
            this.btnReportDirectly.TabIndex = 2;
            this.btnReportDirectly.Text = "Report Directly";
            this.btnReportDirectly.UseVisualStyleBackColor = true;
            this.btnReportDirectly.Click += new System.EventHandler(this.BtnReportDirectly_Click);
            // 
            // btnShowSimpleForm1
            // 
            this.btnShowSimpleForm1.Location = new System.Drawing.Point(410, 11);
            this.btnShowSimpleForm1.Name = "btnShowSimpleForm1";
            this.btnShowSimpleForm1.Size = new System.Drawing.Size(100, 21);
            this.btnShowSimpleForm1.TabIndex = 3;
            this.btnShowSimpleForm1.Text = "Simple Form 1";
            this.btnShowSimpleForm1.UseVisualStyleBackColor = true;
            this.btnShowSimpleForm1.Click += new System.EventHandler(this.BtnShowSimpleForm1_Click);
            // 
            // btnShowSimpleForm2
            // 
            this.btnShowSimpleForm2.Location = new System.Drawing.Point(516, 11);
            this.btnShowSimpleForm2.Name = "btnShowSimpleForm2";
            this.btnShowSimpleForm2.Size = new System.Drawing.Size(100, 21);
            this.btnShowSimpleForm2.TabIndex = 4;
            this.btnShowSimpleForm2.Text = "Simple Form 2";
            this.btnShowSimpleForm2.UseVisualStyleBackColor = true;
            this.btnShowSimpleForm2.Click += new System.EventHandler(this.BtnShowSimpleForm2_Click);
            // 
            // taskReportControl1
            // 
            this.taskReportControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.taskReportControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.taskReportControl1.DetailTextColor = System.Drawing.Color.Gray;
            this.taskReportControl1.ErrorTextColor = System.Drawing.Color.Crimson;
            this.taskReportControl1.InfoTextColor = System.Drawing.SystemColors.ControlText;
            this.taskReportControl1.Location = new System.Drawing.Point(12, 38);
            this.taskReportControl1.Name = "taskReportControl1";
            this.taskReportControl1.ProcessCancelledTextColor = System.Drawing.Color.OrangeRed;
            this.taskReportControl1.ProcessEndTextColor = System.Drawing.Color.DarkBlue;
            this.taskReportControl1.ProcessStartTextColor = System.Drawing.Color.Blue;
            this.taskReportControl1.ProgressBarDefaultColor = System.Drawing.Color.SkyBlue;
            this.taskReportControl1.ProgressBarErrorColor = System.Drawing.Color.Salmon;
            this.taskReportControl1.ProgressBarSuccessColor = System.Drawing.Color.MediumSeaGreen;
            this.taskReportControl1.ProgressBarWarningColor = System.Drawing.Color.Gold;
            this.taskReportControl1.Size = new System.Drawing.Size(887, 210);
            this.taskReportControl1.SuccessTextColor = System.Drawing.Color.DarkGreen;
            this.taskReportControl1.TabIndex = 6;
            this.taskReportControl1.WarningTextColor = System.Drawing.Color.DarkGoldenrod;
            // 
            // btnShowSimpleForm3
            // 
            this.btnShowSimpleForm3.Location = new System.Drawing.Point(622, 11);
            this.btnShowSimpleForm3.Name = "btnShowSimpleForm3";
            this.btnShowSimpleForm3.Size = new System.Drawing.Size(100, 21);
            this.btnShowSimpleForm3.TabIndex = 5;
            this.btnShowSimpleForm3.Text = "Simple Form 3";
            this.btnShowSimpleForm3.UseVisualStyleBackColor = true;
            this.btnShowSimpleForm3.Click += new System.EventHandler(this.BtnShowSimpleForm3_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(728, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(153, 21);
            this.button1.TabIndex = 7;
            this.button1.Text = "Simple Form 4 MVP";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 258);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnShowSimpleForm3);
            this.Controls.Add(this.btnShowSimpleForm2);
            this.Controls.Add(this.btnShowSimpleForm1);
            this.Controls.Add(this.taskReportControl1);
            this.Controls.Add(this.btnReportDirectly);
            this.Controls.Add(this.btnStartFailingTask);
            this.Controls.Add(this.btnStartTask);
            this.Name = "MainForm";
            this.Text = "Task Reporting Demo V3";
            this.ResumeLayout(false);

    }

    private void BtnStartTask_Click(object sender, EventArgs e)
    {
        var wrapper = taskReportControl1.TaskWrapperInstance;
        if (wrapper.CurrentState == TaskExecutionState.Running || wrapper.CurrentState == TaskExecutionState.Starting)
        {
            MessageBox.Show("A task is already running in the main control.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        taskReportControl1.ResetReportControl();

        wrapper.TextProgressBarMinimum = 0;
        wrapper.TextProgressBarMaximum = 100; // Corresponds to 10 steps * 10

        wrapper.DoWorkFuncAsync = async (token, progress) =>
        {
            for (int i = 0; i <= 10; i++) // 11 steps, 0 to 10
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(300, token).ConfigureAwait(false);

                progress.Report(new TaskProgressInfo(
                    content: $"Step {i + 1} completed. Log entry...{Environment.NewLine}",
                    progressText: $"Processing item {i + 1} of 11",
                    progressValue: (i + 1) * (100 / 11), // Approximate percentage
                    level: i > 7 ? ReportLevel.Warning : ReportLevel.Information
                ));
            }
            await Task.Delay(200, token).ConfigureAwait(false);
            progress.Report(new TaskProgressInfo("All steps processed successfully!", ReportLevel.Success));
            return null;
        };

        wrapper.BeforeDoWorkAction = () =>
        {
            taskReportControl1.ReportDirectContent($"Main task preparation started at {DateTime.Now}{Environment.NewLine}", ReportLevel.Detail);
        };

        wrapper.AfterCompletionAction = () =>
        {
            string endMessage = $"Main task's AfterCompletionAction: Task ended with state: {wrapper.CurrentState}.{Environment.NewLine}";
            taskReportControl1.ReportDirectContent(endMessage, ReportLevel.Detail);
        };

        wrapper.StartTaskAsync();
    }

    private void BtnStartFailingTask_Click(object sender, EventArgs e)
    {
        var wrapper = taskReportControl1.TaskWrapperInstance;
        if (wrapper.CurrentState == TaskExecutionState.Running || wrapper.CurrentState == TaskExecutionState.Starting)
        {
            MessageBox.Show("A task is already running in the main control.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        taskReportControl1.ResetReportControl();

        wrapper.TextProgressBarMinimum = 0;
        wrapper.TextProgressBarMaximum = 10; // Short task

        wrapper.DoWorkFuncAsync = async (token, progress) =>
        {
            progress.Report(new TaskProgressInfo($"Task designed to fail is starting...{Environment.NewLine}", "Starting failing task...", 0, ReportLevel.Information));
            await Task.Delay(1000, token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();
            progress.Report(new TaskProgressInfo("Simulating work before failure..." + Environment.NewLine, "Working before fail...", 5, ReportLevel.Warning));
            await Task.Delay(1000, token).ConfigureAwait(false);
            throw new InvalidOperationException("This is a deliberate, simulated failure from MainForm!");
        };

        wrapper.StartTaskAsync(reportWrapperStatusMessages: true);
    }

    private void BtnReportDirectly_Click(object sender, EventArgs e)
    {
        taskReportControl1.ReportDirectContent(
            $"This is a direct informational message at {DateTime.Now.ToLongTimeString()}{Environment.NewLine}",
            ReportLevel.Information
        );
        taskReportControl1.ReportDirectProgress("Direct Progress Update", 66, ReportLevel.StatusUpdate); // 66%
        taskReportControl1.ReportDirectContent(
             $"This is a direct warning!{Environment.NewLine}",
            ReportLevel.Warning
        );
    }

    private void BtnShowSimpleForm1_Click(object sender, EventArgs e)
    {
        using (var form = new SimpleProgressForm()) // Assuming SimpleProgressForm is in global namespace or using added
        {
            form.ShowDialog(this);
        }
    }

    private void BtnShowSimpleForm2_Click(object sender, EventArgs e)
    {
        using (var form = new SimpleProgressForm02()) // Assuming SimpleProgressForm02
        {
            form.ShowDialog(this);
        }
    }

    private void BtnShowSimpleForm3_Click(object sender, EventArgs e)
    {
        using (var form = new SimpleProgressForm03()) // Assuming SimpleProgressForm03
        {
            form.ShowDialog(this);
        }
    }

        private void button1_Click(object sender, EventArgs e)
        {
            new SimpleProgressForm04().Show();
        }
    }
}

