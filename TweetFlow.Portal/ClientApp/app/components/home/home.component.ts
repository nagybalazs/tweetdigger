import { Component, OnInit } from '@angular/core';
import { Channel, Tweet } from '../../classes/classes';
import { TweetType } from '../../classes/enum/tweettype.enum';
import { ChannelService } from '../../services/services';
import { HubConnection, TransportType } from '@aspnet/signalr';

@Component({
    templateUrl: 'home.component.html',
    styleUrls: ['home.component.css']
})
export class HomeComponent implements OnInit {

    hubConnection: HubConnection;
    channels: Channel[];
    initialized: boolean = false;

    constructor(private channelService: ChannelService) { }

    ngOnInit() {
        this.hubConnection =
            new HubConnection('/tweets', { transport: TransportType.ServerSentEvents });

        this.hubConnection.start()
            .then(() => {
                console.log('started');
                this.initialized = true;
            })
            .catch(err => {
                console.log(JSON.stringify(err));
            });

        this.channelService.getChannels()
            .subscribe(data => {
                this.channels = data;
            });
    }

}