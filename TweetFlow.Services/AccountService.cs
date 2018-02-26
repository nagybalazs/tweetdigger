using TweetFlow.DatabaseModel;
using TweetFlow.Providers;

namespace TweetFlow.Services
{
    public class AccountService
    {
        private TWAccountProvider tWAccountProvider;
        public AccountService(TWAccountProvider tWAccountProvider)
        {
            this.tWAccountProvider = tWAccountProvider;
        }

        public AddAccountResultType SaveEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return AddAccountResultType.EmailEmpty;
            }
            return this.tWAccountProvider.Add(email);
        }
    }
}
