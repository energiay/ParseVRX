using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;

namespace ParseVRX
{
    class vrxThread
    {
        int count;
        Parse parse = new Parse();
        List<Thread> thList = new List<Thread>();

        public vrxThread()
        {
            count = 1;

            Thread watching = new Thread(Watching);
            watching.Name = "Watching";
            watching.Start();

        }

        public void Run()
        {
            Parse parse = new Parse();

            File.WriteAllText("ksota.csv", parse.Utf8ToWin1251("sep=|\n"), Encoding.GetEncoding("windows-1251"));

            string head = "Операция (аренда/Продажа)|дата публикации|заголовок|тип квартиры|общая площадь|цена|материал стен" + "\n";
            File.AppendAllText("ksota.csv", parse.Utf8ToWin1251(head), Encoding.GetEncoding("windows-1251"));

            //Загрузка первой стр. HTML в текстовый файл (C:\ksota.txt)
            string url = "http://www.ksota.ru/catalog/flat/";
            parse.Download(url);

            //Парсим HTML и возвращаем общее кол-во страниц
            int page = parse.GetContent();

            //Считываем все страницы
            Console.Write("Страниц прочитано: 1 ");
            for (int i = 1; i < page; i++)
            {
                url = "http://www.ksota.ru/catalog/flat/?p=" + i.ToString(); //+"&sq_min=&sq_max=";
                parse.ConnectAndParse(url);
                Console.Write ((i + 1).ToString() + " ");
            }
        }

        public void Watching()
        {
            // создание потоков
            for (int i = 0; i < count; i++)
            {
                thList.Add(new Thread(Run));
                thList[thList.Count - 1].Name = "Thread" + i;
                thList[thList.Count - 1].Start();
            }

            // мониторинг потоков
            /*while (true)
            {

            }*/
        }
    }
}
