using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VMware.Horizon.VirtualChannel.RDPVCBridgeInterop;
using static VMware.Horizon.VirtualChannel.PipeMessages.v1;

namespace VMware.Horizon.VirtualChannel.AgentAPI
{
    public class VirtualChannelAgent
    {

        public event ThreadMessageCallback LogMessage;
        public delegate void ThreadMessageCallback(int severity, string message);

        public event ChannelConnectedHandler ChannelConnectionChange;
        public delegate void ChannelConnectedHandler(bool Connected);

        public bool isClosing = false;

        public event ThreadExceptionHandler ObjectException;
        public delegate void ThreadExceptionHandler(Exception ex);

        public event SyncLocalVolumeHandler SyncLocalVolume;
        public delegate void SyncLocalVolumeHandler(VolumeStatus vs);

        private bool FirstProbe = true;
        public bool Connected = false;

        private bool HasRequestedLocalAudio = false;
        private System.Timers.Timer PulseTimer = null;

        public IntPtr Handle { get; set; }
        public object Lock { get; set; }
        public VirtualChannelAgent(string ChannelName)
        {
            Lock = new object();
            int sid = System.Diagnostics.Process.GetCurrentProcess().SessionId;
            Handle = RDPVCBridge.VDP_VirtualChannelOpen(RDPVCBridgeInterop.VirtualChannelStructures.WTS_CURRENT_SERVER_HANDLE, sid, ChannelName);
            if (Handle == IntPtr.Zero)
            {
                var er = Marshal.GetLastWin32Error();
                throw new Exception("Could not Open the virtual Channel: " + er);
            }

        }
        ~VirtualChannelAgent()
        {
            if (Handle != IntPtr.Zero)
            {
                try
                {
                    RDPVCBridge.VDP_VirtualChannelClose(Handle);
                }
                catch { }

                try
                {
                    PulseTimer.Stop();
                }
                catch { }
                try
                {
                    PulseTimer.Dispose();
                }

                catch { }
            }
        }

        private void InitializePulseTimer()
        {
            PulseTimer = new System.Timers.Timer
            {
                Enabled = true,
                Interval = 5000
            };
            PulseTimer.Start();
        }
        public void Destroy()
        {

            LogMessage?.Invoke(3, string.Format("Closing object."));
            RDPVCBridge.VDP_VirtualChannelClose(Handle);
            Handle = IntPtr.Zero;
            ChannelConnectionChange?.Invoke(false);
            PulseTimer.Stop();
            PulseTimer.Dispose();
            Connected = false;
        }

        private void ChangeConnectivity(bool _Connected)
        {
            if (Connected != _Connected)
            {
                Connected = _Connected;
                ChannelConnectionChange?.Invoke(Connected);
            }
        }

        public async Task<bool> Open()
        {
            InitializePulseTimer();
            PulseTimer.Elapsed += PulseTimer_Elapsed;

            ChannelResponse ChannelResponse = await Probe();
            if (ChannelResponse.Successful)
            {

                return true;
            }
            return false;
        }

        private async void PulseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            if ((await Probe()).Successful)
            {
                
                if (!HasRequestedLocalAudio)
                {
                    var LocalVolume = await GetClientVolume();
                    SyncLocalVolume?.Invoke(LocalVolume);
                    HasRequestedLocalAudio = true;
                }
            }
        }

        private async Task<object> SendMessage(ChannelCommand MessageObject, Type returnType)
        {
            return await Task.Run(() =>
            {
                LogMessage?.Invoke(3, string.Format("send requested, awaiting lock."));

                lock (Lock)
                {
                    LogMessage?.Invoke(3, string.Format("send requested, lock received."));

                    int written = 0;
                    string serialisedMessage = (JsonConvert.SerializeObject(MessageObject));
                    LogMessage?.Invoke(3, string.Format("Sending Message : {0}", serialisedMessage));
                    byte[] msg = BinaryConverters.StringToBinary(serialisedMessage);
                    bool SendResult = RDPVCBridge.VDP_VirtualChannelWrite(Handle, msg, msg.Length, ref written);
                    LogMessage?.Invoke(3, string.Format("Sending Message result: {0} - Written: {1}", SendResult, written));
                    if (!SendResult)
                    {
                        LogMessage?.Invoke(2, string.Format("Sending the command was not succesful"));
                        ChangeConnectivity(false);
                        return null;
                    }

                    byte[] buffer = new byte[10240];
                    int actualRead = 0;

                    bool ReceiveResult = RDPVCBridge.VDP_VirtualChannelRead(Handle, 5000, buffer, buffer.Length, ref actualRead);
                    LogMessage?.Invoke(3, string.Format("VDP_VirtualChannelRead result: {0} - ActualRead: {1}", ReceiveResult, actualRead));
                    if (!ReceiveResult)
                    {
                        ChangeConnectivity(false);
                        LogMessage?.Invoke(3, string.Format("Did not receive a response in a timely fashion or we received an error"));
                        return null;
                    }
                    byte[] receivedContents = new byte[actualRead];
                    Buffer.BlockCopy(buffer, 0, receivedContents, 0, actualRead);
                    string serialisedResponse = BinaryConverters.BinaryToString(receivedContents);
                    LogMessage?.Invoke(3, string.Format("Received: {0}", serialisedResponse));
                    return JsonConvert.DeserializeObject(serialisedResponse, (Type)returnType, (JsonSerializerSettings)null);
                }
            });
        }
        public async Task<ChannelResponse> Probe()
        {
            try
            {
                    object ProbeResponse = await SendMessage(new ChannelCommand(CommandType.Probe,null),typeof(ChannelResponse));
                    if (ProbeResponse != null)
                    {
                        ChannelResponse Response = (ChannelResponse)ProbeResponse;
                        if (Response.Successful)
                        {
                            ChangeConnectivity(true);                     
                        }
                        else
                        {
                            ChangeConnectivity(false);                           
                        }
                        return Response;
                    }
                    else
                    {
                        ChangeConnectivity(false);
                        LogMessage(1, "Receive Failed during probe");
                        return new ChannelResponse { Successful = false, Details = "Receive Failed" };
                    }               
            }
            catch(Exception ex)
            {
                LogMessage?.Invoke(1,string.Format( "Exception trapped in Probe: {0}", ex.ToString()));
                return new ChannelResponse {
                    Successful = false,
                    Details = ex.ToString(),
                };
                
            }
          
        }
        public async Task<ChannelResponse> SetVolume(VolumeStatus sv)
        {          
            if (Connected)
            {
                LogMessage?.Invoke(3, string.Format("SetVolume requested, connection open."));
                ChannelCommand cc = new ChannelCommand(CommandType.SetVolume, sv);
                return (ChannelResponse) await SendMessage(cc,typeof(ChannelResponse));
            }
            else
            {
                LogMessage?.Invoke(3, string.Format("SetVolume failed. Channel Closed."));
                ChangeConnectivity(false);
                return null;
            }

        }
        public async Task<VolumeStatus> GetClientVolume()
        {

            if (Connected)
            {
                LogMessage?.Invoke(3, string.Format("GetVolume requested, connnection open."));
                LogMessage?.Invoke(3, string.Format("Getting Volume Status"));
                return (VolumeStatus)await SendMessage(new ChannelCommand(CommandType.GetVolume, null),typeof(VolumeStatus));
            }
            else
            {
                LogMessage?.Invoke(3, string.Format("GetVolume failed. Channel Closed."));
                ChangeConnectivity(false);
                return null;
            }

        }
    }
}
