import { Component, OnInit } from '@angular/core';
import { Channel, Tweet } from '../../classes/classes';
import { TweetType } from '../../classes/enum/tweettype.enum';
import { ChannelService } from '../../services/services';

@Component({
    templateUrl: 'home.component.html',
    styleUrls: ['home.component.css']
})
export class HomeComponent implements OnInit {

    bitcoin: Tweet[] = [];
    ethereum: Tweet[] = [];
    ripple: Tweet[] = [];
    litecoin: Tweet[] = [];

    channels: Channel[];

    constructor(private channelService: ChannelService) { }

    ngOnInit() {
        this.channelService.getCachedTweets()
            .subscribe(data => {
                this.bitcoin = data.filter(tweet => tweet.type == TweetType.Ethereum).map(tweet => Tweet.create(tweet));
                this.ethereum = data.filter(tweet => tweet.type == TweetType.Bitcoin).map(tweet => Tweet.create(tweet));
                this.ripple = data.filter(tweet => tweet.type == TweetType.Ripple).map(tweet => Tweet.create(tweet));
                this.litecoin = data.filter(tweet => tweet.type == TweetType.LiteCoin).map(tweet => Tweet.create(tweet));
                this.channels = ChannelService.getChannels();
            });
        
    }

    getInitialTweet(type: string): Tweet[] {
        if (type == "bitcoin") {
            return this.bitcoin;
        }
        if (type == "ethereum") {
            return this.ethereum;
        }
        if (type == "ripple") {
            return this.ripple;
        }
        if (type == "litecoin") {
            return this.litecoin;
        }
        return new Array<Tweet>();
    }

}