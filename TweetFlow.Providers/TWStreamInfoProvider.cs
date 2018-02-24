using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TweetFlow.DatabaseModel;
using TweetFlow.EF;

namespace TweetFlow.Providers
{
    public class TWStreamInfoProvider
    {
        public TWStreamInfo Add(TWStreamInfo tWStreamInfo)
        {
            using(var context = new TweetFlowContext())
            {
                context.TWStreamInfo.Add(tWStreamInfo);
                context.SaveChanges();
                return tWStreamInfo;
            }
        }

        public List<TWStreamInfo> GetAll()
        {
            using(var context = new TweetFlowContext())
            {
                return context.TWStreamInfo.AsNoTracking().ToList();
            }
        }

        public TWStreamInfo GetLast()
        {
            using (var context = new TweetFlowContext())
            {
                return context.TWStreamInfo.OrderByDescending(p => p.OccuredAt).FirstOrDefault();
            }
        }
    }
}
