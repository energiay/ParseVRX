using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xNet;
using HtmlAgilityPack;
using System.IO;

namespace ParseVRX
{
    class VRXParse
    {
        string pageParse; //неполная строка url
        string prePage = "&page="; // префикс для страниц
        public static int countPage = 0; // номер страцицы
        public static int countPageAll;  // общее кол-во страниц

        static object lockerPage = new object();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="web">строка парсинга</param>
        public VRXParse(string web)
        {
            pageParse = web;

            // Загружаю HTML и передаю ее в класс HtmlDocument
            HtmlDocument doc = ReadHtml( Download(pageParse) );
            countPageAll = GetPage(doc);
        }


        /// <summary>
        /// Загружаем HTML
        /// </summary>
        /// <param name="url">ссылка, что загружать</param>
        public string Download(string url)
        {
            string html;
            using (var request = new HttpRequest())
            {
                HttpResponse response = request.Get(url);
                html = response.ToString();
                //File.WriteAllText("VRX.txt", html);
            }
            return html;
        }


        /// <summary>
        /// Считываес HTML в класс
        /// </summary>
        /// <param name="txt">имя текстового файла где хранится HTML</param>
        /// <returns></returns>
        HtmlDocument ReadHtml(string content)
        {
            HtmlDocument doc = new HtmlDocument(); //Создаём экземпляр класса
            doc.LoadHtml(content); //Загружаем в класс (парсер) наш html

            return doc;
        }


        /// <summary>
        /// Получаем общее кол-во страниц
        /// </summary>
        /// <returns>Возвращаем общее кол-во страниц</returns>
        int GetPage(HtmlDocument doc)
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

        public string GetUrl(string url)
        {
            lock(lockerPage)
            {
                countPage++;
                url += countPage;
            }

            return url;
        }

    }
}
