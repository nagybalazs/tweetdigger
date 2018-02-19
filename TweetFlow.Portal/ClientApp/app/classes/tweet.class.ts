import * as moment from 'moment';
import { User } from './classes';
import { TweetType } from './enum/tweettype.enum';

export class Tweet {
    strId: string;
    fullText: string;
    createdAt: moment.Moment;
    quoteCount: number;
    replyCount: number;
    retweetCount: number;
    favoriteCount: number;
    favorited: boolean;
    isRetweet: boolean;
    user: User;
    type: TweetType;

    static create(tweet: Tweet): Tweet {
        let created = new Tweet();

        created.strId = tweet.strId;
        created.fullText = tweet.fullText;
        created.createdAt = moment.utc(tweet.createdAt);

        created.quoteCount = +tweet.quoteCount;
        if (isNaN(created.quoteCount)) {
            created.quoteCount = 0;
        }

        created.replyCount = +tweet.replyCount;
        if (isNaN(created.replyCount)) {
            created.replyCount = 0;
        }

        created.retweetCount = +tweet.replyCount;
        if (isNaN(created.replyCount)) {
            created.replyCount = 0;
        }

        created.favoriteCount = +tweet.favoriteCount;
        if (isNaN(created.favoriteCount)) {
            created.favoriteCount = 0;
        }

        created.type = +tweet.type;

        created.favorited = tweet.favorited;
        created.isRetweet = tweet.isRetweet;

        created.user = User.create(tweet.user);

        tweet = created;

        return tweet;
    }
}