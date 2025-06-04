namespace GoldArch.TaskWrapperReport.TaskReportControl
{
    partial class TaskReportRichTextBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Dispose method is in the main partial class (TaskReportRichTextBoxV3.cs)

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskReportRichTextBox));
            this.panel1 = new System.Windows.Forms.Panel();
            this.textProgressBar1 = new GoldArch.TaskWrapperReport.TaskReportControl.TextProgressBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBox报告是否总是滚动到最后 = new System.Windows.Forms.CheckBox();
            this.RichTextBoxLog = new GoldArch.TaskWrapperReport.TaskReportControl.RichTextBoxEx();
            this.button取消 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textProgressBar1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(497, 29);
            this.panel1.TabIndex = 3;
            // 
            // textProgressBar1
            // 
            this.textProgressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textProgressBar1.Location = new System.Drawing.Point(0, 0);
            this.textProgressBar1.Name = "textProgressBar1";
            this.textProgressBar1.ProgressColor = System.Drawing.Color.LightGreen;
            this.textProgressBar1.Size = new System.Drawing.Size(497, 29);
            this.textProgressBar1.TabIndex = 2;
            this.textProgressBar1.TextColor = System.Drawing.Color.Black;
            this.textProgressBar1.TextFont = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.checkBox报告是否总是滚动到最后);
            this.panel2.Controls.Add(this.button取消);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 236);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(497, 32);
            this.panel2.TabIndex = 4;
            // 
            // checkBox报告是否总是滚动到最后
            // 
            this.checkBox报告是否总是滚动到最后.AutoSize = true;
            this.checkBox报告是否总是滚动到最后.Location = new System.Drawing.Point(7, 7);
            this.checkBox报告是否总是滚动到最后.Name = "checkBox报告是否总是滚动到最后";
            this.checkBox报告是否总是滚动到最后.Size = new System.Drawing.Size(132, 16);
            this.checkBox报告是否总是滚动到最后.TabIndex = 64;
            this.checkBox报告是否总是滚动到最后.Text = "是否总是滚动到末行";
            this.checkBox报告是否总是滚动到最后.UseVisualStyleBackColor = true;
            this.checkBox报告是否总是滚动到最后.CheckedChanged += new System.EventHandler(this.checkBox报告是否总是滚动到最后_CheckedChanged);
            // 
            // RichTextBoxLog
            // 
            this.RichTextBoxLog.BackColor = System.Drawing.SystemColors.Info;
            this.RichTextBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RichTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RichTextBoxLog.Location = new System.Drawing.Point(0, 29);
            this.RichTextBoxLog.Name = "RichTextBoxLog";
            this.RichTextBoxLog.ReadOnly = true;
            this.RichTextBoxLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.RichTextBoxLog.Size = new System.Drawing.Size(497, 207);
            this.RichTextBoxLog.TabIndex = 5;
            this.RichTextBoxLog.Text = "";
            // 
            // button取消
            // 
            this.button取消.Dock = System.Windows.Forms.DockStyle.Right;
            this.button取消.Enabled = false;
            this.button取消.Image = ((System.Drawing.Image)(resources.GetObject("button取消.Image")));
            this.button取消.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button取消.Location = new System.Drawing.Point(443, 0);
            this.button取消.Name = "button取消";
            this.button取消.Size = new System.Drawing.Size(54, 32);
            this.button取消.TabIndex = 3;
            this.button取消.Text = "取消";
            this.button取消.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button取消.UseVisualStyleBackColor = true;
            this.button取消.Click += new System.EventHandler(this.button取消_Click);
            // 
            // TaskReportRichTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.RichTextBoxLog);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "TaskReportRichTextBox";
            this.Size = new System.Drawing.Size(497, 268);
            this.Load += new System.EventHandler(this.ProgressControl_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private TextProgressBar textProgressBar1;
        private System.Windows.Forms.Panel panel2;
        private RichTextBoxEx RichTextBoxLog;
        private System.Windows.Forms.Button button取消;
        private System.Windows.Forms.CheckBox checkBox报告是否总是滚动到最后;
    }
}