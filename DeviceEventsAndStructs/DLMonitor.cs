using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEventsAndStructs
{
    public class DLMonitor : DLDevice
    {
        public DLMonitor(string name, WindowsDevice wDevice) :
            base(name, wDevice)
        {

        }

        public override string ToString()
        {
            string result = "Monitor " + m_name;
            return result;
        }

        public override string ToJson()
        {
            string result = "";

            result += "\"device\":\"monitor\"";

            return result;
        }
    }
}
