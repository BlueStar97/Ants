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

        public static Error add(Error first, Process pr, String m)
        {
            first.Proc=pr;
            first.Message = m;
            first.Next = new Error();
            first = first.Next;
            return first;
        }

        public static Error StartRemoveP(Error first, int IdProc)
        {
            if (first.mProc.Id == IdProc)
            {
                first = first.Next;
                GC.Collect();
                return Error.StartRemoveP(first, IdProc);
            }
            else
            {
                Error ffirst = first;
                Error.RemoveP(ffirst, IdProc);
                GC.Collect();
                return first;
            }
        }

        public static void RemoveP(Error first, int IdProc)
        {
            if (first.Next.Message != "")
            {
                if (first.Next.mProc != null)
                {
                    if (first.Next.mProc.Id == IdProc)
                    {
                        if (first.Next.Next.Message != "")
                        {
                            first.Next = first.Next.Next;
                            Error.RemoveP(first.Next, IdProc);
                        }
                        else
                        {
                            first.Next = new Error();
                        }
                    }
                    else
                    {
                        first = first.Next;
                        Error.RemoveP(first.Next, IdProc);
                    }
                }
                else
                {
                    first = first.Next;
                    Error.RemoveP(first.Next, IdProc);
                }
            }
        }

        public static Error StartRemoveF(Error first, String path)
        {
            if (first.Message == path)
            {
                first = first.Next;
                GC.Collect();
                return Error.StartRemoveF(first.Next, path);
            }
            else
            {
                Error ffirst = first;
                Error.RemoveF(ffirst, path);
                GC.Collect();
                return first;
            }
        }

        public static void RemoveF(Error first, String path)
        {
            if (first.Next.Message != "")
            {
                if (first.Next.Message == path)
                {
                    if (first.Next.Next.Message != "")
                    {
                        first.Next = first.Next.Next;
                        Error.RemoveF(first.Next, path);
                    }
                    else
                    {
                        first.Next = new Error();
                    }
                }
                else
                {
                    first = first.Next;
                    Error.RemoveF(first.Next, path);
                }
            }
        }
    }
}
