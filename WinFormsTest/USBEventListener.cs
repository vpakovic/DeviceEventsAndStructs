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

        [StructLayout(LayoutKind.Sequential)]
        private struct DevInterface
        {
            internal int Size;
            internal int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }

        private const int UsbDeviceInterface = 5;
        private static readonly Guid DeviceInterrfaceGuid = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
        public const int NewUsbDeviceArrived = 0x8000;        
        public const int UsbDeviceRemoved = 0x8004; 
        public const int UsbDeviceChanged = 0x0219; 
    
        private static IntPtr notificationHandle;

        public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
        {
            DevInterface dbi = new DevInterface
            {
                DeviceType = UsbDeviceInterface,
                Reserved = 0,
                ClassGuid = DeviceInterrfaceGuid,
                Name = 0
            };

            dbi.Size = Marshal.SizeOf(dbi);
            IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, buffer, true);

            notificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
        }

        public static void UnregisterUsbDeviceNotification()
        {
            UnregisterDeviceNotification(notificationHandle);
        }
    }
}
