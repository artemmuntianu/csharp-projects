using System.Net.Http;
using System.Threading.Tasks;

namespace Crawler
{
    public abstract class AbstractCrawler
    {
        async public Task<string> RequestWebPageSource(string pageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "User-Agent");
                var pageSource = await httpClient.GetStringAsync(pageUrl);
                return pageSource;
            }
        }
    }
}
