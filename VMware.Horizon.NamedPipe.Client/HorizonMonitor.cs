using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMware.Horizon.VirtualChannel.PipeMessages.v1;
using NAudio.CoreAudioApi;
using Newtonsoft.Json.Linq;
using VMwareHorizonClientController;
using System.ServiceModel.Dispatcher;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Converters;

namespace VMware.Horizon.VirtualChannel.Client
{
    public class HorizonMonitor
    {
        private BatteryStatus GetBatteryStatus()
        {
            System.Windows.Forms.BatteryChargeStatus bcs = System.Windows.Forms.SystemInformation.PowerStatus.BatteryChargeStatus;
            if (bcs == System.Windows.Forms.BatteryChargeStatus.NoSystemBattery || bcs == System.Windows.Forms.BatteryChargeStatus.Unknown)
            {
                return new BatteryStatus(false, false, 0);

            }
            else if (bcs == System.Windows.Forms.BatteryChargeStatus.Charging)
            {
                return new BatteryStatus(true, true, System.Windows.Forms.SystemInformation.PowerStatus.BatteryLifePercent);
            }
            else
            {
                return new BatteryStatus(true, false, System.Windows.Forms.SystemInformation.PowerStatus.BatteryLifePercent);
            }
        }
        private void SetVolumeStatus(SetVolume sv)
        {
            using (NAudio.CoreAudioApi.MMDeviceEnumerator en = new NAudio.CoreAudioApi.MMDeviceEnumerator())
            {

                var device = en.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Console);
                device.AudioEndpointVolume.Mute = sv.Muted;
                device.AudioEndpointVolume.MasterVolumeLevelScalar = sv.VolumeLevel;
            }
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            if (data.Muted)
            {
                this.ThreadMessage?.Invoke(3, "Volume Muted");
            }
            else
            {
                this.ThreadMessage?.Invoke(3, "Volume changed to: " + data.MasterVolume.ToString());
            }
        }


        public event ThreadMessageCallback ThreadMessage;
        public delegate void ThreadMessageCallback(int severity, string message);

        public bool isClosing = false;
        public event ThreadExceptionHandler ThreadException;
        public delegate void ThreadExceptionHandler(Exception ex);

        public string BatteryStatus = "";

        MMDeviceEnumerator en = null;
        MMDevice device = null;

        //private IVMwareHorizonClientVChan VMwareHorizonVirtualChannelAPI = null;

        private IVMwareHorizonClient4 vmhc = null;
        VMwareHorizonVirtualChannelEvents ChannelEvents = null;

        public bool Initialise()
        {


            /// Open Audio Callbacks
            /// 
            en = new NAudio.CoreAudioApi.MMDeviceEnumerator();
            device = en.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Console);
            device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            GC.SuppressFinalize(en);
            GC.SuppressFinalize(device);
            ThreadMessage?.Invoke(3, "Opened Audio API");


            /// Open Horizon Client Listener
            /// 
            vmhc = (IVMwareHorizonClient4)new VMwareHorizonClient();
            IVMwareHorizonClientEvents5 HorizonEvents = (IVMwareHorizonClientEvents5)new VMwareHorizonClientEvents(this);
            vmhc.Advise2(HorizonEvents, VmwHorizonClientAdviseFlags.VmwHorizonClientAdvise_DispatchCallbacksOnUIThread);
            GC.SuppressFinalize(vmhc);
            ThreadMessage?.Invoke(3, "Opened Horizon API");

            /// Register Virtual Channel Callback
            /// 
            ChannelEvents = new VMwareHorizonVirtualChannelEvents(this);
            object APIObject = null;
            VMwareHorizonClientChannelDefinition[] Channels = new VMwareHorizonClientChannelDefinition[1];
            Channels[0] = new VMwareHorizonClientChannelDefinition("ExampleChannel", 0);
            vmhc.RegisterVirtualChannelConsumer2(Channels, ChannelEvents, out APIObject);
            ChannelEvents.HorizonClientVirtualChannel = (IVMwareHorizonClientVChan)APIObject;
            GC.SuppressFinalize(ChannelEvents);
            ThreadMessage.Invoke(3, "Opened Virtual Channel Listener");
            return true;
        }

       
        public void Start()
        {

            try
            {
                while (!isClosing)
                {
                    System.Threading.Thread.Sleep(500);
                }

                en.Dispose();
                device.Dispose();

                GC.ReRegisterForFinalize(ChannelEvents);
                GC.ReRegisterForFinalize(en);
                GC.ReRegisterForFinalize(device);
                GC.ReRegisterForFinalize(vmhc);
            }
            catch(Exception ex)
            {
                this.ThreadMessage?.Invoke(1, string.Format("The Horizon Monitor thread reported a fatal Exception: {0}", ex.ToString()));
                this.ThreadException?.Invoke(ex);
            }        
        }

        public void Close()
        {
            this.isClosing = true;
        }


        public class VMwareHorizonClientChannelDefinition : IVMwareHorizonClientChannelDef
        {
            public VMwareHorizonClientChannelDefinition(string name, uint options)
            {
                mName = name;
                mOptions = options;
            }
            public string name { get { return mName; } }
            public uint options { get { return mOptions; } }

            private string mName;
            private uint mOptions;
        }

        public class VMwareHorizonVirtualChannelEvents : IVMwareHorizonClientVChanEvents
        {

            public IVMwareHorizonClientVChan HorizonClientVirtualChannel = null;
            private HorizonMonitor CallbackObject;

            public VMwareHorizonVirtualChannelEvents(HorizonMonitor callback)
            {
                CallbackObject = callback;
            }
            public void ConnectEventProc(uint serverId, string sessionToken, uint eventType, Array eventData)
            {
                RDPVCBridgeInterop.VirtualChannelStructures.ChannelEvents currentEventType = (RDPVCBridgeInterop.VirtualChannelStructures.ChannelEvents)eventType;
                CallbackObject.ThreadMessage?.Invoke(3, "ConnectEventProc() called: " + currentEventType.ToString());
                //  SharedObjects.hvm.ThreadMessage?.Invoke(3, "ConnectEventProc() called ");

                if (eventType == (uint)RDPVCBridgeInterop.VirtualChannelStructures.ChannelEvents.Connected)

                {
                    try
                    {
                        HorizonClientVirtualChannel.VirtualChannelOpen(serverId, sessionToken, "VVCAM", out mChannelHandle);
                        CallbackObject.ThreadMessage?.Invoke(3, "!! VirtualChannelOpen() succeeded");
                    }
                    catch (Exception ex)
                    {
                        CallbackObject.ThreadMessage?.Invoke(3, string.Format( "VirtualChannelOpen() failed: {0}", ex.ToString()));
                        mChannelHandle = 0;
                    }
                }
            }
            public void InitEventProc(uint serverId, string sessionToken, uint rc)
            {
                CallbackObject.ThreadMessage?.Invoke(3, "InitEventProc()");
            }
            public void ReadEventProc(uint serverId, string sessionToken, uint channelHandle, uint eventType, Array eventData, uint totalLength, uint dataFlags)
            {
                RDPVCBridgeInterop.VirtualChannelStructures.ChannelEvents currentEventType = (RDPVCBridgeInterop.VirtualChannelStructures.ChannelEvents)eventType;
                RDPVCBridgeInterop.VirtualChannelStructures.ChannelFlags cf = (RDPVCBridgeInterop.VirtualChannelStructures.ChannelFlags)dataFlags;
                CallbackObject.ThreadMessage?.Invoke(3, "ReadEventProc(): " + currentEventType.ToString() + " - Flags: " + cf.ToString() + " - Length: " + totalLength);

                bool isFirst = (dataFlags & (uint)RDPVCBridgeInterop.VirtualChannelStructures.ChannelFlags.First) != 0;
                bool isLast = (dataFlags & (uint)RDPVCBridgeInterop.VirtualChannelStructures.ChannelFlags.Last) != 0;

                if (isFirst)
                {
                    mPingTestMsg = new Byte[totalLength];
                    mPingTestCurLen = 0;
                }
                eventData.CopyTo(mPingTestMsg, mPingTestCurLen);
                mPingTestCurLen += eventData.Length;

                if (isLast)
                {
                    //if (totalLength != mPingTestMsg.Length)
                    //{
                    //    SharedObjects.hvm.ThreadMessage?.Invoke(3, "Received {mPingTestMsg.Length} bytes but expected {totalLength} bytes!");
                    //}

                    //string message = RDPVCBridgeInterop.BinaryConverters.BinaryToString(mPingTestMsg);
                    //ChannelObjects.ChannelCommand cc = Newtonsoft.Json.JsonConvert.DeserializeObject<ChannelObjects.ChannelCommand>(message);
                    //SharedObjects.hvm.ThreadMessage?.Invoke(3, "Received: " + cc.CommandType.ToString() + " = " + RDPVCBridgeInterop.BinaryConverters.BinaryToString(mPingTestMsg));

                    //try
                    //{
                    //    switch (cc.CommandType)
                    //    {
                    //        case ChannelObjects.CommandType.SetVolume:
                    //            JObject jo = (JObject)cc.CommandParameters;
                    //            ChannelObjects.SetVolume sv = jo.ToObject<ChannelObjects.SetVolume>();
                    //            SharedObjects.hvm.SetVolumeStatus(sv);
                    //            g_vchanApi.VirtualChannelWrite(serverId, sessionToken, channelHandle, RDPVCBridgeInterop.BinaryConverters.StringToBinary(Newtonsoft.Json.JsonConvert.SerializeObject(new ChannelObjects.ChannelResponse())));
                    //            break;
                    //        case ChannelObjects.CommandType.Probe:
                    //            g_vchanApi.VirtualChannelWrite(serverId, sessionToken, channelHandle, RDPVCBridgeInterop.BinaryConverters.StringToBinary(Newtonsoft.Json.JsonConvert.SerializeObject(new ChannelObjects.ChannelResponse())));
                    //            break;
                    //        case ChannelObjects.CommandType.GetBattery:
                    //            g_vchanApi.VirtualChannelWrite(serverId, sessionToken, channelHandle, RDPVCBridgeInterop.BinaryConverters.StringToBinary(Newtonsoft.Json.JsonConvert.SerializeObject(SharedObjects.hvm.GetBatteryStatus())));
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    CallbackObject.ThreadMessage?.Invoke(3, string.Format("VirtualChannelWrite failed: {0}", ex.ToString()));
                    //}
                }
            }

            uint mChannelHandle = 0;
            Byte[] mPingTestMsg = null;
            int mPingTestCurLen = 0;
            Byte[] mServerPingFragment = new byte[] { 0x50 /* 'P' */, 0x69 /* 'i' */, 0x6E /* 'n' */, 0x67 /* 'g' */ };
            Byte[] mClientPingFragment = new byte[] { 0x50 /* 'P' */, 0x6F /* 'o' */, 0x6E /* 'n' */, 0x67 /* 'g' */ };
        }
        public class VMwareHorizonClientEvents : IVMwareHorizonClientEvents5
        {

            public class Helpers
            {
                [Flags]
                public enum SupportedProtocols
                {
                    VmwHorizonClientProtocol_Default = 0,
                    VmwHorizonClientProtocol_RDP = 1,
                    VmwHorizonClientProtocol_PCoIP = 2,
                    VmwHorizonClientProtocol_Blast = 4
                }
                [Flags]
                public enum LaunchItemType
                {
                    VmwHorizonLaunchItem_HorizonDesktop = 0,
                    VmwHorizonLaunchItem_HorizonApp = 1,
                    VmwHorizonLaunchItem_XenApp = 2,
                    VmwHorizonLaunchItem_SaaSApp = 3,
                    VmwHorizonLaunchItem_HorizonAppSession = 4,
                    VmwHorizonLaunchItem_DesktopShadowSession = 5,
                    VmwHorizonLaunchItem_AppShadowSession = 6
                }

                public class launchItem
                {
                    public string name { get; set; }

                    public string id { get; set; }

                    [JsonConverter(typeof(StringEnumConverter))]
                    public LaunchItemType type { get; set; }

                    [JsonConverter(typeof(StringEnumConverter))]
                    public SupportedProtocols supportedProtocols { get; set; }
                    [JsonConverter(typeof(StringEnumConverter))]
                    public VmwHorizonClientProtocol defaultProtocol { get; set; }
                    public launchItem(IVMwareHorizonClientLaunchItemInfo item)
                    {
                        name = item.name;
                        id = item.id;
                        type = (LaunchItemType)item.type;
                        supportedProtocols = (SupportedProtocols)item.supportedProtocols;
                        defaultProtocol = item.defaultProtocol;
                    }
                }
                public class LaunchItem2
                {
                    public string name { get; set; }

                    public string id { get; set; }

                    [JsonConverter(typeof(StringEnumConverter))]
                    public LaunchItemType type { get; set; }

                    [JsonConverter(typeof(StringEnumConverter))]
                    public SupportedProtocols supportedProtocols { get; set; }

                    [JsonConverter(typeof(StringEnumConverter))]
                    public VmwHorizonClientProtocol defaultProtocol { get; set; }

                    public uint hasRemotableAssets { get; set; }
                    public LaunchItem2(IVMwareHorizonClientLaunchItemInfo2 i)
                    {
                        name = i.name;
                        id = i.id;
                        type = (LaunchItemType)i.type;
                        supportedProtocols = (SupportedProtocols)i.supportedProtocols;
                        defaultProtocol = i.defaultProtocol;
                        hasRemotableAssets = i.hasRemotableAssets;
                    }
                }


                public static List<launchItem> GetLaunchItems(Array ItemList)
                {
                    List<launchItem> returnList = new List<launchItem>();
                    foreach (var item in ItemList)
                    {
                        returnList.Add(new launchItem((IVMwareHorizonClientLaunchItemInfo)item));
                    }
                    return returnList;
                }

                public static List<LaunchItem2> GetLaunchItems2(Array ItemList)
                {
                    List<LaunchItem2> returnList = new List<LaunchItem2>();
                    foreach (var item in ItemList)
                    {
                        returnList.Add(new LaunchItem2((IVMwareHorizonClientLaunchItemInfo2)item));
                    }
                    return returnList;
                }
            }

            private HorizonMonitor CallbackObject;

            public VMwareHorizonClientEvents(HorizonMonitor callback)
            {
                CallbackObject = callback;
            }

            private void DispatchMessage(int severity, string message)
            {
                CallbackObject.ThreadMessage?.Invoke(severity, message);
            }

            private string SerialiseObject(object Value)
            {
                return JsonConvert.SerializeObject(Value, Formatting.Indented);
            }

            public void OnStarted()
            {
                DispatchMessage(3, "Started Called");
            }

            public void OnExit()
            {
                DispatchMessage(3, "Exit Called");
            }

            public void OnConnecting(object serverInfo)
            {
                IVMwareHorizonClientServerInfo Info = (IVMwareHorizonClientServerInfo)serverInfo;
                DispatchMessage(3, string.Format("Connecting, Server Address: {0}, ID: {1}, Type:{2} ",
                   Info.serverAddress, Info.serverId, Info.serverType.ToString()));
            }

            public void OnConnectFailed(uint serverId, string errorMessage)
            {
                DispatchMessage(3, string.Format("Connect Failed, Server ID: {0}, Message: {1}",
                            serverId, errorMessage));
            }

            public void OnAuthenticationRequested(uint serverId, VmwHorizonClientAuthType authType)
            {
                DispatchMessage(3, string.Format("Authentication Requested, Server ID: {0}, AuthType: {1}",
                           serverId, authType.ToString()));
            }

            public void OnAuthenticating(uint serverId, VmwHorizonClientAuthType authType, string user)
            {
                DispatchMessage(3, string.Format("Authenticating, Server ID: {0}, AuthType: {1}, User: {2}",
                            serverId, authType.ToString(), user));
            }

            public void OnAuthenticationDeclined(uint serverId, VmwHorizonClientAuthType authType)
            {
                DispatchMessage(3, string.Format("Authentication Declined, Server ID: {0}, AuthType: {1}",
                           serverId, authType.ToString()));
            }

            public void OnAuthenticationFailed(uint serverId, VmwHorizonClientAuthType authType, string errorMessage, int retryAllowed)
            {
                DispatchMessage(3, string.Format("Authentication Failed, Server ID: {0}, AuthType: {1}, Error: {2}, retry allowed?: {3}",
                            serverId, authType.ToString(), errorMessage, retryAllowed));
            }

            public void OnLoggedIn(uint serverId)
            {
                DispatchMessage(3, string.Format("Logged In, Server ID: {0}", serverId));
            }

            public void OnDisconnected(uint serverId)
            {
                DispatchMessage(3, string.Format("Disconnected, Server ID: {0}", serverId));
            }

            public void OnReceivedLaunchItems(uint serverId, Array launchItems)
            {
                DispatchMessage(3, string.Format("Received Launch Items, Server ID: {0}, Item Count: {1}", serverId,
                            launchItems.Length));
                var Items = Helpers.GetLaunchItems(launchItems);
                foreach(var item in Items)
                {
                    DispatchMessage(3, String.Format("Launch Item: Server ID: {0}, Name: {1}, Type: {2}, ID: {3}", serverId, item.name, item.type.ToString(), item.id));

                }
            }

            public void OnLaunchingItem(uint serverId, VmwHorizonLaunchItemType type, string launchItemId, VmwHorizonClientProtocol protocol)
            {
                DispatchMessage(3, string.Format("Launching Item, Server ID: {0}, type: {1}, Item ID: {2}, Protocol: {3}", serverId,
                           type.ToString(), launchItemId, protocol.ToString()));
            }

            public void OnItemLaunchSucceeded(uint serverId, VmwHorizonLaunchItemType type, string launchItemId)
            {
                DispatchMessage(3, string.Format("Launch Item Succeeded, Server ID: {0}, Type: {1}, ID: {2}", serverId, type.ToString(), launchItemId));
            }

            public void OnItemLaunchFailed(uint serverId, VmwHorizonLaunchItemType type, string launchItemId, string errorMessage)
            {
                DispatchMessage(3, string.Format("Launch Item Succeeded, Server ID: {0}, type: {1}, Item ID: {2}", serverId,
                            type.ToString(), launchItemId));
            }

            public void OnNewProtocolSessionCreated(uint serverId, string sessionToken, VmwHorizonClientProtocol protocol, VmwHorizonClientSessionType type, string clientId)
            {
                DispatchMessage(3, string.Format("New Protocol Session Created, Server ID: {0}, Token: {1}, Protocol: {2}, Type: {3}, ClientID: {4}",
                           serverId, sessionToken, protocol.ToString(), type.ToString(), clientId));
            }

            public void OnProtocolSessionDisconnected(uint serverId, string sessionToken, uint connectionFailed, string errorMessage)
            {
                DispatchMessage(3, string.Format("" +
                            "Protocol Session Disconnected, Server ID: {0}, Token: {1}, ConnectFailed: {2}, Error: {3}",
                            serverId, sessionToken, connectionFailed, errorMessage));
            }

            public void OnSeamlessWindowsModeChanged(uint serverId, string sessionToken, uint enabled)
            {
                DispatchMessage(3, string.Format("Seamless Window Mode Changed, Server ID: {0}, Token: {1}, Enabled: {2}",
                            serverId, sessionToken, enabled));
            }

            public void OnSeamlessWindowAdded(uint serverId, string sessionToken, string windowPath, string entitlementId, int windowId, long windowHandle, VmwHorizonClientSeamlessWindowType type)
            {
                DispatchMessage(3, string.Format(
                           "Seamless Window Added, Server ID: {0}, Token: {1}, WindowPath: {2}, EntitlementID: {3}, WindowID: {4}, WindowHandle: {5}, Type: {6}",
                           serverId, sessionToken, windowPath, entitlementId, windowId, windowHandle, type.ToString()));
            }

            public void OnSeamlessWindowRemoved(uint serverId, string sessionToken, int windowId)
            {
                DispatchMessage(3, string.Format(
                            "Seamless Window Removed, Server ID: {0}, Token: {1}, WindowID: {2}",
                            serverId, sessionToken, windowId));
            }

            public void OnUSBInitializeComplete(uint serverId, string sessionToken)
            {
                DispatchMessage(3, string.Format(
                           "USB Initialize Complete, Server ID: {0}, Token: {1}",
                          serverId, sessionToken));
            }

            public void OnConnectUSBDeviceComplete(uint serverId, string sessionToken, uint isConnected)
            {
                DispatchMessage(3, string.Format(
                          "Connect USB Device Complete, Server ID: {0}, Token: {1}, IsConnected: {2}",
                          serverId, sessionToken, isConnected));
            }

            public void OnUSBDeviceError(uint serverId, string sessionToken, string errorMessage)
            {
                DispatchMessage(3, string.Format(
                          "Connect USB Device Error, Server ID: {0}, Token: {1}, Error: {2}",
                          serverId, sessionToken, errorMessage));
            }

            public void OnAddSharedFolderComplete(uint serverId, string fullPath, uint succeeded, string errorMessage)
            {
                DispatchMessage(3, string.Format(
                          "Add Shared Folder Complete, Server ID: {0}, FullPath: {1}, Succeeded: {2}, Error: {3}",
                          serverId, fullPath, succeeded, errorMessage));
            }

            public void OnRemoveSharedFolderComplete(uint serverId, string fullPath, uint succeeded, string errorMessage)
            {
                DispatchMessage(3, string.Format(
                          "Remove Shared Folder Complete, Server ID: {0}, FullPath: {1}, Succeeded: {2}, Error: {3}",
                          serverId, fullPath, succeeded, errorMessage));
            }

            public void OnFolderCanBeShared(uint serverId, string sessionToken, uint canShare)
            {
                DispatchMessage(3, string.Format(
                          "Folder Can Be Shared, Server ID: {0}, Token: {1}, canShare: {2}",
                          serverId, sessionToken, canShare));
            }

            public void OnCDRForcedByAgent(uint serverId, string sessionToken, uint forcedByAgent)
            {
                DispatchMessage(3, string.Format(
                         "CDR Forced By Agent, Server ID: {0}, Token: {1}, Forced: {2}",
                        serverId, sessionToken, forcedByAgent));
            }

            public void OnItemLaunchSucceeded2(uint serverId, VmwHorizonLaunchItemType type, string launchItemId, string sessionToken)
            {
                DispatchMessage(3, string.Format("Item Launch Succeeded(2), Server ID: {0}, Type: {1}, ID: {2}, token: {3}", serverId,
                            type.ToString(), launchItemId, sessionToken));
            }

            public void OnReceivedLaunchItems2(uint serverId, Array launchItems)
            {
                DispatchMessage(3, string.Format("Received Launch Items2, Server ID: {0}, Item Count: {1}", serverId, launchItems.Length));
                var Items = Helpers.GetLaunchItems2(launchItems);
                foreach(var item in Items)
                {
                    DispatchMessage(3, String.Format("Launch Item: Server ID: {0}, Name: {1}, Type: {2}, ID: {3}, Remotable: {4}",serverId, item.name, item.type.ToString(), item.id, item.hasRemotableAssets));
                }
            }
        }     
    }
}
