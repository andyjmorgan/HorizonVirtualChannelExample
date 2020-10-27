using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace VMware.Horizon.VirtualChannel.RegistryHelpers
{
    public class ClientHelpers
    {
        public static string ClientPath = @"SOFTWARE\VMware, Inc.\VMware VDM\Client";
        public static bool IsAgentInstalled()
        {
            try
            {
                using (RegistryKey MachineHive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {                  
                    using (RegistryKey ClientKey = MachineHive.OpenSubKey(ClientPath))
                    {
                        if (ClientKey != null)
                        {
                            var ClientVersion = ClientKey.GetValue("Version", null);
                            if (ClientVersion != null)
                            {
                                NLog.LogManager.GetCurrentClassLogger().Info("Client Version: {0}", ClientVersion);
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
