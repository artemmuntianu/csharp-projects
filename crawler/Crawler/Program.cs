using Crawler.Lun;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            new JewelersOrgCrawler().Start().Wait();
        }
    }
}
