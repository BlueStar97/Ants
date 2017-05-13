using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ants
{
    class Conn
    {
        private int mIdProc;
        private String mIP;
        private String mPort;
        public Conn Next;

        public Conn()
        {
            IdProc = 0;
            IP = "";
            Port = "";
        }

        public int IdProc
        {
            get { return mIdProc; }
            set { mIdProc = value; }
        }

        public String IP
        {
            get { return mIP; }
            set { mIP = value; }
        }

        public String Port
        {
            get { return mPort; }
            set { mPort = value; }
        }
    }
}
