using System;
using System.Threading;

namespace ParseVRX
{
    class Program
    {
        static void Main(string[] args)
        {
            VRX vrxThread;

            string web; // строка страницы
            string findfolders; int page;  // Параметры ПОСТ запроса
            string vtorichka = "vrx_Vtorichka";
            string novostrojki = "vrx_Novostrojki";
            string nezhil = "vrx_Nezhil";
            string doma_kottedzhi = "vrx_DomaKottedzhi";
            string uchastki = "vrx_Uchastki";
            string garazhi = "vrx_Garazhi";

            /*
            
            
            */

            Console.WriteLine("1. Что бы парсить www.ksota.ru вбейте: '1' и нажмите Enter");
            Console.WriteLine("2. Что бы парсить www.vrx.ru вбейте: '2' и нажмите Enter");
            Console.WriteLine("---");
            Console.Write("Выберите нужный вариант: ");

            /*
            VRX.ConsoleWriteLine("1. Что бы парсить www.ksota.ru вбейте: '1' и нажмите Enter", VRX.left, VRX.top);
            VRX.ConsoleWriteLine("2. Что бы парсить www.vrx.ru вбейте: '2' и нажмите Enter", VRX.left, VRX.top);
            VRX.ConsoleWriteLine("---", VRX.left, VRX.top);
            VRX.ConsoleWriteLine("Выберите нужный вариант: ", VRX.left, VRX.top);
            */

            string parseWeb = Console.ReadLine();


            if (parseWeb == "1")
            {
                /*

                web = "http://www.ksota.ru/catalog/flat/";
                vrxThread vrxThreadOther = new vrxThread(web);

                */
            }
            else if (parseWeb == "2")
            {
                // получение сегодняшней даты и времени
                string strDate = DateTime.Now.ToString();
                string[] datetime = strDate.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] date = datetime[0].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                string[] time = datetime[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                int consoleLogTop = 6;
                web = "http://www.vrx.ru/data/base.php?city=36&apptype=1";
                page = 1;



                //------------------- ВТОРИЧКА ---------------------

                Console.WriteLine();
                Console.WriteLine("Начало парсинга ВТОРИЧКИ: " + strDate);

                findfolders = "1";
                vtorichka += "_" + date[2] + "_" + date[1] + "_" + date[0]+".csv";
                vrxThread = new VRX(web,findfolders,page, vtorichka, consoleLogTop); //парсим

                // получение сегодняшней даты и времени
                strDate = DateTime.Now.ToString();
                Console.SetCursorPosition(0, consoleLogTop+1);
                Console.WriteLine("Окончание парсинга ВТОРИЧКИ: " + strDate);



                //------------------- НОВОСТРОЙКИ ---------------------

                Console.SetCursorPosition(0, consoleLogTop + 2);//+1
                Console.WriteLine("");
                Console.WriteLine("Начало парсинга НОВОСТРОЕК: " + strDate);

                findfolders = "2";
                novostrojki += "_" + date[2] + "_" + date[1] + "_" + date[0] + ".csv";
                vrxThread = new VRX(web, findfolders, page, novostrojki, consoleLogTop+4); //+2 //парсим

                // получение сегодняшней даты и времени
                strDate = DateTime.Now.ToString();
                Console.SetCursorPosition(0, consoleLogTop + 5); //+1
                Console.WriteLine("Окончание парсинга НОВОСТРОЕК: " + strDate);



                //------------------- НЕЖИЛОЕ ---------------------

                Console.SetCursorPosition(0, consoleLogTop + 6); //+1
                Console.WriteLine("");
                Console.WriteLine("Начало парсинга НЕЖИЛЫХ: " + strDate);

                findfolders = "3";
                nezhil += "_" + date[2] + "_" + date[1] + "_" + date[0] + ".csv";
                vrxThread = new VRX(web, findfolders, page, nezhil, consoleLogTop + 8); //+2

                // получение сегодняшней даты и времени
                strDate = DateTime.Now.ToString();
                Console.SetCursorPosition(0, consoleLogTop + 9); //+1
                Console.WriteLine("Окончание парсинга НЕЖИЛЫХ: " + strDate);



                //------------------- Дома и коттеджи ---------------------

                Console.SetCursorPosition(0, consoleLogTop + 10); //+1
                Console.WriteLine("");
                Console.WriteLine("Начало парсинга ДОМА и КОТТЕДЖИ: " + strDate);

                findfolders = "4";
                doma_kottedzhi += "_" + date[2] + "_" + date[1] + "_" + date[0] + ".csv";
                vrxThread = new VRX(web, findfolders, page, doma_kottedzhi, consoleLogTop + 12); //+2

                // получение сегодняшней даты и времени
                strDate = DateTime.Now.ToString();
                Console.SetCursorPosition(0, consoleLogTop + 13); //+1
                Console.WriteLine("Окончание парсинга ДОМА и КОТТЕДЖИ: " + strDate);



                //------------------- Участки ---------------------

                Console.SetCursorPosition(0, consoleLogTop + 14); //+1
                Console.WriteLine("");
                Console.WriteLine("Начало парсинга УЧАСТКИ: " + strDate);

                findfolders = "5";
                uchastki += "_" + date[2] + "_" + date[1] + "_" + date[0] + ".csv";
                vrxThread = new VRX(web, findfolders, page, uchastki, consoleLogTop + 16); //+2

                // получение сегодняшней даты и времени
                strDate = DateTime.Now.ToString();
                Console.SetCursorPosition(0, consoleLogTop + 17); //+1
                Console.WriteLine("Окончание парсинга УЧАСТКИ: " + strDate);



                //------------------- Гаражи ---------------------

                Console.SetCursorPosition(0, consoleLogTop + 18); //+1
                Console.WriteLine("");
                Console.WriteLine("Начало парсинга ГАРАЖИ: " + strDate);

                findfolders = "6";
                garazhi += "_" + date[2] + "_" + date[1] + "_" + date[0] + ".csv";
                vrxThread = new VRX(web, findfolders, page, uchastki, consoleLogTop + 20); //+2

                // получение сегодняшней даты и времени
                strDate = DateTime.Now.ToString();
                Console.SetCursorPosition(0, consoleLogTop + 21); //+1
                Console.WriteLine("Окончание парсинга ГАРАЖИ: " + strDate);
            }
            else
            {
                Console.WriteLine("Вы не выбрали ни чего из выше перечисленного. Программа завершает работу.");
            }
            Console.WriteLine("Запросов в Yandex за геопозицией: " + VRXParsePage.countYandex);

            Console.ReadLine();
        }
    }
}
