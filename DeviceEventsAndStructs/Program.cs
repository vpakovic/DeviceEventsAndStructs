using System;
using System.Collections.Generic;
using DLMExtension;

namespace DeviceEventsAndStructs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started ...");
            
            //
            List<WindowsDevice> windowsDeviceList = new List<WindowsDevice>();
            WindowsDeviceEnumerator wde = new WindowsDeviceEnumerator();
            windowsDeviceList = wde.EnumerateDevices();
            //devices is now enumerated, now we should build tree
            DLDeviceTreeBuilder dlDeviceBuilder = new DLDeviceTreeBuilder();
            dlDeviceBuilder.WindowsDevices = windowsDeviceList;
            DLDevice root = dlDeviceBuilder.Build();
            Console.WriteLine(root.ToJson());
            //

            Console.WriteLine("End ...");
            Console.ReadLine();
        }
    }
}
