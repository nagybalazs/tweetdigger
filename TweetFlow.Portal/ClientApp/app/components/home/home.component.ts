import { Component, OnInit } from '@angular/core';
import { Channel, Tweet } from '../../classes/classes';
import { TweetType } from '../../classes/enum/tweettype.enum';
import { ChannelService } from '../../services/services';
import { HubConnection, TransportType } from '@aspnet/signalr';

const defaultOpenedChannelCount = 2;

@Component({
    templateUrl: 'home.component.html',
    styleUrls: ['home.component.css']
})
export class HomeComponent implements OnInit {

    hubConnection: HubConnection;
    channels: Channel[] = new Array<Channel>();
    initialized: boolean = false;

    get closedChannels(): Channel[] {
        return this.channels.filter(channel => channel.closed);
    }

    get openedChannels(): Channel[] {
        return this.channels.filter(channel => !channel.closed);
    }

    selectedChannel: Channel;
    addChannelToggled: boolean = false;

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
                if (data) {
                    let count = data.length < defaultOpenedChannelCount ? data.length : defaultOpenedChannelCount;
                    for (let i = 0; i < data.length; i++) {
                        if (i < count) {
                            this.channels.push(data[i]);
                        }
                        else {
                            data[i].closed = true;
                            this.channels.push(data[i]);
                        }
                    }
                }
            });
    }

    openChannel(channelToOpen: Channel) {
        this.closeAddChannelMenu();
        if (channelToOpen) {
            channelToOpen.closed = false;
        }
    }

    closeChannel(endpointOfChannelToClose: string) {
        let channelToClose = this.getChannelByEndpoint(endpointOfChannelToClose);
        if (channelToClose) {
            channelToClose.closed = true;
        }
    }

    toggleAddChannelMenu() {
        this.channels.sort(p => p.column);
        this.addChannelToggled = !this.addChannelToggled;
    }

    closeAddChannelMenuOnOutSideClick($event: any) {
        if ($event && $event.target && !$event.target.classList.contains('prevent-close')) {
            this.closeAddChannelMenu();
        }
    }

    closeAddChannelMenu() {
        this.addChannelToggled = false;
    }

    changeChannel(oldChannel: Channel) {
        oldChannel.closed = true;
        this.selectedChannel.closed = false;
    }

    private getChannelByEndpoint(endpoint: string): Channel | undefined {
        return this.channels.find(channel => channel.endpoint == endpoint);
    }

}