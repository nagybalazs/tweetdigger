import { Component, Input } from '@angular/core';
import { Tweet, User } from '../../classes/classes';
import * as moment from 'moment';

@Component({
    selector: 'tweet',
    styleUrls: ['tweet.component.css'],
    templateUrl: 'tweet.component.html'
})
export class TweetComponent {

    private _tweet: Tweet;

    @Input()
    set tweet(tweet: Tweet) {
        this._tweet = tweet;
    }

    get tweet(): Tweet {
        return this._tweet;
    }

    get minutes(): string {
        let duration = moment.duration(moment.utc().diff(this.tweet.createdAt)).asMinutes();
        if (duration < 1) {
            return '< 1m';
        }
        return Math.floor(duration).toString() + 'm';
    }

}