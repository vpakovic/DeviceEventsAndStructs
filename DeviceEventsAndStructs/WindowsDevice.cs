using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEventsAndStructs
{
    public class WindowsDevice
    {
        private string m_id;
        private string m_name;
        private string m_class;
        private List<string> m_childrenIds;
        private string m_parentId;
        private string m_driverVersion;

        private WindowsDevice m_parent;
        private List<WindowsDevice> m_children;
        
        public WindowsDevice(string id, string name, string pclass, List<string> childrenIds, string parent, string driverVersion)
        {
            m_id = id;
            m_name = name;
            m_class = pclass;
            m_childrenIds = childrenIds;
            m_parentId = parent;
            m_driverVersion = driverVersion;
            m_children = new List<WindowsDevice>();
        }

        public string Id                            { get { return m_id; } }
        public string Name                          { get { return m_name; } }
        public string Class                         { get { return m_class; } }
        public string ParentId                      { get { return m_parentId; } }
        public List<string> ChildrenIds             { get { return m_childrenIds; } }
        public WindowsDevice Parent                 { get { return m_parent; } set { m_parent = value; } }
        public List<WindowsDevice> Children         { get { return m_children; } set { m_children = value; } }
        public string DriverVersion                 { get { return m_driverVersion; } }
    }
}
