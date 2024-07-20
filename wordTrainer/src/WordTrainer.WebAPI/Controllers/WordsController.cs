using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WordTrainer.Domain;

namespace WordTrainer.WebAPI.Controllers
{
    public class WordsController : ApiController
    {
        public IEnumerable<int> Get()
        {
            return DbWords.GetAll().Select(w=>w.Id);
        }

        public Word Random(List<int> idxsToSkip)
        {
            return DbWords.GetRandom(idxsToSkip);
        }
    }
}