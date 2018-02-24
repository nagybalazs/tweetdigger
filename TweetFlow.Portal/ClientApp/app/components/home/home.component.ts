import { Component, OnInit } from '@angular/core';
import { Channel, Tweet } from '../../classes/classes';
import { TweetType } from '../../classes/enum/tweettype.enum';
import { ChannelService } from '../../services/services';

@Component({
    templateUrl: 'home.component.html',
    styleUrls: ['home.component.css']
})
export class HomeComponent implements OnInit {

    channels: Channel[];

    constructor(private channelService: ChannelService) { }

    ngOnInit() {
        this.channels = ChannelService.getChannels();
    }

}