using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ants
{
    
    
    class RAMUsage
    {
        //Importing DLL kernel32.dll, needed for getting the total physical memory
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        private float mPercentage;
        private long mTotMemory;

        public RAMUsage()
        {
            Percentage = 1;
            GetPhysicallyInstalledSystemMemory(out mTotMemory);
        }

        public long TotMemory
        {
            get { return mTotMemory; }
            set { mTotMemory = value; }
        }

        public float Percentage
        {
            get { return mPercentage; }
            set { if ((value >= 0) && (value <= 100)) mPercentage = value; }
        }
    }
}