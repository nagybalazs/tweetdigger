using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TweetFlow.DatabaseModel;
using TweetFlow.EF;

namespace TweetFlow.Providers
{
    public class TWAccountProvider : TWBaseProvider
    {
        public TWAccountProvider(DbContextOptions<TweetFlowContext> options)
            : base(options) { }

        public AddAccountResultType Add(string email)
        {
            using(var context = new TweetFlowContext(this.options))
            {
                var existing = context.TWAccount.FirstOrDefault(account => account.Email == email);
                if(existing != null)
                {
                    return AddAccountResultType.AlreadyExists;
                }

                context.TWAccount.Add(new DatabaseModel.TWAccount
                {
                    Email = email
                });

                try
                {
                    context.SaveChanges();
                    return AddAccountResultType.Success;
                }
                catch (Exception)
                {
                    return AddAccountResultType.Unknown;
                }
            }
        }
    }
}
