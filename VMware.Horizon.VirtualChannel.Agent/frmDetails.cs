using Microsoft.Win32;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMware.Horizon.VirtualChannel.AgentAPI;
using static VMware.Horizon.VirtualChannel.PipeMessages.v1;

namespace VMware.Horizon.VirtualChannel.Agent
{
    public partial class frmDetails : Form
    {


        VirtualChannelAgent VCA = null;

        MMDeviceEnumerator en = null;
        MMDevice device = null;



        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            AgentThread_ThreadMessage(3, string.Format("audio Changed, Muted: {0}, Volume: {1}", data.Muted, data.MasterVolume));
            if (tsmiSyncVolume.Checked)
            {
                VolumeStatus sv = new VolumeStatus(data.Muted, data.MasterVolume);
                if (VCA.Connected)
                {

                    VCA.SetVolume(sv);
                }
            }        
        }

        public void OpenAudio()
        {
            try
            {
                AgentThread_ThreadMessage(3, "Mapping audio components");
                en = new MMDeviceEnumerator();
                device = en.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Console);
                AgentThread_ThreadMessage(3, "adding event for audio components");
                AgentThread_ThreadMessage(3, "GC KeepAlive audio components");
                GC.SuppressFinalize(device);
                GC.SuppressFinalize(en);
                device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            }
            catch (Exception ex)
            {
                if (en != null)
                {
                    try
                    {
                        en.Dispose();
                    }
                    catch { }
                }

                if (device != null)
                {
                    try
                    {
                        device.Dispose();
                    }
                    catch { }

                }
                AgentThread_ThreadMessage(1, string.Format("Failed to map Audio Device: {0}", ex.ToString()));
            }
        }

        public void CloseAudio()
        {
            try
            {
                device.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
                en.Dispose();
                device.Dispose();
                GC.ReRegisterForFinalize(device);
                GC.ReRegisterForFinalize(en);
            }
            catch { }
            
        }


        public frmDetails()
        {
            InitializeComponent();
        }


        private void OpenAgent()
        {
            lbDetails.Items.Add("Registering Agent Thread");
            VCA = new VirtualChannelAgent("VVCAM");
            VCA.LogMessage += AgentThread_ThreadMessage;
            VCA.ObjectException += AgentThread_ThreadException;
            VCA.ChannelConnectionChange += VCA_ChannelConnectionChange;
            VCA.SyncLocalVolume += VCA_SyncLocalVolume;
            var Result = VCA.Open();
            AgentThread_ThreadMessage(3,string.Format( "Agent Open Response: {0}", Result));
            
        }
        private void VCA_SyncLocalVolume(VolumeStatus vs)
        {
            if(en!= null)
            {
                if(device != null)
                {
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = vs.VolumeLevel;
                    device.AudioEndpointVolume.Mute = vs.Muted;
                }
            }
        }

        private void CloseAgent()
        {
            
            lbDetails.Items.Add("Unregistering Agent Thread");
            if (VCA != null)
            {
                VCA.LogMessage -= AgentThread_ThreadMessage;
                VCA.ObjectException -= AgentThread_ThreadException;
                VCA.ChannelConnectionChange -= VCA_ChannelConnectionChange;
                VCA.SyncLocalVolume -= VCA_SyncLocalVolume;
                VCA.Destroy();
                VCA = null;
            }
            
        }

        private void VCA_ChannelConnectionChange(bool Connected)
        {
            AgentThread_ThreadMessage(3, string.Format("Connectivity Change: {0}", Connected));
            niAgent.Text = string.Format("Pipe Agent - Connected: {0}", Connected);
        }

        private void frmDetails_Load(object sender, EventArgs e)
        {
            if (!RDPVCBridgeInterop.RDPVCBridge.VDP_IsViewSession((uint)Process.GetCurrentProcess().SessionId))
            {
                MessageBox.Show("This is not a Horizon Session, closing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            else
            {
                SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
                OpenAudio();
                try
                {
                    OpenAgent();

                }
                catch(Exception ex)
                {
                    MessageBox.Show(string.Format("Could not open virtual channel: {0}", ex.ToString()), "Failed to start", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                }
            }       
        }

        private async void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            lbDetails.Items.Add(string.Format("Session Change: {0}", e.Reason.ToString()));
            switch (e.Reason)
            {
                case SessionSwitchReason.ConsoleDisconnect:
                case SessionSwitchReason.RemoteDisconnect:
                    CloseAgent();
                    CloseAudio();
                    break;
                case SessionSwitchReason.RemoteConnect:
                case SessionSwitchReason.ConsoleConnect:                    
                    var isView = RDPVCBridgeInterop.RDPVCBridge.VDP_IsViewSession((uint)Process.GetCurrentProcess().SessionId);
                    lbDetails.Items.Add(string.Format("IsViewSession: {0}", isView));
                    if (isView)
                    {
                        /// giving the audio component a chance to catchup
                        await Task.Run(() =>
                        {
                            Task.Delay(3000);
                        });
                        OpenAudio();
                        OpenAgent();                     
                    }
                    break;
                default:
                    break;
            }           
        }


        private void FollowListBoxTail()
        {
            if (lbDetails.InvokeRequired)
            {
                lbDetails.Invoke((Action)FollowListBoxTail);
            }
            else
            {
                if (tsmiFollowTail.Checked)
                {
                    lbDetails.SelectedIndex = lbDetails.Items.Count - 1;
                }
            }
        }

        private void AgentThread_ThreadException(Exception ex)
        {
            if (lbDetails.InvokeRequired)
            {
                lbDetails.Invoke((Action<Exception>)AgentThread_ThreadException, ex);
            }
            else
            {
                NLog.LogManager.GetCurrentClassLogger().Error("{0}", ex.ToString());
                lbDetails.Items.Add(ex.ToString());
                FollowListBoxTail();
            }
        }

        private void AgentThread_ThreadMessage(int severity, string message)
        {
            string Entry = string.Format("Sev: {0} - Message: {1}", severity, message);
            if (lbDetails.InvokeRequired)
            {
                lbDetails.Invoke((Action<int, string>)AgentThread_ThreadMessage, severity, message);
            }
            else
            {
                NLog.LogManager.GetCurrentClassLogger().Info("{0} - {1}", severity, message);
                lbDetails.Items.Add(Entry);
                while(lbDetails.Items.Count > 100)
                {
                    lbDetails.Items.RemoveAt(0);
                }
                FollowListBoxTail();
            }
        }

        private void frmDetails_FormClosing(object sender, FormClosingEventArgs e)
        {

            CloseAgent();
            CloseAudio();
        }

        private void tsmiDetails_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        private void niClient_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
        }

        private void frmDetails_Shown(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void tsmiFollowTail_Click(object sender, EventArgs e)
        {
            tsmiFollowTail.Checked = !tsmiFollowTail.Checked;
        }

        private void tsmiHide_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void tsmiSyncVolume_Click(object sender, EventArgs e)
        {
            tsmiSyncVolume.Checked = !tsmiSyncVolume.Checked;
        }
    }
}
