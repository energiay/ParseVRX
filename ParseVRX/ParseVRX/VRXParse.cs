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
        public static string pageParse; //неполная строка url
        string prePage = "&page="; // префикс для страниц
        public static int countPage = 0; // номер страцицы
        public static int countPageAll;  // общее кол-во страниц
        //string html; // HTML Страницы
        HtmlDocument docPageParse; // Загруженный html листа в класс HtmlDocument

        static object lockerPage = new object();




        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="web">строка парсинга</param>
        public VRXParse(string web)
        {
            pageParse = web;

            // Загружаю HTML и передаю ее в класс HtmlDocument
            Download( pageParse );
            countPageAll = GetPageParse( docPageParse );
            Console.WriteLine( countPageAll );
        }


        public VRXParse()
        {

        }


        /// <summary>
        /// Загружаем HTML
        /// </summary>
        /// <param name="url">ссылка, что загружать</param>
        public void Download(string url)
        {

            /*
            
            using (var request = new HttpRequest())
            {
                HttpResponse response = request.Get(url);
                string html = response.ToString();
                ReadHtml(html);
                File.WriteAllText("VRX.txt", html);
            }
            
             */

            using (var request = new HttpRequest())
            {
                var reqParams = new RequestParams();
                
                reqParams["viewtype"] = "0";
                reqParams["onpage"] = "100";        // Кол-во записей на странице
                reqParams["findfolders"] = "(1)";   // Какой раздел отображаем (Вторичка, Новостройка, ...)
                //reqParams["findfolders"] = "("+ findfolders + ")";   // Какой раздел отображаем (Вторичка, Новостройка, ...)
                reqParams["page"] = "1";            // номер страницы для отображения
                reqParams["findobject"] = "";       // Объект (не заполняется)

                string html = request.Post(url, reqParams).ToString();
                ReadHtml(html);
                //File.WriteAllText("VRX.txt", html);
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
        /// Получение ссылки на нужную страницу
        /// </summary>
        /// <param name="url">недострока</param>
        /// <returns>Строка с префисои и номером страницы</returns>
        public string GetUrl()
        {
            lock (lockerPage)
            {
                countPage++;
            }

            return pageParse + prePage + countPage;
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
