using System;
using System.Runtime.InteropServices;

namespace WinFormsTest
{
    internal class USBEventListener
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DevInterface
        {
            public int Size;
            public int DeviceType;
            public int Reserved;
            public Guid ClassGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string Name;
        }

        private const int UsbDeviceInterface = 5;
        private static readonly Guid DeviceInterrfaceGuid = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
        public const int NewUsbDeviceArrived = 0x8000;        
        public const int UsbDeviceRemoved = 0x8004; 
        public const int UsbDeviceChanged = 0x0219; 
    
        private static IntPtr notificationHandle;

        public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
        {
            DevInterface devInterface = new DevInterface
            {
                DeviceType = UsbDeviceInterface,
                Reserved = 0,
                ClassGuid = DeviceInterrfaceGuid,
                Name = ""
            };

            devInterface.Size = Marshal.SizeOf(devInterface);
            IntPtr buffer = Marshal.AllocHGlobal(devInterface.Size);
            Marshal.StructureToPtr(devInterface, buffer, true);

            notificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
        }

        public static void UnregisterUsbDeviceNotification()
        {
            UnregisterDeviceNotification(notificationHandle);
        }
    }
}
