namespace EngineTestTool
{
    partial class ParallelTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ProcessStartLabel = new System.Windows.Forms.Label();
            this.ProcessingLabel = new System.Windows.Forms.Label();
            this.ProcessingEndLabel = new System.Windows.Forms.Label();
            this.StartProcessButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ProcessStartLabel
            // 
            this.ProcessStartLabel.AutoSize = true;
            this.ProcessStartLabel.Location = new System.Drawing.Point(13, 13);
            this.ProcessStartLabel.Name = "ProcessStartLabel";
            this.ProcessStartLabel.Size = new System.Drawing.Size(29, 13);
            this.ProcessStartLabel.TabIndex = 0;
            this.ProcessStartLabel.Text = "Start";
            // 
            // ProcessingLabel
            // 
            this.ProcessingLabel.AutoSize = true;
            this.ProcessingLabel.Location = new System.Drawing.Point(71, 13);
            this.ProcessingLabel.Name = "ProcessingLabel";
            this.ProcessingLabel.Size = new System.Drawing.Size(59, 13);
            this.ProcessingLabel.TabIndex = 1;
            this.ProcessingLabel.Text = "Processing";
            // 
            // ProcessingEndLabel
            // 
            this.ProcessingEndLabel.AutoSize = true;
            this.ProcessingEndLabel.Location = new System.Drawing.Point(153, 13);
            this.ProcessingEndLabel.Name = "ProcessingEndLabel";
            this.ProcessingEndLabel.Size = new System.Drawing.Size(26, 13);
            this.ProcessingEndLabel.TabIndex = 2;
            this.ProcessingEndLabel.Text = "End";
            // 
            // StartProcessButton
            // 
            this.StartProcessButton.Location = new System.Drawing.Point(632, 13);
            this.StartProcessButton.Name = "StartProcessButton";
            this.StartProcessButton.Size = new System.Drawing.Size(75, 23);
            this.StartProcessButton.TabIndex = 3;
            this.StartProcessButton.Text = "Start Process";
            this.StartProcessButton.UseVisualStyleBackColor = true;
            this.StartProcessButton.Click += new System.EventHandler(this.StartProcessButton_Click);
            // 
            // ParallelTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1476, 655);
            this.Controls.Add(this.StartProcessButton);
            this.Controls.Add(this.ProcessingEndLabel);
            this.Controls.Add(this.ProcessingLabel);
            this.Controls.Add(this.ProcessStartLabel);
            this.Name = "ParallelTest";
            this.Text = "ParallelTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ProcessStartLabel;
        private System.Windows.Forms.Label ProcessingLabel;
        private System.Windows.Forms.Label ProcessingEndLabel;
        private System.Windows.Forms.Button StartProcessButton;
    }
}