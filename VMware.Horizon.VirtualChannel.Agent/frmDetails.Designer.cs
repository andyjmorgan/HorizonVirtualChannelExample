namespace VMware.Horizon.VirtualChannel.Agent
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
            this.lbDetails = new System.Windows.Forms.ListBox();
            this.niAgent = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFollowTail = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSyncVolume = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHide = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsMain.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbDetails
            // 
            this.lbDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDetails.FormattingEnabled = true;
            this.lbDetails.Location = new System.Drawing.Point(0, 24);
            this.lbDetails.Name = "lbDetails";
            this.lbDetails.Size = new System.Drawing.Size(610, 294);
            this.lbDetails.TabIndex = 0;
            // 
            // niAgent
            // 
            this.niAgent.ContextMenuStrip = this.cmsMain;
            this.niAgent.Icon = ((System.Drawing.Icon)(resources.GetObject("niAgent.Icon")));
            this.niAgent.Text = "Pipe Client";
            this.niAgent.Visible = true;
            this.niAgent.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.niClient_MouseDoubleClick);
            // 
            // cmsMain
            // 
            this.cmsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExit});
            this.cmsMain.Name = "cmsMain";
            this.cmsMain.Size = new System.Drawing.Size(94, 26);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(93, 22);
            this.tsmiExit.Text = "Exit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(610, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmiMenu
            // 
            this.tsmiMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFollowTail,
            this.tsmiSyncVolume,
            this.toolStripSeparator1,
            this.tsmiHide});
            this.tsmiMenu.Name = "tsmiMenu";
            this.tsmiMenu.Size = new System.Drawing.Size(50, 20);
            this.tsmiMenu.Text = "Menu";
            // 
            // tsmiFollowTail
            // 
            this.tsmiFollowTail.Checked = true;
            this.tsmiFollowTail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiFollowTail.Name = "tsmiFollowTail";
            this.tsmiFollowTail.Size = new System.Drawing.Size(142, 22);
            this.tsmiFollowTail.Text = "Follow Tail";
            this.tsmiFollowTail.Click += new System.EventHandler(this.tsmiFollowTail_Click);
            // 
            // tsmiSyncVolume
            // 
            this.tsmiSyncVolume.Checked = true;
            this.tsmiSyncVolume.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiSyncVolume.Name = "tsmiSyncVolume";
            this.tsmiSyncVolume.Size = new System.Drawing.Size(142, 22);
            this.tsmiSyncVolume.Text = "Sync Volume";
            this.tsmiSyncVolume.Click += new System.EventHandler(this.tsmiSyncVolume_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(139, 6);
            // 
            // tsmiHide
            // 
            this.tsmiHide.Name = "tsmiHide";
            this.tsmiHide.Size = new System.Drawing.Size(142, 22);
            this.tsmiHide.Text = "Hide";
            this.tsmiHide.Click += new System.EventHandler(this.tsmiHide_Click);
            // 
            // frmDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 318);
            this.Controls.Add(this.lbDetails);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDetails";
            this.Text = "Pipe Agent Details:";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDetails_FormClosing);
            this.Load += new System.EventHandler(this.frmDetails_Load);
            this.Shown += new System.EventHandler(this.frmDetails_Shown);
            this.cmsMain.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbDetails;
        private System.Windows.Forms.NotifyIcon niAgent;
        private System.Windows.Forms.ContextMenuStrip cmsMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiFollowTail;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiHide;
        private System.Windows.Forms.ToolStripMenuItem tsmiSyncVolume;
    }
}

