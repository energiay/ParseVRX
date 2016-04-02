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
            countThread = 30;

            Thread watching = new Thread(Watching);
            watching.Name = "Watching";
            watching.Start();

        }

        public void Run(object url)
        {

            Thread t = Thread.CurrentThread;
            parse.Download( (string)url, t.Name.ToString()+".txt" );

            parse.GetContent(t.Name.ToString() + ".txt");

        }

        public void Watching()
        {
            string urlParse = "http://www.ksota.ru/catalog/flat/?p=";

            // Потготовка CSV файла для записи
            File.WriteAllText("ksota.csv", parse.Utf8ToWin1251("sep=|\n"), Encoding.GetEncoding("windows-1251"));
            string head = "Операция (аренда/Продажа)|дата публикации|заголовок|тип квартиры|общая площадь|цена|материал стен" + "\n";
            File.AppendAllText("ksota.csv", parse.Utf8ToWin1251(head), Encoding.GetEncoding("windows-1251"));

            Console.Write("Страниц прочитано: ");

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
                    if (thList[i].ThreadState.ToString() == "Stopped")
                    {
                        thList[i] = new Thread(new ParameterizedThreadStart(Run));
                        thList[i].Name = "Thread" + i;
                        thList[i].Start(parse.GetUrl(urlParse));
                    }
                }
             }
        }
    }
}
