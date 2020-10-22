namespace VMware.Horizon.VirtualChannel.Client
{
    partial class frmDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDetails));
            this.scDetails = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.niClient = new System.Windows.Forms.NotifyIcon(this.components);
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.scDetails)).BeginInit();
            this.scDetails.Panel1.SuspendLayout();
            this.scDetails.Panel2.SuspendLayout();
            this.scDetails.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.cmsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // scDetails
            // 
            this.scDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scDetails.Location = new System.Drawing.Point(0, 0);
            this.scDetails.Name = "scDetails";
            this.scDetails.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scDetails.Panel1
            // 
            this.scDetails.Panel1.Controls.Add(this.listView1);
            // 
            // scDetails.Panel2
            // 
            this.scDetails.Panel2.Controls.Add(this.groupBox1);
            this.scDetails.Size = new System.Drawing.Size(1231, 596);
            this.scDetails.SplitterDistance = 103;
            this.scDetails.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1231, 489);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Debug Details:";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 16);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1225, 470);
            this.listBox1.TabIndex = 0;
            // 
            // cmsMain
            // 
            this.cmsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDetails,
            this.toolStripSeparator1,
            this.tsmiExit});
            this.cmsMain.Name = "cmsMain";
            this.cmsMain.Size = new System.Drawing.Size(113, 54);
            // 
            // tsmiDetails
            // 
            this.tsmiDetails.Name = "tsmiDetails";
            this.tsmiDetails.Size = new System.Drawing.Size(180, 22);
            this.tsmiDetails.Text = "Details:";
            this.tsmiDetails.Click += new System.EventHandler(this.tsmiDetails_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(180, 22);
            this.tsmiExit.Text = "Exit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // niClient
            // 
            this.niClient.ContextMenuStrip = this.cmsMain;
            this.niClient.Icon = ((System.Drawing.Icon)(resources.GetObject("niClient.Icon")));
            this.niClient.Text = "Pipe Client";
            this.niClient.Visible = true;
            this.niClient.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.niClient_MouseDoubleClick);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2,
            this.columnHeader4});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1231, 103);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Server ID:";
            this.columnHeader1.Width = 72;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Resource:";
            this.columnHeader2.Width = 233;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Server Name:";
            this.columnHeader3.Width = 333;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Token:";
            this.columnHeader4.Width = 583;
            // 
            // frmDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1231, 596);
            this.Controls.Add(this.scDetails);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDetails";
            this.Text = "Pipe Client Details:";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDetails_FormClosing);
            this.Load += new System.EventHandler(this.frmDetails_Load);
            this.Shown += new System.EventHandler(this.frmDetails_Shown);
            this.scDetails.Panel1.ResumeLayout(false);
            this.scDetails.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDetails)).EndInit();
            this.scDetails.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.cmsMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scDetails;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ContextMenuStrip cmsMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiDetails;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.NotifyIcon niClient;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}