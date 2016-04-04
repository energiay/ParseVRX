﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ParseVRX
{
    class VRX
    {
        string pageParse;
        int threadCount;
        List<Thread> thList = new List<Thread>();

        public VRX (string web)
        {
            threadCount = 1;
            pageParse = web;
            //Thread thWatch = new Thread();
            VRXParse vrx = new VRXParse( pageParse );
            //ThreadVRX();
        }


        public void Run(object url)
        {
            VRXParse parse = (VRXParse)url;

            parse.Download( parse.GetUrl() );
            
        }


        public void ThreadVRX ()
        {

            // Потготовка CSV файла для записи
            File.WriteAllText("vrx_all.csv", VRXParse.Utf8ToWin1251("sep=;\n"), Encoding.GetEncoding("windows-1251"));
            string head = "Объект;Район;Место;Адрес;Площадь(общ);Этаж;Этажей" + "\n";
            File.AppendAllText("vrx_all.csv", VRXParse.Utf8ToWin1251(head), Encoding.GetEncoding("windows-1251"));


            for (int i = 0; i < threadCount; i++)
            {
                thList.Add( new Thread( new ParameterizedThreadStart(Run)) );
                thList[thList.Count - 1].Name = "Thread#" + i;
                thList[thList.Count - 1].Start( new VRXParse() );
            }


            while (VRXParse.countPage < VRXParse.countPageAll)
            {
                for (int i = 0; i < threadCount; i++)
                {
                    /* 
                    
                    if ()
                    {

                    }
                    
                    */
                }
            }


        }
    }
}
