using System;
using DLMExtension;

namespace DeviceEventsAndStructs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started ...");
            DriverVersionGetter dg = new DriverVersionGetter();
            Console.WriteLine(dg.GetDriverVersion());
            Console.WriteLine("End ...");
            Console.ReadLine();
        }
    }
}
