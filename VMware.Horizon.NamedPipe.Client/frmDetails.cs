using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMware.Horizon.VirtualChannel.Client
{
    public partial class frmDetails : Form
    {
        public frmDetails()
        {
            InitializeComponent();
        }

        private void frmDetails_Load(object sender, EventArgs e)
        {
            Runtime.HorizonMonitor = new HorizonMonitor();
            Runtime.HorizonMonitor.ThreadMessage += HorizonMontor_ThreadMessage;
            if (Runtime.HorizonMonitor.Initialise())
            {

                System.Threading.Thread MonitorThread = new Thread(Runtime.HorizonMonitor.Start);
                MonitorThread.Start();
                
            }
            else
            {
                MessageBox.Show("Failed to register horizon client", "Failed to register Horizon client", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void HorizonMontor_ThreadMessage(int severity, string message)
        {
            string Entry = string.Format("Sev: {0} - Message: {1}", severity, message);
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke((Action<int, string>)HorizonMontor_ThreadMessage, severity, message);
            }
            else
            {
                listBox1.Items.Add(message);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }

        private void niClient_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
           
        }

        private void frmDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            Runtime.HorizonMonitor.isClosing = true;
        }

        private void frmDetails_Shown(object sender, EventArgs e)
        {
            Visible = false;

        }

        private void tsmiDetails_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }
    }
}
