namespace EngineTestTool
{
    partial class Gmap
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
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.start = new System.Windows.Forms.Button();
            this.stateText = new System.Windows.Forms.TextBox();
            this.commutersTreeView = new EngineTestTool.BufferedTreeView();
            this.poolersTreeView = new EngineTestTool.BufferedTreeView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // gMapControl1
            // 
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(22, 78);
            this.gMapControl1.Margin = new System.Windows.Forms.Padding(4);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 18;
            this.gMapControl1.MinZoom = 0;
            this.gMapControl1.MouseWheelZoomEnabled = true;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Fractional;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(1470, 1148);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 12D;
            this.gMapControl1.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl1_OnMarkerClick);
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            this.gMapControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseClick);
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(1502, 1174);
            this.start.Margin = new System.Windows.Forms.Padding(4);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(161, 52);
            this.start.TabIndex = 3;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // stateText
            // 
            this.stateText.Location = new System.Drawing.Point(1502, 761);
            this.stateText.Margin = new System.Windows.Forms.Padding(6);
            this.stateText.Multiline = true;
            this.stateText.Name = "stateText";
            this.stateText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.stateText.Size = new System.Drawing.Size(670, 401);
            this.stateText.TabIndex = 4;
            // 
            // commutersTreeView
            // 
            this.commutersTreeView.Location = new System.Drawing.Point(1502, 82);
            this.commutersTreeView.Margin = new System.Windows.Forms.Padding(4);
            this.commutersTreeView.Name = "commutersTreeView";
            this.commutersTreeView.Size = new System.Drawing.Size(330, 669);
            this.commutersTreeView.TabIndex = 2;
            this.commutersTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.commutersTreeView_AfterSelect);
            // 
            // poolersTreeView
            // 
            this.poolersTreeView.Location = new System.Drawing.Point(1842, 82);
            this.poolersTreeView.Margin = new System.Windows.Forms.Padding(4);
            this.poolersTreeView.Name = "poolersTreeView";
            this.poolersTreeView.Size = new System.Drawing.Size(330, 669);
            this.poolersTreeView.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(22, 1235);
            this.textBox1.Margin = new System.Windows.Forms.Padding(6);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(1467, 224);
            this.textBox1.TabIndex = 5;
            // 
            // Gmap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2169, 1470);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.stateText);
            this.Controls.Add(this.start);
            this.Controls.Add(this.commutersTreeView);
            this.Controls.Add(this.poolersTreeView);
            this.Controls.Add(this.gMapControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Gmap";
            this.Text = "Gmap";
            this.Load += new System.EventHandler(this.Gmap_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private BufferedTreeView poolersTreeView;
        private BufferedTreeView commutersTreeView;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.TextBox stateText;
        private System.Windows.Forms.TextBox textBox1;
    }
}