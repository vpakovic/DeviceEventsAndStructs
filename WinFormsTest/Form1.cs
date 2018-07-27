using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WinFormsTest.USBEventListener;

namespace WinFormsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            USBEventListener.RegisterUsbDeviceNotification(this.Handle);
        }
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (message.Msg == USBEventListener.UsbDeviceChanged)
            {
                switch ((int)message.WParam)
                {
                    case USBEventListener.NewUsbDeviceArrived:
                        DeviceAdded(message);
                        break;
                    case USBEventListener.UsbDeviceRemoved:
                        DeviceRemoved(message);
                        break;
                }
            }
        }

        private void DeviceAdded(Message message)
        {
            this.textBox1.Text = "Nesto utaknuto, da prostis...";
        }

        private void DeviceRemoved(Message message)
        {
            dynamic eventOrigin = Marshal.PtrToStructure(message.LParam, typeof(DevInterface));
            string name = eventOrigin.Name.ToString();
            this.textBox1.AppendText("Nesto iscupano, ali sta? To nisam ja to je moja Shauma!!!  " + name);
        }
    }
}
