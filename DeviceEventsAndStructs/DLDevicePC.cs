using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEventsAndStructs
{
    public class DLDevicePC : DLDevice
    {
        public DLDevicePC(string name, WindowsDevice wDevice)
            : base(name, wDevice)
        {

        }

        public override string ToString()
        {
            return "PC";
        }

        public override string ToJson()
        {
            string result = "";

            result += "{\"root\":\"PC\", \"children\":{";
            result += ChildrenToJson();

            result += "}}";

            return result;
        }
    }
}
