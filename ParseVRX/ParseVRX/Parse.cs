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
        public void Download (string url)
        {
            using (var request = new HttpRequest())
            {

                HttpResponse response = request.Get( url );
                // Принимаем тело сообщения в виде строки.
                string content = response.ToString();
                File.WriteAllText("ksota.txt", content);
            }
        }

        /*public Task<int> ConnectAndParse(string url)
        {
            Download(url);
            return Task.Run (() =>
            {
                return GetContent();
            });
        }*/

        public void ConnectAndParse(string url)
        {
            Download(url);
            GetContent();
        }

        public int GetContent()
        {
            string sTypeFlat = "";  //тип квартирв
            string sArea = "";      //размер квартиры
            string sWall = "";      //отделка
            string sPage = "";      //кол-во страниц

            // Создаём экземпляр класса
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            // Присваиваем текстовой переменной k html-код
            string html = System.IO.File.ReadAllText("ksota.txt");

            // Загружаем в класс (парсер) наш html
            doc.LoadHtml(html);

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


                // Извлекаем кол-во страниц
                HtmlNodeCollection pageNodes = doc.DocumentNode.SelectNodes("//ul[@class='paging']/li");
                foreach (var page in pageNodes)
                {
                    sPage = page.InnerText;
                    if (sPage.IndexOf("...")!=-1)
                    {

                        string str = "";
                        for (int j = 0; j < sPage.Length; j++)
                        {
                            if (Convert.ToInt32( sPage[j]) == 46)
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
                File.AppendAllText("ksota.csv", Utf8ToWin1251(body), Encoding.GetEncoding("windows-1251"));

            }

            //textBox1.Text = bodyNodeCol.;
            if (sPage == "175")
                return Convert.ToInt32(sPage);
            else
                return 175;
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
