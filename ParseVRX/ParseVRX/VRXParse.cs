using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xNet;
using HtmlAgilityPack;
using System.IO;
using System.Threading;

namespace ParseVRX
{
    class VRXParse
    {
        public static string pageParse; // Cтрока url
        
        //public static int countPage = 0; // номер страцицы
        public static int countPageParse; // номер страцицы
        public static int countPageAll;   // общее кол-во страниц

        string findfoldersParse;
        //string prePage = "&page="; // префикс для страниц
        
        //string html; // HTML Страницы
        HtmlDocument docPageParse; // Загруженный html листа в класс HtmlDocument

        string html = "";

        static object lockerPage = new object();
        static object lockerSave = new object();
        static object lockerSaveError = new object();
        



        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="web">строка парсинга</param>
        public VRXParse(string web, string findfolders, int page)
        {
            pageParse = web;
            findfoldersParse = findfolders;
            countPageParse = page;

            // Загружаю HTML и передаю ее в класс HtmlDocument
            Download();

            if (html == "error")
            {
                int i = 0; // кол-во попыток для запроса html
                while (html == "error" || i < 10)
                {
                    Download();
                    i++;
                }

                if (html == "error")
                {
                    Thread t = Thread.CurrentThread;
                    t.Abort();
                    SaveError("Не удалось открыть страницу " + pageParse);
                }
            }

            countPageAll = GetPageParse( docPageParse );
            
            //Console.WriteLine( countPageAll );
            countPageParse = 0; // 
        }


        public VRXParse(string findfolders)
        {
            findfoldersParse = findfolders;
        }


        public void Run()
        {
            Download();

            if (html == "error")
            {
                int i = 0; // кол-во попыток для запроса html
                while (html == "error" || i < 10)
                {
                    Download();
                    i++;
                }

                if (html == "error")
                {
                    Thread t = Thread.CurrentThread;
                    t.Abort();
                    SaveError("Не удалось открыть страницу " + pageParse);
                }
            }

            GetContentVip();
            GetContent();
        }


        /// <summary>
        /// Загружаем HTML
        /// </summary>
        /// <param name="url">ссылка, что загружать</param>
        public void Download()
        {
            using (var request = new HttpRequest())
            {
                var reqParams = new RequestParams();
                
                reqParams["viewtype"] = "1";
                reqParams["onpage"] = "100";          // Кол-во записей на странице
                //reqParams["findfolders"] = "(2)";   // Какой раздел отображаем (Вторичка, Новостройка, ...)
                reqParams["findfolders"] = "("+ findfoldersParse + ")";   // Какой раздел отображаем (1-Вторичка, 2-Новостройка, 3-Нежмлое, 4-дома и котеджи, 5-участки, 6-гаражи)
                reqParams["page"] = countPageParse.ToString();            // номер страницы для отображения
                reqParams["findobject"] = "";                             // Объект (не заполняется)

                

                try
                {
                    html = request.Post(pageParse, reqParams).ToString();
                    ReadHtml(html);
                }
                catch
                {
                    html = "error";
                }

                /*
                Console.WriteLine("Загрузка страницы " + pageParse);
                string html = request.Post(pageParse, reqParams).ToString();
                Console.WriteLine("Загрузка страницы окончена");
                Console.WriteLine("");
                */

                

                /*
                Console.WriteLine("Чтение страницы " + pageParse);
                ReadHtml(html);
                Console.WriteLine("Чтение страницы окончена");
                Console.WriteLine("");
                */
                /*
                lock (lockerSave)
                {
                    File.WriteAllText("VRX.txt", html);
                }
                */
            }



        }


        /// <summary>
        /// Считываес HTML в класс
        /// </summary>
        /// <param name="txt">имя текстового файла где хранится HTML</param>
        /// <returns></returns>
        void ReadHtml(string content)
        {
            HtmlDocument doc = new HtmlDocument(); //Создаём экземпляр класса
            doc.LoadHtml(content); //Загружаем в класс (парсер) наш html
            docPageParse = doc;
        }


        /// <summary>
        /// Получаем общее кол-во страниц
        /// </summary>
        /// <returns>Возвращаем общее кол-во страниц</returns>
        int GetPageParse(HtmlDocument doc)
        {          
            string sPage = "0";     //кол-во страниц

            HtmlNodeCollection pageNodes = doc.DocumentNode.SelectNodes("//div[@id='my_pages_btm']/a");

            // В этом диве ( div[@id='my_pages_btm'] ) смотрим все a
            foreach (var item in pageNodes)
            {
                // если в теге A текст = "Следующая" то получаем прошлую A
                if ( (item.InnerText).IndexOf("Следующая") > -1 )
                {
                    return Convert.ToInt32(sPage);
                }
                sPage = item.InnerText;
            }

            return 0;
        }


        /// <summary>
        /// След.страница
        /// </summary>
        public static void GetNextPage()
        {
            lock (lockerPage)
            {
                //string str = "Страниц прочитано: " + countPageParse + " из " + countPageAll;
                //VRX.ConsoleWriteLine(str, VRX.left, VRX.top);
                //Console.WriteLine("Страниц прочитано: " + countPageParse +" из "+ countPageAll);
                countPageParse++;
            }
        }


        /// <summary>
        /// Сохраняем запись
        /// </summary>
        void SaveRecord(string saveRecord)
        {
            lock (lockerSave)
            {
                File.AppendAllText("vrx_all.csv", VRX.Utf8ToWin1251(saveRecord), Encoding.GetEncoding("windows-1251"));
            }
        }

        /// <summary>
        /// Сохраняем ошибки
        /// </summary>
        static public void SaveError(string saveRecord)
        {
            lock (lockerSaveError)
            {
                File.AppendAllText("vrx_error.txt", VRX.Utf8ToWin1251(saveRecord), Encoding.GetEncoding("windows-1251"));
            }
        }


        public void GetContentVip()
        {
            //HtmlNode pageNodes = docPageParse.DocumentNode.SelectSingleNode("//tr[@class='vip']");
            //Console.WriteLine(pageNodes.Id);
            if (docPageParse != null)
            {
                HtmlNodeCollection pageNodes = docPageParse.DocumentNode.SelectNodes("//tr[@class='vip']");
                if (pageNodes != null)
                {
                    foreach (var item in pageNodes)
                    {
                        VRXParsePage record = new VRXParsePage((item.Id).Replace("td", ""));
                        SaveRecord(record.saveRecord);
                    }
                }
            }
            else
            {
                SaveError("Content is not null (VIP) " + pageParse );
            }
        }


        public void GetContent()
        {
            if (docPageParse != null)
            {
                HtmlNodeCollection pageNodes = docPageParse.DocumentNode.SelectNodes("//tr");

                if (pageNodes != null)
                {
                    foreach (var item in pageNodes)
                    {
                        HtmlAttributeCollection str = item.Attributes;
                        foreach (var item1 in str)
                        {
                            if (item1.Name == "style")
                            {
                                if (item1.Value == "cursor:pointer;")
                                {
                                    VRXParsePage record = new VRXParsePage((item.Id).Replace("td", ""));
                                    SaveRecord(record.saveRecord);
                                }
                            }
                        }
                    }
                }
                else
                {
                    SaveError("Content is not null (NOT VIP) " + pageParse );
                }
            }
        }


        

    }
}
