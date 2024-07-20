using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Crawler.Kadorr
{
    class KadorrCrawler : ICrawler
    {
        async public Task Start()
        {
            var pageUrlPattern = "http://vtor.kadorrgroup.com/vtorichnaya/page/{0}";
            var allProducts = new List<KadorrEntity>();
            for (int i = 1; i <= 5; i++)
            {
                allProducts.AddRange(await GetProductsFromPage(pageUrlPattern, i));
            }
            allProducts = allProducts.OrderByDescending(p => p.LivingComplexName).ToList();
            using (var file = new StreamWriter(@"F:\Other projects\Crawler\ResultsOfKadorrCrawler\" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + ".txt"))
                foreach (var product in allProducts)
                    file.WriteLine($"\"{product.Id}\"|\"{product.LivingComplexName}\"|\"{product.AmountOfSqM}\"|\"{product.Price}\"|\"{product.PriceForSqM}\"|\"{product.Floor}\"|\"{product.NumberOfRooms}\"");
        }

        async private Task<List<KadorrEntity>> GetProductsFromPage(string pageUrlPattern, int pageNumber)
        {
            var result = new List<KadorrEntity>();
            var pageSource = string.Empty;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "User-Agent");
                pageSource = await httpClient.GetStringAsync(string.Format(pageUrlPattern, pageNumber));
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(pageSource);
            var offerNodes = doc.DocumentNode.SelectNodes("//div[contains(concat(' ', normalize-space(@class), ' '), ' property ')]");
            if (offerNodes != null)
            {
                foreach (var offerNode in offerNodes)
                {
                    try
                    {
                        var amountOfSqM = offerNode.SelectSingleNode(".//table//tbody//tr[2]//td[2]").InnerText;
                        var price = offerNode.SelectSingleNode(".//table//tbody//tr[3]//td[2]").InnerText;
                        var priceForSqM = offerNode.SelectSingleNode(".//table//tbody//tr[5]//td[2]").InnerText;
                        var floor = offerNode.SelectSingleNode(".//table//tbody//tr[4]//td[2]").InnerText;
                        var numberOfRooms = offerNode.SelectSingleNode(".//table//tbody//tr[6]//td[2]").InnerText;
                        var newEntity = new KadorrEntity
                        {
                            Id = offerNode.SelectSingleNode(".//hgroup//h1//a").InnerText,
                            LivingComplexName = offerNode.SelectSingleNode(".//table//tbody//tr[1]//td[2]").InnerText,
                            AmountOfSqM = amountOfSqM.ExtractNumber().ParseToDoubleNullable(),
                            Price = price.ExtractNumber().ParseToDoubleNullable(),
                            PriceForSqM = priceForSqM.ExtractNumber().ParseToDoubleNullable(),
                            Floor = floor.ExtractNumber().ParseToIntNullable(),
                            NumberOfRooms = numberOfRooms.ExtractNumber().ParseToIntNullable()
                        };
                        if (newEntity.AmountOfSqM.HasValue)
                        {
                            result.Add(newEntity);
                        };
                    }
                    catch (Exception e)
                    {
                        // empty
                    }
                }
            }
            return result;
        }
    }
}
