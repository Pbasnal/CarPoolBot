namespace EngineTestTool
{
    partial class TripRequestManagerTest
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
            this.commuters = new BufferedTreeView();
            this.poolers = new BufferedTreeView();
            this.addCommuterOfficeRequestButton = new System.Windows.Forms.Button();
            this.addPoolerOfficeRequestButton = new System.Windows.Forms.Button();
            this.addCommuterHomeRequestButton = new System.Windows.Forms.Button();
            this.addPoolerHomeRequestButton = new System.Windows.Forms.Button();
            this.commuterTree = new System.Windows.Forms.Label();
            this.poolerTree = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // commuters
            // 
            this.commuters.Location = new System.Drawing.Point(13, 47);
            this.commuters.Name = "commuters";
            this.commuters.Size = new System.Drawing.Size(218, 487);
            this.commuters.TabIndex = 0;
            // 
            // poolers
            // 
            this.poolers.Location = new System.Drawing.Point(238, 47);
            this.poolers.Name = "poolers";
            this.poolers.Size = new System.Drawing.Size(237, 487);
            this.poolers.TabIndex = 1;
            // 
            // addCommuterOfficeRequestButton
            // 
            this.addCommuterOfficeRequestButton.Location = new System.Drawing.Point(508, 152);
            this.addCommuterOfficeRequestButton.Name = "addCommuterOfficeRequestButton";
            this.addCommuterOfficeRequestButton.Size = new System.Drawing.Size(403, 43);
            this.addCommuterOfficeRequestButton.TabIndex = 2;
            this.addCommuterOfficeRequestButton.Text = "Add Commuter Going To Office";
            this.addCommuterOfficeRequestButton.UseVisualStyleBackColor = true;
            this.addCommuterOfficeRequestButton.Click += new System.EventHandler(this.addCommuterOfficeRequestButton_Click);
            // 
            // addPoolerOfficeRequestButton
            // 
            this.addPoolerOfficeRequestButton.Location = new System.Drawing.Point(508, 280);
            this.addPoolerOfficeRequestButton.Name = "addPoolerOfficeRequestButton";
            this.addPoolerOfficeRequestButton.Size = new System.Drawing.Size(403, 42);
            this.addPoolerOfficeRequestButton.TabIndex = 3;
            this.addPoolerOfficeRequestButton.Text = "Add Pooler Going To Office";
            this.addPoolerOfficeRequestButton.UseVisualStyleBackColor = true;
            this.addPoolerOfficeRequestButton.Click += new System.EventHandler(this.addPoolerOfficeRequestButton_Click);
            // 
            // addCommuterHomeRequestButton
            // 
            this.addCommuterHomeRequestButton.Location = new System.Drawing.Point(508, 202);
            this.addCommuterHomeRequestButton.Name = "addCommuterHomeRequestButton";
            this.addCommuterHomeRequestButton.Size = new System.Drawing.Size(403, 42);
            this.addCommuterHomeRequestButton.TabIndex = 4;
            this.addCommuterHomeRequestButton.Text = "Add Commuter Going To Home";
            this.addCommuterHomeRequestButton.UseVisualStyleBackColor = true;
            this.addCommuterHomeRequestButton.Click += new System.EventHandler(this.addCommuterHomeRequestButton_Click);
            // 
            // addPoolerHomeRequestButton
            // 
            this.addPoolerHomeRequestButton.Location = new System.Drawing.Point(508, 329);
            this.addPoolerHomeRequestButton.Name = "addPoolerHomeRequestButton";
            this.addPoolerHomeRequestButton.Size = new System.Drawing.Size(403, 42);
            this.addPoolerHomeRequestButton.TabIndex = 5;
            this.addPoolerHomeRequestButton.Text = "Add Pooler Going To Home";
            this.addPoolerHomeRequestButton.UseVisualStyleBackColor = true;
            this.addPoolerHomeRequestButton.Click += new System.EventHandler(this.addPoolerHomeRequestButton_Click);
            // 
            // commuterTree
            // 
            this.commuterTree.AutoSize = true;
            this.commuterTree.Location = new System.Drawing.Point(75, 31);
            this.commuterTree.Name = "commuterTree";
            this.commuterTree.Size = new System.Drawing.Size(79, 13);
            this.commuterTree.TabIndex = 6;
            this.commuterTree.Text = "Commuter Tree";
            // 
            // poolerTree
            // 
            this.poolerTree.AutoSize = true;
            this.poolerTree.Location = new System.Drawing.Point(319, 31);
            this.poolerTree.Name = "poolerTree";
            this.poolerTree.Size = new System.Drawing.Size(62, 13);
            this.poolerTree.TabIndex = 7;
            this.poolerTree.Text = "Pooler Tree";
            // 
            // TripRequestManagerTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 546);
            this.Controls.Add(this.poolerTree);
            this.Controls.Add(this.commuterTree);
            this.Controls.Add(this.addPoolerHomeRequestButton);
            this.Controls.Add(this.addCommuterHomeRequestButton);
            this.Controls.Add(this.addPoolerOfficeRequestButton);
            this.Controls.Add(this.addCommuterOfficeRequestButton);
            this.Controls.Add(this.poolers);
            this.Controls.Add(this.commuters);
            this.Name = "TripRequestManagerTest";
            this.Text = "TripRequestManagerTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BufferedTreeView commuters;
        private BufferedTreeView poolers;
        private System.Windows.Forms.Button addCommuterOfficeRequestButton;
        private System.Windows.Forms.Button addPoolerOfficeRequestButton;
        private System.Windows.Forms.Button addCommuterHomeRequestButton;
        private System.Windows.Forms.Button addPoolerHomeRequestButton;
        private System.Windows.Forms.Label commuterTree;
        private System.Windows.Forms.Label poolerTree;
    }
}