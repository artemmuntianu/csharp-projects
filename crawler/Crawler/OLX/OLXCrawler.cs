using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Crawler.OLX
{
    class OLXCrawler: ICrawler
    {
        async public Task Start()
        {
            //var pageUrlPattern = "http://olx.ua/elektronika/kompyutery/noutbuki/q-macbook?page={0}";
            var pageUrlPattern = "http://dnepropetrovsk.dnp.olx.ua/nedvizhimost/prodazha-kvartir/?search%5Bfilter_float_price%3Afrom%5D=14500&search%5Bfilter_float_price%3Ato%5D=15500&search%5Bfilter_float_number_of_rooms%3Afrom%5D=1&search%5Bfilter_float_number_of_rooms%3Ato%5D=1&search%5Bphotos%5D=1&page={0}&currency=USD";
            var allProducts = new List<OLXEntity>();
            for (int i = 1; i <= 5; i++)
            {
                allProducts.AddRange(await GetProductsFromPage(pageUrlPattern, i));
            }
            allProducts = allProducts.OrderByDescending(p => p.Price).ToList();
            using (var file = new StreamWriter(@"F:\Work\OLXResearch\" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + ".txt"))
                foreach (var product in allProducts)
                    file.WriteLine($"\"{product.Price}\"|\"{product.Name}\"|\"{product.Url}\"");
        }

        async private Task<List<OLXEntity>> GetProductsFromPage(string pageUrlPattern, int pageNumber)
        {
            var result = new List<OLXEntity>();
            var pageSource = string.Empty;
            using (var httpClient = new HttpClient())
            {
                pageSource = await httpClient.GetStringAsync(string.Format(pageUrlPattern, pageNumber));
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(pageSource);
            var offerNodes = doc.DocumentNode.SelectNodes("//td[contains(concat(' ', normalize-space(@class), ' '), ' offer ')]");
            if (offerNodes != null)
                foreach (var offerNode in offerNodes)
                {
                    try
                    {
                        var linkNode = offerNode.SelectSingleNode(".//a[contains(concat(' ', normalize-space(@class), ' '), ' link ') and contains(concat(' ', normalize-space(@class), ' '), ' linkWithHash ')]");
                        var priceNode = offerNode.SelectSingleNode(".//p[contains(concat(' ', normalize-space(@class), ' '), ' price ')]");
                        if (linkNode != null && priceNode != null)
                        {
                            result.Add(new OLXEntity
                            {
                                Name = linkNode.SelectSingleNode("strong").InnerText,
                                Url = linkNode.Attributes["href"].Value,
                                Price = int.Parse(priceNode.InnerText
                                    .Replace("\t", "").Replace("\n", "")
                                    .Replace("грн.", "").Replace("$", "")
                                    .Replace(" ", "")
                                )
                            });
                        }
                    }
                    catch (Exception)
                    {
                        // empty
                    }
                }
            return result;
        }
    }
}
