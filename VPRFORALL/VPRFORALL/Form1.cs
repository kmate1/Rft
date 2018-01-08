using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VPRFORALL
{
    public partial class Form1 : Form
    {
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVTYP_VOLUME = 0x00000002;
        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel3.Text = DateTime.Now.ToString() + " " + Environment.MachineName+" Kontakt:Simon Adrián:704904204";
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    switch ((int)m.WParam)

                    {
                        case DBT_DEVICEARRIVAL:
                            
                            toolStripStatusLabel1.Text = "New Device Arrived";

                           OnDriveArrived();

                            break;

                        case DBT_DEVICEREMOVECOMPLETE:
                            toolStripStatusLabel1.Text = "Device Removed";
                            OnDriveRemoved();
                            break;

                    }
                    break;
            }
    }
        private void OnDriveArrived()
        {
            textBox1.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            if (textBox2.Text != string.Empty && textBox3.Text != string.Empty)
            {

                //Hálózati eszközök listázása
                List<string> landev = new List<string>();
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    landev.Add(nic.Name);

                }
                //hálózati eszközök kikapcsolása
                for (int i = 0; i < landev.Count; i++)
                {
                    Disable(landev[i]);
                }
                //Vírusírtós batch file indítása
                Process.Start(Directory.GetCurrentDirectory().ToString() + @"\start.bat");

                //hány db faájl van a pendrive-on => később
                //DirectoryInfo dir = new System.IO.DirectoryInfo("E:\\");
                //int count = dir.GetFiles().Length;

                //progressBar1.Style = ProgressBarStyle.Marquee;

                //Hálózati eszközök listázása
                //List<string> landev2 = new List<string>();
                //foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                //{
                //    landev2.Add(nic.Name);
                //}
                //hálózati eszközök visszakapcsolása
                for (int i = 0; i < landev.Count; i++)
                {
                    Enable(landev[i]);
                }

                try
                {
                    TopMost = false;

                    //log másolása
                    var path = System.Configuration.ConfigurationManager.AppSettings["Path"];
                    string oldname = String.Format("{0:MMddyyyy}", DateTime.Today) + ".log";
                    string frompath = path + oldname;
                    string flname = String.Format("{0:MMddyyyy}", DateTime.Today) + ".log";



                    string topath = Directory.GetCurrentDirectory().ToString() + @"\" + flname;

                    {
                        File.Delete(topath);
                        File.Copy(frompath, topath);
                    }


                }
                catch (Exception)
                {
                    MessageBox.Show("Másolás nem sikerült");

                }
                //// email küldés

               MailSender.SendMailMessage("noreplyVPREger@bosch.com", "adrian.simon2@bosch.com", "RBASHU:Vírus Ellenőrzés Történt Egerben", "A J Csarnokban Pendrive ellenőrzés történt " + DateTime.Now.ToString() + " időpontban.Az ellenőrzést végezte: " + textBox1.Text + ". A Vendég neve: " + textBox2.Text + ". A Vendég Cég Neve " + textBox3.Text+"Látogatás Célja:"+textBox4.Text);

            }
            else
            {
                MessageBox.Show("Kérem vegye ki a Pendrive-ot és töltse ki a szükséges információkat");
            }









        }
        private void OnDriveRemoved()
        {
            // TODO: do clean up here, etc. Letter of the removed drive is in e.Drive;

            // Just add report to the listbox
            
            TopMost = false;

            Process.Start(Directory.GetCurrentDirectory().ToString() + @"\end.bat");
            List<string> values = new List<string>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                values.Add(nic.Name);
            }
            for (int i = 0; i < values.Count; i++)
            {
                Enable(values[i]);
            }

            Process.Start(Directory.GetCurrentDirectory().ToString() + @"\masolos.bat");

        }

        public void Enable(string interfaceName)
        {
            System.Diagnostics.ProcessStartInfo psi =
                new System.Diagnostics.ProcessStartInfo("netsh", "interface set interface \"" + interfaceName + "\" enable");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            p.Start();
        }
        public void Disable(string interfaceName)
        {
            System.Diagnostics.ProcessStartInfo psi =
                new System.Diagnostics.ProcessStartInfo("netsh", "interface set interface \"" + interfaceName + "\" disable");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            p.Start();
        }
        public List<string> net_adapters()
        {
            List<string> values = new List<string>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                values.Add(nic.Name);
            }
            return values;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = comboBox1.SelectedItem.ToString();
        }
    }
}


