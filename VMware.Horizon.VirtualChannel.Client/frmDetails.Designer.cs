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
            this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.niClient = new System.Windows.Forms.NotifyIcon(this.components);
            this.bwAgent = new System.ComponentModel.BackgroundWorker();
            this.lbDebug = new System.Windows.Forms.ListBox();
            this.cmsMain.SuspendLayout();
            this.SuspendLayout();
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
            // niClient
            // 
            this.niClient.ContextMenuStrip = this.cmsMain;
            this.niClient.Icon = ((System.Drawing.Icon)(resources.GetObject("niClient.Icon")));
            this.niClient.Text = "Pipe Client";
            this.niClient.Visible = true;
            this.niClient.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.niClient_MouseDoubleClick);
            // 
            // lbDebug
            // 
            this.lbDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDebug.FormattingEnabled = true;
            this.lbDebug.Location = new System.Drawing.Point(0, 0);
            this.lbDebug.Name = "lbDebug";
            this.lbDebug.Size = new System.Drawing.Size(1231, 596);
            this.lbDebug.TabIndex = 1;
            // 
            // frmDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1231, 596);
            this.Controls.Add(this.lbDebug);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDetails";
            this.Text = "Virtual Channel Client Details:";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDetails_FormClosing);
            this.Load += new System.EventHandler(this.frmDetails_Load);
            this.Shown += new System.EventHandler(this.frmDetails_Shown);
            this.cmsMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip cmsMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.NotifyIcon niClient;
        private System.ComponentModel.BackgroundWorker bwAgent;
        private System.Windows.Forms.ListBox lbDebug;
    }
}