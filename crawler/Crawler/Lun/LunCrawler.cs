using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Crawler.Lun
{
    class LunCrawler : AbstractCrawler, ICrawler
    {
        async public Task Start()
        {
            var pageUrlPattern = "http://novostroyki.lun.ua/%D0%B2%D1%81%D0%B5-%D0%BD%D0%BE%D0%B2%D0%BE%D1%81%D1%82%D1%80%D0%BE%D0%B9%D0%BA%D0%B8-%D0%BE%D0%B4%D0%B5%D1%81%D1%81%D1%8B?page={0}";
            //
            var productPageUrls = new List<string>();
            for (int i = 1; i <= 5; i++)
            {
                productPageUrls.AddRange(await GetProductsPageUrls(string.Format(pageUrlPattern, i)));
            }
            //
            var allProducts = new List<LunEntity>();
            foreach (var pageUrl in productPageUrls)
            {
                allProducts.AddRange(await GetProducts(pageUrl));
            }
            allProducts = allProducts.OrderByDescending(p => p.Price).ToList();
            using (var file = new StreamWriter(@"F:\Other projects\Crawler\ResultsOfLunCrawler\" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-') + ".txt"))
                foreach (var product in allProducts)
                    file.WriteLine($"\"{product.LivingComplexName}\"|\"{product.AmountOfSqM}\"|\"{product.Price}\"|\"{product.PriceForSqM}\"|\"{product.NumberOfRooms}\"|\"{product.Queue}\"|\"{product.Url}\"");
        }

        async private Task<List<string>> GetProductsPageUrls(string pageUrl)
        {
            var result = new List<string>();
            var pageSource = await RequestWebPageSource(pageUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(pageSource);
            var offerNodes = doc.DocumentNode.SelectNodes("//a[contains(concat(' ', normalize-space(@class), ' '), ' no-decor ')]");
            if (offerNodes != null)
            {
                foreach (var offerNode in offerNodes)
                {
                    result.Add($"http://novostroyki.lun.ua{offerNode.Attributes["href"].Value}");
                }
            }
            return result;
        }

        async private Task<List<LunEntity>> GetProducts(string pageUrl)
        {
            var result = new List<LunEntity>();
            var pageSource = await RequestWebPageSource(pageUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(pageSource);
            try
            {
                var livingComplexName = doc.DocumentNode.SelectSingleNode("//h1[@class=\"page-header__text\"]").InnerText.Trim();
                var queueNodes = doc.DocumentNode.SelectNodes("//div[@class=\"developer-offers__table-wrap queue-info\"]");
                if (queueNodes != null)
                {
                    if (queueNodes.Count > 1)
                    {
                        queueNodes.RemoveAt(0);
                    }
                    for (var i = 0; i < queueNodes.Count; i++)
                    {
                        var queueNode = queueNodes[i];
                        var queue = string.Empty;
                        var queueStateNode = doc.DocumentNode.SelectSingleNode($"//select[@class=\"select select_full queue-select\"]//option[{i + 2}]");
                        if (queueStateNode != null)
                        {
                            queue = queueStateNode.NextSibling.InnerText;
                        }
                        else
                        {
                            queueStateNode = doc.DocumentNode.SelectSingleNode("//div[@class=\"developer-offers__one-queue_text\"]");
                            if (queueStateNode != null)
                                queue = queueStateNode.InnerText;
                        }
                        queue = queue.Trim().Replace("&nbsp;", "");
                        var queueOffersNodes = queueNode.SelectNodes(".//tr[@class=\"developer-offers__table-row\"]");
                        for (int j = 0; j < queueOffersNodes.Count; j++)
                        {
                            var amountOfSqM = queueNode.SelectSingleNode(".//tr[" + (j + 2) + "]/td[2]").InnerText.Trim().Replace("&nbsp;", "");
                            var priceForSqM = queueNode.SelectSingleNode(".//tr[" + (j + 2) + "]/td[4]").InnerText.Trim().Replace("&nbsp;", "");
                            var numberOfRooms = queueNode.SelectSingleNode(".//tr[" + (j + 2) + "]/td[1]").InnerText.Trim().Replace("&nbsp;", "");
                            var newEntity = new LunEntity
                            {
                                LivingComplexName = livingComplexName,
                                AmountOfSqM = amountOfSqM.ExtractNumber().ParseToDoubleNullable(),
                                PriceForSqM = priceForSqM.ExtractNumber().ParseToDoubleNullable(),
                                NumberOfRooms = numberOfRooms.ExtractNumber().ParseToIntNullable(),
                                Queue = queue,
                                Url = pageUrl
                            };
                            if (newEntity.AmountOfSqM.HasValue
                                && newEntity.PriceForSqM.HasValue
                                && newEntity.NumberOfRooms.HasValue)
                            {
                                result.Add(newEntity);
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // empty
            }
            return result;
        }
    }
}
