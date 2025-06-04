namespace GoldArch.TaskWrapperReport.TaskReportControl
{
    partial class StatusProgressDisplay
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblStatus;
        private TextProgressBar textProgressBar;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblStatus = new System.Windows.Forms.Label();
            this.textProgressBar = new TextProgressBar();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(3, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(64, 13); // Example size, text will determine actual
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "×´Ì¬: ¿ÕÏÐ"; // Default text
            // 
            // textProgressBar
            // 
            this.textProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textProgressBar.CustomText = "×¼±¸¾ÍÐ÷";
            this.textProgressBar.Location = new System.Drawing.Point(3, 18);
            this.textProgressBar.Name = "textProgressBar";
            this.textProgressBar.ProgressColor = System.Drawing.Color.LightGray; // Initial color
            this.textProgressBar.Size = new System.Drawing.Size(334, 23); // Default designer size
            this.textProgressBar.TabIndex = 1;
            this.textProgressBar.TextColor = System.Drawing.Color.Black;
            this.textProgressBar.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textProgressBar.VisualMode = ProgressBarDisplayMode.CustomText;
            // 
            // StatusProgressDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textProgressBar);
            this.Controls.Add(this.lblStatus);
            this.Name = "StatusProgressDisplay";
            this.Size = new System.Drawing.Size(340, 45); // Default UserControl size
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}