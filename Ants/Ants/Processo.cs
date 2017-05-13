using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Ants
{
    class Processo
    {
        #region Creation of Variables and Properties
        //Processes Before and Now
        private Process[] mStart;
        private Process[] mNow;

        //Checking increasing RAM relatively to mStart and mNow
        private float mIncrease;

        //Manage RAM Usage
        private RAMUsage mRam;

        //Timer to do the checking each X milliseconds
        private Timer mChecking;

        //connection established
        private Process cmd;
        private ProcessStartInfo startInfo;

        //Initializing
        public Processo()
        {
            Start = Process.GetProcesses();
            Now = Start;

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
            Conn connect = new Conn();
            Conn addConn = connect;

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

            for (int i = 4; i < rows.Length+4; i++)
            {
                tmp3 = rows[i].Split(' ');
                addConn.IdProc = int.Parse(tmp3[4]);
                sock = tmp3[2].Split(':');
                addConn.IP = sock[0];
                addConn.Port = sock[1];
                addConn = addConn.Next;
            }

            //salvare in lista di più stringhe per poi fare il checking

            #endregion
        }
        #endregion
    }
}
