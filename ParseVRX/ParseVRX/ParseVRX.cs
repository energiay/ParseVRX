using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using xNet;
using System.IO;
using HtmlAgilityPack;


namespace ParseVRX
{
    class ParseVRX
    {

        public static int countUrl = 0;
        public static int countUrlAll;
        static object locker = new object();


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="urlPage">Считываем общее кол-во страниц</param>
        public ParseVRX(string urlPage)
        {
            Download(urlPage, "content/countUrlAll_VRX.txt"); //скачиваем html в текстовый файл
            //countUrlAll = GetPage("countUrlAll_VRX.txt"); //Парсим HTML и возвращаем общее кол-во страниц
        }

        
        /// <summary>
        /// Загружаем HTML в TXT
        /// </summary>
        /// <param name="url">ссылка, что загружать</param>
        public void Download(string url, string txt)
        {
            using (var request = new HttpRequest())
            {
                HttpResponse response = request.Get(url);
                // Принимаем тело сообщения в виде строки.
                string content = response.ToString();
                File.WriteAllText(txt, content);
            }
        }

        /*
        /// <summary>
        /// Считываес HTML в класс
        /// </summary>
        /// <param name="txt">имя текстового файла где хранится HTML</param>
        /// <returns></returns>
        HtmlDocument ReadHtml(string txt)
        {
            HtmlDocument doc = new HtmlDocument(); //Создаём экземпляр класса
            string html = System.IO.File.ReadAllText(txt); //Присваиваем текстовой переменной html-код
            doc.LoadHtml(html); //Загружаем в класс (парсер) наш html

            return doc;
        }*/
    }
}
