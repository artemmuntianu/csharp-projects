using System;
using System.Collections.Generic;
using System.Linq;

namespace WordTrainer.Domain
{
    public static class DbWords
    {
        public static IEnumerable<Word> GetAll()
        {
            return DbUtils.OpenDbContext(db =>
            {
                return db.Words.ToArray();
            });
        }

        public static Word GetRandom<T>(T idxsToSkip = null) 
            where T: class, IEnumerable<int>, new()
        {
            var result = default(Word);
            idxsToSkip = idxsToSkip ?? new T();
            return DbUtils.OpenDbContext(db =>
            {
                var wordsAmount = db.Words.Where(w => !idxsToSkip.Contains(w.Id)).Count();
                // if there is no such words in Db
                if (wordsAmount == 0)
                {
                    result = null;
                }
                else
                {
                    var wordsToSkip = new Random().Next(0, wordsAmount);
                    result = db.Words.Where(w => !idxsToSkip.Contains(w.Id)).OrderBy(o => o.Id).Skip(wordsToSkip).Take(1).First();
                }
                return result;
            });
        }
    }
}