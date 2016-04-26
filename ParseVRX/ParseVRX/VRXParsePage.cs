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
    class VRXParsePage
    {
        public static string pageRecordParse = "http://www.vrx.ru/data/"; // Cтрока url для записи
        public static int count=0;
        public string saveRecord = "";

        // Свойства с данными после парсинга
        string subject = "";          // Объект
        string location = "";       // Район
        string place = "";          // Место
        string address = "";        // Адрес (Улица,дом)
        string area = "";           // Общая площадь
        string floor = "";          // Этаж
        string floorHouse = "";     // Этажей в доме
        string material = "";         // Материал
        string type = "";             // Тип
        string sector = "";           // Размер участка
        string price = "";               // Цена
        string editDate = "";            // Дата изменения об-ия

        string operation = "";        // Продажа
        string flats = "";            // Кол-во комнат
        string balcony = "";          // Балкон
        string phone = "";            // Наличие телефота
        string bathroom = "";         // Санузел
        string basement = "";         // Подвал
        string electricit = "";       // Электричество
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


        public VRXParsePage(string idRecord)
        {
            idRecord = pageRecordParse + idRecord + ".htm";

            //Console.WriteLine("Загрузка " + idRecord);
            HtmlDocument record = Download(idRecord);

            // Если html получить не удалось
            if (record==null)
            {
                int i = 0; // кол-во попыток для запроса html
                while (record==null | i < 10) 
                {
                    record = Download(idRecord);
                    i++;
                }

                if (record == null)
                {
                    Thread t = Thread.CurrentThread;
                    t.Abort();
                    VRXParse.SaveError("Не удалось открыть страницу " + idRecord);
                }
            }
            //Console.WriteLine("Загрцзка " + idRecord + " окончена");
            //Console.WriteLine("");

            GetObj(record);
            GetSeller(record);
            SaveRecord();
            count++;
            //VRX.ConsoleWriteLineClear("Прочитано: " + count + " объяв. на стр. " + VRXParse.countPageParse, VRX.left, VRX.top);
        }


        HtmlDocument Download(string url)
        {

            try
            {
                using (var request = new HttpRequest())
                {
                    /*
                    Console.WriteLine("Загрузка страницы " + url);
                    HttpResponse response = request.Get(url);
                    Console.WriteLine("Загрузка страницы окончена");
                    Console.WriteLine("");
                    */

                    HttpResponse response = request.Get(url);

                    string html = response.ToString();
                    HtmlDocument doc = new HtmlDocument(); //Создаём экземпляр класса
                    doc.LoadHtml(html); //Загружаем в класс (парсер) наш html

                    //File.WriteAllText("VRX_other.txt", html);

                    return doc;
                }
            }
            catch
            {
                //Thread.Sleep(5000);
                return null;
            }

        }


        // получаем нужный формат объекта
        void GetSubject(string _subject)
        {
            if (_subject.IndexOf("комнатная") > -1 & _subject.IndexOf("квартира") > -1)
            {
                subject = (_subject.Replace("комнатная", "к.")).Replace("квартира", "кв.");
            }
            else
            {
                subject = _subject;
            }
        }


        // получаем район и место нахождения об-та
        void GetLocation(string _location)
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
            Decimal sum = 0;
            string[] split = _area.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] split1 = split[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 2; i++)
            {
                if (split[i] != "-")
                {
                    sum += Convert.ToDecimal(split[i].Replace(".", ","));
                }

            }
            if ( split1[0] != "-" )
            {
                sum += Convert.ToDecimal(split1[0].Replace(".", ","));
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
            string[] split = _sector.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            sector = split[0];
        }

        void GetPrise(string _prise)
        {
            string[] split = _prise.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < split.Length - 1; i++)
            {
                price += split[i];
            }
        }

        void GetEditDate(string _editDate)
        {
            editDate = _editDate.Substring(0, _editDate.Length - 3);
        }

        void GetOperation(string _operation)
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
            string[] split = _firm.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (split[1][0] == '(')
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
                //Console.WriteLine(j + ") " + pageNodes[j].InnerText);

                if ((pageNodes[j].InnerText).IndexOf("Объект:") > -1)
                {
                    GetSubject(pageNodes[j + 1].InnerText);
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
                if ((pageNodes[j].InnerText).IndexOf("Площадь:") == 0)
                {
                    if ( (pageNodes[j + 1].InnerText).IndexOf("/") > -1 )
                    {
                        GetArea(pageNodes[j + 1].InnerText);
                    }
                    else
                    {
                        //GetArea(pageNodes[j + 1].InnerText);
                    }
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
                    GetComment(pageNodes[j + 1].InnerText);
                }

                if ((pageNodes[j].InnerText).IndexOf("Фирма продавец:") > -1)
                {
                    GetFirm(pageNodes[j + 1].InnerText);
                }



            }


            //int i = 0;
        }




        void GetSeller(HtmlDocument record)
        {
            HtmlNodeCollection pageNodes = record.DocumentNode.SelectNodes("//table[@class='text tbldetail']/tr/td/table[@class='text']/tr/td");

            if (pageNodes != null)
            {
                for (int j = 0; j < pageNodes.Count - 1; j++)
                {
                    //Console.WriteLine(j + ")) " + pageNodes[j].InnerText);

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


        string GetStrContent()
        {
            subject = subject.Replace(";", " ");
            location = location.Replace(";", " ");
            place = place.Replace(";", " ");
            address = address.Replace(";", " ");
            area = area.Replace(";", " ");
            floor = floor.Replace(";", " ");
            floorHouse = floorHouse.Replace(";", " ");
            material = material.Replace(";", " ");
            type = type.Replace(";", " ");
            sector = sector.Replace(";", " ");
            price = price.Replace(";", " ");
            editDate = editDate.Replace(";", " ");

            operation = operation.Replace(";", " ");
            flats = flats.Replace(";", " ");
            balcony = balcony.Replace(";", " ");
            phone = phone.Replace(";", " ");
            bathroom = bathroom.Replace(";", " ");
            basement = basement.Replace(";", " ");
            electricit = electricit.Replace(";", " ");
            comment = comment.Replace(";", " ");

            firm = firm.Replace(";", " ");
            fioAgent = fioAgent.Replace(";", " ");
            phoneAgent = phoneAgent.Replace(";", " ");
            mailAgent = mailAgent.Replace(";", " ");

            return subject + ";" + location + ";" + place + ";" + address + ";" + area + ";" + floor + ";" + floorHouse + ";" + material + ";" + type + ";" + sector + ";" + price + ";" + firm + "," + fioAgent + "," + phoneAgent + "," + mailAgent + ";" + editDate + ";" + comment + ";" + "координаты"/*тут должны быть координаты*/ + ";" + operation + "-" + subject + ";" + "optional"/*optional*/ + ";" + "Rayon"/*Rayon*/ + ";" + "img1"/*тут пошел список картинок*/+ "\n";
        }


        void SaveRecord()
        {
            //string head = "Объект;Район;Место;Адрес;Площадь(Общ);Этаж;Этажей;Материал;Тип;Участок(соток);Цена;Контакты;Дата;Текст;Координаты;Заголовок;Optional;Rayon;image1;image2;image3;image4;image5;image6;image7;image8;image9;image10" + "\n";

            saveRecord = GetStrContent();

            //File.AppendAllText("vrx_all.csv", VRX.Utf8ToWin1251(record), Encoding.GetEncoding("windows-1251"));

        }

        /*
        void GetRecord(string idRecord)
        {
            idRecord = pageRecordParse + idRecord + ".htm";

            Console.WriteLine("Загрузка " + idRecord);
            HtmlDocument record = Download(idRecord);
            Console.WriteLine("Загрцзка " + idRecord + " окончена");
            Console.WriteLine("");

            GetObj(record);
            GetSeller(record);
            SaveRecord();
        }*/
    }
}
