using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Dolinay;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private DriveDetector driveDetector = null;
        

        public Form1()
        {
            InitializeComponent();
           

            driveDetector = new DriveDetector();
            driveDetector.DeviceArrived += new DriveDetectorEventHandler(OnDriveArrived);
            driveDetector.DeviceRemoved += new DriveDetectorEventHandler(OnDriveRemoved);
            driveDetector.QueryRemove += new DriveDetectorEventHandler(OnQueryRemove);
            
            
     
        }
        // Called by DriveDetector when removable device in inserted 
        private void OnDriveArrived(object sender, DriveDetectorEventArgs e)
        {
            textBox1.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            if (textBox2.Text!=string.Empty && textBox3.Text != string.Empty)
            {
                
                // Report the event in the listbox.
                // e.Drive is the drive letter for the device which just arrived, e.g. "E:\\"
                string s = "Drive arrived " + e.Drive;
                listBox1.Items.Add(s);
                List<string> values = new List<string>();
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    values.Add(nic.Name);

                }
                for (int i = 0; i < values.Count; i++)
                {
                    Disable(values[i]);
                }

                Process.Start(Directory.GetCurrentDirectory().ToString() + @"\start.bat");

                DirectoryInfo dir = new System.IO.DirectoryInfo("E:\\");
                int count = dir.GetFiles().Length;

                progressBar1.Style = ProgressBarStyle.Marquee;

                List<string> values2 = new List<string>();
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    values2.Add(nic.Name);
                }
                for (int i = 0; i < values2.Count; i++)
                {
                    Enable(values2[i]);
                }

                try
                {
                    TopMost = false;


                    var path = System.Configuration.ConfigurationManager.AppSettings["Path"];

                    string frompath = path;
                    string flname = String.Format("{0:MMddyyyy}", DateTime.Today) + ".log";



                    string topath = Directory.GetCurrentDirectory().ToString() + @"\" + flname;
                    //dt=File.GetLastWriteTime(topath);
                    //richTextBox1.Text = dt.ToString();


                    //if (path != null && path.Contains("AppPath"))
                    //{
                    //    var filepath = Path.Combine(
                    //        System.Reflection.Assembly.GetExecutingAssembly().Location,
                    //        path.Replace("AppPath", string.Empty).ToString());

                    //    Console.WriteLine(filepath);
                    //}


                    //if (File.Exists(topath))
                    {
                        File.Delete(topath);
                        File.Copy(frompath, topath);
                    }
                    //else
                    //{
                    //    File.Copy(frompath, topath);
                    //}


                }
                catch (Exception)
                {
                    MessageBox.Show("Másolás nem sikerült");

                }
                richTextBox1.Width = 400;
                richTextBox1.Height = 500;
                ////richTextBox1.LoadFile(@"C:\Users\ge70840\Documents\Visual Studio 2015\Projects\VPRFORALL\WindowsFormsApplication1\bin\Debug\teszt.txt");
                //richTextBox1.Text = File.ReadAllText(@"C:\Users\ge70840\Documents\Visual Studio 2015\Projects\VPRFORALL\WindowsFormsApplication1\bin\Debug\08202017.log", Encoding.Default);
                MailHelper.SendMailMessage("VPRallomaseger@bosch.com", "adrian.simon2@bosch.com", "Vírus Ellenőrzés Történt Egerben", "A J Csarnokban Pendrive ellenőrzés történt " + DateTime.Now.ToString() + " időpontban.Az ellenőrzést végezte: " + textBox1.Text + ". A Vendég neve: "+textBox2.Text+". Látogatás Célja:"+textBox3.Text);

            }
            else
            {
                MessageBox.Show("Kérem vegye ki a Pendrive-ot és töltse ki a szükséges információkat");
            }








            // If you want to be notified when drive is being removed (and be able to cancel it), 
            // set HookQueryRemove to true 
            if (checkBoxAskMe.Checked)
                e.HookQueryRemove = true;
        }

        // Called by DriveDetector after removable device has been unpluged 
        private void OnDriveRemoved(object sender, DriveDetectorEventArgs e)
        {
            // TODO: do clean up here, etc. Letter of the removed drive is in e.Drive;

            // Just add report to the listbox
            string s = "Drive removed " + e.Drive;
            listBox1.Items.Add(s);
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

        // Called by DriveDetector when removable drive is about to be removed
        private void OnQueryRemove(object sender, DriveDetectorEventArgs e)
        {
            // Should we allow the drive to be unplugged?
            if (checkBoxAskMe.Checked)
            {
                if (MessageBox.Show("Allow remove?", "Query remove",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    e.Cancel = false;       // Allow removal
                else
                    e.Cancel = true;        // Cancel the removal of the device  
            }
        }
        private void checkBoxAskMe_CheckedChanged(object sender, EventArgs e)
        {

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

        private void button1_Click(object sender, EventArgs e)
        {
            //List<string> values = new List<string>();
            //foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            //{
            //    values.Add(nic.Name);
            //    listBox2.Items.Add(nic.Name);
            //}
            //for (int i = 0; i < values.Count ; i++)
            //{
            //    Disable(values[i]);
            //}
            //Disable(values[2]);
            //Console.ReadKey();
            //Enable(values[1]);
            //Console.ReadKey();

        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            //Disable(listBox2.SelectedItem.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<string> values = new List<string>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                values.Add(nic.Name);
            }
            for (int i = 0; i < values.Count; i++)
            {
                Enable(values[i]);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MailHelper.SendMailMessage("VPRallomaseger@bosch.com", "adrian.simon2@bosch.com", "Vírus Ellenőrzés Történt Egerben","A J Csarnokban Pendrive ellenőrzés történt "+DateTime.Now.ToString()+ " időpontban.Az ellenőrzést végezte: "+textBox1.Text);
            MessageBox.Show("Üzenet elküldve");
            //Eredeti config
            //MailHelper.SendMailMessage("adrian.simon2@bosch.com", "adrian.simon2@bosch.com", "adrian.simon2@bosch.com", "adrian.simon2@bosch.com", "Sample Subject", "Sample body of text for mail message");
        }
    }
}
