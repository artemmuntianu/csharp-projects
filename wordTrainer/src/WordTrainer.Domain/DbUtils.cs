using System;
using System.Data;

namespace WordTrainer.Domain
{
    /// <summary>
    /// Contains auxiliary db methods.
    /// </summary>
    public static class DbUtils
    {
        public static T OpenDbContext<T>(Func<WordTrainerDbEntities, T> f)
        {
            try
            {
                using (var db = new WordTrainerDbEntities())
                {
                    return f(db);
                }
            }
            // DataException is the common exception for all EntityFramework exceptions  
            catch (DataException e)
            {
                /*  To see entity validation errors in Watch window type next
                    "((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors" */
                throw e;
            }
        }

        public static void OpenDbContext(Action<WordTrainerDbEntities> f)
        {
            try
            {
                using (var db = new WordTrainerDbEntities())
                {
                    f(db);
                }
            }
            // DataException is the common exception for all EntityFramework exceptions  
            catch (DataException e)
            {
                /*  To see entity validation errors in Watch window type next
                    "((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors" */
                throw e;
            }
        }
    }
}
