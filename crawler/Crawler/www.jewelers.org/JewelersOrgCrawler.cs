using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Crawler.Lun
{
    class JewelersOrgCrawler : AbstractCrawler, ICrawler
    {
        async public Task Start()
        {
            var pageUrlPattern = "https://www.jewelers.org/find-a-jeweler?start={0}";
            //
            var productPageUrls = new List<string>();
            for (var i = 0; i < 255; i++)
            {
                productPageUrls.AddRange(await GetProductsPageUrls(string.Format(pageUrlPattern, i * 30)));
            }
            //
            var allProducts = new List<JewelersOrgEntity>();
            foreach (var pageUrl in productPageUrls)
            {
                allProducts.AddRange(await GetProducts(pageUrl));
            }
            using (var file = new StreamWriter(@"F:\Other projects\Crawler\ResultsOfJewelersOrgCrawler\" + DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace('/', '-').Replace(':', '-').Replace(' ', '-') + ".txt"))
                foreach (var product in allProducts)
                    file.WriteLine(product.Url);
        }

        async private Task<List<string>> GetProductsPageUrls(string pageUrl)
        {
            var result = new List<string>();
            var pageSource = await RequestWebPageSource(pageUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(pageSource);
            var offerNodes = doc.DocumentNode.SelectNodes("//a[contains(concat(' ', normalize-space(@class), ' '), ' namelink ')]");
            if (offerNodes != null)
                foreach (var offerNode in offerNodes)
                    result.Add($"https://www.jewelers.org{offerNode.Attributes["href"].Value}");
            Console.WriteLine($"Processed {pageUrl}");
            return result;
        }

        async private Task<List<JewelersOrgEntity>> GetProducts(string pageUrl)
        {
            var result = new List<JewelersOrgEntity>();
            var pageSource = await RequestWebPageSource(pageUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(pageSource);
            try
            {
                var orgUrl = doc.DocumentNode.SelectSingleNode("//h3//a").InnerText.Trim();
                result.Add(new JewelersOrgEntity
                {
                    Url = orgUrl
                });
            }
            catch (Exception e)
            {
                // empty
            }
            finally
            {
                Console.WriteLine($"Processed {pageUrl}");
            }
            return result;
        }
    }
}
