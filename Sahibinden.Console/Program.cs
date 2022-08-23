using System.IO;
using System.Net;

namespace Sahibinden.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sahibinden.com sitesinin anasayfasının tüm htmli string olarak alınır
            WebClient client = new WebClient();
            string mainPage = client.DownloadString("https://www.sahibinden.com/");

            //string mainPage = File.ReadAllText("MainPage.txt");

            //HtmlAgilityPack paketinin yardımıyla html elementleri içerisinde dolaşabilmek için html stringi nesneye çevirilir.
            HtmlAgilityPack.HtmlDocument mainPageDocument = new HtmlAgilityPack.HtmlDocument();
            mainPageDocument.OptionFixNestedTags = true;
            mainPageDocument.LoadHtml(mainPage);

            //Html nesnesinin içerisindeki ilgili ilanların html elementlerini yakalamak için sorgulama yapılır
            var products = mainPageDocument.DocumentNode.SelectNodes("/html/body//div[@class='homepage-content']//div[@class='uiBox showcase']/ul/li");

            decimal totalPrice = 0;
            string resultText = "";

            //Elementlerin içerisinde dolaşılır
            foreach (var product in products)
            {
                var detailLink = product.SelectSingleNode("a");

                //Link içerisindeki başlık bilgisi çekilir
                var productTitle = detailLink.Attributes["title"]?.Value;

                //Link içerisindeki href bilgisi çekilir
                var detailLinkPath = detailLink.Attributes["href"]?.Value;
                if (!string.IsNullOrEmpty(detailLinkPath))
                {
                    //Detay sayfasının html stringi çekilir
                    detailLinkPath = "https://www.sahibinden.com" + detailLinkPath;
                    string detailPage = client.DownloadString(detailLinkPath);
                    //string detailPage = File.ReadAllText("DetailPage.txt");

                    //Html string nesneye çevirilir
                    HtmlAgilityPack.HtmlDocument detailPageDocument = new HtmlAgilityPack.HtmlDocument();
                    detailPageDocument.OptionFixNestedTags = true;
                    detailPageDocument.LoadHtml(detailPage);

                    //Detay sayfasındaki fiyat bilgisi çekilir
                    var productPrice = detailPageDocument.DocumentNode.SelectSingleNode("/html/body//div[@class='classifiedDetailContent']//div[@class='classifiedInfo ']//input[@id='favoriteClassifiedPrice']").Attributes["value"].Value.Replace("TL", "").Replace("EUR", "").Replace("USD", "").Trim();

                    string result = $"Product: { productTitle} - Price: { productPrice}";

                    resultText += result + "\n";

                    //Console'a yazdırma işlemi yapılır
                    System.Console.WriteLine(result);


                    //Toplam fiyat hesaplanır
                    totalPrice += System.Convert.ToDecimal(productPrice);
                }
            }

            System.Console.WriteLine("--------------");

            //Ortalama hesaplanıp yazdırılır
            System.Console.WriteLine($"Avarage Price: { totalPrice / products.Count }");


            //Sonuç txt dosyasına yazılır, txt dosyası bin klasörünün içerisinde oluşturulur
            string path = System.Environment.CurrentDirectory;
            File.WriteAllText(Path.Combine(path, "Result.txt"), resultText);

            System.Console.ReadLine();
        }
    }
}
