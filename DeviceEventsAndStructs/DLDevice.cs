using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEventsAndStructs
{
    public class DLDevice
    {
        protected DLDevice m_parent;
        protected List<DLDevice> m_children = new List<DLDevice>();

        protected string m_name;
        protected WindowsDevice m_windowDevice;

        public DLDevice(string name, WindowsDevice windowDevice)
        {
            m_name = name;
            m_windowDevice = windowDevice;
        }

        public virtual string ToJson()
        {
            return "";
        }

        protected string ChildrenToJson()
        {
            string result = "";

            foreach (var child in Children)
            {
                result += child.ToJson();
                result += ",";
            }

            if (Children.Count > 0)
            {
                result = result.Remove(result.Length - 1);
            }

            return result;
        }

        public DLDevice Parent          { get { return m_parent; } set { m_parent = value; } }
        public List<DLDevice> Children  { get { return m_children; } set { m_children = value; } }
        public WindowsDevice WindowsDevice { get { return m_windowDevice; } set { m_windowDevice = value; } }
    }
}
