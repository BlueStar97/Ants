using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Timers;

namespace Ants
{
    public partial class Form1 : Form
    {
        String path = "", perpre="", perram="";
        FolderBrowserDialog src = new FolderBrowserDialog();
        Socket receive;
        IPAddress addr;
        int port;
        byte[] bytesIP;
        int bytesRecIP;
        byte[] bytesPort;
        int bytesRecPort;
        FileStream fs;
        String selector;
        Processo proc;
        private System.Timers.Timer mError;

        public delegate void addlisting(object o, ElapsedEventArgs e);
        public event addlisting addlistview;


        public Form1()
        {
            InitializeComponent();
            src.Description = "Seleziona la cartella download predefinita:";
            src.RootFolder = Environment.SpecialFolder.MyComputer;
            notifyIcon1.BalloonTipText = "La finestra è stata minimizzata.";
            notifyIcon1.BalloonTipTitle = "Ants";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.Icon = new Icon(Directory.GetCurrentDirectory() + @"\antEater.ico");
            notifyIcon1.Visible = true;
            this.TopMost = true;
            bytesIP = new byte[4096];
            bytesRecIP = 0;
            bytesPort = new byte[4096];
            bytesRecPort = 0;
            selector = "";
            try
            {
                addr = Dns.GetHostAddresses("ec2-54-218-1-173.us-west-2.compute.amazonaws.com")[0];
            }
            catch(SocketException e)
            {
                
            }
            port = 3355;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            #region Update Black List
            receive = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serveradd = new IPEndPoint(addr, port);
            try
            {
                receive.Connect(serveradd);
                bytesRecIP = receive.Receive(bytesIP);
                bytesRecPort = receive.Receive(bytesPort);
                receive.Shutdown(SocketShutdown.Both);
                receive.Close();

                String oldIP = File.ReadAllText(Directory.GetCurrentDirectory() + @"\addresses.txt");

                if (oldIP.Length != bytesIP.ToString().Length)
                {
                    byte[] newIP = new byte[4096];
                    using (fs = File.OpenWrite(Directory.GetCurrentDirectory() + @"\addresses.txt"))
                    {
                        Array.Copy(bytesIP, oldIP.Length, newIP, 0, bytesIP.ToString().Length - oldIP.Length);
                        fs.Write(newIP, 0, bytesIP.ToString().Length - oldIP.Length);
                    }
                }

                String oldPort = File.ReadAllText(Directory.GetCurrentDirectory() + @"\ports.txt");

                if (oldPort.Length != bytesPort.ToString().Length)
                {
                    byte[] newPort = new byte[4096];
                    using (fs = File.OpenWrite(Directory.GetCurrentDirectory() + @"\ports.txt"))
                    {
                        Array.Copy(bytesPort, oldPort.Length, newPort, 0, bytesPort.ToString().Length - oldPort.Length);
                        fs.Write(newPort, 0, bytesPort.ToString().Length - oldPort.Length);
                    }
                }
            }
            catch (SocketException ss)
            { }
            #endregion

            #region Initialization data

            if (File.Exists(Directory.GetCurrentDirectory() + @"\infos.txt"))
            {
                String[] str = File.ReadAllText(Directory.GetCurrentDirectory() + @"\infos.txt").Split(',');

                int check1, check2;

                if ((int.TryParse(str[0], out check1)) && (int.TryParse(str[1], out check2)) && (Directory.Exists(str[2])))
                {
                    perpre = str[0];
                    perram = str[1];
                    path = str[2];
                }
                else
                {
                    File.Delete(Directory.GetCurrentDirectory() + @"\infos.txt");
                }
            }

            if (!File.Exists(Directory.GetCurrentDirectory() + @"\infos.txt"))
            {
                if (src.ShowDialog() == DialogResult.OK)
                    path = src.SelectedPath;
                else
                {
                    System.Windows.Forms.MessageBox.Show("Errore nella selezione del Folder dei Download.");
                    Application.Exit();
                }
                Percentuali p = new Percentuali();
                p.TopMost = true;
                p.ShowDialog(this);

                perpre = p.Perpre;
                perram = p.Perram;

                FileStream fs = new FileStream(Directory.GetCurrentDirectory() + @"\infos.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);

                fs.Write(Encoding.ASCII.GetBytes(perpre + "," + perram + "," + path), 0, Encoding.ASCII.GetBytes(perpre + "," + perram + "," + path).Length);
            }

            mError = new System.Timers.Timer(10000);
            mError.AutoReset = true;
            mError.Elapsed += Proc_addlist;
            mError.Start();

            proc = new Processo(this, addr, path, int.Parse(perram), int.Parse(perpre));
            addlistview += Proc_addlist;
            #endregion
        }

        public void Proc_addlist(object o, ElapsedEventArgs e)
        {
            List<ListViewItem> add = new List<ListViewItem>();

            foreach (ListViewItem item in listView1.Items)
            {
                item.Remove();
            }

            add = proc.addvalue();

            for (int i = 0; i < add.Count; i++)
            {
                listView1.Items.Add(add[i]);
            }

            //listView1 = this.adding(add);
        }

        public ListView adding(List<ListViewItem> ll)
        {
            ListView res = new ListView();
            
            return res;
        }

        #region CloseMinimize Handler
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(500);
            this.Hide();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }

        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.TopMost = true;
        }

        private void killdelete_Click(object sender, EventArgs e)
        {
            if (this.Text != "Uccidi/Elimina" && selector != "")
            {
                if (this.Text == "Uccidi")
                {
                    Process[] tmp = Process.GetProcesses();
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (tmp[i].Id == int.Parse(selector))
                        {
                            tmp[i].Kill();
                        }
                    }
                }
                else if (this.Text == "Elimina")
                {
                    File.Delete(selector);
                }

                foreach (ListViewItem itemRow in listView1.Items)
                {
                    if (itemRow.SubItems[0].Text == selector)
                    {
                        itemRow.Remove();
                    }
                }
                this.Text = "Uccidi/Elimina";
                selector = "";
            }
        }

        private void control_Click(object sender, EventArgs e)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                proc.WholeCheck(d.Name);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.TopMost = true;
        }
        #endregion

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selector = listView1.SelectedItems[0].Text;
            switch (int.Parse(selector[0].ToString()))
            {
                case 0:
                    killdelete.Text = "Uccidi";
                    break;
                case 1:
                    killdelete.Text = "Uccidi";
                    break;
                case 2:
                    killdelete.Text = "Uccidi";
                    break;
                case 3:
                    killdelete.Text = "Uccidi";
                    break;
                case 4:
                    killdelete.Text = "Uccidi";
                    break;
                case 5:
                    killdelete.Text = "Uccidi";
                    break;
                case 6:
                    killdelete.Text = "Uccidi";
                    break;
                case 7:
                    killdelete.Text = "Uccidi";
                    break;
                case 8:
                    killdelete.Text = "Uccidi";
                    break;
                case 9:
                    killdelete.Text = "Uccidi";
                    break;
                default:
                    killdelete.Text = "Elimina";
                    break;
            }
        }
    }
}
