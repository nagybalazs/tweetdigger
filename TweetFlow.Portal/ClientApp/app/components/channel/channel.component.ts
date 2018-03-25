import { Component, OnInit, Input, ChangeDetectorRef } from '@angular/core';
import { HubConnection, TransportType } from '@aspnet/signalr';
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

    @Input()
    hub: HubConnection;

    public tweets: Tweet[] = [];

    constructor(private channelService: ChannelService, private changeDecetionService: ChangeDetectorRef) { }

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
        this.hub.invoke('JoinGroup', this.endpoint);

        this.hub.on('JoinGroup', (data: string) => {
            console.log(data);
        });
        
        this.hub.on('Send', (data: any) => {
            let tweet = Tweet.create(data);
            if (tweet.type != this.endpoint) {
                return;
            }
            if (this.tweets.length >= 100) {
                this.tweets.splice(-1, 1);
            }
            this.tweets.unshift(tweet);
            this.changeDecetionService.detectChanges();
        });
    }

}
