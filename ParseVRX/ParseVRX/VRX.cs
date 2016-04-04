using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ParseVRX
{
    class VRX
    {
        string pageParse;
        int threadCount;
        List<Thread> thList;

        public VRX (string web)
        {
            threadCount = 1;
            pageParse = web;
            //Thread thWatch = new Thread();
            VRXParse vrx = new VRXParse( pageParse );
        }


        public void Run(object url)
        {

        }


        public void ThreadVRX ()
        {
            for (int i = 0; i < threadCount; i++)
            {
                thList.Add( new Thread( new ParameterizedThreadStart(Run)) );
                thList[thList.Count - 1].Name = "Thread#" + i;
                thList[thList.Count - 1].Start(url);
            }

            while ()
            {

            }
        }
    }
}
