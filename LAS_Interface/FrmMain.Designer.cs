namespace LAS_Interface
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dgvDisplay = new System.Windows.Forms.DataGridView();
            this.btnEnd = new System.Windows.Forms.Button();
            this.btnConnectAcl = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnSetPreset = new System.Windows.Forms.Button();
            this.lstMain = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(29)))), ((int)(((byte)(35)))));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 632);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1101, 26);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(151, 20);
            this.toolStripStatus.Text = "toolStripStatusLabel1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstMain);
            this.splitContainer1.Size = new System.Drawing.Size(1101, 632);
            this.splitContainer1.SplitterDistance = 545;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dgvDisplay);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.btnEnd);
            this.splitContainer2.Panel2.Controls.Add(this.btnConnectAcl);
            this.splitContainer2.Panel2.Controls.Add(this.btnStart);
            this.splitContainer2.Panel2.Controls.Add(this.btnSetPreset);
            this.splitContainer2.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer2_Panel2_Paint);
            this.splitContainer2.Size = new System.Drawing.Size(545, 632);
            this.splitContainer2.SplitterDistance = 327;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // dgvDisplay
            // 
            this.dgvDisplay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDisplay.Location = new System.Drawing.Point(0, 0);
            this.dgvDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.dgvDisplay.Name = "dgvDisplay";
            this.dgvDisplay.RowHeadersWidth = 51;
            this.dgvDisplay.Size = new System.Drawing.Size(545, 327);
            this.dgvDisplay.TabIndex = 0;
            // 
            // btnEnd
            // 
            this.btnEnd.Location = new System.Drawing.Point(337, 44);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(127, 36);
            this.btnEnd.TabIndex = 3;
            this.btnEnd.Text = "End Transaction";
            this.btnEnd.UseVisualStyleBackColor = true;
            this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // btnConnectAcl
            // 
            this.btnConnectAcl.Location = new System.Drawing.Point(68, 115);
            this.btnConnectAcl.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnectAcl.Name = "btnConnectAcl";
            this.btnConnectAcl.Size = new System.Drawing.Size(189, 28);
            this.btnConnectAcl.TabIndex = 2;
            this.btnConnectAcl.Text = "Connect Accuload";
            this.btnConnectAcl.UseVisualStyleBackColor = true;
            this.btnConnectAcl.Click += new System.EventHandler(this.btnConnectAcl_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(203, 44);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(127, 36);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start Load";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnSetPreset
            // 
            this.btnSetPreset.Location = new System.Drawing.Point(68, 44);
            this.btnSetPreset.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetPreset.Name = "btnSetPreset";
            this.btnSetPreset.Size = new System.Drawing.Size(127, 36);
            this.btnSetPreset.TabIndex = 0;
            this.btnSetPreset.Text = "Set Preset";
            this.btnSetPreset.UseVisualStyleBackColor = true;
            this.btnSetPreset.Click += new System.EventHandler(this.btnSetPreset_Click);
            // 
            // lstMain
            // 
            this.lstMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(44)))), ((int)(((byte)(54)))));
            this.lstMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lstMain.ForeColor = System.Drawing.Color.Crimson;
            this.lstMain.FormattingEnabled = true;
            this.lstMain.HorizontalScrollbar = true;
            this.lstMain.ItemHeight = 20;
            this.lstMain.Location = new System.Drawing.Point(0, 0);
            this.lstMain.Margin = new System.Windows.Forms.Padding(4);
            this.lstMain.Name = "lstMain";
            this.lstMain.ScrollAlwaysVisible = true;
            this.lstMain.Size = new System.Drawing.Size(551, 632);
            this.lstMain.TabIndex = 6;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1101, 658);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmMain";
            this.Text = "FrmMain";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dgvDisplay;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnSetPreset;
        private System.Windows.Forms.ListBox lstMain;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnConnectAcl;
        private System.Windows.Forms.Button btnEnd;
    }
}

