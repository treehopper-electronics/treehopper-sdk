using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using Treehopper;
using LibUsbDotNet.DeviceNotify;
namespace FirmwareManager
{
    public partial class FirmwareManagerForm : Form
    {
        IDeviceNotifier devNotifier;
        TreehopperBoard Board;
        RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        public FirmwareManagerForm()
        {
            InitializeComponent();
            devNotifier = DeviceNotifier.OpenDeviceNotifier();
            // devNotifier.OnDeviceNotify += devNotifier_OnDeviceNotify;

            TreehopperManager manager = new TreehopperManager();
            manager.BoardAdded += manager_BoardAdded;
            manager.BoardRemoved += manager_BoardRemoved;
        }

        void devNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        void manager_BoardRemoved(object sender, TreehopperBoard board)
        {
            Board = null;
            if (serialLabel.InvokeRequired)
            {
                if (IsHandleCreated)
                    serialLabel.Invoke((MethodInvoker)delegate { serialLabel.Text = "No boards connected"; });
            }
            else
            {
                serialLabel.Text = "No boards connected";
            }
        }

        void manager_BoardAdded(object sender, TreehopperBoard board)
        {
            Board = board;
            if(serialLabel.InvokeRequired)
            { 
            if(IsHandleCreated)
                serialLabel.Invoke((MethodInvoker)delegate{ serialLabel.Text = board.Description; });
            }
            else
            {
                serialLabel.Text = board.Description;
            }
        }

        private void rebootBtn_Click(object sender, EventArgs e)
        {
            if (Board == null)
                return;
            Board.Open();
            Board.Reboot();

        }

        private void firmwareBtn_Click(object sender, EventArgs e)
        {
            if (Board == null)
                return;
            Board.Open();
            Board.RebootIntoBootloader();
        }

        private void updateSerialBtn_Click(object sender, EventArgs e)
        {
            if (Board == null)
                return;
            Board.Open();
            Board.UpdateSerialNumber(textBox1.Text);
        }

        private void FirmwareManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            rngCsp.Dispose();
            if (Board == null)
                return;
            Board.Open();
            Board.Dispose();
        }

        private string getSerialNumber()
        {
            int serialNumberLength = 15;
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            byte[] randomIndex = new byte[serialNumberLength];
            rngCsp.GetBytes(randomIndex);
            string result = "";
            foreach (var number in randomIndex)
            {
                result += chars[number % chars.Length];
            }
            return result;
        }

        private void generateSerialBtn_Click(object sender, EventArgs e)
        {
            if (Board == null)
                return;
            Board.Open();
            Board.UpdateSerialNumber(getSerialNumber());
            Board.UpdateDeviceName("MyTreehopper");
            Board.Reboot();
            Board.Close();
        }

        private void updateNameBtn_Click(object sender, EventArgs e)
        {
            if (Board == null)
                return;
            Board.Open();
            Board.UpdateDeviceName(nameBox.Text);
            Board.Reboot();
            if(Board != null)
                Board.Close();
        }
    }
}
