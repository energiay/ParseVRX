using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace ParseVRX
{
    class vrxThread
    {
        int countThread; //кол-во потоков
        Parse parse = new Parse("http://www.ksota.ru/catalog/flat/");
        Thread thParse; //потки
        List<Thread> thList = new List<Thread>();

        public vrxThread()
        {
            countThread = 3;

            Thread watching = new Thread(Watching);
            watching.Name = "Watching";
            watching.Start();

        }

        public void Run(object url)
        {
            /*
            File.WriteAllText("ksota.csv", parse.Utf8ToWin1251("sep=|\n"), Encoding.GetEncoding("windows-1251"));

            string head = "Операция (аренда/Продажа)|дата публикации|заголовок|тип квартиры|общая площадь|цена|материал стен" + "\n";
            File.AppendAllText("ksota.csv", parse.Utf8ToWin1251(head), Encoding.GetEncoding("windows-1251"));

            //Загрузка первой стр. HTML в текстовый файл (C:\ksota.txt)
            url = "http://www.ksota.ru/catalog/flat/";
            parse.Download( (string)url, "ksota.txt" ); //???????????????????????????????????????????????

            parse.GetContent();
            */

            Thread.Sleep(120);
            
        }

        public void Watching()
        {
            string urlParse = "http://www.ksota.ru/catalog/flat/?p=";

            // создание потоков
            for (int i = 0; i < countThread; i++)
            {
                thList.Add( new Thread( new ParameterizedThreadStart(Run) ) );
                thList[thList.Count - 1].Name = "Thread" + i;
                thList[thList.Count - 1].Start(parse.GetUrl(urlParse));
            }


            // Пока страницы существуют, мы передаем ссылки освободившимся потокам
             while (Parse.countUrl < Parse.countUrlAll)
             {
                for (int i = 0; i < thList.Count; i++)
                {
                    //Console.WriteLine(thList[i].Name + " = " +thList[i].ThreadState );
                    if (thList[i].ThreadState == "Stopped")
                    {
                        b = true;
                    }
                }
             }
        }
    }
}
