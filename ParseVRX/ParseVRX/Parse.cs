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
    class Parse
    {
        public static int countUrl = 0;
        public static int countUrlAll;
        static object locker = new object();


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="urlPage">Считываем общее кол-во страниц</param>
        public Parse(string urlPage)
        {
            Download(urlPage, "countUrlAll.txt"); //скачиваем html в текстовый файл
            countUrlAll = GetPage("countUrlAll.txt"); //Парсим HTML и возвращаем общее кол-во страниц
        }


        /// <summary>
        /// Загружаем HTML в TXT
        /// </summary>
        /// <param name="url">ссылка, что загружать</param>
        public void Download (string url, string txt)
        {
            using (var request = new HttpRequest())
            {

                HttpResponse response = request.Get( url );
                // Принимаем тело сообщения в виде строки.
                string content = response.ToString();
                File.WriteAllText(txt, content);
            }
        }


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
        }


        /// <summary>
        /// Получаем ссылку на след.стр.
        /// </summary>
        /// <param name="url">Ссылка без индекса</param>
        /// <returns>Ссылка</returns>
        public string GetUrl(string url)
        {
            string urlParse = url + countUrl;
            Console.WriteLine(Parse.countUrl + " из " + Parse.countUrlAll);
            countUrl++;

            return urlParse;
        }


        /// <summary>
        /// Получаем общее кол-во страниц
        /// </summary>
        /// <returns>Возвращаем общее кол-во страниц</returns>
        int GetPage(string txt)
        {
            string sPage = "";      //кол-во страниц
            HtmlDocument doc = ReadHtml(txt);

            // Извлекаем кол-во страниц
            HtmlNodeCollection pageNodes = doc.DocumentNode.SelectNodes("//ul[@class='paging']/li");
            foreach (var page in pageNodes)
            {
                sPage = page.InnerText;
                if (sPage.IndexOf("...") != -1)
                {

                    string str = "";
                    for (int j = 0; j < sPage.Length; j++)
                    {
                        if (Convert.ToInt32(sPage[j]) == 46)
                        {

                        }
                        else if (Convert.ToInt32(sPage[j]) == 32)
                        {

                        }
                        else
                        {
                            str += sPage[j];
                        }
                    }

                    sPage = str;
                }
            }

            return Convert.ToInt32(sPage);
        }













        /*public Task<int> ConnectAndParse(string url)
        {
            Download(url);
            return Task.Run (() =>
            {
                return GetContent();
            });
        }*/




        public void GetContent(string txt)
        {
            string sTypeFlat = "";  //тип квартирв
            string sArea = "";      //размер квартиры
            string sWall = "";      //отделка

            HtmlDocument doc = ReadHtml(txt);

            // Извлекаем всё текстовое, что есть в теге <div> с классом bla1
            HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//ul[@class='product-list']");
            HtmlNodeCollection bodyNodeCol = bodyNode.SelectNodes("li");

            foreach (HtmlNode item in bodyNodeCol)
            {
                //Операция
                HtmlNode Operation = item.SelectSingleNode("div/div/span[@class='lbl-sale']");

                //Дата публикации
                HtmlNode DateShow = item.SelectSingleNode("div/span[@class='date']");

                HtmlNodeCollection colTTH = bodyNode.SelectNodes("li");

                //Title
                HtmlNode Title = item.SelectSingleNode("div/strong/a");

                //Title
                HtmlNode Price = item.SelectSingleNode("div/div/div[@class='price']/span[@class='lbl-price']");

                //группа
                HtmlNodeCollection hTypeFlat = item.SelectNodes("div/div/ul/li");
                var i = 1;
                foreach (var itemLi in hTypeFlat)
                {
                    if (i == 1)
                    {
                        //Type
                        string[] aTypeFlat = (itemLi.InnerText).Split(' ');
                        sTypeFlat = aTypeFlat[aTypeFlat.Count() - 1];
                    }
                    else if (i == 2)
                    {
                        //Общая площадь
                        string[] aArea = (itemLi.InnerText).Split('/');
                        Int32 iArea = 0;
                        foreach (var str in aArea)
                        {
                            iArea += Convert.ToInt32(str.Substring(0, str.IndexOf(".")));
                        }
                        sArea = iArea.ToString();
                    }
                    else if (i == 3)
                    {
                        // тут может быть этаж
                    }
                    else if (i == 4)
                    {
                        //отделка (стены)
                        sWall = itemLi.InnerText;
                    }
                    else if (i == 5)
                    {
                        // тут может быть кол-во фото
                    }

                    i++;
                }


                


                /*
                MessageBox.Show(    Operation.InnerText + "\n - " +
                                    DateShow.InnerText + "\n - " +
                                    Title.InnerText + "\n - " +
                                    sTypeFlat + "\n - " +
                                    sArea + "\n - " +
                                    sWall + "\n - " +
                                    Price.InnerText + "\n - " +
                                    "ok                                  "+ sPage
                                );
                */
                //string body = "Операция (аренда/Продажа)|дата публикации|заголовок|тип квартиры|общая площадь|цена|материал стен";
                string body = Operation.InnerText +"|"+ DateShow.InnerText + "|" + Title.InnerText + "|" + sTypeFlat + "|" + sArea + "|" + ((Price.InnerText).Replace(" ", "")).Replace("Р", "") + "|" + sWall + "\n";

                lock (locker)
                {
                    File.AppendAllText("ksota.csv", Utf8ToWin1251(body), Encoding.GetEncoding("windows-1251"));
                }

            }

            //textBox1.Text = bodyNodeCol.;
            /*if (sPage == "175")
                return Convert.ToInt32(sPage);
            else
                return 175;*/
        }

        public string Utf8ToWin1251(string str)
        {
            Encoding srcEncodingFormat = Encoding.UTF8;
            Encoding dstEncodingFormat = Encoding.GetEncoding("windows-1251");

            byte[] originalByteString = srcEncodingFormat.GetBytes(str);
            byte[] convertedByteString = Encoding.Convert(srcEncodingFormat, dstEncodingFormat, originalByteString);

            return dstEncodingFormat.GetString(convertedByteString);
        }
    }
}
