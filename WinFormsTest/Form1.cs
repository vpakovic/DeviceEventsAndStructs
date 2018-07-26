using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                        DeviceAdded();
                        break;
                    case USBEventListener.UsbDeviceRemoved:
                        DeviceRemoved();
                        break;
                }
            }
        }

        private void DeviceAdded()
        {
            this.textBox1.Text = "Nesto utaknuto, da prostis...";
        }

        private void DeviceRemoved()
        {
            this.textBox1.Text = "Nesto iscupano";
        }
    }
}
