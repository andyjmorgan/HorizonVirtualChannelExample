using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMware.Horizon.VirtualChannel.Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!RegistryHelpers.ClientHelpers.IsAgentInstalled())
            {
                MessageBox.Show("This app cannot function without a horizon client", "Horizon Client Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmDetails());
            }
            
        }
    }
}
