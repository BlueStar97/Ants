using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using System.IO;

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
        private Timer mChecking, mDownloads;

        //connection established
        private Process cmd;
        private ProcessStartInfo startInfo;

        //Initializing
        public Processo()
        {
            Start = Process.GetProcesses();
            Now = Start;

            downloadPath = "";

            #region cmd for getting connections
            cmd =new Process();
            startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C netstat -ao";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            cmd.StartInfo = startInfo;
            #endregion
            
            Ram = new RAMUsage();
            Increase = 1000;
            mChecking = new Timer(2000);
            mChecking.Elapsed += Checks;
            mChecking.AutoReset = true;
            mChecking.Start();
            mDownloads = new Timer(360000);
            mDownloads.Elapsed += newFiles;
            mDownloads.AutoReset = true;
            mDownloads.Start();
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
            //Declaring error list and adding for adding new errors
            Error List = new Error();
            Error adding = List;

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

            for (int i = 0; i < Now.Length; i++)
            {
                tmp1 = Now[i];

                for (int c = 0; (c < Start.Length) && ((Start[i].ProcessName).Equals(tmp2.ProcessName)); c++)
                {
                    tmp2 = Start[c];

                    //checking if the process from beginning increased more than the percentage
                    if((float)(tmp2.WorkingSet64)*(Increase+1)<(float)(tmp1.WorkingSet64))
                    {
                        adding.add(tmp1,"percentage_from_start_error");
                        
                    }
                }

                //checking if the process uses more than the percentage allowed in RAM
                if ((float)(tmp1.WorkingSet64) > (float)(Ram.TotMemory) * Ram.Percentage)
                {
                    adding.add(tmp1,"percentage_from_total_ram_error");
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

            List<String> addresses = JsonConvert.DeserializeObject<List<String>>(File.ReadAllText(Directory.GetCurrentDirectory() + @"\addresses.json"));
            List<String> ports = JsonConvert.DeserializeObject<List<String>>(File.ReadAllText(Directory.GetCurrentDirectory() + @"\port.json"));

            bool notFound = true;

            for (int i = 4; i < rows.Length+4; i++)
            {
                tmp3 = rows[i].Split(' ');
                //addConn.IdProc = int.Parse(tmp3[4]);
                sock = tmp3[2].Split(':');
                IP = sock[0];
                Port = sock[1];

                for (int c = 0; c < addresses.Count; c++)
                {
                    if (IP == addresses[c])
                    {
                        notFound=true;

                        for (int h = 0; (h < Now.Length) && (notFound); h++)
                        {
                            if (Now[h].Id == int.Parse(tmp3[4]))
                            {
                                adding.add(Now[h], "IP_address_in_BL");
                                adding = adding.Next;
                                notFound = false;
                            }
                        }
                    }
                }

                for (int c = 0; c < ports.Count; c++)
                {
                    if (Port == ports[c])
                    {
                        notFound = true;

                        for (int h = 0; (h < Now.Length) && (notFound); h++)
                        {
                            if (Now[h].Id == int.Parse(tmp3[4]))
                            {
                                adding.add(Now[h], "Port_in_BL");
                                adding = adding.Next;
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

            for (int i = 0; i < Files.Length; i++)
            {
                if ((File.GetCreationTime(downloadPath + Files[i]).Day == DateTime.Now.Day) && (File.GetCreationTime(downloadPath + Files[i]).Month == DateTime.Now.Month) && (File.GetCreationTime(downloadPath + Files[i]).Year == DateTime.Now.Year))
                { 
                    //SHA-256 da applicare
                    //String hash = 
                }
            }
        }
        #endregion

        /*public void foo()
        {
            object l = new object();
            string s = JsonConvert.SerializeObject(l);
            object l2 = JsonConvert.DeserializeObject<object>(s);
        }*/
    }
}
