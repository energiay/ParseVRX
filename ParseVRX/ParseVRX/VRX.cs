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
        string pageParse;
        int threadCount;
        string findfoldersParse;
        int countPageParse;
        List<Thread> thList = new List<Thread>();

        public VRX (string web, string findfolders, int page)
        {
            threadCount = 1;
            pageParse = web;
            findfoldersParse = findfolders;
            countPageParse = page;
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

            for (int i = 0; i < threadCount; i++)
            {
                thList.Add( new Thread( new ParameterizedThreadStart(Run)) );
                thList[thList.Count - 1].Name = "Thread#" + i;
                thList[thList.Count - 1].Start( new VRXParse(findfoldersParse, countPageParse) );
                VRXParse.GetNextPage();
            }


            /*while (countPageParse <= VRXParse.countPageAll)
            {
                for (int i = 0; i < threadCount; i++)
                {
                    
                    
                    if (thList[0])
                    {

                    }
                    
                    
                }
            }*/


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
