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
        public static string pageParse; // Cтрока url
        public static string pageRecordParse = "http://www.vrx.ru/data/"; // Cтрока url для записи
        
        //public static int countPage = 0; // номер страцицы
        int countPageParse;
        public static int countPageAll;  // общее кол-во страниц

        string findfoldersParse;
        //string prePage = "&page="; // префикс для страниц
        
        //string html; // HTML Страницы
        HtmlDocument docPageParse; // Загруженный html листа в класс HtmlDocument

        static object lockerPage = new object();
        static object lockerPageCount = new object();

        // Свойства с данными после парсинга
        string subject;         // Объект
        string location;        // Район
        string place;           // Место
        string address;         // Адрес (Улица,дом)
        string area;            // Общая площадь

        string editDate;
        string operation;
        string flats;
        string floor;
        string material;
        string type;
        string price;
        /*string water;
        string gas;
        string sewerage;
        string bathroom;
        string phone;
        string electricity;
        string heating;*/
        ////////////////////////////////////


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
            countPageAll = GetPageParse( docPageParse );
            Console.WriteLine( countPageAll );
        }


        public VRXParse(string findfolders, int _countPageParse)
        {
            findfoldersParse = findfolders;
            countPageParse = _countPageParse;
        }


        HtmlDocument Download(string url)
        {

           using (var request = new HttpRequest())
           {
               HttpResponse response = request.Get(url);
               string html = response.ToString();
                HtmlDocument doc = new HtmlDocument(); //Создаём экземпляр класса
                doc.LoadHtml(html); //Загружаем в класс (парсер) наш html
                return doc;
            }

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

                Console.WriteLine("Загрузка страницы " + pageParse);
                string html = request.Post(pageParse, reqParams).ToString();
                Console.WriteLine("Загрузка страницы окончена");
                Console.WriteLine("");
                
                lock (lockerPageCount)
                {
                    GetNextPage();
                }

                Console.WriteLine("Чтение страницы " + pageParse);
                ReadHtml(html);
                Console.WriteLine("Чтение страницы окончена");
                Console.WriteLine("");

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
        /// След.страница
        /// </summary>
        public void GetNextPage()
        {
            lock (lockerPage)
            {
                countPageParse++;
            }
        }


        // получаем нужный формат объекта
        void GetSubject(string _subject)
        {
            if (_subject.IndexOf("комнатная")>-1 & _subject.IndexOf("квартира") > -1)
            {
                subject = (_subject.Replace("комнатная", "к.")).Replace("квартира", "кв.");
            }
            else
            {
                subject = _subject;
            }
        }


        // получаем район и место нахождения об-та
        void GetLocation (string _location)
        {
            _location = _location.Replace("&raquo", "");
            _location = _location.Replace("р-н", "");

            string[] split = _location.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            location = split[2].Trim();
            place = split[3].Trim();
        }


        // Получаем общую площадь
        void GetArea(string _area)
        {
            string[] split = _area.Split(new char[] { '/'}, StringSplitOptions.RemoveEmptyEntries);
            string[] split1 = split[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            area = (Convert.ToDecimal(split[0]) + Convert.ToDecimal(split[1]) + Convert.ToDecimal(split1[0])).ToString();
        }


        void GetObj(HtmlDocument record)
        {
            //HtmlNode bodyNode = record.DocumentNode.SelectSingleNode("//ul[@class='product-list']");
            HtmlNodeCollection pageNodes = record.DocumentNode.SelectNodes("//table[@class='text tbldetail']/tr/td");
            int i = 0;
            foreach (var item in pageNodes)
            {
                // Дата и время изменения об-ия
                if (i==2)
                {
                    editDate = item.InnerText;
                }

                // Операция (продажа)
                if (i==4)
                {
                    operation = item.InnerText;
                }

                // Объект (2х комнатная квартира)
                if (i==8)
                {
                    GetSubject(item.InnerText);
                }

                // Расположение
                if (i==10)
                {
                    GetLocation( item.InnerText );
                }

                // Адрес
                if (i==12)
                {
                    
                    address = item.InnerText;
                }

                // Комнат
                if(i==14)
                {
                    flats = item.InnerText;
                }

                // Этаж/этажей
                if (i==16)
                {
                    floor = item.InnerText;
                }

                // Материал стен
                if (i==18)
                {
                    material = item.InnerText;
                }

                // Площадь квартиры
                if (i==20)
                {
                    GetArea(item.InnerText);
                }

                // Тип квартиры
                if (i==22)
                {
                    type = item.InnerText;
                }

                // Цена
                if (i==24)
                {
                    price = item.InnerText;
                }

                /*

                // Санузел
                if (i==35)
                {
                    bathroom = item.InnerText;
                }

                // Наличие телефона
                if (i==37)
                {
                    phone = item.InnerText;
                }

                // Электричество
                if (i==41)
                {
                    electricity = item.InnerText;
                }

                // Отопление
                if (i==43)
                {
                    heating = item.InnerText;
                }

                // Вода
                if (i==45)
                {
                    water = item.InnerText;
                }

                // Газ
                if (i==47)
                {
                    gas = item.InnerText;
                }

                // Канализация
                if (i==49)
                {
                    sewerage = item.InnerText;
                }

                */

                Console.WriteLine(i +" "+item.InnerText);
                i++;
            }

        }


        void GetRecord(string idRecord)
        {
            idRecord = pageRecordParse + idRecord + ".htm";
            Console.WriteLine("Загрузка "+ idRecord);
            HtmlDocument record = Download( idRecord );
            Console.WriteLine("Загрцзка " + idRecord + " окончена");
            Console.WriteLine("");
            GetObj(record);
        }


        public void GetContent()
        {
            //HtmlNode pageNodes = docPageParse.DocumentNode.SelectSingleNode("//tr[@class='vip']");
            //Console.WriteLine(pageNodes.Id);

            HtmlNodeCollection pageNodes = docPageParse.DocumentNode.SelectNodes("//tr[@class='vip']");
            
            foreach (var item in pageNodes)
            {
                GetRecord( (item.Id).Replace("td", "") );
            }
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
