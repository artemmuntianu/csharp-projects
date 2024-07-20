using System.Collections.Generic;
using System.Web.Mvc;
using WordTrainer.Domain;

namespace WordTrainer.WebUI.Controllers
{
    public class HomeController : Controller
    {
        [OutputCache(Duration = 0)]
        public ActionResult Word()
        {
            var showedWordsIdxs = default(List<int>);
            var sessionShowedWordsIdxs = Session["ShowedWordsIdxs"];
            if (sessionShowedWordsIdxs != null)
            {
                showedWordsIdxs = (List<int>)sessionShowedWordsIdxs;
            }
            else
            {
                showedWordsIdxs = new List<int>();
            }
            var nextWord = DbWords.GetRandom(showedWordsIdxs);
            // if all words from Db have already been shown to user
            if (nextWord == null)
            {
                showedWordsIdxs = new List<int>();
                nextWord = DbWords.GetRandom(showedWordsIdxs);
            }
            showedWordsIdxs.Add(nextWord.Id);
            Session.Add("ShowedWordsIdxs", showedWordsIdxs);
            return View(nextWord);
        }
    }
}