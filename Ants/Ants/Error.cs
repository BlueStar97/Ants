using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ants
{
    class Error
    {
        private Process mProc;
        private String mMessage;
        private Error mNext;

        public Error()
        {
            Proc = new Process();
            Message = "";
        }

        public Error(Process pr, String str)
        {
            Proc = pr;
            Message = str;
        }

        public Process Proc
        {
            get { return mProc; }
            set { mProc = value; }
        }

        public String Message
        {
            get { return mMessage; }
            set { mMessage = value; }
        }

        public Error Next
        {
            get { return mNext; }
            set { mNext = value; }
        }

        public void add(Process pr, String m)
        {
            Proc = pr;
            Message = m;
        }

        public static Error StartRemove(Error first, int IdProc)
        {
            if (first.mProc.Id == IdProc)
            {
                first = first.Next;
                GC.Collect();
                return Error.StartRemove(first, IdProc);
            }
            else
            {
                first.Remove(IdProc);
                GC.Collect();
                return first;
            }
        }

        public void Remove(int IdProc)
        {
            if (this.Next.mProc != null)
            {
                if (this.Next.mProc.Id == IdProc)
                {
                    if (this.Next.Next.mProc != null)
                    {
                        this.Next = this.Next.Next;
                        this.Remove(IdProc);
                    }
                    this.Next = new Error();
                }
                else
                {
                    this = this.Next;
                    this.Remove(IdProc);
                }
            }
        }
    }
}
