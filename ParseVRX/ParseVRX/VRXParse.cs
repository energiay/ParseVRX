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
        string subject="";          // Объект
        string location = "";       // Район
        string place = "";          // Место
        string address = "";        // Адрес (Улица,дом)
        string area = "";           // Общая площадь
        string floor = "";          // Этаж
        string floorHouse = "";     // Этажей в доме
        string material="";         // Материал
        string type="";             // Тип
        string sector="";           // Размер участка
        string price;               // Цена
        string editDate;            // Дата изменения об-ия

        string operation="";        // Продажа
        string flats="";            // Кол-во комнат
        string balcony="";          // Балкон
        string phone="";            // Наличие телефота
        string bathroom="";         // Санузел
        string basement="";         // Подвал
        string electricit="";       // Электричество
        string comment = "";        // Комментарий к заявке

        string firm = "";           // Фирма продавец
        string fioAgent = "";       // ФИО агента
        string phoneAgent = "";     // телефон агента
        string mailAgent = "";      // email агента
        

        /*string water;
        string gas;
        string sewerage;
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

                File.WriteAllText("VRX_other.txt", html);

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

                File.WriteAllText("VRX.txt", html);
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
            if (split.Length > 2)
            {
                location = split[2].Trim();
            }
            if (split.Length > 3)
            {
                place = split[3].Trim();
            }
        }


        // Получаем общую площадь
        void GetArea(string _area)
        {
            Decimal sum=0;
            string[] split = _area.Split(new char[] { '/'}, StringSplitOptions.RemoveEmptyEntries);
            string[] split1 = split[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 2; i++)
            {
                if (split[i]!="-")
                {
                    sum += Convert.ToDecimal(split[i]);
                }

            }
            if (split1[0] != "-")
            {
                sum += Convert.ToDecimal(split1[0]);
            }
            area = sum.ToString();
        }


        // Получаем этажность дома и этаж квартиры
        void GetFloor(string _floor)
        {
            string[] split = _floor.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            floor = split[0];
            floorHouse = split[1];
        }


        // Получаем материал дома
        void GetMaterial(string _material)
        {
            material = _material;
        }

        // Получаем тип квартик
        void GetType(string _type)
        {
            type = _type;
        }

        // Получаем адрес
        void GetAddress(string _address)
        {
            address = _address;
        }

        void GetSector(string _sector)
        {
            string[] split = _sector.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            sector = split[0];
        }

        void GetPrise(string _prise)
        {
            string[] split = _prise.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < split.Length-1; i++)
            {
                price += split[i];
            }
        }

        void GetEditDate(string _editDate)
        {
            editDate = _editDate.Substring(0, _editDate.Length-3);
        }

        void GetOperation (string _operation)
        {
            operation = _operation;
        }

        void GetFlats(string _flats)
        {
            flats = _flats;
        }

        void GetBalcony(string _balcony)
        {
            balcony = _balcony;
        }

        void GetComment(string _comment)
        {
            comment = _comment.Replace("&quot;", "'");
        }


        void GetFirm(string _firm)
        {
            string[] split = _firm.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (split[1][0]=='(')
            {
                firm = split[0] + " " + split[1];
            }
            else
            {
                firm = split[0];
            }
        }


        void GetObj(HtmlDocument record)
        {
            //HtmlNode bodyNode = record.DocumentNode.SelectSingleNode("//ul[@class='product-list']");
            HtmlNodeCollection pageNodes = record.DocumentNode.SelectNodes("//table[@class='text tbldetail']/tr/td");

            for (int j = 0; j < pageNodes.Count; j++)
            {
                Console.WriteLine(j + ") " + pageNodes[j].InnerText);

                if ( (pageNodes[j].InnerText).IndexOf("Объект:") > -1 )
                {
                    GetSubject(pageNodes[j+1].InnerText);
                }

                // Расположение
                if ((pageNodes[j].InnerText).IndexOf("Месторасположение:") > -1)
                {
                    GetLocation(pageNodes[j + 1].InnerText);
                }

                if ((pageNodes[j].InnerText).IndexOf("Адрес:") > -1)
                {
                    GetAddress(pageNodes[j + 1].InnerText);
                }

                // площадь
                if ((pageNodes[j].InnerText).IndexOf("Площадь:") > -1)
                {
                    GetArea(pageNodes[j + 1].InnerText);
                }


                // Этажность
                if ((pageNodes[j].InnerText).IndexOf("Этаж/этажей:") > -1)
                {
                    GetFloor(pageNodes[j + 1].InnerText);
                }

                if ((pageNodes[j].InnerText).IndexOf("Материал:") > -1)
                {
                    GetMaterial(pageNodes[j + 1].InnerText);
                }

                if ((pageNodes[j].InnerText).IndexOf("Тип:") > -1)
                {
                    GetType(pageNodes[j + 1].InnerText);
                }
            
                if ((pageNodes[j].InnerText).IndexOf("Участок:") > -1)
                {
                    GetSector(pageNodes[j + 1].InnerText);
                }
            
                if ((pageNodes[j].InnerText).IndexOf("Стоимость:") > -1)
                {
                    GetPrise(pageNodes[j + 1].InnerText);
                }

                if ((pageNodes[j].InnerText).IndexOf("Изменено:") > -1)
                {
                    GetEditDate(pageNodes[j + 1].InnerText);
                }

                if ((pageNodes[j].InnerText).IndexOf("Операция:") > -1)
                {
                    GetOperation(pageNodes[j + 1].InnerText);
                }

                if ((pageNodes[j].InnerText).IndexOf("Комнат:") > -1)
                {
                    GetFlats(pageNodes[j + 1].InnerText);
                }

                if ((pageNodes[j].InnerText).IndexOf("Балкон:") > -1)
                {
                    GetBalcony(pageNodes[j + 1].InnerText);
                }

                if ((pageNodes[j].InnerText).IndexOf("Санузел:") > -1)
                {
                    bathroom = pageNodes[j + 1].InnerText;
                }

                if ((pageNodes[j].InnerText).IndexOf("Наличие телефона:") > -1)
                {
                    phone = pageNodes[j + 1].InnerText;
                }

                if ((pageNodes[j].InnerText).IndexOf("Подвал:") > -1)
                {
                    basement = pageNodes[j + 1].InnerText;
                }

                if ((pageNodes[j].InnerText).IndexOf("Электричество:") > -1)
                {
                    electricit = pageNodes[j + 1].InnerText;
                }

                if ((pageNodes[j].InnerText).IndexOf("Комментарий к заявке:") > -1)
                {
                    GetComment( pageNodes[j + 1].InnerText );
                }

                if ((pageNodes[j].InnerText).IndexOf("Фирма продавец:") > -1)
                {
                    GetFirm(pageNodes[j + 1].InnerText);
                }

                

            }


            int i = 0;
        }




        void GetSeller(HtmlDocument record)
        {
            HtmlNodeCollection pageNodes = record.DocumentNode.SelectNodes("//table[@class='text tbldetail']/tr/td/table[@class='text']/tr/td");
            if (pageNodes != null)
            {
                for (int j = 0; j < pageNodes.Count-1; j++)
                {
                    Console.WriteLine(j + ")) " + pageNodes[j].InnerText);

                    if ((pageNodes[j].InnerText).IndexOf("Агент:") > -1)
                    {
                        fioAgent = pageNodes[j + 1].InnerText;
                    }

                    if ((pageNodes[j].InnerText).IndexOf("Телефон агента:") > -1)
                    {
                        phoneAgent = pageNodes[j + 1].InnerText;
                    }

                    if ((pageNodes[j].InnerText).IndexOf("e-mail:") > -1)
                    {
                        mailAgent = pageNodes[j + 1].InnerText;
                    }
                }
            }
            else
            {
                HtmlNodeCollection pageNodesOther = record.DocumentNode.SelectNodes("//table[@class='text tbldetail']/tr/td");

                var bmailAgent = true; // проверка на первое вхождение

                for (int j = 0; j < pageNodesOther.Count; j++)
                {
                    if ((pageNodesOther[j].InnerText).IndexOf("Агент:") > -1)
                    {
                        fioAgent = pageNodesOther[j + 1].InnerText;
                    }

                    if ((pageNodesOther[j].InnerText).IndexOf("Телефон агента:") > -1)
                    {
                        phoneAgent = pageNodesOther[j + 1].InnerText;
                    }

                    if ((pageNodesOther[j].InnerText).IndexOf("e-mail:") > -1)
                    {
                        // если попал, то в цикле больше не попадешь )
                        if (bmailAgent)
                        {
                            mailAgent = pageNodesOther[j + 1].InnerText;
                            bmailAgent = false;
                        }
                    }
                }
            }

        }


        void SaveRecord()
        {
            string record = subject + ";" + location + ";" + place + ";" + area + ";" + floor + ";" + floorHouse + ";" + material + ";" + type + ";" + sector + ";" + firm + "" + phoneAgent + "" + fioAgent + "" + mailAgent + ";" + price + ";" + editDate + ";" + comment + ";" + ""/*тут должны быть координаты*/ + ";" + operation + " " + subject + ";" + ""/*optional*/ + ";" + ""/*Rayon*/ + ";" + ""/*тут пошел список картинок*/+"\n";

            File.AppendAllText("vrx_all.csv", VRXParse.Utf8ToWin1251(record), Encoding.GetEncoding("windows-1251"));

        }


        void GetRecord(string idRecord)
        {
            idRecord = pageRecordParse + idRecord + ".htm";
            Console.WriteLine("Загрузка "+ idRecord);
            HtmlDocument record = Download( idRecord );
            Console.WriteLine("Загрцзка " + idRecord + " окончена");
            Console.WriteLine("");
            GetObj(record);
            GetSeller(record);
            SaveRecord();
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
