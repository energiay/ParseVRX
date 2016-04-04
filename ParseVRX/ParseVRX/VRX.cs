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

        public VRX (string web)
        {
            pageParse = web;
            //Thread thWatch = new Thread();
            VRXParse vrx = new VRXParse( pageParse );
        }
    }
}
