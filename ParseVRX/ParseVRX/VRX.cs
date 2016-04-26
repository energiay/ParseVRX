using System;
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
        int top=4;
        int left=0;

        string pageParse;
        int threadCount;
        string findfoldersParse;
        List<Thread> thList = new List<Thread>();

        public VRX (string web, string findfolders, int page)
        {
            threadCount = 20;
            pageParse = web;
            findfoldersParse = findfolders;
            //Thread thWatch = new Thread();
            VRXParse vrx = new VRXParse( pageParse, findfolders, page );
            ThreadVRX();
        }


        public void Run(object url)
        {
            VRXParse parse = (VRXParse)url;
            parse.Download();
            parse.GetContentVip();
            parse.GetContent();
        }


        public void ThreadVRX ()
        {

            // Потготовка CSV файла для записи
            //File.WriteAllText("vrx_all.csv", VRXParse.Utf8ToWin1251("sep=;\n"), Encoding.GetEncoding("windows-1251"));
            string head = "Объект;Район;Место;Адрес;Площадь(Общ);Этаж;Этажей;Материал;Тип;Участок(соток);Цена;Контакты;Дата;Текст;Координаты;Заголовок;Optional;Rayon;image1;image2;image3;image4;image5;image6;image7;image8;image9;image10" + "\n";
            File.WriteAllText("vrx_all.csv", Utf8ToWin1251(head), Encoding.GetEncoding("windows-1251"));
            File.WriteAllText("vrx_error.txt", Utf8ToWin1251("Ошибки:"), Encoding.GetEncoding("windows-1251"));
            
            for (int i = 0; i < threadCount; i++)
            {
                VRXParse.GetNextPage();
                thList.Add( new Thread( new ParameterizedThreadStart(Run)) );
                thList[thList.Count - 1].Name = "Thread#" + i;
                thList[thList.Count - 1].Start( new VRXParse(findfoldersParse) );
            }

            ConsoleWriteLine("Всего страниц: "+ VRXParse.countPageAll);


            while (VRXParse.countPageParse <= VRXParse.countPageAll)
            {
                for (int i = 0; i < threadCount; i++)
                {


                     if (thList[i].ThreadState.ToString() == "Stopped")
                     {
                        VRXParse.GetNextPage();
                        thList[i] = new Thread(new ParameterizedThreadStart(Run));
                        thList[i].Name = "Thread#" + i;
                        thList[i].Start(new VRXParse(findfoldersParse));
                    }

                    //Console.WriteLine(thList[i].ThreadState.ToString());

                    /*ConsoleWriteLineClear(thList[i].ThreadState.ToString(), left, top);
                    Thread.Sleep(1000);*/
                    //ConsoleWriteLineClear("Ждите, идет чтение "+ VRXParse.countPageParse.ToString() + " стр.", left, top);
                    //ConsoleWriteLineClear("Прочитано: " + VRXParsePage.count + " объяв. на стр. " + VRXParse.countPageParse.ToString(), VRX.left, VRX.top);

                    if (VRXParse.countPageParse <= VRXParse.countPageAll)
                    {
                        ConsoleWriteLineClear("Идет чтение " + VRXParse.countPageParse.ToString() + " страницы...");
                    }

                    Thread.Sleep(100);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Ждем завершения " + threadCount + " потоков:");

            bool raning = true;
            while (raning)
            {
                raning = false;
                for (int i = 0; i < threadCount; i++)
                {
                    if ( thList[i] != null )
                    {
                        if ((thList[i].ThreadState.ToString() == "Stopped") )
                        {
                            Console.WriteLine("Thread#" + i + " завершил свою работу.");
                            thList[i] = null;
                        }
                    }
                    if (thList[i] != null)
                    {
                        raning = true;
                    }
                }
            }

        }



        
        public void ConsoleWriteLine (string str)
        {
            Console.SetCursorPosition(left, top);
            Console.SetCursorPosition(left, top);
            Console.Write(str);

            top++;
        }

        public void ConsoleWriteLineClear(string str)
        {
            top++;

            Console.SetCursorPosition(0, top);
            Console.Write("                                                         ");

            Console.SetCursorPosition(left, top);
            Console.Write(str);

            top--;
        }

        static public string Utf8ToWin1251(string str)
        {
            Encoding srcEncodingFormat = Encoding.UTF8;
            Encoding dstEncodingFormat = Encoding.GetEncoding("windows-1251");

            byte[] originalByteString = srcEncodingFormat.GetBytes(str);
            byte[] convertedByteString = Encoding.Convert(srcEncodingFormat, dstEncodingFormat, originalByteString);

            return dstEncodingFormat.GetString(convertedByteString);
        }
    }
}
