import { Component, OnInit, Input, Output, ChangeDetectorRef, EventEmitter } from '@angular/core';
import { HubConnection, TransportType } from '@aspnet/signalr';
import { Tweet, Channel } from '../../classes/classes';
import { ChannelService } from '../../services/services';

@Component({
    selector: 'channel',
    templateUrl: './channel.component.html',
    styleUrls: ['channel.component.css']
})
export class ChannelComponent implements OnInit {

    private initialized: boolean = false;

    @Input()
    channel: Channel;

    @Input()
    hub: HubConnection;

    @Output()
    channelClosed: EventEmitter<string> = new EventEmitter<"">();

    constructor(private channelService: ChannelService, private changeDecetionService: ChangeDetectorRef) { }

    ngOnInit() {

        if (!this.channel || !this.channel.endpoint) {
            throw Error("endpoint is required");
        }

        this.channelService.getCachedTweets(this.channel.endpoint)
            .subscribe(cachedTweets => {
                this.channelService.joinHubGroup(this.channel.endpoint);
                this.channelService.tweetReceived
                    .subscribe(tweet => {
                        if (tweet.type != this.channel.endpoint) {
                            return;
                        }
                        if (this.channel.tweets.length > 100) {
                            this.channel.tweets.splice(-1, 1);
                        }
                        this.channel.tweets.unshift(tweet);
                        this.changeDecetionService.detectChanges();
                    });
            });
    }

    closeChannel() {
        this.channelClosed.emit(this.channel.endpoint);
    }

}
