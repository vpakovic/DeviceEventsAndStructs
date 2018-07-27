using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEventsAndStructs
{
    public class DLDock : DLDevice
    {
        public DLDock(string name, WindowsDevice wDevice) :
            base(name, wDevice)
        {

        }

        public override string ToString()
        {
            string result = "Dock " + m_name;
            return result;
        }

        public override string ToJson()
        {
            string result = "";

            result += "\"device\":\"dock\", \"children\":{";
            result += ChildrenToJson();
            result += "}";


            return result;
        }
    }
}
