namespace GoldArch.TaskWrapperReport.TaskReportControl
{
    partial class StatusProgressBar
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
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
            this.textProgressBar = new TextProgressBar();
            this.SuspendLayout();
            // 
            // textProgressBar
            // 
            this.textProgressBar.CustomText = "准备就绪"; // Default text
            this.textProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textProgressBar.Location = new System.Drawing.Point(0, 0);
            this.textProgressBar.Name = "textProgressBar";
            this.textProgressBar.ProgressColor = System.Drawing.Color.LightGray; // Initial color
            this.textProgressBar.Size = new System.Drawing.Size(300, 23); // Default designer size
            this.textProgressBar.TabIndex = 0;
            this.textProgressBar.TextColor = System.Drawing.Color.Black; // Default text color
            // Default font, consider setting from constructor or making it a property of StatusProgressBar
            this.textProgressBar.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textProgressBar.VisualMode = ProgressBarDisplayMode.CustomText;
            // 
            // StatusProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textProgressBar);
            this.Name = "StatusProgressBar";
            this.Size = new System.Drawing.Size(300, 23); // Default UserControl size, should match TextProgressBar height
            this.ResumeLayout(false);

        }

        #endregion
    }
}