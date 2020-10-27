using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMware.Horizon.VirtualChannel.RegistryHelpers
{
    public class AgentHelpers
    {
        public static string AgentPath = @"SOFTWARE\VMware, Inc.\VMware VDM";

        public static bool IsAgentInstalled()
        {
            try
            {
                using (RegistryKey MachineHive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (RegistryKey AgentKey = MachineHive.OpenSubKey(AgentPath))
                    {
                        if (AgentKey != null)
                        {
                            var AgentVersion = AgentKey.GetValue("ProductVersion", null);
                            if (AgentVersion != null)
                            {
                                NLog.LogManager.GetCurrentClassLogger().Info("Agent Version: {0}", AgentVersion);
                                return true;
                            }
                        }
                        NLog.LogManager.GetCurrentClassLogger().Error("VMware Horizon Client not detected");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("Failed to validate Horizon Client installation: {0}", ex.ToString());
                return false;
            }

        }
      
    }
}
