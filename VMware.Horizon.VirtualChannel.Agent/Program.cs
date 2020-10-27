using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMware.Horizon.VirtualChannel.RegistryHelpers;
namespace VMware.Horizon.VirtualChannel.Agent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!AgentHelpers.IsAgentInstalled())
            {
                MessageBox.Show("This app cannot function without a horizon agent", "Horizon Agent Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
