using System;
using System.Threading;

namespace ParseVRX
{
    class Program
    {
        static void Main(string[] args)
        {

            vrxThread vrxThreadOther = new vrxThread();
            //Thread thvrx = new Thread(vrxThreadOther.Run); // поток для выполнения дочерних потоков
            //thvrx.Start();

            Console.ReadLine();
        }
    }
}
