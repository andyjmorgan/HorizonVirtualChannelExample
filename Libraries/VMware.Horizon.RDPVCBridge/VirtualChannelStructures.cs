using System;
using System.Collections.Generic;
using System.Text;

namespace VMware.Horizon.VirtualChannel.RDPVCBridgeInterop
{
    public class VirtualChannelStructures
    {
        public static Byte[] PingFragment = new byte[] { 0x50 /* 'P' */, 0x69 /* 'i' */, 0x6E /* 'n' */, 0x67 /* 'g' */ };
        public static Byte[] PongFragment = new byte[] { 0x50 /* 'P' */, 0x6F /* 'o' */, 0x6E /* 'n' */, 0x67 /* 'g' */ };

        public static IntPtr WTS_CURRENT_SERVER_HANDLE = (IntPtr)null;
        public enum ChannelEvents
        {
            Initialized = 0,
            Connected = 1,
            V1Connected = 2,
            Disconnected = 3,
            Terminated = 4,
            DataReceived = 10,
            WriteComplete = 11,
            WriteCanceled = 12
        }

        [Flags]
        public enum ChannelFlags
        {
            First = 0x01,
            Last = 0x02,
            Only = First | Last,
            Middle = 0,
            Fail = 0x100,
            ShowProtocol = 0x10,
            Suspend = 0x20,
            Resume = 0x40
        }

        [Flags]
        public enum ChannelOptions : uint
        {
            Initialized = 0x80000000,
            EncryptRDP = 0x40000000,
            EncryptSC = 0x20000000,
            EncryptCS = 0x10000000,
            PriorityHigh = 0x08000000,
            PriorityMedium = 0x04000000,
            PriorityLow = 0x02000000,
            CompressRDP = 0x00800000,
            Compress = 0x00400000,
            ShowProtocol = 0x00200000
        }

        public enum ChannelReturnCodes
        {
            Ok = 0,
            AlreadyInitialized = 1,
            NotInitialized = 2,
            AlreadyConnected = 3,
            NotConnected = 4,
            TooManyChannels = 5,
            BadChannel = 6,
            BadChannelHandle = 7,
            NoBuffer = 8,
            BadInitHandle = 9,
            NotOpen = 10,
            BadProc = 11,
            NoMemory = 12,
            UnknownChannelName = 13,
            AlreadyOpen = 14,
            NotInVirtualchannelEntry = 15,
            NullData = 16,
            ZeroLength = 17
        }
    }
}