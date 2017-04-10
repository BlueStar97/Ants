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
        //Processes Before and Now
        private Process[] mStart;
        private Process[] mNow;

        //Checking increasing RAM relatively to mStart and mNow
        private float mIncrease;

        //Manage RAM Usage
        private RAMUsage mRam;

        //Timer to do the checking each X milliseconds
        private Timer mChecking;

        //Initializing
        public Processo()
        {
            Start = Process.GetProcesses();
            Now = Start;
            Ram = new RAMUsage();
            Increase = 1000;
            mChecking.Elapsed += RAMCheck;
            mChecking = new Timer(2000);
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

        //Function used for checking
        private void RAMCheck(object a, ElapsedEventArgs e)
        {
            //Using Garbage collector to wipe unused data
            GC.Collect();

            //Switching processes
            Start = Now;
            Now = Process.GetProcesses();

            //Declaring processes tmp
            Process tmp1, tmp2 = null;

            //Declaring error list and adding for adding new errors
            Error List = new Error();
            Error adding = List;

            for (int i = 0; i < Now.Length; i++)
            {
                tmp1 = Now[i];

                for (int c = 0; (c < Start.Length) && ((Start[i].ProcessName).Equals(tmp2.ProcessName)); c++)
                {
                    tmp2 = Start[c];

                    //checking if the process from beginning increased more than the percentage
                    if((float)(tmp2.WorkingSet64)*(Increase+1)<(float)(tmp1.WorkingSet64))
                    {
                        adding.Proc = tmp1;
                        adding.Message = "percentage_from_start_error";
                        adding = adding.Next;
                    }
                }

                //checking if the process uses more than the percentage allowed in RAM
                if ((float)(tmp1.WorkingSet64) > (float)(Ram.TotMemory) * Ram.Percentage)
                {
                    adding.Proc = tmp1;
                    adding.Message = "percentage_from_total_ram_error";
                    adding = adding.Next;
                }

                tmp2 = null;
            }
            mChecking.Interval = 2000;
        }

    }
}
