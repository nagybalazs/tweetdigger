import { Component, OnInit, Input } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { Tweet } from '../../classes/classes';
import { ChannelService } from '../../services/services';

@Component({
    selector: 'channel',
    templateUrl: './channel.component.html'
})
export class ChannelComponent implements OnInit {

    private initialized: boolean = false;

    @Input()
    endpoint: string;

    public tweets: Tweet[] = [];
    private _hubConnetion: HubConnection;

    constructor(private channelService: ChannelService) { }

    ngOnInit() {

        if (!this.endpoint) {
            throw Error("endpoint is required");
        }

        this.channelService.getCachedTweets(this.endpoint)
            .subscribe(cachedTweets => {
                this.tweets = cachedTweets.map(tweet => Tweet.create(tweet));
                this.initialize();
            });
    }

    initialize() {
        this._hubConnetion = new HubConnection('/' + this.endpoint);

        this._hubConnetion.on('Send', (data: any) => {
            if (this.tweets.length >= 100) {
                this.tweets.splice(-1, 1);
            }
            let tweet = Tweet.create(data);
            this.tweets.unshift(tweet);
            console.log(JSON.stringify(tweet));
        });

        this._hubConnetion.start()
            .then(() => {
                console.log('started');
            })
            .catch(err => {
                console.log(JSON.stringify(err));
            });
    }

}
