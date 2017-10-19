namespace EngineTestTool
{
    partial class TestTool
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.KeyList = new System.Windows.Forms.ListBox();
            this.addPoolerHomeRequestButton = new System.Windows.Forms.Button();
            this.addCommuterHomeRequestButton = new System.Windows.Forms.Button();
            this.addPoolerOfficeRequestButton = new System.Windows.Forms.Button();
            this.addCommuterOfficeRequestButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea6.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea6);
            legend6.Name = "Legend1";
            this.chart1.Legends.Add(legend6);
            this.chart1.Location = new System.Drawing.Point(22, 22);
            this.chart1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chart1.Name = "chart1";
            series6.ChartArea = "ChartArea1";
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.chart1.Series.Add(series6);
            this.chart1.Size = new System.Drawing.Size(1509, 890);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // KeyList
            // 
            this.KeyList.FormattingEnabled = true;
            this.KeyList.ItemHeight = 24;
            this.KeyList.Location = new System.Drawing.Point(1656, 22);
            this.KeyList.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.KeyList.Name = "KeyList";
            this.KeyList.Size = new System.Drawing.Size(305, 892);
            this.KeyList.TabIndex = 1;
            // 
            // addPoolerHomeRequestButton
            // 
            this.addPoolerHomeRequestButton.Location = new System.Drawing.Point(773, 1063);
            this.addPoolerHomeRequestButton.Margin = new System.Windows.Forms.Padding(6);
            this.addPoolerHomeRequestButton.Name = "addPoolerHomeRequestButton";
            this.addPoolerHomeRequestButton.Size = new System.Drawing.Size(739, 78);
            this.addPoolerHomeRequestButton.TabIndex = 9;
            this.addPoolerHomeRequestButton.Text = "Add Pooler Going To Home";
            this.addPoolerHomeRequestButton.UseVisualStyleBackColor = true;
            this.addPoolerHomeRequestButton.Click += new System.EventHandler(this.addPoolerHomeRequestButton_Click);
            // 
            // addCommuterHomeRequestButton
            // 
            this.addCommuterHomeRequestButton.Location = new System.Drawing.Point(22, 1063);
            this.addCommuterHomeRequestButton.Margin = new System.Windows.Forms.Padding(6);
            this.addCommuterHomeRequestButton.Name = "addCommuterHomeRequestButton";
            this.addCommuterHomeRequestButton.Size = new System.Drawing.Size(739, 78);
            this.addCommuterHomeRequestButton.TabIndex = 8;
            this.addCommuterHomeRequestButton.Text = "Add Commuter Going To Home";
            this.addCommuterHomeRequestButton.UseVisualStyleBackColor = true;
            this.addCommuterHomeRequestButton.Click += new System.EventHandler(this.addCommuterHomeRequestButton_Click);
            // 
            // addPoolerOfficeRequestButton
            // 
            this.addPoolerOfficeRequestButton.Location = new System.Drawing.Point(773, 973);
            this.addPoolerOfficeRequestButton.Margin = new System.Windows.Forms.Padding(6);
            this.addPoolerOfficeRequestButton.Name = "addPoolerOfficeRequestButton";
            this.addPoolerOfficeRequestButton.Size = new System.Drawing.Size(739, 78);
            this.addPoolerOfficeRequestButton.TabIndex = 7;
            this.addPoolerOfficeRequestButton.Text = "Add Pooler Going To Office";
            this.addPoolerOfficeRequestButton.UseVisualStyleBackColor = true;
            this.addPoolerOfficeRequestButton.Click += new System.EventHandler(this.addPoolerOfficeRequestButton_Click);
            // 
            // addCommuterOfficeRequestButton
            // 
            this.addCommuterOfficeRequestButton.Location = new System.Drawing.Point(22, 971);
            this.addCommuterOfficeRequestButton.Margin = new System.Windows.Forms.Padding(6);
            this.addCommuterOfficeRequestButton.Name = "addCommuterOfficeRequestButton";
            this.addCommuterOfficeRequestButton.Size = new System.Drawing.Size(739, 79);
            this.addCommuterOfficeRequestButton.TabIndex = 6;
            this.addCommuterOfficeRequestButton.Text = "Add Commuter Going To Office";
            this.addCommuterOfficeRequestButton.UseVisualStyleBackColor = true;
            this.addCommuterOfficeRequestButton.Click += new System.EventHandler(this.addCommuterOfficeRequestButton_Click);
            // 
            // TestTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1986, 1161);
            this.Controls.Add(this.addPoolerHomeRequestButton);
            this.Controls.Add(this.addCommuterHomeRequestButton);
            this.Controls.Add(this.addPoolerOfficeRequestButton);
            this.Controls.Add(this.addCommuterOfficeRequestButton);
            this.Controls.Add(this.KeyList);
            this.Controls.Add(this.chart1);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "TestTool";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ListBox KeyList;
        private System.Windows.Forms.Button addPoolerHomeRequestButton;
        private System.Windows.Forms.Button addCommuterHomeRequestButton;
        private System.Windows.Forms.Button addPoolerOfficeRequestButton;
        private System.Windows.Forms.Button addCommuterOfficeRequestButton;
    }
}

