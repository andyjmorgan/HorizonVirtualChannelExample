using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMware.Horizon.VirtualChannel.PipeMessages
{
    public class v1
    {

        public class VolumeStatus
        {
            public bool Muted { get; set; }
            public float VolumeLevel { get; set; }
            public VolumeStatus(bool _Muted, float _VolumeLevel)
            {
                Muted = _Muted;
                VolumeLevel = _VolumeLevel;
            }
        }

        public class ChannelResponse
        {
            public bool Successful { get; set; }
            public string Details { get; set; }
            public ChannelResponse(bool _Success, string _Details)
            {
                Successful = _Success;
                Details = _Details;
            }
            public ChannelResponse()
            {
                Successful = true;
            }
        }
        public class ChannelCommand
        {
            public CommandType CommandType { get; set; }
            public object CommandParameters { get; set; }
            public ChannelCommand(CommandType ct, object Parameters)
            {
                CommandType = ct;
                CommandParameters = Parameters;
            }
        }

        public enum CommandType
        {
            SetVolume,
            GetSecurity,
            Probe,
            GetVolume,
        }
    }
}
