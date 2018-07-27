using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEventsAndStructs
{
    class DLDeviceTreeBuilder
    {
        private const string DL_HARDWARE_ID = "VID_17E9";
        private const string DEVICE_CLASS_DISPLAY = "Display";

        private List<WindowsDevice> m_windowsDevices;
        private DLDevice m_PC = null;

        public List<WindowsDevice> WindowsDevices { get { return m_windowsDevices; } set { m_windowsDevices = value; } }
        public DLDevice Build()
        {
            m_PC = new DLDevicePC("PC", null);
            var displayAdapters = FindDLDisplayAdapters();

            foreach (var displayAdapter in displayAdapters)
            {
                var dockRepresentation = FindDeviceWhichRepresentingDock(displayAdapter);
                var existingDock = FindDeviceInDLThree(dockRepresentation);
                if (existingDock == null)
                {
                    DLDevice dock = new DLDock(displayAdapter.Name, dockRepresentation);
                    existingDock = dock;

                    DLDevice parent = FindParent(dock);
                    
                    if (parent == null)
                    {
                        parent = m_PC;
                    }

                    parent.Children.Add(dock);
                    dock.Parent = parent;

                    foreach (var sibling in parent.Children)
                    {
                        if (IsInParentChildRelationship(dock.WindowsDevice, sibling.WindowsDevice))
                        {
                            dock.Children.Add(sibling);
                            sibling.Parent = dock;
                        }
                    }

                    foreach(var siblingForDelete in dock.Children)
                    {
                        parent.Children.Remove(siblingForDelete);
                    }
                }

                foreach (var monitorRepresentation in displayAdapter.Children)
                {
                    DLDevice monitor = new DLMonitor(monitorRepresentation.Name, monitorRepresentation);
                    existingDock.Children.Add(monitor);
                }
            }

            return m_PC;
        }

        private List<WindowsDevice> FindDLDisplayAdapters()
        {
            List<WindowsDevice> displayAdapters = new List<WindowsDevice>();

            foreach (var device in m_windowsDevices)
            {
                if (device.Class == DEVICE_CLASS_DISPLAY && device.Id.Contains(DL_HARDWARE_ID))
                {
                    displayAdapters.Add(device);
                }
            }

            return displayAdapters;
        }

        private WindowsDevice FindDeviceWhichRepresentingDock(WindowsDevice displayAdapter)
        {
            WindowsDevice dock = null;

            if (displayAdapter.Parent != null)
            {
                if (displayAdapter.Parent.Parent != null)
                {
                    dock = displayAdapter.Parent.Parent;
                }
            }

            return dock;
        }

        private DLDevice FindDeviceInDLThree(WindowsDevice dockRepresentation)
        {
            DLDevice result = null;

            Stack<DLDevice> stack = new Stack<DLDevice>();
            DLDevice node = m_PC;

            while (node != null)
            {
                if (node.WindowsDevice == dockRepresentation)
                {
                    result = node;
                    break;
                }

                foreach (var e in node.Children)
                {
                    stack.Push(e);
                }

                node = null;
                if (stack.Count > 0)
                {
                    node = stack.Pop();
                }
            }

            return result;
        }

        private bool IsInParentChildRelationship(WindowsDevice parent, WindowsDevice child)
        {
            bool result = false;

            if (parent != null && child != null)
            {
                WindowsDevice device = child;
                while (device.Parent != null && device != device.Parent)
                {
                    if (device.Parent == parent)
                    {
                        result = true;
                        break;
                    }

                    device = device.Parent;
                }
            }

            return result;
        }

        private DLDevice FindParent(DLDevice device)
        {
            DLDevice result = null;

            Stack<DLDevice> stack = new Stack<DLDevice>();
            DLDevice node = m_PC;

            while (node != null)
            {
                if (IsInParentChildRelationship(node.WindowsDevice, device.WindowsDevice))
                {
                    result = node;
                }

                foreach (var e in node.Children)
                {
                    stack.Push(e);
                }

                node = null;
                if (stack.Count > 0)
                {
                    node = stack.Pop();
                }
            }

            return result;
        }
    }
}
