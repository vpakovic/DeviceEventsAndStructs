using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DLMExtension
{
    public class ConfigManagerAPI
    {
        public struct DeviceInfo
        {
            public DeviceInfo(Guid g, UInt32 p)
            {
                guid = g;
                pid = p;
            }

            public Guid guid;
            public UInt32 pid;
        };

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
        public static extern uint CM_Get_Device_ID_List_SizeW(ref UInt32 pulLen, ref String pszFilter, UInt32 ulFlags);

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
        public static extern uint CM_Get_Device_ID_ListW(ref String filter, [Out]IntPtr buffer, UInt32 bufferLen, UInt32 flags);

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
        public static extern uint CM_Locate_DevNodeW(ref UInt32 devInstance, [In]IntPtr deviceID, UInt32 flags);

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
        public static extern uint CM_Get_DevNode_PropertyW(UInt32 dnDevInst, [In]ref DeviceInfo devicePropertyKey, ref UInt32 propertyType, IntPtr buffer, ref UInt32 bufferLen, UInt32 flags);
    }
}