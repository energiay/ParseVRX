using System;
using System.Threading;

namespace ParseVRX
{
    class Program
    {
        static void Main(string[] args)
        {
            string web; // строка страницы
            string findfolders; int page;  // Пар-ры ПОСТ запроса

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
                web = "http://www.vrx.ru/data/base.php?city=36&apptype=1";
                findfolders = "1";
                page = 1;
                VRX vrxThread = new VRX(web,findfolders,page);
            }
            else
            {
                Console.WriteLine("Вы не выбрали ни чего из выше перечисленного. Программа завершает работу.");
            }
            Console.WriteLine("EXIT");

            Console.ReadLine();
        }
    }
}
