using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMware.Horizon.VirtualChannel.RDPVCBridgeInterop
{
    public class BinaryConverters
    {
        public static byte[] StringToBinary(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }
        public static string BinaryToString(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }
    }
}
