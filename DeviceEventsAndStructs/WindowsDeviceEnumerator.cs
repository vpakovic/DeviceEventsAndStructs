using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DLMExtension;

namespace DeviceEventsAndStructs
{
    class WindowsDeviceEnumerator
    {
        private const string DEV_INFO_DRIVER_VERSION_GUID = "{A8B865DD-2E3D-4094-AD97-E593A70C75D6}";
        private const int DEV_INFO_DRIVER_VERSION_PID = 3;

        private const string DEV_INFO_DEVICE_CLASS_GUID = "{A45C254E-DF1C-4EFD-8020-67D146A850E0}";
        private const int DEV_INFO_DEVICE_CLASS_PID = 9;

        private const string DEV_INFO_CHILDREN_ID_GUID = "{4340A6C5-93FA-4706-972C-7B648008A5A7}";
        private const int DEV_INFO_CHILDREN_ID_PID = 9;

        private const string DEV_INFO_PARENT_ID_GUID = "{4340a6c5-93fa-4706-972c-7b648008a5a7}";
        private const int DEV_INFO_PARENT_ID_PID = 8;
        //private const string DEV_

        private const string DL_HARDWARE_ID = "VID_17E9";
        private const string DEVICE_CLASS_DISPLAY = "Display";
        private const int SMALL_BAFFER = 26;
        private const int STRING_TYPE = 18;
        private const int LIST_TYPE = 8192;

        public List<WindowsDevice> EnumerateDevices()
        {
            List<WindowsDevice> devices = new List<WindowsDevice>();

            uint result = 0;

            UInt32 size = 0;                                                                            //size iz number of characters 
            string filter = "";
            result = ConfigManagerAPI.CM_Get_Device_ID_List_SizeW(ref size, ref filter, 0);
            if (result == 0)
            {
                IntPtr buffer = Marshal.AllocHGlobal((int)size * 2);                                    //width of wchar is 2
                UInt32 flags = 256;

                result = ConfigManagerAPI.CM_Get_Device_ID_ListW(ref filter, buffer, size, flags);
                if (result == 0)
                {
                    List<Tuple<string, int>> devicesList = ParseBuffer(buffer, size);

                    foreach (var device in devicesList)
                    {
                        UInt32 deviceInstance = 0;
                        IntPtr ptrToDeviceId = IntPtr.Add(buffer, device.Item2);
                        result = ConfigManagerAPI.CM_Locate_DevNodeW(ref deviceInstance, ptrToDeviceId, 0);

                        if (result == 0)
                        {
                            string id = device.Item1;
                            string deviceClass = GetPropertString(deviceInstance, new ConfigManagerAPI.DeviceInfo(new Guid(DEV_INFO_DEVICE_CLASS_GUID), DEV_INFO_DEVICE_CLASS_PID));
                            string parent = GetPropertString(deviceInstance, new ConfigManagerAPI.DeviceInfo(new Guid(DEV_INFO_PARENT_ID_GUID), DEV_INFO_PARENT_ID_PID));
                            string driverVersion = GetPropertString(deviceInstance, new ConfigManagerAPI.DeviceInfo(new Guid(DEV_INFO_DRIVER_VERSION_GUID), DEV_INFO_DRIVER_VERSION_PID));
                            List<string> children = GetPropertStringList(deviceInstance, new ConfigManagerAPI.DeviceInfo(new Guid(DEV_INFO_CHILDREN_ID_GUID), DEV_INFO_CHILDREN_ID_PID));

                            WindowsDevice windowsDevice = new WindowsDevice(id, id, deviceClass, children, parent, driverVersion);
                            devices.Add(windowsDevice);
                        }
                    }
                }

                Marshal.FreeHGlobal(buffer);
            }

            InitializeDeviceSiblings(devices);

            return devices;
        }

        private List<Tuple<string, int>> ParseBuffer(IntPtr buffer, UInt32 bufferSize)
        {
            List<Tuple<string, int>> resultList = new List<Tuple<string, int>>();

            string device = "";
            int of = 0;

            int offset = 0;
            while (offset < bufferSize)
            {
                string character = Marshal.PtrToStringAnsi(IntPtr.Add(buffer, offset));

                if (!String.IsNullOrEmpty(character))
                {
                    device += character;
                }
                else
                {
                    if (String.IsNullOrEmpty(device))
                    {
                        break;
                    }

                    resultList.Add(new Tuple<string, int>(device, of));
                    
                    of = offset + 2;
                    device = "";
                }

                offset += 2;
            }

            return resultList;
        }

        private string GetPropertString(UInt32 deviceInstance, ConfigManagerAPI.DeviceInfo devPropInfo)
        {
            string property = "";

            UInt32 propertyType = 0;
            IntPtr buf = new IntPtr();
            UInt32 bufSize = 0;

            uint result = ConfigManagerAPI.CM_Get_DevNode_PropertyW(deviceInstance, ref devPropInfo, ref propertyType, buf, ref bufSize, 0);

            if (result == SMALL_BAFFER && propertyType == STRING_TYPE)
            {
                buf = Marshal.AllocHGlobal((int)bufSize * 2);

                result = ConfigManagerAPI.CM_Get_DevNode_PropertyW(deviceInstance, ref devPropInfo, ref propertyType, buf, ref bufSize, 0);

                if (result == 0)
                {
                    property = ReadNullTerminatedStringFromBuffer(buf, bufSize);
                }

                Marshal.FreeHGlobal(buf);
            }

            return property;
        }

        private List<string> GetPropertStringList(UInt32 deviceInstance, ConfigManagerAPI.DeviceInfo devPropInfo)
        {
            List<string> property = new List<string>();

            UInt32 propertyType = 0;
            IntPtr buf = new IntPtr();
            UInt32 bufSize = 0;

            uint result = ConfigManagerAPI.CM_Get_DevNode_PropertyW(deviceInstance, ref devPropInfo, ref propertyType, buf, ref bufSize, 0);

            if (result == SMALL_BAFFER && propertyType == (STRING_TYPE + LIST_TYPE))
            {
                buf = Marshal.AllocHGlobal((int)bufSize * 2);

                result = ConfigManagerAPI.CM_Get_DevNode_PropertyW(deviceInstance, ref devPropInfo, ref propertyType, buf, ref bufSize, 0);

                if (result == 0)
                {
                    IntPtr pointer = buf;
                    while (bufSize > 0)
                    {
                        string element = ReadNullTerminatedStringFromBuffer(pointer, bufSize);
                        if (String.IsNullOrEmpty(element))
                        {
                            break;
                        }
                        int offset = element.Length + 1;
                        offset *= 2;
                        property.Add(element);
                        pointer = IntPtr.Add(pointer, offset);
                    }
                }

                Marshal.FreeHGlobal(buf);
            }

            return property;
        }

        private string ReadNullTerminatedStringFromBuffer(IntPtr buffer, UInt32 bufferSize)
        {
            string result = "";
            string prop = "";
            int offset = 0;
            while (offset < bufferSize * 2)
            {
                string character = Marshal.PtrToStringAnsi(IntPtr.Add(buffer, offset));

                if (!String.IsNullOrEmpty(character))
                {
                    prop += character;
                }
                else
                {
                    result = prop;
                    break;
                }

                offset += 2;
            }

            return result;
        }

        private void InitializeDeviceSiblings(List<WindowsDevice> devices)
        {
            foreach (var sibling in devices)
            {
                foreach (var device in devices)
                {
                    if (device.ParentId == sibling.Id)
                    {
                        device.Parent = sibling;
                    }
                    
                    foreach(var childId in device.ChildrenIds)
                    {
                        if (childId == sibling.Id)
                        {
                            device.Children.Add(sibling);
                            break;
                        }
                    }
                }
            }
        }
    }
}
