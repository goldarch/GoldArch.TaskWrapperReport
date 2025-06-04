// GoldArch.TaskWrapper.Simple/SimpleProgressForm.cs

using System;
using System.Drawing;
using System.Windows.Forms;
using GoldArch.TaskWrapperReport.Common;
using GoldArch.TaskWrapperReport.TaskReportControl;
// using System.Threading.Tasks; // No longer directly used for task logic
// using GoldArch.TaskWrapperReport.TaskWrapperCore; // TaskWrapperCore types accessed via Presenter or View Interface

// Add this for MVP components

namespace GoldArch.TaskWrapperReport.Simple.MVP
{
    // Original class declaration: public class SimpleProgressForm : Form
    public class SimpleProgressForm04 : Form, ISimpleProgressView // Implement the view interface
    {
        private TextProgressBar textProgressBarTask;
        private Button btnStartSimpleTask;
        private Button btnStartErrorTask;
        private Button btnCancelSimpleTask;
        private Label lblStatus;

        // private TaskWrapperReport.TaskWrapperCore.TaskWrapper _taskWrapper; // Moved to Presenter
        private SimpleProgressPresenter _presenter;

        // UI Color Scheme properties are now in Presenter or implicitly handled by it.

        public SimpleProgressForm04()
        {
            InitializeComponent();
            // InitializeTaskWrapper(); // This is now handled by presenter
            _presenter = new SimpleProgressPresenter(this);
            // Trigger Load event for presenter after components are initialized
            this.Load += (sender, e) => LoadView?.Invoke(this, EventArgs.Empty);
        }

        #region ISimpleProgressView Implementation

        public string StatusText
        {
            get => lblStatus.Text;
            set => ControlInvokeHelper.ControlInvoke(lblStatus, () => lblStatus.Text = value);
        }

        public int ProgressBarValue
        {
            get => textProgressBarTask.Value;
            set => ControlInvokeHelper.ControlInvoke(textProgressBarTask, () => textProgressBarTask.Value = value);
        }
        public int ProgressBarMinimum
        {
            get => textProgressBarTask.Minimum;
            set => ControlInvokeHelper.ControlInvoke(textProgressBarTask, () => textProgressBarTask.Minimum = value);
        }
        public int ProgressBarMaximum
        {
            get => textProgressBarTask.Maximum;
            set => ControlInvokeHelper.ControlInvoke(textProgressBarTask, () => textProgressBarTask.Maximum = value);
        }
        public string ProgressBarCustomText
        {
            get => textProgressBarTask.CustomText;
            set => ControlInvokeHelper.ControlInvoke(textProgressBarTask, () => textProgressBarTask.CustomText = value);
        }
        public Color ProgressBarProgressColor
        {
            get => textProgressBarTask.ProgressColor;
            set => ControlInvokeHelper.ControlInvoke(textProgressBarTask, () => textProgressBarTask.ProgressColor = value);
        }
        public bool StartSimpleTaskEnabled
        {
            get => btnStartSimpleTask.Enabled;
            set => ControlInvokeHelper.ControlInvoke(btnStartSimpleTask, () => btnStartSimpleTask.Enabled = value);
        }
        public bool StartErrorTaskEnabled
        {
            get => btnStartErrorTask.Enabled;
            set => ControlInvokeHelper.ControlInvoke(btnStartErrorTask, () => btnStartErrorTask.Enabled = value);
        }
        public bool CancelTaskEnabled
        {
            get => btnCancelSimpleTask.Enabled;
            set => ControlInvokeHelper.ControlInvoke(btnCancelSimpleTask, () => btnCancelSimpleTask.Enabled = value);
        }

        public event EventHandler LoadView;
        public event EventHandler StartSimpleTaskClicked;
        public event EventHandler StartErrorTaskClicked;
        public event EventHandler CancelTaskClicked;
        public event FormClosingEventHandler ViewClosing; // Already part of Form, just expose via interface

        public void ShowMessage(string caption, string text, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            ControlInvokeHelper.ControlInvoke(this, () => MessageBox.Show(this, text, caption, buttons, icon));
        }
        public void CloseView()
        {
            ControlInvokeHelper.ControlInvoke(this, () => this.Close());
        }

        #endregion

        private void InitializeComponent()
        {
            this.textProgressBarTask = new GoldArch.TaskWrapperReport.TaskReportControl.TextProgressBar();
            this.btnStartSimpleTask = new System.Windows.Forms.Button();
            this.btnStartErrorTask = new System.Windows.Forms.Button();
            this.btnCancelSimpleTask = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textProgressBarTask
            // 
            this.textProgressBarTask.CustomText = "准备就绪";
            this.textProgressBarTask.Location = new System.Drawing.Point(12, 46);
            this.textProgressBarTask.Name = "textProgressBarTask";
            this.textProgressBarTask.ProgressColor = System.Drawing.Color.LightGreen;
            this.textProgressBarTask.Size = new System.Drawing.Size(440, 21);
            this.textProgressBarTask.TabIndex = 0;
            this.textProgressBarTask.TextColor = System.Drawing.Color.Black;
            this.textProgressBarTask.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // btnStartSimpleTask
            // 
            this.btnStartSimpleTask.Location = new System.Drawing.Point(12, 83);
            this.btnStartSimpleTask.Name = "btnStartSimpleTask";
            this.btnStartSimpleTask.Size = new System.Drawing.Size(120, 21);
            this.btnStartSimpleTask.TabIndex = 1;
            this.btnStartSimpleTask.Text = "启动正常任务";
            this.btnStartSimpleTask.UseVisualStyleBackColor = true;
            this.btnStartSimpleTask.Click += new System.EventHandler(this.btnStartSimpleTask_Click);
            // 
            // btnStartErrorTask
            // 
            this.btnStartErrorTask.Location = new System.Drawing.Point(138, 83);
            this.btnStartErrorTask.Name = "btnStartErrorTask";
            this.btnStartErrorTask.Size = new System.Drawing.Size(120, 21);
            this.btnStartErrorTask.TabIndex = 2;
            this.btnStartErrorTask.Text = "启动错误任务";
            this.btnStartErrorTask.UseVisualStyleBackColor = true;
            this.btnStartErrorTask.Click += new System.EventHandler(this.btnStartErrorTask_Click);
            // 
            // btnCancelSimpleTask
            // 
            this.btnCancelSimpleTask.Enabled = false;
            this.btnCancelSimpleTask.Location = new System.Drawing.Point(264, 83);
            this.btnCancelSimpleTask.Name = "btnCancelSimpleTask";
            this.btnCancelSimpleTask.Size = new System.Drawing.Size(75, 21);
            this.btnCancelSimpleTask.TabIndex = 3;
            this.btnCancelSimpleTask.Text = "取消";
            this.btnCancelSimpleTask.UseVisualStyleBackColor = true;
            this.btnCancelSimpleTask.Click += new System.EventHandler(this.btnCancelSimpleTask_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 18);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(65, 12);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "状态: 空闲";
            // 
            // SimpleProgressForm04
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 130);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCancelSimpleTask);
            this.Controls.Add(this.btnStartErrorTask);
            this.Controls.Add(this.btnStartSimpleTask);
            this.Controls.Add(this.textProgressBarTask);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SimpleProgressForm04";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "简单进度条示例 (MVP)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        // Removed TaskWrapper_ProgressChanged, TaskWrapper_StateChanged, HandleTaskCompletion
        // Removed BtnStartSimpleTask_Click, BtnStartErrorTask_Click, BtnCancelSimpleTask_Click direct logic
        // Removed IsTaskRunnable, StartTaskWrapperInternal

        private void SimpleProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ViewClosing?.Invoke(this, e); // Notify presenter
            _presenter?.Dispose(); // Dispose presenter
        }

        private void btnStartSimpleTask_Click(object sender, EventArgs e)
        {
            StartSimpleTaskClicked?.Invoke(this, e); // Forward to presenter
        }

        private void btnStartErrorTask_Click(object sender, EventArgs e)
        {
            StartErrorTaskClicked?.Invoke(this, e); // Forward to presenter
        }

        private void btnCancelSimpleTask_Click(object sender, EventArgs e)
        {
            CancelTaskClicked?.Invoke(this, e); // Forward to presenter
        }
    }
}