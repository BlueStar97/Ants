using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Ants
{
    class Processo
    {
        #region Creation of Variables and Properties
        //Processes Before and Now
        private Process[] mStart;
        private Process[] mNow;

        //download Path
        private String downloadPath;

        //Checking increasing RAM relatively to mStart and mNow
        private float mIncrease;

        //Manage RAM Usage
        private RAMUsage mRam;

        //Timer to do the checking each X milliseconds
        private System.Timers.Timer mChecking, mDownloads;

        //connection established
        private Process cmd;
        private ProcessStartInfo startInfo;

        private IPAddress serverIP;
        private int ShaPort;

        //Declaring error list and adding for adding new errors
        Error List;
        Error adding;

        Form1 frm;

        //Initializing
        //perc related to ram, incr related to state
        public Processo(Form1 mfrm, IPAddress ip, String dPath, float perc, float incr)
        {
            //Declaring error list and adding for adding new errors
            List = new Error();
            adding = List;

            Start = Process.GetProcesses();
            Now = Start;

            downloadPath = "";

            serverIP = ip;
            ShaPort = 3356;

            #region cmd to get connections
            cmd = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C netstat -ao";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            cmd.StartInfo = startInfo;
            #endregion

            Ram = new RAMUsage();
            Increase = 1000;
            mChecking = new System.Timers.Timer(5000);
            mChecking.Elapsed += Checks;
            mChecking.AutoReset = true;
            mChecking.Start();
            mDownloads = new System.Timers.Timer(360000);
            mDownloads.Elapsed += newFiles;
            mDownloads.AutoReset = true;
            mDownloads.Start();

            frm = mfrm;
        }

        //Properties
        public Process[] Start
        {
            get { return mStart; }
            set { mStart = value; }
        }

        public Process[] Now
        {
            get { return mNow; }
            set { mNow = value; }
        }

        private RAMUsage Ram
        {
            get { return mRam; }
            set { mRam = value; }
        }

        public float Increase
        {
            get { return mIncrease; }
            set { mIncrease = value; }
        }

        public float Percent
        {
            get { return Ram.Percentage; }
            set { Ram.Percentage = value; }
        }

        private double Timing
        {
            set { mChecking.Interval = value; }
        }

        #endregion

        #region Functions
        //Function used for checking
        private void Checks(object a, ElapsedEventArgs e)
        {

            //Creating the list of connections
            String IP = "", Port = "";

            //Using Garbage collector to wipe unused data
            GC.Collect();

            //Switching processes
            Start = Now;
            Now = Process.GetProcesses();


            #region RAM Checking
            //Declaring processes tmp
            Process tmp1, tmp2 = null;

            Error tmp = List;
            bool found = false;

            for (int i = 0; i < Now.Length; i++)
            {
                tmp1 = Now[i];
                int c = 0;
                for (tmp2 = Start[c]; (c < Start.Length) && ((tmp1.ProcessName).Equals(tmp2.ProcessName)); c++)
                {

                    //checking if the process from beginning increased more than the percentage
                    if ((float)(tmp2.WorkingSet64) * (Increase + 1) < (float)(tmp1.WorkingSet64))
                    {
                        while (tmp.Message != "")
                        {
                            if (tmp.Message == "percentage_from_start_error" && tmp.Proc.Id == tmp1.Id)
                                found = true;
                            tmp = tmp.Next;
                        }
                        if (!found)
                            Error.add(adding, tmp1, "percentage_from_start_error");
                    }
                }

                //checking if the process uses more than the percentage allowed in RAM
                if ((float)(tmp1.WorkingSet64) > (float)(Ram.TotMemory) * Ram.Percentage)
                {
                    found = false;
                    tmp = List;

                    while (tmp.Message != "" && !found)
                    {
                        if (tmp.Message == "percentage_from_total_ram_error" && tmp.Proc.Id == tmp1.Id)
                        {
                            found = true;
                        }
                        tmp = tmp.Next;
                    }
                    if (!found)
                        Error.add(adding, tmp1, "percentage_from_total_ram_error");
                }
                tmp2 = null;
            }
            #endregion


            #region Connection Checking
            cmd.Start();
            String data = cmd.StandardOutput.ReadToEnd();
            cmd.WaitForExit();

            String[] rows = data.Split('\n');
            String[] tmp3, sock;


            String[] addresses = File.ReadAllText(Directory.GetCurrentDirectory() + @"\addresses.txt").Split(',');


            String[] ports = File.ReadAllText(Directory.GetCurrentDirectory() + @"\ports.txt").Split(',');

            bool notFound = true;

            for (int i = 4; i < rows.Length - 1; i++)
            {
                tmp3 = rows[i].Split(' ');
                sock = tmp3[6].Split(':');
                IP = sock[0];
                Port = sock[1];

                for (int c = 0; c < addresses.Length; c++)
                {
                    if (IP == addresses[c])
                    {
                        notFound = true;

                        for (int h = 0; (h < Now.Length) && (notFound); h++)
                        {
                            if (Now[h].Id == int.Parse(tmp3[4]))
                            {
                                Error.add(adding, Now[h], "IP_address_in_BL");
                                notFound = false;
                            }
                        }
                    }
                }

                for (int c = 0; c < ports.Length; c++)
                {
                    if (Port == ports[c])
                    {
                        notFound = true;

                        for (int h = 0; (h < Now.Length) && (notFound); h++)
                        {
                            if (Now[h].Id == int.Parse(tmp3[4]))
                            {
                                Error.add(adding, Now[h], "Port_in_BL");
                                notFound = false;
                            }
                        }
                    }
                }
            }

            #endregion

            //generazione evento per riportare tutti gli errori
        }

        private void newFiles(object a, ElapsedEventArgs e)
        {
            String[] Files = Directory.GetFiles(downloadPath);

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(new IPEndPoint(serverIP, ShaPort));

            byte[] check = new byte[16];

            for (int i = 0; i < Files.Length; i++)
            {
                if ((File.GetCreationTime(downloadPath + Files[i]).Day == DateTime.Now.Day) && (File.GetCreationTime(downloadPath + Files[i]).Month == DateTime.Now.Month) && (File.GetCreationTime(downloadPath + Files[i]).Year == DateTime.Now.Year))
                {
                    SHA256 nowsha = SHA256Managed.Create();
                    byte[] hashValue;

                    FileStream fStream = File.OpenRead(downloadPath + Files[i]);
                    fStream.Position = 0;
                    hashValue = nowsha.ComputeHash(fStream);
                    sock.Send(hashValue);
                    sock.Receive(check);
                    if (check.ToString() == "found")
                    {
                        Error.add(adding, null, "dangerousfile_" + downloadPath + Files[i]);
                    }
                }
            }
            sock.Send(Encoding.ASCII.GetBytes("finished"));

            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }

        public void InitCheck()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                this.WholeCheck(d.ToString());
            }
        }

        public void WholeCheck(String path)
        {

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(new IPEndPoint(serverIP, ShaPort));

            subcheck(sock, path);
            
            sock.Send(Encoding.ASCII.GetBytes("finished"));

            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }

        private void subcheck(Socket sock, String path)
        {
            String[] Files = Directory.GetFiles(path);
            byte[] check = new byte[16];

            for (int i = 0; i < Files.Length; i++)
            {
                if ((File.GetCreationTime(path + @"\" + Files[i]).Day == DateTime.Now.Day) && (File.GetCreationTime(path + @"\" + Files[i]).Month == DateTime.Now.Month) && (File.GetCreationTime(path + @"\" + Files[i]).Year == DateTime.Now.Year))
                {
                    SHA256 nowsha = SHA256Managed.Create();
                    byte[] hashValue;

                    FileStream fStream = File.OpenRead(downloadPath + Files[i]);
                    fStream.Position = 0;
                    hashValue = nowsha.ComputeHash(fStream);
                    sock.Send(hashValue);
                    sock.Receive(check);
                    if (check.ToString() == "found")
                    {
                        Error.add(adding, null, "dangerousfile_" + downloadPath + Files[i]);
                    }
                }
            }

            String[] dirs = Directory.GetDirectories(path);

            for (int i = 0; i < Files.Length; i++)
            {
                subcheck(sock, path + dirs[i]);
            }
        }

        private void remove(String prob)
        {
            adding = List;
            switch (int.Parse(prob[0].ToString()))
            {
                case 0:
                    Error.StartRemoveP(adding, int.Parse(prob));
                    break;
                case 1:
                    Error.StartRemoveP(adding, int.Parse(prob));
                    break;
                case 2:
                    Error.StartRemoveP(adding, int.Parse(prob));
                    break;
                case 3:
                    Error.StartRemoveP(adding, int.Parse(prob));
                    break;
                case 4:
                    Error.StartRemoveP(adding, int.Parse(prob));
                    break;
                case 5:
                    Error.StartRemoveP(adding, int.Parse(prob));
                    break;
                case 6:
                    Error.StartRemoveP(adding, int.Parse(prob));
                    break;
                case 7:
                    Error.StartRemoveP(adding, int.Parse(prob));
                    break;
                case 8:
                    Error.StartRemoveP(adding, int.Parse(prob));
                    break;
                case 9:
                    Error.StartRemoveP(adding, int.Parse(prob));
                    break;
                default:
                    Error.StartRemoveF(adding, prob);
                    break;

            }
        }

        public List<ListViewItem> addvalue()
        {
            adding = List;

            List<ListViewItem> ritorno = new List<ListViewItem>();
            ListViewItem s = new ListViewItem();

            while (adding.Message != "")
            {
                if (adding.Proc != null)
                {
                    s = new ListViewItem(adding.Proc.Id.ToString());
                    
                    switch(adding.Message)
                    {
                        case "percentage_from_start_error":
                            s.SubItems.Add("Memoria usata superata confrontando a 2 secondi prima");
                            break;
                        case "percentage_from_total_ram_error":
                            s.SubItems.Add("Memoria usata superata confrontando con la memoria totale");
                            break;
                        case "IP_address_in_BL":
                            s.SubItems.Add("Connessione ad IP sconsigliato");
                            break;
                        case "Port_in_BL":
                            s.SubItems.Add("Connessione tramite una Porta sconsigliata");
                            break;
                    }
                }
                else
                {
                    s = new ListViewItem(adding.Message);
                    s.SubItems.Add("Digest del file trovato nel database");
                }


                ritorno.Add(s);
                
                adding = adding.Next;
            }
            return ritorno;
        }
        #endregion
    }
}