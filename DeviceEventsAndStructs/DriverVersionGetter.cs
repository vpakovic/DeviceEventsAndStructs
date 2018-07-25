using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DLMExtension
{
    public class DriverVersionGetter
    {
        private const string DEV_INFO_DRIVER_VERSION_GUID = "{A8B865DD-2E3D-4094-AD97-E593A70C75D6}";
        private const int DEV_INFO_DRIVER_VERSION_PID = 3;

        private const string DEV_INFO_DEVICE_CLASS_GUID = "{A45C254E-DF1C-4EFD-8020-67D146A850E0}";
        private const int DEV_INFO_DEVICE_CLASS_PID = 9;

        private const string DL_HARDWARE_ID = "VID_17E9";
        private const string DEVICE_CLASS_DISPLAY = "Display";
        private const int SMALL_BAFFER = 26;
        private const int STRING_TYPE = 18;


        public DriverVersionGetter()
        {

        }

        public string GetDriverVersion()
        {
            string driverVersion = "";

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
                            string driverVer = GetProperty(deviceInstance, new ConfigManagerAPI.DeviceInfo(new Guid(DEV_INFO_DRIVER_VERSION_GUID), DEV_INFO_DRIVER_VERSION_PID));
                            string devClass = GetProperty(deviceInstance, new ConfigManagerAPI.DeviceInfo(new Guid(DEV_INFO_DEVICE_CLASS_GUID), DEV_INFO_DEVICE_CLASS_PID));

                            if (devClass == DEVICE_CLASS_DISPLAY)
                            {
                                driverVersion = driverVer;
                                break;
                            }
                        }
                    }
                }

                Marshal.FreeHGlobal(buffer);
            }

            return driverVersion;

        }

        private string GetProperty(UInt32 deviceInstance, ConfigManagerAPI.DeviceInfo devPropInfo)
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
                    string prop = "";
                    int offset = 0;
                    while (offset < bufSize * 2)
                    {
                        string character = Marshal.PtrToStringAnsi(IntPtr.Add(buf, offset));

                        if (!String.IsNullOrEmpty(character))
                        {
                            prop += character;
                        }
                        else
                        {
                            property = prop;
                            break;
                        }

                        offset += 2;
                    }
                }

                Marshal.FreeHGlobal(buf);
            }

            return property;
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

                    if (device.Contains(DL_HARDWARE_ID))
                    {
                        resultList.Add(new Tuple<string, int>(device, of));
                    }

                    of = offset + 2;
                    device = "";
                }

                offset += 2;
            }

            return resultList;
        }
    }
}
