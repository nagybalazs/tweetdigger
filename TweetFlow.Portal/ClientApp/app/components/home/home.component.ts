import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Channel, StoredChannel, Tweet } from '../../classes/classes';
import { TweetType } from '../../classes/enum/tweettype.enum';
import { ChannelService } from '../../services/services';
import { HubConnection, TransportType } from '@aspnet/signalr';

const defaultOpenedChannelCount: number = 4;
const channelStoreKey: string = "channels";

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

    selectedChannel: Channel | null = null;
    addChannelToggled: boolean = false;

    constructor(private channelService: ChannelService, private changeDetectionService: ChangeDetectorRef) { }

    ngOnInit() {
        this.channelService
            .createHubConnection()
            .startHubConnection()
                .then(() => {
                    this.initialized = true;
                    this.channelService.startChannel();
                })
                .catch(err => {
                    console.log(err);
                });

        this.channelService.getChannels()
            .subscribe(data => {
                if (data) {
                    let storedChannels: StoredChannel[] = this.getStoredChannels();
                    if (storedChannels && storedChannels.length > 0) {
                        let tempChannels: Channel[] = new Array<Channel>();
                        storedChannels.forEach(storedChannel => {
                            let restoredChannel: Channel = new Channel();
                            restoredChannel.closed = storedChannel.closed;
                            restoredChannel.column = storedChannel.column;
                            restoredChannel.endpoint = storedChannel.endpoint;
                            restoredChannel.name = storedChannel.name;
                            let currentChannel = data.find(channel => channel.endpoint == storedChannel.endpoint);
                            restoredChannel.tweets = new Array<Tweet>();
                            if (currentChannel) {
                                restoredChannel.tweets = currentChannel.tweets;
                            }
                            tempChannels.push(restoredChannel);
                        });
                        this.channels = tempChannels;
                        this.setChannelClosableFlags();
                    }
                    else {
                        let count = data.length < defaultOpenedChannelCount ? data.length : defaultOpenedChannelCount;
                        let indexer = 0;
                        for (let i = 0; i < data.length; i++) {
                            if (i < count) {
                                data[i].column = indexer;
                                this.channels.push(data[i]);
                                indexer++;
                            }
                            else {
                                data[i].closed = true;
                                data[i].column = -1;
                                this.channels.push(data[i]);
                            }
                        }
                        this.setChannelClosableFlags();
                    }
                }
            });
    }

    openChannel(channelToOpen: Channel) {
        this.closeAddChannelMenu();
        if (channelToOpen) {
            let columns = this.channels.map(channel => channel.column);
            let maxColumn = Math.max(...columns);
            channelToOpen.column = maxColumn + 1;
            channelToOpen.closed = false;
            this.sortChannels();
            this.setChannelClosableFlags();
            this.storeChannels();
        }
    }

    closeChannel(endpointOfChannelToClose: string) {
        let channelToClose = this.getChannelByEndpoint(endpointOfChannelToClose);
        if (channelToClose) {
            let originalColumn = channelToClose.column;
            channelToClose.column = -1;
            this.channels.forEach(channel => {
                if (channel.column >= originalColumn && channel.column != -1) {
                    channel.column -= 1;
                }
            });
            channelToClose.closed = true;
            this.sortChannels();
            this.setChannelClosableFlags();
            this.storeChannels();
        }
    }

    toggleAddChannelMenu() {
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
        if (this.selectedChannel) {
            this.selectedChannel.column = oldChannel.column;
        }
        oldChannel.column = -1;
        this.sortChannels();
        oldChannel.closed = true;
        if (this.selectedChannel) {
            this.selectedChannel.closed = false;
        }
        this.setChannelClosableFlags();
        this.storeChannels();
    }

    storeChannels() {
        let channelsToStore: StoredChannel[] = new Array<StoredChannel>();
        this.channels.forEach(channel => {
            let channelToStore: StoredChannel = {
                closed: channel.closed,
                column: channel.column,
                endpoint: channel.endpoint,
                name: channel.name
            };
            channelsToStore.push(channelToStore);
        });
        localStorage.setItem(channelStoreKey, JSON.stringify(channelsToStore));
    }

    private setChannelClosableFlags() {
        let closable = this.openedChannels.length > 1;
        this.channels.forEach(channel => {
            channel.closable = closable;
        });
    }

    private sortChannels() {
        this.channels.sort((channel1, channel2) => { return channel1.column - channel2.column });
    }

    private getStoredChannels(): StoredChannel[] {
        let json = localStorage.getItem(channelStoreKey);
        if (!json) {
            return new Array<StoredChannel>();
        }

        let data = JSON.parse(json);
        return data;
    }

    private getChannelByEndpoint(endpoint: string): Channel | undefined {
        return this.channels.find(channel => channel.endpoint == endpoint);
    }

}